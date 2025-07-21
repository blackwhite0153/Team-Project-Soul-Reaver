using System.Collections;
using UnityEngine;

// �÷��̾��� ���� ���¸� �����ϴ� PlayerAttackState Ŭ���� (PlayerController�� ����)
public class PlayerAttackState : State<PlayerController>
{
    private Coroutine _coAttackAnimDelay;

    private PlayerStats _playerStats;
    public override void OnInitialized()
    {
        _playerStats = context.GetComponent<PlayerStats>();
        _coAttackAnimDelay = null;
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
        if (_coAttackAnimDelay == null)
        {
            _coAttackAnimDelay = context.StartCoroutine(CoAttackDelay());
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // ü���� 0���Ϸ� ������ ���
        if (context.IsDeath)
        {
            // ��� ���� (DieState)�� ��ȯ
            stateMachine.ChangeState<PlayerDieState>();
            return;
        }
        // �ǰ� ������ ���
        //else if (context.IsDamaged)
        //{
        //    // �ǰ� ���� (HitState)�� ��ȯ
        //    stateMachine.ChangeState<PlayerHitState>();
        //    return;
        //}

        // ���� ���� �Է� ������ ������ ���
        if (GameManager.Instance.MoveDirection != Vector3.zero)
        {
            // �̵� ���� (MoveState)�� ��ȯ
            stateMachine.ChangeState<PlayerMoveState>();
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
                    stateMachine.ChangeState<PlayerPursuitState>();
                    return;
                }
                // ���� ���� �ȿ� Ÿ���� �ִ� ��� ���� ���ε�
                else
                {
                    // ���� ���� (Attacktate)�� ��ȯ
                    stateMachine.ReloadState<PlayerAttackState>();
                    return;
                }
            }

            // Ÿ���� ���� ���
            if (context.Target == null)
            {
                // ��� ���� (IdleState)�� ��ȯ
                stateMachine.ChangeState<PlayerIdleState>();
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

    // ���� �� �ִϸ��̼� ���̸�ŭ ������
    private IEnumerator CoAttackDelay()
    {
        yield return null;

        AnimatorStateInfo stateInfo = context.Animator.GetCurrentAnimatorStateInfo(0);
        float baseAnimLength = stateInfo.length;

        float attackSpeed = Mathf.Max(_playerStats.AttackSpeed); // 0���� ������ �� ����

        // ���� �ӵ��� �������� �ִϸ��̼� ���̴� ª������ ��
        float adjustedDelay = baseAnimLength / attackSpeed;

        yield return new WaitForSeconds(adjustedDelay);

        context.PerformAttackDamage();

        context.IsAttack = false;

        context.StopCoroutine(_coAttackAnimDelay);
        _coAttackAnimDelay = null;


    }
}