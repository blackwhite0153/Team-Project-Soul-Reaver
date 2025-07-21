using System.Collections;
using UnityEngine;

public class EnemyAttackState : State<EnemyController>
{
    private Coroutine _coAttackAnim;

    public override void OnInitialized()
    {
        _coAttackAnim = null;
    }

    public override void OnEnter()
    {
        base.OnEnter();

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
                case ClassType.Warrior:
                    _coAttackAnim = context.StartCoroutine(CoWarriorAttack());
                    break;
                case ClassType.Archer:
                    _coAttackAnim = context.StartCoroutine(CoArcherAttack());
                    break;
                case ClassType.Wizard:
                    _coAttackAnim = context.StartCoroutine(CoWizardAttack());
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
            stateMachine.ChangeState<EnemyDieState>();
            return;
        }
        // �ǰ� ������ ���
        else if (context.IsDamaged)
        {
            // �ǰ� ���� (HitState)�� ��ȯ
            stateMachine.ChangeState<EnemyHitState>();
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
                    stateMachine.ChangeState<EnemyPursuitState>();
                    return;
                }
                // ���� ���� �ȿ� Ÿ���� �ִ� ��� ���� ���ε�
                else
                {
                    // ���� ���� (Attacktate)�� ��ȯ
                    stateMachine.ReloadState<EnemyAttackState>();
                    return;
                }
            }

            // Ÿ���� ���� ���
            if (context.Target == null)
            {
                // ��� ���� (IdleState)�� ��ȯ
                stateMachine.ChangeState<EnemyIdleState>();
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
        //PerformAreaDamage(_hitOffset, _hitRadius, context.EnemyRuntimeStats.AttackDamage);S

        // ��ٿ� ���
        yield return new WaitForSeconds(context.RuntimeStats.AttackCooldown);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }

    private IEnumerator CoWarriorAttack()
    {
        //Warrior warrior = context as Warrior;
        Enemy enemy = context as Enemy;

        // �ִϸ��̼� �󿡼� Ÿ���� �Ͼ�� ����(��: 0.3��)��ŭ ���
        yield return new WaitForSeconds(0.3f);

        // PerformAreaDamage ���� ȣ�� �� hitOffset, hitRadius, damage�� �Ѱ� �ָ� �ȴ�.
        PerformAreaDamage(enemy.HitOffset, enemy.HitRadius, enemy.RuntimeStats.AttackDamage);

        // ��ٿ� ���
        yield return new WaitForSeconds(context.RuntimeStats.AttackCooldown);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }

    private IEnumerator CoArcherAttack()
    {
        //Archer archer = context as Archer;
        Enemy enemy = context as Enemy;

        yield return new WaitForSeconds(0.4f); // �߻� Ÿ�̹�

        if (enemy.ArrowProjectile != null && context.Target != null)
        {
            Vector3 direction = (context.Target.transform.position - context.transform.position).normalized;
            direction.y = 0.0f;

            Vector3 spawnOffset = new Vector3(0.0f, 1.5f, 0.0f);
            Vector3 spawnPos = context.transform.position + direction * enemy.ArrowFireOffset + spawnOffset;

            GameObject projectile = PoolManager.Instance.ActivateObj(enemy.ArrowProjectile, spawnPos, Quaternion.identity);
            ArrowProjectile arrow = projectile.GetComponent<ArrowProjectile>();

            if (arrow != null)
            {
                // ���� ���ݷ�(runtimeStats.attackDamage)�� ����ü�� ����
                arrow.SetArrowDamage(context.RuntimeStats.AttackDamage);

                // ����ü ���� ����
                arrow.SetDirectionWithAtan2(context.Target.transform.position);
                arrow.SetArrowSpeed(enemy.ArrowProjectileSpeed);
            }
        }

        yield return new WaitForSeconds(0.2f);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }

    private IEnumerator CoWizardAttack()
    {
        //Wizard wizard = context as Wizard;
        Enemy enemy = context as Enemy;

        yield return new WaitForSeconds(0.4f); // �߻� Ÿ�̹�

        if (enemy.MagicProjectile != null && context.Target != null)
        {
            Vector3 direction = (context.Target.transform.position - context.transform.position).normalized;
            direction.y = 0.0f;

            Vector3 spawnOffset = new Vector3(0.0f, 1.5f, 0.0f);
            Vector3 spawnPos = context.transform.position + direction * enemy.MagicFireOffset + spawnOffset;

            GameObject projectile = PoolManager.Instance.ActivateObj(enemy.MagicProjectile, spawnPos, Quaternion.identity);
            MagicProjectile magicBall = projectile.GetComponent<MagicProjectile>();

            if (magicBall != null)
            {
                // ���� ���ݷ�(runtimeStats.attackDamage)�� ����ü�� ����
                magicBall.SetMagicDamage(context.RuntimeStats.AttackDamage);

                // ����ü ���� ����
                magicBall.SetDirectionWithAtan2(context.Target.transform.position);
                magicBall.SetMagicSpeed(enemy.MagicProjectileSpeed);
            }
        }

        yield return new WaitForSeconds(context.RuntimeStats.AttackCooldown);

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnim);
        _coAttackAnim = null;
    }
}