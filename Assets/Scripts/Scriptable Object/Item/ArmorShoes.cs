using UnityEngine;

[CreateAssetMenu(fileName = "NewAromorShoesData", menuName = "Item/ArmorShoes")]

public class ArmorShoesDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Shoes; // Ÿ�� �Ź�

    public float MoveSpeed;             // �̵��ӵ� (%)
}
