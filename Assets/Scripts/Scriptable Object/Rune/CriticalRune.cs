using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Critical")]
public class CriticalRune : RuneBase, IRuneOption, IRuneEffect
{
    public float criChance; // ġ��Ÿ Ȯ�� (%)
    public float criDamage; // ġ��Ÿ ���� (%)

    public string GetOptionText()
    {
        return $"ġ��Ÿ Ȯ��: {criChance}%\nġ��Ÿ ����: {criDamage}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.CriChanceBonus = criChance;
        manager.CriDamageBonus = criDamage;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.CriChanceBonus = 0;
        manager.CriDamageBonus = 0;
    }
}