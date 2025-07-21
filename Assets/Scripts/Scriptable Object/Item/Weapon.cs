using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Item/Weapon")]

public class WaponDataSo : ItemBase
{
    public override EquipmentType EquipType => EquipmentType.Weapon; // Ÿ�� ����

    public float SkillDamage;           // ��ų ������ (%)
    public float AtkDamage;             // ���ݷ�      (%)

}
