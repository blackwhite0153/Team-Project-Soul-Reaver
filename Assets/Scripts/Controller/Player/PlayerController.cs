using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Base_Controller, IDamageable
{
    private PlayerStats _player;        // �÷��̾��� ����

    private Transform _spriteObject;    // �÷��̾� ��������Ʈ ������Ʈ

    private Animator _animator;     // �÷��̾��� �ִϸ����� ������Ʈ
    private Rigidbody _rigidbody;   // �÷��̾��� ����(Rigidbody) ������Ʈ
    private CapsuleCollider _capsuleCollider;       // �÷��̾��� Collider ������Ʈ

    [SerializeField] private GameObject _target;    // ���� ����� ���� �޾ƿ� ����
    [SerializeField] private List<GameObject> _randomTargets;   // ���� ����� ���� �޾ƿ� ����

    [SerializeField] private float _radius;
    private float _targetDistance;

    private bool _isAttack;         // ���� ���� ����
    private bool _isDamaged;        // �ǰ� ���� ����
    private bool _isFacingRight;    // ���� ĳ���Ͱ� �������� ���� �ִ��� ����

    // ���� �ӽ� (State Machine) ��ü, �÷��̾��� ���¸� ����
    protected StateMachine<PlayerController> _stateMachine;

    public StateMachine<PlayerController> StateMachine => _stateMachine;

    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;
    public Transform SpriteObject => _spriteObject;

    public GameObject Target => _target;
    public List<GameObject> RandomTargets => _randomTargets;

    // Ÿ�ٱ������� �Ÿ�
    public float TargetDistance
    {
        get { return _targetDistance; }
        set { _targetDistance = value; }
    }

    // �÷��̾� �ִϸ��̼��� �̵� ����
    public bool IsMove
    {
        get { return _animator.GetBool(Define.IsMove); }
        set { _animator.SetBool(Define.IsMove, value); }
    }

    // �÷��̾� �ִϸ��̼��� ��� ����
    public bool IsDeath
    {
        get { return _animator.GetBool(Define.IsDeath); }
        set { _animator.SetBool(Define.IsDeath, value); }
    }

    // �÷��̾��� ���� ����
    public bool IsAttack
    {
        get { return _isAttack; }
        set { _isAttack = value; }
    }

    // �÷��̾��� �ǰ� ����
    public bool IsDamaged
    {
        get { return _isDamaged; }
        set { _isDamaged = value; }
    }

    // �÷��̾ �ٶ󺸴� ���� �Ǵ�
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set { _isFacingRight = value; }
    }

    protected override void Initialized()
    {
        Setting();
    }

    private void Start()
    {
        DisableAllBuffs();
    }

    private void FixedUpdate()
    {
        // ���� ������Ʈ���� ���� �ӽ� ������Ʈ ����
        _stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    private void Update()
    {
        // �� ������ ���� �ӽ� ������Ʈ ����
        _stateMachine.OnUpdate(Time.deltaTime);

        FindPlayerObject();
        FindTargetObject();
        FindRandomTargetObject();
    }

    // ���� �ʱ�ȭ
    private void Setting()
    {
        // Player ���� ��������
        _player = GetComponent<PlayerStats>();

        // GameObject�� ������Ʈ ��������
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // ���� �ӽ� �ʱ�ȭ �� ���� �߰�
        _stateMachine = new StateMachine<PlayerController>(this, new PlayerIdleState());    // �⺻ ���¸� Idle�� ����
        _stateMachine.AddState(new PlayerMoveState());      // �̵� ���� �߰�
        _stateMachine.AddState(new PlayerPursuitState());   // ���� ���� �߰�
        _stateMachine.AddState(new PlayerAttackState());    // ���� ���� �߰�
        /*_stateMachine.AddState(new PlayerHitState());*/       // �ǰ� ���� �߰�
        _stateMachine.AddState(new PlayerDieState());       // ��� ���� �߰�

        // Transform ����
        transform.position = new Vector3(0.0f, 0.15f, 0.0f);
        transform.rotation = Quaternion.Euler(new Vector3(50.0f, 0.0f, 0.0f));
        transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);

        // Rigidbody ����
        //_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _rigidbody.mass = 100.0f;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        // Capsule Collider ����
        _capsuleCollider.center = new Vector3(0.0f, 0.44f, -0.1f);
        _capsuleCollider.radius = 0.4f;
        _capsuleCollider.height = 0.5f;

        _targetDistance = 0.0f;

        _radius = 10.0f;
        _targetDistance = 0.0f;

        _isAttack = false;
        _isDamaged = false;

        // ���� HP �ʱ�ȭ
        PlayerStats.Instance.CurrentHp = PlayerStats.Instance.MaxHp;
    }

    // ù ���� �� ���� ���� ��Ȱ��ȭ
    private void DisableAllBuffs()
    {
        string[] buffNames = { "Blessing of Regeneration", "Blessing of the Goddess", "Lightning Armament" };

        foreach (var buffName in buffNames)
        {
            Transform buffTransform = transform.Find($"Buff/{buffName}");

            if (buffTransform != null)
            {
                buffTransform.gameObject.SetActive(false);
            }
        }
    }

    // �÷��̾� ������Ʈ Ž��
    private void FindPlayerObject()
    {
        if (_spriteObject != null) return;

        var player = FindAnyObjectByType<PlayerController>();

        if (player != null)
        {
            _spriteObject = player.transform.Find("Sprite")?.transform;

            // ĳ������ ���� ���� (Local Scale ����)���� _isFacingRight ����
            _isFacingRight = _spriteObject.localScale.x < 0.0f; ;
        }
    }

    // ���� ����� �� Ž�� �� �Ÿ� ���
    private void FindTargetObject()
    {
        _target = gameObject.GetNearestTarget(Define.Enemy_Layer, _radius);

        if (_target != null)
        {
            _targetDistance = Vector3.Distance(new Vector3(_target.transform.position.x, 0.0f, _target.transform.position.z),
                                               new Vector3(transform.position.x, 0.0f, transform.position.z));
        }
        else
        {
            _targetDistance = float.MaxValue;
        }
    }

    // ���� �� �� ���� ����
    private void FindRandomTargetObject()
    {
        _randomTargets = gameObject.GetRandomTarget(Define.Enemy_Layer, _radius);
    }


    // ��������Ʈ ���� ó��
    private void SetFacing(bool shouldFaceRight)
    {
        if (IsFacingRight == shouldFaceRight) return;

        IsFacingRight = shouldFaceRight;

        Vector3 scale = SpriteObject.localScale;
        scale.x = Mathf.Abs(scale.x) * (shouldFaceRight ? -1 : 1);
        SpriteObject.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ������ü(MagicProjectile)�� �浹
        if (other.CompareTag(Define.MagicProjectile_Tag))
        {
            MagicProjectile magic = other.GetComponent<MagicProjectile>();

            TakeDamage(magic.Damage, magic.transform);
        }
        // ȭ��(ArrowProjectile)�� �浹
        else if (other.CompareTag(Define.ArrowProjectile_Tag))
        {
            ArrowProjectile arrow = other.GetComponent<ArrowProjectile>();

            TakeDamage(arrow.Damage, arrow.transform);
        }
    }

    public void PerformAttackDamage()
    {
     
        // 1) ���� Ÿ���� ���ų�, ���� �ٱ��̸� �ƹ��͵� ���� ����
        if (_target == null)
            return;

        // 2) Ÿ�ٰ��� �Ÿ��� ��Ȯ��
        float dist = Vector3.Distance(
            new Vector3(_target.transform.position.x, 0f, _target.transform.position.z),
            new Vector3(transform.position.x, 0f, transform.position.z)
        );
        if (dist > _radius)
            return;

        // 3) IDamageable�� ������ ������ ������ ����
        if (_target.TryGetComponent<IDamageable>(out var enemy))
        {
            // 3-1) ġ��Ÿ ���� ����
            bool isCritical = Random.value < (PlayerStats.Instance.CritChance / 100f);
            float baseDamage = PlayerStats.Instance.Attack;
            float finalDamage = baseDamage;

            // 3-2) ġ��Ÿ�� ��� ���� ������ ���
            if (isCritical)
            {
                finalDamage = baseDamage * (1 + PlayerStats.Instance.CritDamage / 100f);
            }
            else
            {
                finalDamage = baseDamage;
            }

            // 3-3) ���� TakeDamage �Լ��� ���� �������� ġ��Ÿ ���θ� �Բ� ����
            enemy.TakeDamage(finalDamage, transform, isCritical);
        }
    }

    // �ڵ� �̵� �� ���� ó��
    public void UpdateFacingByTarget()
    {
        if (_target == null) return;

        bool shouldFaceRight = _target.transform.position.x > transform.position.x;
        SetFacing(shouldFaceRight);
    }

    // ���� �̵� �� ���� ó��
    public void UpdateFacingByInput(Vector3 moveDirection)
    {
        if (moveDirection.x == 0) return;

        bool shouldFaceRight = moveDirection.x > 0;
        SetFacing(shouldFaceRight);
    }

    // �ǰ� �� ȣ��
    public void TakeDamage(float damage, Transform attacker = null, bool isCritical = false)
    {
        if (IsDamaged || IsDeath) return;

        // ���� ����: �ּ� 1 ������ ����
        float damageAfterDef = damage - PlayerStats.Instance.Defense;
        damageAfterDef = Mathf.Max(damageAfterDef, 1f);

        // ü�� ����
        PlayerStats.Instance.CurrentHp -= damageAfterDef;
        PlayerStats.Instance.CurrentHp = Mathf.Max(PlayerStats.Instance.CurrentHp, 0f);

        if (PlayerStats.Instance.CurrentHp <= 0 && !IsDeath)
        {
            StateMachine.ChangeState<PlayerDieState>();
        }
        _isDamaged = true;
    }

    // �÷��̾� ������
    public void ReSpawnPlayer()
    {
        IsDeath = false;
        Setting();
        PlayerStats.Instance.CurrentHp = PlayerStats.Instance.MaxHp;
        PlayerStats.Instance.CurrentMp = PlayerStats.Instance.MaxMp;

        StateMachine.ChangeState<PlayerIdleState>();
    }
}