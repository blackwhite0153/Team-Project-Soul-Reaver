using UnityEngine;

[CreateAssetMenu(fileName = "NewAromorBottomData", menuName = "Item/ArmorBottom")]

public class ArmorBottomDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Bottom; // Ÿ�� ����

    public float Defense;               // ����      (%)
    public float Hp;                    // ü��        (%)
    public float Mp;                    // ����        (%)
}
