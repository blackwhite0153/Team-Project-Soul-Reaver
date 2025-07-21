using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string mapName;

    [Header("�ʿ� ������ ��Ƽ�����")]
    public List<Material> materials; // ����, �ٴ�, �� � ������ ��Ƽ���� ����Ʈ

    [Header("���� ����")]
    public EnemyMapSet enemySetData;
}
