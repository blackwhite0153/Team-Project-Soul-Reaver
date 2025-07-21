using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public InventoryUIManager InventoryUIManager;
    public SyntheticUIManager SyntheticManager;
    public List<ItemInstance> InventoryItems = new();   // ����Ʈ�� ������ ����Ʈ ����
    public List<RuneInstance> InventoryRunes = new();   // ����Ʈ�� �� ����Ʈ ����

    public void AddItem(ItemBase itemSo, int enhanceLevel = 0)  // ������ �߰�
    {
        if (itemSo == null) return;

        InventoryItems.Add(new ItemInstance { ItemSo = itemSo, EnhanceLevel = enhanceLevel });
        InventoryUIManager.RefreshInventory();
        SyntheticManager.RefreshInventory();
        UpdateInventoryToBackend();
    }

    // ����
    public void SaveInventoryToBackend()
    {
        InventorySaveData saveData = new InventorySaveData();

        // �κ��丮 ������ ����
        foreach (var item in InventoryItems)
        {
            saveData.items.Add(new InventoryItemData
            {
                itemId = item.ItemSo.ID,
                enhanceLevel = item.EnhanceLevel,
                isEquipped = item.IsEquipped
            });
        }
        // �� ����
        foreach (var rune in InventoryRunes)
        {
            saveData.runes.Add(new InventoryRuneData
            {
                runeId = rune.RuneSo.ID
            });
        }

        // ��� ���� ����
        saveData.equippedItems.weaponId = CharacterInventoryManager.Instance._equippedWeapon?.ItemSo.ID;
        saveData.equippedItems.topId = CharacterInventoryManager.Instance._equippedTop?.ItemSo.ID;
        saveData.equippedItems.bottomId = CharacterInventoryManager.Instance._equippedBottom?.ItemSo.ID;
        saveData.equippedItems.shoesId = CharacterInventoryManager.Instance._equippedShoes?.ItemSo.ID;
        saveData.equippedItems.glovesId = CharacterInventoryManager.Instance._equippedGloves?.ItemSo.ID;
        saveData.equippedItems.runeId = CharacterInventoryManager.Instance._equippedRunes?.RuneSo.ID;

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.Insert("USER_INVENTORY", param); // ���� ����
        if (bro.IsSuccess())
        {
            Debug.Log("�κ��丮 ���� ����");
            inventoryRowInDate = bro.GetInDate(); // inDate ����
        }
        else
        {
            Debug.LogError("�κ��丮 ���� ����: " + bro);
        }
    }

    // �����ϱ�
    private string inventoryRowInDate = string.Empty; // inDate ���� ���� �߰�

    public void UpdateInventoryToBackend()
    {
        if (string.IsNullOrEmpty(inventoryRowInDate))
        {
            Debug.LogError("���� SaveInventoryToBackend()�� ���� �����͸� �����ؾ� �մϴ�.");
            return;
        }

        InventorySaveData saveData = new InventorySaveData();

        foreach (var item in InventoryItems)
        {
            saveData.items.Add(new InventoryItemData
            {
                itemId = item.ItemSo.ID,
                enhanceLevel = item.EnhanceLevel,
                isEquipped = item.IsEquipped
            });
        }

        // �� ����
        foreach (var rune in InventoryRunes)
        {
            saveData.runes.Add(new InventoryRuneData
            {
                runeId = rune.RuneSo.ID
            });
        }


        saveData.equippedItems.weaponId = CharacterInventoryManager.Instance._equippedWeapon?.ItemSo?.ID;
        saveData.equippedItems.topId = CharacterInventoryManager.Instance._equippedTop?.ItemSo?.ID;
        saveData.equippedItems.bottomId = CharacterInventoryManager.Instance._equippedBottom?.ItemSo?.ID;
        saveData.equippedItems.shoesId = CharacterInventoryManager.Instance._equippedShoes?.ItemSo?.ID;
        saveData.equippedItems.glovesId = CharacterInventoryManager.Instance._equippedGloves?.ItemSo?.ID;
        saveData.equippedItems.runeId = CharacterInventoryManager.Instance._equippedRunes?.RuneSo?.ID;

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2("USER_INVENTORY", inventoryRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
            Debug.Log("�κ��丮 ���� ����");
        else
            Debug.LogError("�κ��丮 ���� ����: " + bro);
    }

    // �ҷ�����
    public void LoadInventoryFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_INVENTORY", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("�κ��丮 ��ȸ ����: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();

        if (rows.Count <= 0)
        {
            Debug.Log("����� �κ��丮 ����");
            SaveInventoryToBackend(); // �ʱ� ����

            return;
        }

        inventoryRowInDate = rows[0]["inDate"].ToString();  // inDate ����
        string json = rows[0]["StatData"].ToString();

        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        InventoryItems.Clear();

        InventoryRunes.Clear();

        foreach (var savedRune in saveData.runes)
        {
            RuneBase runeSo = ItemDatabase.Instance.GetRuneById(savedRune.runeId);
            if (runeSo != null)
            {
                InventoryRunes.Add(new RuneInstance { RuneSo = runeSo });
            }
        }


        foreach (var savedItem in saveData.items)
        {
            ItemBase itemSo = ItemDatabase.Instance.GetItemById(savedItem.itemId);
            if (itemSo != null)
            {
                ItemInstance newItem = new ItemInstance
                {
                    ItemSo = itemSo,
                    EnhanceLevel = savedItem.enhanceLevel,
                    IsEquipped = false
                };

                InventoryItems.Add(newItem);

                if (savedItem.isEquipped)
                    CharacterInventoryManager.Instance.CharacterItemWear(newItem);
            }
        }

        TryEquipById(saveData.equippedItems.weaponId);
        TryEquipById(saveData.equippedItems.topId);
        TryEquipById(saveData.equippedItems.bottomId);
        TryEquipById(saveData.equippedItems.shoesId);
        TryEquipById(saveData.equippedItems.glovesId);
        TryEquipRuneById(saveData.equippedItems.runeId);

        InventoryUIManager.RefreshInventory();
    }

    // ������ ID�� ��� ����
    private void TryEquipById(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        var equipped = InventoryItems.FirstOrDefault(x => x.ItemSo.ID == id);

        if (equipped != null)
        {
            equipped.IsEquipped = true;
            CharacterInventoryManager.Instance.CharacterItemWear(equipped);
        }
    }

    private void TryEquipRuneById(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        var equippedRune = InventoryRunes.FirstOrDefault(x => x.RuneSo.ID == id);

        if (equippedRune != null)
        {
            equippedRune.IsEquipped = true;
            CharacterInventoryManager.Instance.CharacterRuneWear(equippedRune);
        }
    }

    // �� �߰�
    public void RuneAddItem(RuneBase runeSo)
    {
        if (runeSo == null) return;

        InventoryRunes.Add(new RuneInstance { RuneSo = runeSo });
        InventoryUIManager.RefreshInventory();
        SyntheticManager.RefreshInventory();
    }

    public void RemoveRuneItem(RuneInstance runeItem)
    {
        // �ǸŽ� �κ��丮 ����Ʈ���� �� ����
        if (InventoryRunes.Contains(runeItem))
        {
            InventoryRunes.Remove(runeItem);
            InventoryUIManager.RefreshInventory();
            SyntheticManager.RefreshInventory();
        }
    }
}

// �ڳ� ���� �� �ҷ�����

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItemData> items = new();
    public EquippedItemData equippedItems = new();
    public List<InventoryRuneData> runes = new(); // �� �� ����Ʈ �߰�

}

[System.Serializable]
public class InventoryItemData
{
    public string itemId;
    public int enhanceLevel;
    public bool isEquipped;
}

[System.Serializable]
public class EquippedItemData
{
    public string weaponId;
    public string topId;
    public string bottomId;
    public string shoesId;
    public string glovesId;
    public string runeId;
}

[System.Serializable]
public class InventoryRuneData
{
    public string runeId;
}


