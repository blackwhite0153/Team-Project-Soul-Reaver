using UnityEngine;

public class EnemyIdleState : State<EnemyController>
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
        // Ÿ���� �ִ� ���
        else if (context.Target != null)
        {
            // ���� ���� (PursuitState)�� ��ȯ
            stateMachine.ChangeState<EnemyPursuitState>();
            return;
        }
    }
}