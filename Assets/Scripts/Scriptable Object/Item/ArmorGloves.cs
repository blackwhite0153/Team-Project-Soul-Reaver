using UnityEngine;

[CreateAssetMenu(fileName = "NewAromorGlovesData", menuName = "Item/ArmorGloves")]

public class ArmorGlovesDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Gloves; // Ÿ�� �尩

    public float AttackSpeede;          // ���ݼӵ� (%)
}
