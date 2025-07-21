using UnityEngine;

public interface IRuneOption
{
    string GetOptionText();
}

public interface IRuneEffect
{
    void ApplyEffect(CharacterInventoryManager manager);
    void RemoveEffect(CharacterInventoryManager manager);
}


public abstract class RuneBase : ScriptableObject
{
    public Sprite ItemImage;        // �̹���
    public string ID;               // ���� ���̵�
    public string Name;             // �̸�
    public string Explanation;      // ����
}
