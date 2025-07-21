using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    // �÷��̾��� �پ��� ���� �ɷ�ġ ������
    public float Attack;
    public float Defense;
    public float MaxHp;
    public float CurrentHp;
    public float HpRegen;
    public float MaxMp;
    public float CurrentMp;
    public float MpRegen;
    public float AttackSpeed;
    public float CritChance;
    public float CritDamage;
    public float MoveSpeed;
    public float GoldGain;
    public float SkillDamage;

    private void Awake()
    {
        SetStartStat();
    }

    private void SetStartStat()
    {
        Attack = 10.0f;
        Defense = 10.0f;
        MaxHp = 100.0f;
        CurrentHp = MaxHp;
        HpRegen = 1.0f;
        MaxMp = 100.0f;
        CurrentMp = MaxMp;
        MpRegen = 1.0f;
        AttackSpeed = 1.0f;
        CritChance = 5.0f;
        CritDamage = 50.0f;
        MoveSpeed = 3.0f;
        GoldGain = 0.0f;
        SkillDamage = 0.0f;
    }

    // ���׷��̵�� StatData ����Ʈ�� �޾Ƽ� �÷��̾��� �ɷ�ġ�� �����ϴ� �޼���
    public void ApplyUpgradedStats(StatUpgradeManager upgradeManager, CharacterInventoryManager inventoryManager)
    {
        List<StatType> statTypes = new List<StatType>
    {
        StatType.Attack,
        StatType.Defense,
        StatType.MaxHp,
        StatType.HpRegen,
        StatType.MaxMp,
        StatType.MpRegen,
        StatType.AttackSpeed,
        StatType.CritChance,
        StatType.CritDamage,
        StatType.MoveSpeed,
        StatType.GoldGain,
        StatType.SkillDamage
    };

        foreach (var type in statTypes)
        {
            float baseValue = upgradeManager.GetBaseStat(type);
            float upgradeValue = upgradeManager.GetUpgradeValue(type);
            float equipmentBonus = inventoryManager.GetEquipmentBonus(type);
            float runeBonus = inventoryManager.GetRuneBonus(type);

            float finalValue = baseValue + upgradeValue + equipmentBonus + runeBonus;

            switch (type)
            {
                case StatType.Attack:
                    Attack = finalValue;
                    break;
                case StatType.Defense:
                    Defense = finalValue;
                    break;
                    case StatType.MaxHp:
                    // ��ȭ �� ü�� ���� ���
                    float oldMaxHp = MaxHp;
                    float hpRatio = (oldMaxHp > 0f) ? CurrentHp / oldMaxHp : 1f;
                    // MaxHp ����
                    MaxHp = finalValue;
                    // ���� �����Ͽ� CurrentHp ����
                    CurrentHp = MaxHp * hpRatio;
                    break;

                case StatType.HpRegen:
                    HpRegen = finalValue;
                    break;

                case StatType.MaxMp:
                    // ��ȭ �� ���� ���� ���
                    float oldMaxMp = MaxMp;
                    float mpRatio = (oldMaxMp > 0f) ? CurrentMp / oldMaxMp : 1f;
                    // MaxMp ����
                    MaxMp = finalValue;
                    // ���� �����Ͽ� CurrentMp ����
                    CurrentMp = MaxMp * mpRatio;
                    break;

                case StatType.MpRegen:
                    MpRegen = finalValue;
                    break;
                case StatType.AttackSpeed:
                    AttackSpeed = finalValue;
                    break;
                case StatType.CritChance:
                    CritChance = finalValue;
                    break;
                case StatType.CritDamage:
                    CritDamage = finalValue;
                    break;
                case StatType.MoveSpeed:
                    MoveSpeed = finalValue;
                    break;
                case StatType.GoldGain:
                    GoldGain = finalValue;
                    break;
                case StatType.SkillDamage:
                    SkillDamage = finalValue;
                    break;
            }
        }
    }
}
