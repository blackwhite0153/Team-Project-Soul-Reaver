using System.Collections;
using UnityEngine;

public abstract class Base_Projectile : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private float _lifeTime;

    protected Vector3 MoveDirection;

    [SerializeField] protected float Speed;

    protected virtual void Setting()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = false;

        _lifeTime = 3.0f;
    }

    protected void ProjectileMove()
    {
        if (MoveDirection != Vector3.zero && Speed > 0.0f)
        {
            transform.Translate(MoveDirection * Speed * Time.fixedDeltaTime, Space.World);
        }
    }

    protected virtual void AutoDeactivate(GameObject gameObject)
    {
        StartCoroutine(CoAutoDeactivate(gameObject));
    }

    private IEnumerator CoAutoDeactivate(GameObject gameObject)
    {
        yield return new WaitForSeconds(_lifeTime);

        // Ǯ �Ŵ����� ���� ��Ȱ��ȭ
        PoolManager.Instance.DeactivateObj(gameObject);
    }

    // �ܺο��� �߻�ü�� �ӵ��� ����
    protected virtual void SetSpeed(float speed)
    {
        Speed = speed;

        if (MoveDirection != Vector3.zero)
        {
            GetComponent<Rigidbody>().MovePosition(transform.position + MoveDirection * Speed * Time.fixedDeltaTime);
        }
    }
}