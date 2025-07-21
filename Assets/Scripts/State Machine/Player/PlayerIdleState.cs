using UnityEngine;

// �÷��̾��� ��� ���¸� �����ϴ� PlayerIdleState Ŭ���� (PlayerController�� ����)
public class PlayerIdleState : State<PlayerController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = false;
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
        // �Է� ������ �ִ� ������ ���
        else if (GameManager.Instance.MoveDirection != Vector3.zero)
        {
            // �̵� ���� (MoveState)�� ��ȯ
            stateMachine.ChangeState<PlayerMoveState>();
            return;
        }
        // Ÿ���� �ִ� ���
        else if (GameManager.Instance.MoveDirection == Vector3.zero && context.Target != null)
        {
            // ���� ���� (PursuitState)�� ��ȯ
            stateMachine.ChangeState<PlayerPursuitState>();
            return;
        }
    }
}