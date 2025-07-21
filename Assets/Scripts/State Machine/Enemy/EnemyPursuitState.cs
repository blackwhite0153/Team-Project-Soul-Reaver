using UnityEngine;
using UnityEngine.AI;

public class EnemyPursuitState : State<EnemyController>
{
    private Vector3 _direction;

    // ���� ĳ���Ͱ� �������� ���� �ִ��� ����
    private bool _isFacingRight;

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

        // ĳ������ ���� ���� (Local Scale ����)���� IsFacingRight ����
        context.IsFacingRight = context.SpriteObject.localScale.x < 0.0f;
    }

    public override void OnFixedUpdate(float fixedDeltaTime)
    {
        base.OnFixedUpdate(fixedDeltaTime);

        MoveToPlayer();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        FlipSprite();

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
        // Ÿ���� �����ϸ�, ���� ���� �̳��� ���� ���
        else if (context.Target != null && context.TargetDistance <= context.RuntimeStats.AttackRange)
        {
            // ���� ���� (AttackState)�� ��ȯ
            stateMachine.ChangeState<EnemyAttackState>();
            return;
        }
        // Ÿ���� �������� �ʴ� ���
        else if (context.Target == null)
        {
            // ��� ���� (IdleState)�� ��ȯ
            stateMachine.ChangeState<EnemyIdleState>();
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

    private void MoveToPlayer()
    {
        if (context.Target == null) return;
        if (context.NavMeshAgent == null || !context.NavMeshAgent.isOnNavMesh) return;

        context.NavMeshAgent.SetDestination(context.Target.transform.position);

        Vector3 direction = (context.NavMeshAgent.steeringTarget - context.transform.position).normalized;
        direction.y = 0.0f;

        if (direction != Vector3.zero)
        {
            context.Rigidbody.MovePosition(context.transform.position + direction * context.RuntimeStats.MoveSpeed * Time.deltaTime);
        }
    }

    private void FlipSprite()
    {
        if (context.Target != null)
        {
            // �÷��̾�� ���� ��ġ ���� ���
            Vector3 directionToPlayer = context.Target.transform.position - context.transform.position;

            bool isMovingRight = directionToPlayer.x > 0 ? true : false;
            context.IsFacingRight = isMovingRight;

            Vector3 scale = context.SpriteObject.localScale;
            scale.x = Mathf.Abs(scale.x) * (isMovingRight ? -1 : 1);
            context.SpriteObject.localScale = scale;
        }
    }
}