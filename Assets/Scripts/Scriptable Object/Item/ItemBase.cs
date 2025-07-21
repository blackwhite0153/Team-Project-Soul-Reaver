using UnityEngine;
public enum Grade { Legendary, Rare, Uncommon, Common}
public enum EquipmentType { Weapon, Top, Bottom, Gloves, Shoes}

public abstract class ItemBase : ScriptableObject
{
    public Sprite ItemImage;        // �̹���
    public string ID;               // ���� ���̵�
    public string Name;             // �̸�
    public string Explanation;      // ����
    public Grade grade;             // ��� (���߿� ��ũ ��� �̰ɷ� �����)

    public bool IsUnlocked = false;  // �⺻ ��� ����

    public abstract EquipmentType EquipType { get; }

 }
