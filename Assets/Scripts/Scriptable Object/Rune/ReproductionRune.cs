using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Reproduct")]
public class ReproductionRune : RuneBase, IRuneOption, IRuneEffect
{
    public float healthReproduction;    // ü�� ��� (%)
    public float manaReproduction;      // ���� ��� (%)

    public string GetOptionText()
    {
        return $"ü�� �����: {healthReproduction}%\n���� �����: {manaReproduction}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.HealthRepBonus = healthReproduction;
        manager.ManaRepBonus = manaReproduction;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.HealthRepBonus = 0;
        manager.ManaRepBonus = 0;
    }
}