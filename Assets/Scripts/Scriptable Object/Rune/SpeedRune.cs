using UnityEngine;

[CreateAssetMenu(menuName = "Rune/Speed")]
public class SpeedRune : RuneBase, IRuneOption, IRuneEffect
{
    public float atkSpeed;  // ���ݼӵ� (%)
    public float moveSpeed; // �̵��ӵ� (%)

    public string GetOptionText()
    {
        return $"���� �ӵ�: {atkSpeed}%\n�̵� �ӵ�: {moveSpeed}%";
    }

    public void ApplyEffect(CharacterInventoryManager manager)
    {
        manager.AtkSpeedBonus = atkSpeed;
        manager.MoveSpeedBonus = moveSpeed;
    }

    public void RemoveEffect(CharacterInventoryManager manager)
    {
        manager.AtkSpeedBonus = 0;
        manager.MoveSpeedBonus = 0;
    }
}