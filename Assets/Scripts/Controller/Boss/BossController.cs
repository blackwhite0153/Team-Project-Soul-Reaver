using UnityEngine;

public class BossController : Base_Controller, IDamageable
{
    [Header("Type")]
    [SerializeField] protected ClassType ClassType;    // ���� ���� Ÿ��
    [SerializeField] protected TribeType TribeType;    // ���� ���� Ÿ��

    [Header("Stats")]
    [SerializeField] private EnemyStats _enemyClassStats;   // ���� Ŭ���� �� �ɷ�ġ
    [SerializeField] private EnemyStats _runtimeStats;      // �����ؼ� ����ϴ� ��Ÿ�� ���� Stats

    [Header("Component")]
    [SerializeField] private Animator _animator;     // ���� �ִϸ����� ������Ʈ
    [SerializeField] private Rigidbody _rigidbody;   // ���� ����(Rigidbody) ������Ʈ
    [SerializeField] private CapsuleCollider _capsuleCollider;       // ���� Collider ������Ʈ

    [Header("Sprite")]
    [SerializeField] private Transform _spriteObject;    // �� ��������Ʈ ������Ʈ

    [Header("Target")]
    [SerializeField] private GameObject _target;        // Ÿ���� �޾ƿ� ����
    [SerializeField] private LayerMask _targetLayer;    // Ÿ�� ���̾ ������ ����
    [SerializeField] private float _targetDistance;     // Ÿ�ٰ��� �Ÿ��� ��Ÿ�� ����

    [Header("State")]
    [SerializeField] private bool _isAttack;         // ���� ���� ����
    [SerializeField] private bool _isDamaged;        // �ǰ� ���� ����
    [SerializeField] private bool _isFacingRight;    // ���� ĳ���Ͱ� �������� ���� �ִ��� ����

    [Header("SpawnManager")]
    private SpawnManager _spawnManager;

    [Header("Enemy Type")]
    [SerializeField] private bool isBoss = true;
    public bool IsBoss => isBoss;

    // ���� �ӽ� (State Machine) ��ü, ���� ���¸� ����
    protected StateMachine<BossController> _stateMachine;

    public StateMachine<BossController> StateMachine => _stateMachine;

    public CapsuleCollider CapsuleCollider => _capsuleCollider;

    // Enemy ���� ������Ƽ
    public ClassType EnemyClassType => ClassType;
    public EnemyStats RuntimeStats => _runtimeStats;

    // ������Ʈ ������Ƽ
    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;

    // ��������Ʈ ������Ʈ ������Ƽ
    public Transform SpriteObject => _spriteObject;

    // Ÿ�� ������Ƽ
    public GameObject Target => _target;
    public LayerMask TargetLayer => _targetLayer;

    // Ÿ�ٱ������� �Ÿ�
    public float TargetDistance
    {
        get { return _targetDistance; }
        set { _targetDistance = value; }
    }

    // �� �ִϸ��̼��� �̵� ����
    public bool IsMove
    {
        get { return _animator.GetBool(Define.IsMove); }
        set { _animator.SetBool(Define.IsMove, value); }
    }

    // �� �ִϸ��̼��� ��� ����
    public bool IsDeath
    {
        get { return _animator.GetBool(Define.IsDeath); }
        set { _animator.SetBool(Define.IsDeath, value); }
    }

    // ���� ���� ����
    public bool IsAttack
    {
        get { return _isAttack; }
        set { _isAttack = value; }
    }

    // ���� �ǰ� ����
    public bool IsDamaged
    {
        get { return _isDamaged; }
        set { _isDamaged = value; }
    }

    // ���� �ٶ󺸴� ���� �Ǵ�
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set { _isFacingRight = value; }
    }

    protected override void Initialized() { }

    // ���� �ʱ�ȭ
    protected virtual void Setting()
    {
        // GameObject�� ������Ʈ ��������
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // ���� �ӽ� �ʱ�ȭ �� ���� �߰�
        _stateMachine = new StateMachine<BossController>(this, new BossIdleState());    // �⺻ ���¸� Idle�� ����
        _stateMachine.AddState(new BossPursuitState());   // ���� ���� �߰�
        _stateMachine.AddState(new BossAttackState());    // ���� ���� �߰�
        _stateMachine.AddState(new BossHitState());       // �ǰ� ���� �߰�
        _stateMachine.AddState(new BossDieState());       // ��� ���� �߰�

        // Transform ����
        transform.position = new Vector3(transform.position.x, 0.15f, transform.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(50.0f, 0.0f, 0.0f));
        transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);

        // Rigidbody ����
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;


        // Capsule Collider ����
        _capsuleCollider.center = new Vector3(0.0f, 0.33f, 0.0f);
        _capsuleCollider.radius = 0.3f;
        _capsuleCollider.height = 1f;
        _capsuleCollider.enabled = true;

        _targetLayer = LayerMask.GetMask(Define.Player_Layer);

        _isAttack = false;
        _isDamaged = false;
        _isFacingRight = false;

        DisableReaperCurse();
    }

    protected virtual void ResourceAllLoad()
    {
        switch (ClassType)
        {
            case ClassType.Boss:
                _enemyClassStats = Resources.Load<EnemyStats>(Define.Boss_Stat_Scriptable_Path);
                break;
        }
    }

    // �� ��������Ʈ ������Ʈ Ž��
    protected virtual void FindEnemyObject()
    {
        if (_spriteObject != null) return;

        if (this.gameObject.activeSelf)
        {
            _spriteObject = this.gameObject.transform.Find("Sprite")?.transform;
        }
    }

    // ���� ����� �� Ž�� �� �Ÿ� ���
    protected virtual void FindTargetObject()
    {
        _target = FindAnyObjectByType<PlayerController>().gameObject;

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

    public void InitializeStats(int wave)
    {
        _runtimeStats = _enemyClassStats.Clone();
        _runtimeStats.Health *= Mathf.Pow(1.2f, wave);
        _runtimeStats.AttackDamage *= Mathf.Pow(1.1f, wave);
    }

    private void DisableReaperCurse()
    {
        // Enemy ������ Debuff ������Ʈ ������ �ִ� Curse of the Reaper ������Ʈ ã��
        Transform curseTransform = transform.Find("Debuff/Curse of the Reaper");

        if (curseTransform != null)
        {
            curseTransform.gameObject.SetActive(false);
        }
    }


    public void SetSpawnManager(SpawnManager manager)
    {
        _spawnManager = manager;
    }

    public SpawnManager GetSpawnManager()
    {
        return _spawnManager;
    }

    public void TakeDamage(float damage, Transform attacker = null, bool isCritical = false)
    {
        if (IsDeath) return;

        _runtimeStats.Health -= damage;

        DamageTextManager.Instance.ShowDamageText(damage, this.gameObject, isCritical);

        if (_runtimeStats.Health <= 0)
        {
            IsDeath = true;
        }
        else
        {
            _isDamaged = true;
        }
    }
}