using System.Collections.Generic;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private GameObject questSlotPrefab;
    [SerializeField] private Transform contentParent;

    public void RefreshUI(List<Quest> questList)
    {
        // ���� UI ����
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // ����Ʈ ���� ����
        foreach (var quest in questList)
        {
            GameObject slotGO = Instantiate(questSlotPrefab, contentParent);
            QuestSlotUI slotUI = slotGO.GetComponent<QuestSlotUI>();
            slotUI.Setup(quest);
        }
    }
}
