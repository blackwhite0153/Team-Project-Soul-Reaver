using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : Singleton<InventoryUIManager>
{
    List<ItemSlot> slots = new List<ItemSlot>();

    public ItemInfoPanelManager ItemInfoPanelManager;

    public Transform WeaponContent;
    public Transform TopContent;
    public Transform BottomContent;
    public Transform ShoesContent;
    public Transform GlovesContent;
    public Transform RuneContent;

    public GameObject InventorySlotPrefab;

    private Dictionary<Grade, int> _gradeOrder = new Dictionary<Grade, int>()
    {
        { Grade.Legendary, 0 },
        { Grade.Rare, 1 },
        { Grade.Uncommon, 2 },
        { Grade.Common, 3 }
    };

    public void RefreshInventory()
    {
        ClearContents(); // ���� ���Ե� ����

        // �������� �������� ������ �����ؼ� �׸���
        DrawSortedItems(WeaponContent, 0, 100);
        DrawSortedItems(TopContent, 101, 200);
        DrawSortedItems(BottomContent, 201, 300);
        DrawSortedItems(GlovesContent, 301, 400);
        DrawSortedItems(ShoesContent, 401, 500);
        DrawSortedRunes(RuneContent, 701, 800);

    }

    private void DrawSortedItems(Transform parent, int minID, int maxID)
    {
        // ������ ���͸� 
        var filteredItems = InventoryManager.Instance.InventoryItems
            .Where(item =>
            {
                // ���ڷ� ��ȯ ���� Ȯ�� ����(minID ~ max ID)
                if (int.TryParse(item.ItemSo.ID, out int idNum))
                    return idNum >= minID && idNum < maxID;
                return false;
            })
            // ���� ������ ��� ����
            .OrderByDescending(item => item.IsEquipped)
            // ��� + ��ȭ ��ġ ����
            // ������������ ����
            .ThenBy(item => _gradeOrder.TryGetValue(item.ItemSo.grade, out int order) ? order : int.MaxValue)
            // 2�� ���� ������������
            .ThenByDescending(item => item.EnhanceLevel)
            // �̸� ������ ����
            .ThenBy(item => item.ItemSo.ID)
            // �÷��� List<T> ��ȯ
            .ToList();

        foreach (var item in filteredItems)
        {
            GameObject iconObj = Instantiate(InventorySlotPrefab, parent);
            ItemSlot slot = iconObj.GetComponent<ItemSlot>();
            Button btn = iconObj.GetComponent<Button>();

            slot.Setup(item);
            slot.SetUpEquipped(item.IsEquipped);

            // �κ��丮 ��� Ŭ��
            btn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                ItemInfoPanelManager.ShowInfo(item);
            });

            slots.Add(slot);
        }
    }

    private void DrawSortedRunes(Transform parent, int minID, int maxID)
    {
        // �� ���͸� 
        var filteredItems = InventoryManager.Instance.InventoryRunes
            .Where(item =>
            {
                if (item?.RuneSo == null || string.IsNullOrEmpty(item.RuneSo.ID))
                    return false;

                if (int.TryParse(item.RuneSo.ID, out int idNum))
                    return idNum >= minID && idNum < maxID;

                return false;
            })

        // ID �� ����
        .OrderByDescending(item => item.IsEquipped)
        .ThenBy(item => item.RuneSo.ID)
        .ToList();

        foreach (var item in filteredItems)
        {
            GameObject iconObj = Instantiate(InventorySlotPrefab, parent);
            ItemSlot slot = iconObj.GetComponent<ItemSlot>();
            Button btn = iconObj.GetComponent<Button>();

            slot.RuneSetup(item);
            slot.SetUpEquipped(item.IsEquipped);

            btn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                ItemInfoPanelManager.RuneShowInfo(item);
            });

            slots.Add(slot);
        }
    }

    // ������ ���� ���� üũ
    public void UpdateEquippedSlot(ItemInstance item, bool isEquipped)
    {
        foreach (var slot in slots)
        {
            if (slot.ItemInstance == item)
            {
                slot.SetUpEquipped(isEquipped);
                break;
            }
        }
        InventoryManager.Instance.UpdateInventoryToBackend();
    }

    // �� ���� ���� üũ
    public void UpdateEquippedRuneSlot(RuneInstance item, bool isEquipped)
    {
        foreach (var slot in slots)
        {
            if (slot.RuneInstance == item)
            {
                slot.SetUpEquipped(isEquipped);
                break;
            }
        }
        InventoryManager.Instance.UpdateInventoryToBackend();

    }

    // �κ��丮 ������Ʈ �Լ�
    private void ClearContents()
    {
        foreach (Transform child in WeaponContent) Destroy(child.gameObject);
        foreach (Transform child in TopContent) Destroy(child.gameObject);
        foreach (Transform child in BottomContent) Destroy(child.gameObject);
        foreach (Transform child in ShoesContent) Destroy(child.gameObject);
        foreach (Transform child in GlovesContent) Destroy(child.gameObject);
        foreach (Transform child in RuneContent) Destroy(child.gameObject);

        slots.Clear();
    }
}