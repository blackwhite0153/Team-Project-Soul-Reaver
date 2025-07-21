using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlotUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image rewardIcon;
    public TMP_Text rewardAmountText;
    public Button rewardButton;

    private Quest currentQuest;

    public void Setup(Quest quest)
    {
        currentQuest = quest;

        titleText.text = quest.data.questTitle;
        descriptionText.text = $"���൵: {quest.currentProgress} / {quest.targetAmount}";
        rewardIcon.sprite = quest.data.rewardIcon;
        rewardAmountText.text = $"����: {quest.data.rewardAmount}";

        // ���� ��ư ���� ����
        rewardButton.interactable = quest.isCompleted && !quest.isRewardClaimed;

        // ��ư �̺�Ʈ ����
        rewardButton.onClick.RemoveAllListeners();
        rewardButton.onClick.AddListener(ClaimReward);
    }

    private void ClaimReward()
    {
        if (currentQuest.isCompleted && !currentQuest.isRewardClaimed)
        {
            currentQuest.isRewardClaimed = true;

            // ��: ���� ���� ó�� (��� ���� ��)
            Debug.Log($"���� ����: +{currentQuest.data.rewardAmount}");
            GameManager.Instance.GetGp(500);

            // ����Ʈ �ݺ� ����
            QuestManager.Instance.CreateRepeatableQuest(currentQuest.data.questID);

            // ���� ����
            QuestManager.Instance.ModifyQuestProgress(currentQuest);

            // ����Ʈ ����
            QuestManager.Instance.RemoveClaimedQuests();
        }
    }

}
