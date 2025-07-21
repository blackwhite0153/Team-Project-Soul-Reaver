using UnityEngine;

public class Ground : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        Setting();
    }

    private void Setting()
    {
        // GameObject�� ������Ʈ ��������
        _rigidbody = GetComponent<Rigidbody>();

        // Rigidbody ����
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}