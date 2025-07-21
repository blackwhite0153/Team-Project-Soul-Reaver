using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [Header("����Ʈ ���ø� ����Ʈ")]
    public List<QuestDataSO> questTemplates;

    // �ǽð� ����Ʈ ����Ʈ
    public List<Quest> activeQuests = new List<Quest>();
    private Dictionary<string, int> questLevels = new Dictionary<string, int>();
    private string _questInDate = string.Empty;

    private const string QuestTableName = "USER_QUEST"; // �ڳ� ���̺��

    [System.Serializable]
    public class QuestSaveData
    {
        public string questID;
        public int currentProgress;
        public int targetAmount;
        public bool isCompleted;
        public bool isRewardClaimed;
    }

    // ����Ʈ ����
    public void CreateRepeatableQuest(string questID)
    {
        var data = questTemplates.Find(q => q.questID == questID);
        if (data == null)
        {
            Debug.LogWarning($"[{questID}] ����Ʈ ���ø��� ã�� �� �����ϴ�.");
            return;
        }

        if (!questLevels.ContainsKey(questID))
            questLevels[questID] = 1;
        else
            questLevels[questID] += 1;

        Quest newQuest = new Quest { data = data };
        newQuest.Initialize(questLevels[questID]);
        activeQuests.Add(newQuest);

        FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
    }

    #region �ڳ�

    // ��ü ���� (Insert or Update)
    public void SaveAllQuests()
    {
        List<QuestSaveData> saveList = activeQuests.Select(q => new QuestSaveData
        {
            questID = q.data.questID,
            currentProgress = q.currentProgress,
            targetAmount = q.targetAmount,
            isCompleted = q.isCompleted,
            isRewardClaimed = q.isRewardClaimed
        }).ToList();

        string json = JsonMapper.ToJson(saveList);
        Param param = new Param { { "Quests", json } };

        if (string.IsNullOrEmpty(_questInDate))
        {
            var bro = Backend.GameData.Insert(QuestTableName, param);
            if (bro.IsSuccess())
            {
                _questInDate = bro.GetInDate();
                Debug.Log("��ü ����Ʈ ù ���� �Ϸ�");
            }
            else
            {
                Debug.LogError("��ü ����Ʈ Insert ����: " + bro);
            }
        }
        else
        {
            var bro = Backend.GameData.Update(QuestTableName, new Where(), param);
            if (bro.IsSuccess())
                Debug.Log("��ü ����Ʈ ������Ʈ �Ϸ�");
            else
                Debug.LogError("��ü ����Ʈ Update ����: " + bro);
        }
    }

    // Ư�� ����Ʈ�� �����ؼ� ����
    public void ModifyQuestProgress(Quest modifiedQuest)
    {
        Backend.GameData.GetMyData(QuestTableName, new Where(), bro =>
        {
            if (!bro.IsSuccess())
            {
                Debug.LogError("����Ʈ ���� ���� ���� - ������ ��ȸ ����: " + bro);
                return;
            }

            var rows = bro.FlattenRows();
            if (rows.Count == 0)
            {
                Debug.LogWarning("����� ����Ʈ �����Ͱ� ����. ���� �Ұ�.");
                return;
            }

            var row = rows[0];
            _questInDate = row["inDate"].ToString();

            string json = row["Quests"].ToString();
            List<QuestSaveData> saveList = JsonMapper.ToObject<List<QuestSaveData>>(json);

            // �ش� ����Ʈ ã��
            var existing = saveList.Find(q => q.questID == modifiedQuest.data.questID);
            if (existing != null)
            {
                existing.currentProgress = modifiedQuest.currentProgress;
                existing.targetAmount = modifiedQuest.targetAmount;
                existing.isCompleted = modifiedQuest.isCompleted;
                existing.isRewardClaimed = modifiedQuest.isRewardClaimed;
            }
            else
            {
                saveList.Add(new QuestSaveData
                {
                    questID = modifiedQuest.data.questID,
                    currentProgress = modifiedQuest.currentProgress,
                    targetAmount = modifiedQuest.targetAmount,
                    isCompleted = modifiedQuest.isCompleted,
                    isRewardClaimed = modifiedQuest.isRewardClaimed
                });
            }

            // ������ ��ü ����Ʈ �ٽ� ����
            string updatedJson = JsonMapper.ToJson(saveList);
            Param param = new Param { { "Quests", updatedJson } };

            var updateBro = Backend.GameData.Update(QuestTableName, new Where(), param);
            if (updateBro.IsSuccess())
                Debug.Log($"����Ʈ [{modifiedQuest.data.questID}] ���� ���� �Ϸ�");
            else
                Debug.LogError("����Ʈ ���� ���� ����: " + updateBro);
        });
    }

    // ��ü �ҷ�����
    public void LoadAllQuests()
    {
        Backend.GameData.GetMyData(QuestTableName, new Where(), bro =>
        {
            if (bro.IsSuccess())
            {
                var rows = bro.FlattenRows();
                if (rows.Count == 0)
                {
                    Debug.Log("ó�� ������ ����, ����Ʈ ������ ���� �� �⺻ ����Ʈ ����");

                    // [1] �⺻ ����Ʈ �߰�
                    CreateRepeatableQuest("Monster_001"); // ���ø��� �ִ� questID��

                    // [2] �ٷ� ����
                    SaveAllQuests();
                    return;
                }

                var row = rows[0];
                _questInDate = row["inDate"].ToString();

                string json = row["Quests"].ToString();
                List<QuestSaveData> loaded = JsonMapper.ToObject<List<QuestSaveData>>(json);

                activeQuests.Clear();

                foreach (var saveData in loaded)
                {
                    QuestDataSO dataSO = questTemplates.Find(q => q.questID == saveData.questID);
                    if (dataSO == null) continue;

                    Quest q = new Quest
                    {
                        data = dataSO,
                        currentProgress = saveData.currentProgress,
                        targetAmount = saveData.targetAmount,
                        isCompleted = saveData.isCompleted,
                        isRewardClaimed = saveData.isRewardClaimed
                    };

                    activeQuests.Add(q);

                    if (!questLevels.ContainsKey(saveData.questID))
                        questLevels[saveData.questID] = 1;

                    if (dataSO.questType == QuestType.KillMonster)
                        questLevels[saveData.questID] = Mathf.Max(1, saveData.targetAmount / 5);
                    else if (dataSO.questType == QuestType.ClearStage)
                        questLevels[saveData.questID] = Mathf.Max(1, saveData.targetAmount / 3);
                }

                FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
            }
            else
            {
                Debug.LogWarning("����Ʈ �ҷ����� ����: " + bro);
            }
        });
    }

    #endregion

    // ����Ʈ ����
    public void RemoveClaimedQuests()
    {
        activeQuests.RemoveAll(q => q.isCompleted && q.isRewardClaimed);
        SaveAllQuests(); // ���� ����
        FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
    }

    // ���� ų 
    public void OnMonsterKilled()
    {
        bool questUpdated = false;

        foreach (var quest in activeQuests)
        {
            if (quest.data.questType == QuestType.KillMonster &&
                !quest.isCompleted)
            {
                quest.currentProgress++;

                // �Ϸ� üũ
                if (quest.currentProgress >= quest.targetAmount)
                {
                    quest.currentProgress = quest.targetAmount;
                    quest.isCompleted = true;
                    Debug.Log($"[����Ʈ �Ϸ�] {quest.data.questTitle}");
                }

                // ����Ʈ ���� ����
                ModifyQuestProgress(quest);
                questUpdated = true;
            }
        }

        if (questUpdated)
        {
            FindAnyObjectByType<QuestUIManager>()?.RefreshUI(activeQuests);
        }
    }

}
