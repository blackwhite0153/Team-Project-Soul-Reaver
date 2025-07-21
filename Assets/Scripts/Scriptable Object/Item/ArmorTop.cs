using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorTopData", menuName = "Item/ArmorTop")]

public class ArmorTopDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Top; // Ÿ�� ����

    public float Defense;               // ����      (%)
    public float HpRegen;               // ü�����    (%)
    public float MpRegen;               // �������    (%)
}
