
[System.Serializable]   // ����ȭ
public class ItemInstance
{
    public ItemBase ItemSo;
    public int EnhanceLevel = 0;        // ��ȭ ��ġ
    public bool IsEquipped = false;     // ������ ���� ����
}

[System.Serializable]
public class RuneInstance
{
    public RuneBase RuneSo;
    public bool IsEquipped = false;     // ������ ���� ����
}