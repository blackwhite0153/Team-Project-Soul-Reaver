using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Gacha Database", menuName = "Gacha/Database")]
public class GachaDatabaseSO : ScriptableObject
{
    public List<ItemBase> AllItmes;
    
    public List<ItemBase> GetItemsByGrade(Grade grade) // ���� ��޿��� ������ �̱�
    {
        return AllItmes.FindAll(item => item.grade == grade);
    }
}