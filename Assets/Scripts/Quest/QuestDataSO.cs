using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Quests/Quest")]
public class QuestDataSO : ScriptableObject
{
    public string questID;                      // ���� ID
    public string questTitle;                   // ����Ʈ �̸�
    public string questDescription;             // ����Ʈ ����
    public Sprite rewardIcon;                   // ���� ������
    public int rewardAmount;                    // ���� ��ġ
    public QuestType questType;                 // ����Ʈ Ÿ��
}

public enum QuestType
{
    KillMonster,
    DoGacha,
    ClearStage,
}
