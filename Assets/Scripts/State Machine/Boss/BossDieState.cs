using System.Collections;
using UnityEngine;

public class BossDieState : State<BossController>
{
    private Coroutine _coDisableObject;

    public override void OnInitialized() {}

    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log($"<color=red>{context.name} State : {this.GetType().Name}</color>");

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsMove = false;
        context.IsAttack = false;
        context.IsDamaged = false;
        context.IsDeath = true;

        // CapsuleCollider ��Ȱ��ȭ
        context.CapsuleCollider.enabled = false;

        // ��� ��� Ʈ���� ����
        context.Animator.SetTrigger(Define.Death);

        if (_coDisableObject == null)
        {
            _coDisableObject = context.StartCoroutine(CoDisableAfterDeathAnimation());
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        // ���� ��Ȱ���� ���
        if (!context.IsDeath)
        {
            // ��� ���� (IdleState)�� ��ȯ
            stateMachine.ChangeState<BossIdleState>();
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        // �ִϸ��̼� �Ķ���� �� �ʱ�ȭ
        context.IsDeath = false;
    }

    private IEnumerator CoDisableAfterDeathAnimation()
    {
        yield return new WaitForSeconds(5.0f);

        var spawnManager = context.GetSpawnManager();
        if (spawnManager != null)
        {
            spawnManager.OnBossKilled();
        }
        else
        {
            Debug.LogWarning("SpawnManager ������ �����ϴ�.");
        }

        _coDisableObject = null;

        DropManager.Instance.DropFromEnemy(context.transform.position, context.IsBoss);

        PoolManager.Instance.DeactivateObj(context.gameObject);
    }
}