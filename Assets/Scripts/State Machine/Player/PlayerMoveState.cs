using UnityEngine;

// �÷��̾��� �̵� ���¸� �����ϴ� PlayerMoveState Ŭ���� (PlayerController�� ����)
public class PlayerMoveState : State<PlayerController>
{
    // ĳ������ �̵� �ӵ�
    private float _moveSpeed;

    public override void OnInitialized() {}

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

        _moveSpeed = PlayerStats.Instance.MoveSpeed;

        Move();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        context.UpdateFacingByInput(GameManager.Instance.MoveDirection);

        // ü���� 0���Ϸ� ������ ���
        if (context.IsDeath)
        {
            // ��� ���� (DieState)�� ��ȯ
            stateMachine.ChangeState<PlayerDieState>();
        }
        // �Է� ������ ���� ������ ���
        else if (GameManager.Instance.MoveDirection == Vector3.zero)
        {
            // �̵� ���� (IdleState)�� ��ȯ
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

    // �̵� �� ��������Ʈ ������ ó���ϴ� �Լ�
    private void Move()
    {
        Vector3 direction = GameManager.Instance.MoveDirection;
        Vector3 targetPosition = context.Rigidbody.position + direction.normalized * _moveSpeed * Time.fixedDeltaTime;
        context.Rigidbody.MovePosition(targetPosition);
    }
}