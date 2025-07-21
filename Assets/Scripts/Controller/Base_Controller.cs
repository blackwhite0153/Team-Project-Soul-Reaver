using UnityEngine;

// ��Ʈ�ѷ��� �⺻ ������ �����ϴ� �߻� Ŭ����
public abstract class Base_Controller : MonoBehaviour
{
    // MonoBehaviour�� Awake()�� �������̵��Ͽ� �ʱ�ȭ �Լ� ȣ��
    private void Awake()
    {
        Initialized();
    }

    // �� ��Ʈ�ѷ����� �ݵ�� �����ؾ� �ϴ� �ʱ�ȭ �Լ�
    protected abstract void Initialized();
}