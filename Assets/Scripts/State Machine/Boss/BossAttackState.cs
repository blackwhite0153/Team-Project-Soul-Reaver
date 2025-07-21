using System.Collections;
using UnityEngine;

public class BossAttackState : State<BossController>
{
    private Coroutine _coAttackAnim;

    public override void OnInitialized()
    {
        _coAttackAnim = null;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log($"<color=red>{context.name} State : {this.GetType().Name}</color>");

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsMove = false;
        context.IsAttack = true;
        context.IsDamaged = false;
        context.IsDeath = false;

        // ���� ��� Ʈ���� ����
        context.Animator.SetTrigger(Define.Attack);

        // ���� ������ �ڷ�ƾ ����
        if (_coAttackAnim == null)
        {
            switch (context.EnemyClassType)
            {
                case ClassType.Boss:
                    _coAttackAnim = context.StartCoroutine(CoBossAttack());
                    break;
            }
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // ü���� 0���Ϸ� ������ ���
        if (context.IsDeath)
        {
            // ��� ���� (DieState)�� ��ȯ
            stateMachine.ChangeState<BossDieState>();
            return;
        }
        // �ǰ� ������ ���
        else if (context.IsDamaged)
        {
            // �ǰ� ���� (HitState)�� ��ȯ
            stateMachine.ChangeState<BossHitState>();
            return;
        }

        // ���� �����̰� ���� ���
        if (!context.IsAttack)
        {
            // Ÿ���� �����ϴ� ���
            if (context.Target != null)
            {
                // ���� ���� �ۿ� Ÿ���� �ִ� ���
                if (context.TargetDistance > 3.0f)
                {
                    // ���� ���� (PursuitState)�� ��ȯ
                    stateMachine.ChangeState<BossPursuitState>();
                    return;
                }
                // ���� ���� �ȿ� Ÿ���� �ִ� ��� ���� ���ε�
                else
                {
                    // ���� ���� (Attacktate)�� ��ȯ
                    stateMachine.ReloadState<BossAttackState>();
                    return;
                }
            }

            // Ÿ���� ���� ���
            if (context.Target == null)
            {
                // ��� ���� (IdleState)�� ��ȯ
                stateMachine.ChangeState<BossIdleState>();
                return;
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsAttack = false;
    }

    private void PerformAreaDamage(float offset, float radius, float damage)
    {
        // 1) ���� ���� ���
        Vector3 hitPoint = context.transform.position + context.transform.forward * offset;

        // 2) OverlapSphere�� ����� ��, whatIsPlayer ���̾� ����ũ�� �߰��մϴ�.
        // �̷��� �ϸ� 'Player' ���̾ ���� �ݶ��̴��� �����մϴ�.
        Collider[] hitColliders = Physics.OverlapSphere(hitPoint, radius, context.TargetLayer);

        // 3) ������ Collider�� ���� �÷��̾� ���̹Ƿ�, �ٷ� �������� �ݴϴ�.
        foreach (var col in hitColliders)
        {
            // TryGetComponent�� ������ ���� �����Դϴ�.
            if (col.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage, context.transform);
            }
        }
    }

    private IEnumerator CoBossAttack()
    {
        // �ִϸ��̼� �󿡼� Ÿ���� �Ͼ�� ����(��: 0.3��)��ŭ ���
        yield return new WaitForSeconds(0.3f);

        // PerformAreaDamage ���� ȣ�� �� hitOffset, hitRadius, damage�� �Ѱ� �ָ� �ȴ�.
        //PerformAreaDamage(_hitOffset, _hitRadius, context.EnemyRuntimeStats.AttackDamage);

        // ��ٿ� ���
        yield return new WaitForSeconds(context.RuntimeStats.AttackCooldown);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }
}