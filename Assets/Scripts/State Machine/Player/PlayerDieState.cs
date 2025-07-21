
// �÷��̾��� ��� ���¸� �����ϴ� PlayerDieState Ŭ���� (PlayerController�� ����)
using UnityEngine;

public class PlayerDieState : State<PlayerController>
{
    public override void OnInitialized(){ }

    public override void OnEnter()
    {
        base.OnEnter();

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = true;

        // ��� ��� Ʈ���� ����
        context.Animator.SetTrigger(Define.Death);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // ���� ��Ȱ���� ���
        if (!context.IsDeath)
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
        context.IsDeath = false;
    }
}