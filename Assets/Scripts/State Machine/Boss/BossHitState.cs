using System.Collections;
using UnityEngine;

public class BossHitState : State<BossController>
{
    private Coroutine _coHitAnimDelay;

    public override void OnInitialized()
    {
        _coHitAnimDelay = null;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log($"<color=red>{context.name} State : {this.GetType().Name}</color>");

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = true;
        context.IsDeath = false;

        // �ǰ� ��� Ʈ���� ����
        context.Animator.SetTrigger(Define.Damaged);

        if (_coHitAnimDelay != null)
            return;

        if (_coHitAnimDelay == null)
        {
            _coHitAnimDelay = context.StartCoroutine(CoHitDelay());
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_coHitAnimDelay != null)
        {
            context.StopCoroutine(_coHitAnimDelay);
            _coHitAnimDelay = null;
        }

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsDamaged = false;
    }

    // �ǰ� �� �ִϸ��̼� ���̸�ŭ ������
    private IEnumerator CoHitDelay()
    {
        yield return null;  // �ִϸ��̼� ���� ��� 1������

        AnimatorStateInfo stateInfo = context.Animator.GetCurrentAnimatorStateInfo(0);
        float hitAnimLength = stateInfo.length;

        yield return new WaitForSeconds(hitAnimLength);
        context.IsDamaged = false;
        _coHitAnimDelay = null;

        // ���� ���� ó��
        if (context.IsDeath)
        {
            stateMachine.ChangeState<BossDieState>();
        }
        else
        {
            stateMachine.ChangeState<BossIdleState>();
        }
    }
}