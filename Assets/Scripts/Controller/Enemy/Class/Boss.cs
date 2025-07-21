using UnityEngine;

public class Boss : BossController
{
    [Header("Warrior Specific")]
    [SerializeField] private float _hitOffset;
    [SerializeField] private float _hitRadius;

    [Header("Archer Specific")]
    [SerializeField] private GameObject _arrowProjectilePrefab;
    [SerializeField] private float _arrowProjectileSpeed;
    [SerializeField] private float _arrowFireOffset; // �߽ɿ��� ��¦ ��������

    [Header("Wizard Specific")]
    [SerializeField] private GameObject _magicProjectilePrefab;
    [SerializeField] private float _magicProjectileSpeed;
    [SerializeField] private float _magicFireOffset; // �߽ɿ��� ��¦ ��������

    // Warrior
    public float HitOffset => _hitOffset;
    public float HitRadius => _hitRadius;

    // Archer
    public GameObject ArrowProjectile => _arrowProjectilePrefab;
    public float ArrowProjectileSpeed => _arrowProjectileSpeed;
    public float ArrowFireOffset => _arrowFireOffset;

    // Wizard
    public GameObject MagicProjectile => _magicProjectilePrefab;
    public float MagicProjectileSpeed => _magicProjectileSpeed;
    public float MagicFireOffset => _magicFireOffset;

    protected override void Initialized()
    {
        base.Initialized();
    }

    private void OnEnable()
    {
        InitializeClass();
    }

    private void Update()
    {
        _stateMachine.OnUpdate(Time.deltaTime);

        FindEnemyObject();
        FindTargetObject();
    }

    private void FixedUpdate()
    {
        _stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    private void InitializeClass()
    {
        base.Setting();
        base.ResourceAllLoad();

        switch (ClassType)
        {
            case ClassType.Boss:
                break;
            case ClassType.Archer:
                InitializeArcher();
                break;
            case ClassType.Warrior:
                InitializeWarrior();
                break;
            case ClassType.Wizard:
                InitializeWizard();
                break;
        }

        // wave ���� ���� �ɷ�ġ �ʱ�ȭ
        InitializeStats(1);

        Debug.Log($"<color=blue>{this.name} ���� ���� �Ϸ� [Class: {ClassType}, ���� �ӽ� : {_stateMachine}]</color>");
    }

    private void InitializeWarrior()
    {
        _hitOffset = 0.8f;
        _hitRadius = 10.0f;
    }

    private void InitializeArcher()
    {
        switch (TribeType)
        {
            case TribeType.Devil:
                _arrowProjectilePrefab = Resources.Load<GameObject>(Define.Devil_Arrow_Prefab_Path);
                break;
            case TribeType.Elf:
                _arrowProjectilePrefab = Resources.Load<GameObject>(Define.Elf_Arrow_Prefab_Path);
                break;
            case TribeType.Human:
                _arrowProjectilePrefab = Resources.Load<GameObject>(Define.Human_Arrow_Prefab_Path);
                break;
            case TribeType.Skelton:
                _arrowProjectilePrefab = Resources.Load<GameObject>(Define.Skeleton_Arrow_Prefab_Path);
                break;
        }

        _arrowProjectileSpeed = 15.0f;
        _arrowFireOffset = 0.5f;
    }

    private void InitializeWizard()
    {
        switch (TribeType)
        {
            case TribeType.Devil:
                _magicProjectilePrefab = Resources.Load<GameObject>(Define.Blue_Fire_Ball_Prefabs_Path);
                break;
            case TribeType.Elf:
                _magicProjectilePrefab = Resources.Load<GameObject>(Define.Red_Fire_Ball_Prefab_Path);
                break;
            case TribeType.Human:
                _magicProjectilePrefab = Resources.Load<GameObject>(Define.Red_Fire_Ball_Prefab_Path);
                break;
            case TribeType.Skelton:
                _magicProjectilePrefab = Resources.Load<GameObject>(Define.Green_Fire_Ball_Prefab_Path);
                break;
        }

        _magicProjectileSpeed = 15.0f;
        _magicFireOffset = 0.5f;
    }
}