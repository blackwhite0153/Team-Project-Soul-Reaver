using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Attack,
    Buff,
    Heal,
    Debuff
}

public enum DetectionType
{
    Trigger,
    Collision
}

[System.Serializable]
public class SkillSaveData
{
    public List<SkillData> skills = new List<SkillData>();
}

[System.Serializable]
public class SkillData
{
    public int skillLevel;
    public string skillName;
    public bool isFirstPlay;
}

public abstract class Skill_Base : ScriptableObject
{
    [Header("�⺻ ����")]
    public int SlotIndex;                   // �Ҵ�� ���� ��ġ
    public string Skill_ID;                 // ���� ���̵�
    public Sprite Skill_Icon;               // ��ų ������
    public string Skill_Name;               // ��ų �̸�

    [TextArea]
    public string Skill_Explanation;        // ����

    [Header("����")]
    public SkillType Type;                  // ��ų Ÿ��
    public int Skill_Level;                 // ��ų ����
    public bool IsUnlocked;                 // ��� ����

    [Header("��Ÿ��")]
    public float CooldownTime;              // ���� ��� �ð�
    public float CurrentCooldown;           // ���� ��ٿ� ���� (������)

    [Header("���� �ð�")]
    public float Duration;                  // ȿ�� ���� �ð� (��)
    public float RemainingDuration;         // ���� ȿ�� ���� �ð� (��)

    [Header("�ڿ�")]
    public float ManaCost;                  // ���� �Ǵ� �ڿ� �Ҹ�

    [Header("��ȭ ����")]
    public int GoldCost;                    // ��� ��ȭ ���
    public float DamageScalingPerLevel;     // ������ ����
    public float RegenScalingPerLevel;
    public float BuffOneScalingPerLevel;    // ������ ���� 1 ����
    public float BuffTwoScalingPerLevel;    // ������ ���� 2 ����

    public bool IsFirstPlay = true;

    public abstract void Setting();         // ��ų ����
    public abstract void ConnectResource(); // ��ų ����
    public abstract void UseSkill(int index);   // ��ų ���

    public virtual void OnCoolDownEnd() { }  // ��ų ��Ÿ�� ����
    public virtual void OnDurationEnd() { }  // ��ų ���� �ð� ����

    public virtual void RefreshExplanation() { }    // ���� �ٽ� ���

    public virtual void ResetSkill() { }  // ��ų ����
}