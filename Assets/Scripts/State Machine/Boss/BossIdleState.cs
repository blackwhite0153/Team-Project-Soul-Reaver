using UnityEngine;

public class BossIdleState : State<BossController>
{
    public override void OnInitialized() { }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log($"<color=red>{context.name} State : {this.GetType().Name}</color>");

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
        // Ÿ���� �ִ� ���
        else if (context.Target != null)
        {
            // ���� ���� (PursuitState)�� ��ȯ
            stateMachine.ChangeState<BossPursuitState>();
            return;
        }
    }
}