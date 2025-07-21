using UnityEngine;

[CreateAssetMenu(menuName = "Rune/DefenseRune")]
public class DefenseRune : RuneBase, IRuneOption, IRuneEffect
{
    public float defense;   // ���� (%)
    public float health;    // ü��(%)
    public float mp;        // ����(%)

    public string GetOptionText()
    {
        return $"����: {defense}%\nü��: {health}%\n����: {mp}";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.DefenseBonus = defense;
        manager.HealthBouns = health;
        manager.ManaBouns = mp;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.DefenseBonus = 0;
        manager.HealthBouns = 0;
        manager.ManaBouns = 0;
    }
}