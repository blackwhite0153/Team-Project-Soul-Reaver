using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Power")]
public class PowerRune : RuneBase, IRuneOption, IRuneEffect
{
    public float atkDamage; // ���� ������ (%)
    public float skillDamage; // ��ų ���ط� (%)

    public string GetOptionText()
    {
        return $"���ݷ�: {atkDamage}%\n��ų ���ط�: {skillDamage}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.AtkDamageBonus = atkDamage;
        manager.SkillDamageBonus = skillDamage;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.AtkDamageBonus = 0;
        manager.SkillDamageBonus = 0;
    }
}