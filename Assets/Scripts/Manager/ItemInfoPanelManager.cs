using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoPanelManager : MonoBehaviour
{
    [Header("Scripts")]
    public PopUp PopUp;
    public PopDown PopDown;
    public InventoryUIManager InventoryUIManager;

    public GameObject Panel;
    public Button PanelButton;
    public Button WearButton;
    public Button SellButton;
    public Image ItemIconImage;
    public Image ItemIconBackGround2;

    [Header("TMP_Text")]
    public TMP_Text WearText;
    public TMP_Text ItemIconText;
    public TMP_Text ItemNameText;
    public TMP_Text GradeText;
    public TMP_Text LvText;
    public TMP_Text OptionText;

    private void Start()
    {
        PanelButton.onClick.AddListener(HideInfo);
        CharacterInventoryManager.Instance._equippedWeapon = null;
        CharacterInventoryManager.Instance._equippedTop = null;
        CharacterInventoryManager.Instance._equippedShoes = null;
        CharacterInventoryManager.Instance._equippedGloves = null;
        CharacterInventoryManager.Instance._equippedBottom = null;
        CharacterInventoryManager.Instance._equippedRunes = null;
    }

    // ������ ���� �г�
    public void ShowInfo(ItemInstance itemInstance)
    {
        var item = itemInstance.ItemSo;

        ItemIconBackGround2.color = _gradeColors[item.grade];
        ItemIconImage.sprite = item.ItemImage;
        ItemIconText.text = $"Lv {itemInstance.EnhanceLevel}";
        ItemNameText.text = $"�̸�: {item.Name}";
        GradeText.text = $"{item.grade}";
        LvText.text = $"LV: {itemInstance.EnhanceLevel}";

        // ������ �ɼ� �ؽ�Ʈ(������ ���� �ٸ���)
        if(item is WaponDataSo wapon)
        {
            OptionText.text = $"���ݷ�: {wapon.AtkDamage * (itemInstance.EnhanceLevel + 1) }%\n��ų ������: {wapon.SkillDamage * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorTopDataSo top)
        {
            OptionText.text = $"����: {top.Defense * (itemInstance.EnhanceLevel + 1)}%\nü�� ���: {top.HpRegen * (itemInstance.EnhanceLevel + 1)}%\n���� ���: {top.MpRegen * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorShoesDataSo shoes)
        {
            OptionText.text = $"�̵� �ӵ�: {shoes.MoveSpeed * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorGlovesDataSo gloves)
        {
            OptionText.text = $"���� �ӵ�: {gloves.AttackSpeede * (itemInstance.EnhanceLevel + 1)}%";
        }
        else if(item is ArmorBottomDataSo bottom)
        {
            OptionText.text = $"����: {bottom.Defense * (itemInstance.EnhanceLevel + 1)}%\nü��: {bottom.Hp * (itemInstance.EnhanceLevel + 1)}%\n����: {bottom.Mp * (itemInstance.EnhanceLevel + 1)}%";
        }
        else
        {
            OptionText.text = "���ƾ� ���׾� ����";
        }

        PopUp.PlayGrow(Panel);

        // ����, ����, ��ü ��ư
        WearButton.onClick.RemoveAllListeners(); // ���� ������ ����(���������� ���ο� ������ ����)
        WearButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX("Button");

            switch (item.EquipType)
            {
                case EquipmentType.Weapon:
                    HandleEquip(CharacterInventoryManager.Instance._equippedWeapon, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterWaponItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Top:
                    HandleEquip(CharacterInventoryManager.Instance._equippedTop, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterTopItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Bottom:
                    HandleEquip(CharacterInventoryManager.Instance._equippedBottom, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterBottomItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Gloves:
                    HandleEquip(CharacterInventoryManager.Instance._equippedGloves, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterGlovesItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
                case EquipmentType.Shoes:
                    HandleEquip(CharacterInventoryManager.Instance._equippedShoes, itemInstance,
                        () => CharacterInventoryManager.Instance.CharacterShoesItemUnwear(),
                        () => CharacterInventoryManager.Instance.CharacterItemWear(itemInstance));
                    break;
            }
            HideInfo();
        });

        // ��ư UI �ؽ�Ʈ �ٲٱ�
        switch(item.EquipType)
        {
            case EquipmentType.Weapon:
                TextEqulp(CharacterInventoryManager.Instance._equippedWeapon, itemInstance);
                break;
            case EquipmentType.Top:
                TextEqulp(CharacterInventoryManager.Instance._equippedTop, itemInstance);
                break;
            case EquipmentType.Bottom:
                TextEqulp(CharacterInventoryManager.Instance._equippedBottom, itemInstance);
                break;
            case EquipmentType.Gloves:
                TextEqulp(CharacterInventoryManager.Instance._equippedGloves, itemInstance);
                break;
            case EquipmentType.Shoes:
                TextEqulp(CharacterInventoryManager.Instance._equippedShoes, itemInstance);
                break;
        }

        SellButton.gameObject.SetActive(false);
    }

#region RuneShowInfo

    // �� �ɼ� �ؽ�Ʈ(������ ���� �ٸ���)
    public void RuneShowInfo(RuneInstance runeInstance)
    {
        var rune = runeInstance.RuneSo;
        ItemIconImage.sprite = rune.ItemImage;
        ItemIconBackGround2.color = new Color32(221, 160, 221, 255);
        ItemNameText.text = $"�̸�: {rune.Name}";

        if (rune is IRuneOption optionProvider)
        {
            OptionText.text = optionProvider.GetOptionText();
        }
        else
        {
            OptionText.text = "���ƾ� ���׾� ����";
        }

        PopUp.PlayGrow(Panel);

        // ����, ����, ��ü ��ư
        WearButton.onClick.RemoveAllListeners();
        WearButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX("Button");

            HandleRuneEquip(
                CharacterInventoryManager.Instance._equippedRunes,
                runeInstance,
                () => CharacterInventoryManager.Instance.CharacterRuneUnwear(),
                () => CharacterInventoryManager.Instance.CharacterRuneWear(runeInstance)
            );
            runeInstance.IsEquipped = !runeInstance.IsEquipped; // ���� ���� ���
            HideInfo();
        });

        TextRuneEqulp(CharacterInventoryManager.Instance._equippedRunes, runeInstance);

        SellButton.onClick.RemoveAllListeners();
        SellButton.onClick.AddListener(() => SellRune(runeInstance));
    }

#endregion

    private void SellRune(RuneInstance runeInstance)
    {
        int sellGold = 3000;

        // �ǸŽ� ���Կ��� ��������
        CharacterInventoryManager.Instance.RemoveItem(runeInstance);
        // �ǸŽ� �κ��丮 ����Ʈ���� ����
        InventoryManager.Instance.RemoveRuneItem(runeInstance);

        // ��� ����
        GameManager.Instance.GetMoney(sellGold); // ��� ���� ����� ������Ʈ�� ���� �ٸ�

        // UI ������Ʈ
        InventoryUIManager.RefreshInventory();

        // �г� �����
        HideInfo();
    }

    // �ؽ�Ʈ ������ ��ü, ����, ����
    private void TextEqulp(ItemInstance equipped, ItemInstance clicked)
    {
        if (equipped == null)
        {
            WearText.text = "�����ϱ�";
        }
        else if (ReferenceEquals(equipped, clicked) || equipped == clicked)
        {
            WearText.text = "�����ϱ�";
        }
        else if (equipped.ItemSo != null && clicked.ItemSo != null && equipped.ItemSo == clicked.ItemSo)
        {
            // ���� �������̸� ����
            WearText.text = "��ü�ϱ�";
        }
        else
        {
            WearText.text = "��ü�ϱ�";
        }
    }

    private void TextRuneEqulp(RuneInstance equipped, RuneInstance clicked)
    {
        if (equipped == null)
        {
            WearText.text = "�����ϱ�";
        }
        else if (ReferenceEquals(equipped, clicked) || equipped == clicked)
        {
            WearText.text = "�����ϱ�";
        }
        else if (equipped.RuneSo != null && clicked.RuneSo != null && equipped.RuneSo == clicked.RuneSo)
        {
            // ���� �������̸� ����
            WearText.text = "��ü�ϱ�";
        }
        else
        {
            WearText.text = "��ü�ϱ�";
        }
    }

    // ������ ����, ����, ��ü
    public void HandleEquip(ItemInstance equipped, ItemInstance clicked, Action unwearAction, Action wearAction)
    {
        if (equipped == null)
        {
            wearAction();
            InventoryUIManager.UpdateEquippedSlot(clicked, true);
        }
        else if (equipped == clicked)
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedSlot(equipped, false);
        }
        else
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedSlot(equipped, false);
            wearAction();
            InventoryUIManager.UpdateEquippedSlot(clicked, true);
        }
    }

    private void HandleRuneEquip(RuneInstance equipped, RuneInstance clicked, Action unwearAction, Action wearAction)
    {
        if (equipped == null)
        {
            wearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(clicked, true);
        }
        else if (equipped == clicked)
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(equipped, false);
        }
        else
        {
            unwearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(equipped, false);
            wearAction();
            InventoryUIManager.UpdateEquippedRuneSlot(clicked, true);
        }
    }

    // ��޺� ���� ����
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    public void HideInfo()
    {
        SoundManager.Instance.PlaySFX("Button");

        PopDown.PlayShrink(Panel);
    }
}