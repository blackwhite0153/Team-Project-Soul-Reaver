using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Gold")]
public class GoldRune : RuneBase, IRuneOption, IRuneEffect
{
    public float goldEarned; // ��� ȹ�淮(%)

    public string GetOptionText()
    {
        return $"��� ȹ�淮: {goldEarned}";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.GoldBonus = goldEarned;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.GoldBonus = 0;
    }
}