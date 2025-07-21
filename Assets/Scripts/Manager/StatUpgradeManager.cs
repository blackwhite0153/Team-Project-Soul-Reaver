using BackEnd;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum StatType
{
    Attack, Defense, HpRegen, MaxHp, CurrentHp,
    MaxMp, CurrentMp, MpRegen,
    AttackSpeed, CritChance, CritDamage,
    MoveSpeed, GoldGain, SkillDamage
}

// ���� ���� �����Ϳ� ��� ������ ������ Ŭ����
[Serializable]
public class StatData
{
    public StatType statType;           // ���� ����
    public float baseValue;             // �⺻�� (�ʱ� ����)
    public float increasePerUpgrade;    // ��ȭ 1�ܰ� �� �����ϴ� ��
    public float maxUpgradeValue;       // �ִ� ��ȭ ���� ��ġ (���� ���׷��̵尡 �̰� ���� �� ����)
    public float currentUpgradeValue;   // ���� ��ȭ�� ��
    public bool IsPercent;              // �� ������ % �������� ����

    // % ���� ������ ��, ���� ���׷��̵�� ��ġ�� �ۼ�Ʈ�� ��ȯ (�ƴ� ��� 0)
    public float TotalPercent => IsPercent ? currentUpgradeValue : 0;

    // �⺻�� + ���� ��ȭ���� �ջ��� ���� �� ��ȯ
    public float TotalValue => baseValue + currentUpgradeValue;

    // ������: ���� Ÿ��, �⺻��, �ܰ�� ������, �ִ� ��ȭ��, % ���� ���� ����
    public StatData(StatType type, float baseVal, float perUpgrade, float maxVal, bool isPercent)
    {
        statType = type;
        baseValue = baseVal;
        increasePerUpgrade = perUpgrade;
        maxUpgradeValue = maxVal;
        currentUpgradeValue = 0.0f;
        IsPercent = isPercent;
    }

    // ��ȭ ���� ���� üũ (���� ��ȭ���� �ִ�ġ���� ������)
    public bool CanUpgrade() => currentUpgradeValue < maxUpgradeValue;

    // ��ȭ ���� (�ִ�ġ�� ���� �ʵ��� ����)
    public void Upgrade()
    {
        if (!CanUpgrade()) return;

        currentUpgradeValue += increasePerUpgrade;

        if (currentUpgradeValue > maxUpgradeValue)
            currentUpgradeValue = maxUpgradeValue;
    }

    // ������ ��ȯ (�⺻�� + ��ȭ��)
    public float GetValue()
    {
        return baseValue + currentUpgradeValue;
    }

    // ���� ��ȭ�� ��ġ�� ��ȯ (�⺻�� ����)
    public float GetAddvalue()
    {
        return currentUpgradeValue;
    }
}

// ���� ��ȭ �ý��� ������ Ŭ����
public class StatUpgradeManager : Singleton<StatUpgradeManager>
{
    public List<StatData> stats = new List<StatData>();  // ��� ���� ������ ����Ʈ

    // UI ���� �ؽ�Ʈ�� ��ư �迭
    public TextMeshProUGUI[] statValueTexts;          // �� ������ ���� �� �ؽ�Ʈ
    public TextMeshProUGUI[] statEnhancementTexts;    // �� ������ ��ȭ�� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI[] upgradeCostTexts;        // �� ���� ��ȭ ��� �ؽ�Ʈ
    public Button[] upgradeButtons;                     // �� ���� ��ȭ ��ư

    public int[] upgradeCosts;              // �� ���Ⱥ� ���� ��ȭ ���
    public float upgradeCooldown = 0.3f;    // ��Ÿ ������ ��ٿ� (0.3��)
    private float[] lastUpgradeTimes;       // �� ���Ⱥ� ������ ��ȭ �ð� ����

    public TMP_Text playerGoldText;         // ��� UI �ؽ�Ʈ

    public PlayerStats playerStats;         // �÷��̾� ���� ���ȿ� ������ ��ü

    private string statDataRowInDate = string.Empty;    // ���� Ű��

    private void Awake()
    {
        InitStats();    // ���� ����Ʈ �ʱ�ȭ
    }

    private void Start()
    {
        int statCount = stats.Count;
        upgradeCosts = new int[statCount];          // ��ȭ ��� �迭 �ʱ�ȭ
        lastUpgradeTimes = new float[statCount];    // ������ ��ȭ �ð� �迭 �ʱ�ȭ

        for (int i = 0; i < statCount; i++)
        {
            upgradeCosts[i] = 100;      // �� ���� �ʱ� ��ȭ ��� 100���� ����
            lastUpgradeTimes[i] = 0.0f;
        }

        LoadStatsFromBackend();
        UpdateUI();   // UI ����
    }

    private void Update()
    {
        playerGoldText.text = GameManager.Instance.GoldNum.ToString();
    }

    // �ʱ� ���� ������ ���� (�⺻��, ��ȭ��, �ִ�ġ, % ���� ����)
    private void InitStats()
    {
        stats = new List<StatData>
        {
            new StatData(StatType.Attack, 10, 2, 1000000, false),
            new StatData(StatType.Defense, 10, 2,  50000, false),
            new StatData(StatType.MaxHp, 100, 10, 100000000, false),
            new StatData(StatType.HpRegen, 1, 0.5f, 500f, true),
            new StatData(StatType.AttackSpeed, 1.0f, 0.2f, 50.0f, true),
            new StatData(StatType.CritChance, 5f, 0.1f, 100f, true),
            new StatData(StatType.CritDamage, 50f, 1f, 500f, true),
            new StatData(StatType.MoveSpeed, 3, 0.5f, 200f, true),
            new StatData(StatType.MaxMp, 100, 5, 50000, false),
            new StatData(StatType.MpRegen, 1, 0.2f, 200f, true),
            new StatData(StatType.GoldGain, 0, 1f, 1000f, true),
            new StatData(StatType.SkillDamage, 0, 2f, 500f, true)
        };
    }

    // ��ȭ ��ư Ŭ�� �� ����Ǵ� �Լ�
    public void TryUpgradeStat(int index)
    {
        SoundManager.Instance.PlaySFX("Button");

        if (index < 0 || index >= stats.Count) return;

        var stat = stats[index];

        if (!stat.CanUpgrade()) return;     // �ִ�ġ�� ����

        // ��Ÿ�� ���� Ŭ�� ����
        if (Time.time - lastUpgradeTimes[index] < upgradeCooldown)
            return;

        // ��尡 ����� ��� ��ȭ ����
        if (GameManager.Instance.GoldNum >= upgradeCosts[index])
        {
            GameManager.Instance.LoseMoney(upgradeCosts[index]);
            stat.Upgrade();                         // ���� ��ȭ
            upgradeCosts[index] += 50;              // ��ȭ ��� ���
            lastUpgradeTimes[index] = Time.time;    // ������ ��ȭ �ð� ���
            UpdateStatsToBackend();                 // ��ȭ �� ����(����)
            //SaveStats();                          // ���� ��� (�ʿ� �� Ȱ��ȭ)
        }
        else
        {
            // ��� ���� �� ��� �ؽ�Ʈ�� ���������� ǥ��
            upgradeCostTexts[index].color = Color.red;
            statValueTexts[index].color = Color.red;
            Invoke(nameof(ResetTextColors), 0.5f);  // 0.5�� �� �� ����
        }

        UpdateUI();  // ��ȭ �� UI ����
    }

    // �ؽ�Ʈ ������ ������� �ǵ����� �Լ�
    private void ResetTextColors()
    {
        foreach (var t in upgradeCostTexts) t.color = Color.white;
        foreach (var t in statValueTexts) t.color = Color.white;
        foreach (var t in statEnhancementTexts) t.color = Color.white;
    }

    public float GetBaseStat(StatType statType)
    {
        var stat = stats.Find(s => s.statType == statType);
        return stat != null ? stat.baseValue : 0f;
    }

    public float GetUpgradeValue(StatType statType)
    {
        var stat = stats.Find(s => s.statType == statType);
        return stat != null ? stat.currentUpgradeValue : 0f;
    }


    // UI ��ü ���� ó�� �Լ�
    public void UpdateUI()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            var stat = stats[i];
            StatType type = stat.statType;

            float final = StatUpgradeManager.Instance.GetFinalStat(type);
            float baseValue = stat.baseValue;
            float upgradeOnlyValue = stat.currentUpgradeValue;
            float enhancementBonus = final - baseValue;

            // �ۼ�Ʈ ���� UI
            if (stat.IsPercent)
            {
                statValueTexts[i].text = $"{final:F1}% (+{enhancementBonus:F2}%)";
            }
            else // ���� ���� UI
            {
                statValueTexts[i].text = $"{final:F1} (+{enhancementBonus:F2})";
            }

            // ��ȭ���� ���� ǥ�� (����ó�� currentUpgradeValue��)
            if (statEnhancementTexts.Length > i)
            {
                statEnhancementTexts[i].text = stat.IsPercent
                    ? $"{upgradeOnlyValue:F1}%"
                    : $"{upgradeOnlyValue:F0}";
            }

            // ��ȭ ���
            if (upgradeCostTexts.Length > i)
            {
                upgradeCostTexts[i].text = stat.CanUpgrade() ? $"{upgradeCosts[i]}G" : "MAX";
            }

            // ��ư Ȱ��ȭ ����
            if (upgradeButtons.Length > i)
            {
                upgradeButtons[i].interactable = stat.CanUpgrade();
            }
        }

        // ��� UI
        if (playerGoldText != null)
            playerGoldText.text = $"{GameManager.Instance.GoldNum}G";

        // ���� ���� ����
        if (playerStats != null)
            playerStats.ApplyUpgradedStats(StatUpgradeManager.Instance, CharacterInventoryManager.Instance);
    }

    // ���� + ��� + �齺�� �޾ƿ��� �Լ�
    public float GetFinalStat(StatType type)
    {
        var stat = stats.Find(s => s.statType == type);
        if (stat == null) return 0.0f;

        float upgradedValue = stat.TotalValue;
        float equipmentBonus = 0f;
        var inv = CharacterInventoryManager.Instance;

        switch (type)
        {
            case StatType.Attack:
                equipmentBonus += upgradedValue * (inv.WeaponAtkBonus * 0.01f);
                equipmentBonus += upgradedValue * (inv.AtkDamageBonus * 0.01f);
                break;
            case StatType.SkillDamage:
                equipmentBonus += upgradedValue * (inv.WeaponSkillBonus * 0.01f);
                equipmentBonus += upgradedValue * (inv.SkillDamageBonus * 0.01f);
                break;
            case StatType.Defense:
                equipmentBonus += upgradedValue * (inv.TopDefenseBonus + inv.BottomDefenseBonus) * 0.01f;
                equipmentBonus += upgradedValue * (inv.DefenseBonus * 0.01f);
                break;
            case StatType.HpRegen:
                equipmentBonus += upgradedValue * (inv.TopHpRegenBonus + inv.HealthRepBonus) * 0.01f;
                break;
            case StatType.MpRegen:
                equipmentBonus += upgradedValue * (inv.TopMpRegenBonus + inv.ManaRepBonus) * 0.01f;
                break;
            case StatType.MaxHp:
                equipmentBonus += upgradedValue * (inv.BottomHpBonus + inv.HealthBouns) * 0.01f;
                break;
            case StatType.MaxMp:
                equipmentBonus += upgradedValue * (inv.BottomMpBonus + inv.ManaBouns) * 0.01f;
                break;
            case StatType.MoveSpeed:
                equipmentBonus += upgradedValue * (inv.ShoesMoveBonus + inv.MoveSpeedBonus) * 0.01f;
                break;
            case StatType.AttackSpeed:
                equipmentBonus += upgradedValue * (inv.GlovesAttackSpeedBonus + inv.AtkSpeedBonus) * 0.01f;
                break;
            case StatType.CritChance:
                equipmentBonus += upgradedValue * (inv.CriChanceBonus * 0.01f);
                break;
            case StatType.CritDamage:
                equipmentBonus += upgradedValue * (inv.CriDamageBonus * 0.01f);
                break;
            case StatType.GoldGain:
                equipmentBonus += upgradedValue * (inv.GoldBonus * 0.01f);
                break;
        }

        float buffBonus = 0f;
        var matchingBuff = GetMatchingBuffType(type);
        if (matchingBuff.HasValue)
        {
            float buffPercent = BuffManager.Instance.GetBuffBonus(matchingBuff.Value);
            buffBonus = upgradedValue * (buffPercent * 0.01f);
        }
        return upgradedValue + equipmentBonus + buffBonus;
    }

    private BuffType? GetMatchingBuffType(StatType statType)
    {
        switch (statType)
        {
            case StatType.Attack: return BuffType.AttackUp;
            case StatType.Defense: return BuffType.DefenseUp;
            case StatType.MoveSpeed: return BuffType.MoveSpeed;
            case StatType.CritChance: return BuffType.CritChance;
            // �ʿ��ϸ� �߰�
            default: return null;
        }
    }



    // ����
    public void SaveStatsToBackend()
    {
        StatSaveData saveData = new StatSaveData();

        foreach (var stat in stats)
            saveData.currentUpgradeValues.Add(stat.currentUpgradeValue);
        foreach (var cost in upgradeCosts)
            saveData.upgradeCosts.Add(cost);

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ���� (Insert): " + bro);
            statDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("���� ���� ���� (Insert): " + bro);
        }
    }

    // ����
    public void UpdateStatsToBackend()
    {
        if (string.IsNullOrEmpty(statDataRowInDate))
        {
            Debug.LogWarning("������ �����Ͱ� �����ϴ�. ���� SaveStatsToBackend()�� ȣ���ϼ���.");

            return;
        }

        StatSaveData saveData = new StatSaveData();

        foreach (var stat in stats)
            saveData.currentUpgradeValues.Add(stat.currentUpgradeValue);
        foreach (var cost in upgradeCosts)
            saveData.upgradeCosts.Add(cost);

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2("USER_DATA", statDataRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ���� (Update): " + bro);
        }
        else
        {
            Debug.LogError("���� ���� ���� (Update): " + bro);
        }
    }

    // �ҷ�����
    public void LoadStatsFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (bro.IsSuccess())
        {
            var rows = bro.FlattenRows();

            if (rows.Count <= 0)
            {
                Debug.LogWarning("����� ���� �����Ͱ� �����ϴ�.");
                return;
            }

            string json = rows[0]["StatData"].ToString();
            statDataRowInDate = rows[0]["inDate"].ToString();
            StatSaveData saveData = JsonUtility.FromJson<StatSaveData>(json);

            for (int i = 0; i < stats.Count; i++)
                if (i < saveData.currentUpgradeValues.Count)
                { 
                    stats[i].currentUpgradeValue = saveData.currentUpgradeValues[i];
                    Debug.Log($"[�ε�] {stats[i].statType} = {stats[i].currentUpgradeValue}");
                }

            for (int i = 0; i < upgradeCosts.Length; i++)
                if (i < saveData.upgradeCosts.Count)
                    upgradeCosts[i] = saveData.upgradeCosts[i];

            UpdateUI();

            Debug.Log("���� �ҷ����� ����");
        }
        else
        {
            Debug.LogError("���� �ҷ����� ����: " + bro);
        }
    }
}

[Serializable]
public class StatSaveData
{
    public List<float> currentUpgradeValues = new List<float>();
    public List<int> upgradeCosts = new List<int>();
}