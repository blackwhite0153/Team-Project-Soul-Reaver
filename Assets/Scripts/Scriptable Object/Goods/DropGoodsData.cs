using UnityEngine;

[CreateAssetMenu(menuName = "Drop/DropGoods")]
public class DropGoodsData : ScriptableObject
{
    public int goldAmount;      // ���� ��� ����
    public int gachaAmount;     // ���� �̱���ȭ ����

    public int GetGoldAmount()
    {
        return goldAmount;
    }

    public int GetGachaAmount()
    {
        return gachaAmount;
    }
}