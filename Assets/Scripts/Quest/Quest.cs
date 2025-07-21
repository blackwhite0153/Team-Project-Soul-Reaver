[System.Serializable]
public class Quest
{
    public QuestDataSO data;             // ����Ʈ�� ���ø� ������ (�̸�, Ÿ��, ���� ��)
    public int targetAmount;             // �̹� ����Ʈ���� ��ǥ�� �ؾ� �� ��ġ (��: ���� 10����)
    public int currentProgress;          // ���� �޼��� ��ġ (��: ���� 6���� ����)
    public bool isCompleted;             // ��ǥ �޼� ����
    public bool isRewardClaimed;         // ���� ���� ����

    // ����Ʈ ���� �� ȣ��Ǵ� �ʱ�ȭ �Լ�
    public void Initialize(int level = 1)
    {
        // ����Ʈ Ÿ�Կ� ���� ��ǥ ��ġ�� �ٸ��� ����
        switch (data.questType)
        {
            case QuestType.KillMonster:
                targetAmount = 5 * level;       // ���� 1�� �� 5����, 2�� �� 10����...
                break;

            case QuestType.ClearStage:
                targetAmount = 3 * level;       // ��: 3���̺�, 6���̺�, 9���̺�...
                break;

            default:
                targetAmount = 1;               // �� �� ����Ʈ�� �⺻ 1�� ����
                break;
        }

        // �ʱ� ���൵�� �Ϸ� ���� ����
        currentProgress = 0;
        isCompleted = false;
        isRewardClaimed = false;
    }

    // ����Ʈ ���൵�� �ø��� �Լ�
    public void AddProgress(int amount)
    {
        if (isCompleted) return; // �̹� �Ϸ�� ����Ʈ�� �� �̻� ���� �Ұ�

        currentProgress += amount; // ��ġ�� ����

        // ��ǥ�� �ѱ�� �Ϸ� ó��
        if (currentProgress >= targetAmount)
        {
            currentProgress = targetAmount;
            isCompleted = true;
        }
    }
}
