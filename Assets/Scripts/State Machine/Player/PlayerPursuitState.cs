using UnityEngine;

// �÷��̾��� ���� ���¸� �����ϴ� PlayerPursuitState Ŭ���� (PlayerController�� ����)
public class PlayerPursuitState : State<PlayerController>
{
    private Vector3 _direction;

    // ĳ������ �̵� �ӵ�
    private float _moveSpeed;

    public override void OnInitialized()
    {
        // ���� �ʱ�ȭ
        _direction = Vector3.zero;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsMove = true;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = false;

        _moveSpeed = PlayerStats.Instance.MoveSpeed;
    }

    public override void OnFixedUpdate(float fixedDeltaTime)
    {
        base.OnFixedUpdate(fixedDeltaTime);

        TargetToMove();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        context.UpdateFacingByTarget();

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
        // Ÿ���� �����ϸ�, ���� ���� �̳��� ���� ���
        else if (context.Target != null && context.TargetDistance <= 3.0f)
        {
            // ���� ���� (AttackState)�� ��ȯ
            stateMachine.ChangeState<PlayerAttackState>();
            return;
        }
        // Ÿ���� �������� �ʴ� ���
        else if (context.Target == null)
        {
            // ��� ���� (IdleState)�� ��ȯ
            stateMachine.ChangeState<PlayerIdleState>();
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = false;
    }

    private void TargetToMove()
    {
        if (context.Target == null) return;

        Vector3 position = context.Rigidbody.position; // Rigidbody ���� ��ġ ���
        Vector3 targetPosition = context.Target.transform.position;

        _direction = targetPosition - position;

        Vector3 move = _direction.normalized * _moveSpeed * Time.fixedDeltaTime;
        Vector3 targetMovePosition = position + move;

        context.Rigidbody.MovePosition(targetMovePosition);
    }
}