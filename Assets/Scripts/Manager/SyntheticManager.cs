using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SyntheticManager : MonoBehaviour
{
    List<ItemSlot> slots = new List<ItemSlot>();

    public SyntheticUIManager SyntheticUIManager;

    [Header("SyntheticUI")]
    public Image SyntheticItemIconOne;
    public Image SyntheticItemIconTwo;

    public Image SyntheticBackGroundOne;
    public Image SyntheticBackGroundTwo;

    public TMP_Text SyntheticItemTextOne;
    public TMP_Text SyntheticItemTextTwo;

    public GameObject InventorySlotPrefab;
    public Sprite ItemIcon;

    public Button SyntheticButton;
    public TMP_Text SyntheticButtonText;

    public TMP_Text PlayerMoneyText;
    public TMP_Text ProbabilityText;
    public TMP_Text CostMoneyText;
    public TMP_Text WarningText;

    // �� ��ȭ ������ ���� ����
    private ItemInstance _selectedItemOne;
    private ItemInstance _selectedItemTwo;

    private void Start()
    {
        SyntheticItemIconOne.GetComponent<Button>().onClick.AddListener(ClearSynthesisUI);
        SyntheticButton.onClick.AddListener(DoSynthesis);
    }

    private void Update()
    {
        PlayerMoneyText.text = GameManager.Instance.GoldNum.ToString();
    }

    public void StartSynthat(ItemInstance itemInstance)
    {
        SyntheticUIManager.ClearContents();

        var item = itemInstance.ItemSo;

        // ó�� ��ȭĭ ���ε�
        SyntheticItemIconOne.sprite = item.ItemImage;

        SyntheticItemTextOne.text = $"LV {itemInstance.EnhanceLevel}";

        SoundManager.Instance.PlaySFX("Button");

        // ù��° ��ȭ ��� ����
        _selectedItemOne = itemInstance;

        if (_gradeColors.TryGetValue(item.grade, out var color))
            SyntheticBackGroundOne.color = color;

        if (!int.TryParse(itemInstance.ItemSo.ID, out int baseId)) return;

        // ���� �����۰� ���� ID, ���� ������ ���� �����۸� ������
        var sameItems = InventoryManager.Instance.InventoryItems
            .FindAll(item =>
            {
                return int.TryParse(item.ItemSo.ID, out int idNum) && idNum == baseId && item.EnhanceLevel == itemInstance.EnhanceLevel && item != itemInstance && item.IsEquipped == false;
            });

        // ���� ���� Ÿ�� ã��
        Transform targetParent = null;

        if (baseId >= 0 && baseId < 100)
            targetParent = SyntheticUIManager.WeaponContent;
        else if (baseId >= 101 && baseId < 200)
            targetParent = SyntheticUIManager.TopContent;
        else if (baseId >= 201 && baseId < 300)
            targetParent = SyntheticUIManager.BottomContent;
        else if (baseId >= 301 && baseId < 400)
            targetParent = SyntheticUIManager.GlovesContent;
        else if (baseId >= 401 && baseId < 500)
            targetParent = SyntheticUIManager.ShoesContent;

        if (targetParent == null) return;

        foreach (var items in sameItems)
        {
            GameObject iconObj = Instantiate(InventorySlotPrefab, targetParent);
            ItemSlot slot = iconObj.GetComponent<ItemSlot>();
            Button btn = iconObj.GetComponent<Button>();

            slot.Setup(items);
            slot.SetUpEquipped(false);

            // �� �������� Ŭ���ϸ� SyntheticItemIconTwo�� �̹��� �ֱ�
            btn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                foreach (var s in slots)
                {
                    s.SetUpEquipped(false);
                }

                slot.SetUpEquipped(true);

                // �ι�° ��ȭ ��� ����
                _selectedItemTwo = items;

                SyntheticItemIconTwo.sprite = items.ItemSo.ItemImage;
                SyntheticItemTextTwo.text = $"LV {items.EnhanceLevel}";

                if (_gradeColors.TryGetValue(item.grade, out var color))
                    SyntheticBackGroundTwo.color = color;

                // Ȯ�� �ؽ�Ʈ ǥ��
                if(_selectedItemOne.EnhanceLevel == _selectedItemTwo.EnhanceLevel)
                {
                    ProbabilityTexts(_selectedItemOne.EnhanceLevel);
                    CostMoneyTexts(_selectedItemOne.EnhanceLevel);
                }
                else
                {
                    CostMoneyText.text = "��� : 0";
                    ProbabilityText.text = "Ȯ�� : 0";
                }
            });

            slots.Add(slot);
        }
    }
    
    // ù��° ��ȭ ������ ������ �ʱ�ȭ
    private void ClearSynthesisUI()
    {
        SoundManager.Instance.PlaySFX("Button");

        // ������ �� ��� �ʱ�ȭ
        SyntheticItemIconOne.sprite = ItemIcon;
        SyntheticItemIconTwo.sprite = ItemIcon;
        SyntheticBackGroundOne.color = Color.clear;
        SyntheticBackGroundTwo.color = Color.clear;

        // ���� ���� ����
        foreach (var slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        slots.Clear();

        CostMoneyText.text = "��� : 0";
        ProbabilityText.text = "Ȯ�� : 0";
        SyntheticUIManager.RefreshInventory();
    }

    // ��ȭ ��ư ����
    private void DoSynthesis()
    {
        SoundManager.Instance.PlaySFX("Button");

        // �ռ� ������ ������ �ȵɋ�
        if (_selectedItemOne == null || _selectedItemTwo == null)
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);

            return;
        }

        // �ռ� �������� �ٸ���
        if(_selectedItemOne.ItemSo.ID != _selectedItemTwo.ItemSo.ID)
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);

            return;
        }

        // �ռ� ������ ������ �ٸ���(Ȯ���ҋ��� !=)
        if(_selectedItemOne.EnhanceLevel != _selectedItemTwo.EnhanceLevel)
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);

            return;
        }

        // ��� ����
        if(GameManager.Instance.GoldNum <= (Cost(_selectedItemOne.EnhanceLevel)))
        {
            SyntheticButtonText.color = Color.red;
            Invoke(nameof(SyntheticButtonTextColor), 0.3f);
            return;
        }
        GameManager.Instance.LoseMoney(Cost(_selectedItemOne.EnhanceLevel));

        // ��ȭ Ȯ��
        int itemProbability = Random.Range(1, 101);

        if(itemProbability != Probability(_selectedItemOne.EnhanceLevel))
        {
            WarningText.color = Color.green;
            WarningText.text = "�ռ��� �����Ͽ����ϴ�.";
            InventoryManager.Instance.AddItem(_selectedItemOne.ItemSo, _selectedItemOne.EnhanceLevel+1);

            InventoryManager.Instance.InventoryItems.Remove(_selectedItemOne);
            InventoryManager.Instance.InventoryItems.Remove(_selectedItemTwo);

            _selectedItemOne = null;
            _selectedItemTwo = null;

            InventoryUIManager.Instance.RefreshInventory();
        }
        else
        {
            Debug.Log("���������� ������ ������~~");
            WarningText.color = Color.red;
            WarningText.text = "�ռ��� �����Ͽ����ϴ�.";
            InventoryManager.Instance.InventoryItems.Remove(_selectedItemOne);
            InventoryManager.Instance.InventoryItems.Remove(_selectedItemTwo);
        }

        InventoryManager.Instance.InventoryItems.Remove(_selectedItemOne);
        InventoryManager.Instance.InventoryItems.Remove(_selectedItemTwo);

        ClearSynthesisUI();
    }

    // ������ ��ȭ Ȯ��
    private int Probability (int EnhanceLevel)
    {
        return EnhanceLevel switch
        {
            0 => 100,
            1 => 70,
            2 => 65,
            3 => 55,
            4 => 45,
            5 => 35,
            6 => 25,
            7 => 15,
            8 => 10,
            9 => 5,
            10 => 1,
            _ => 0
        };
    }

    // ������ ��ȭ ���
    private int Cost(int EnhanceLevel)
    {
        return EnhanceLevel switch
        {
            0 => 500,
            1 => 1000,
            2 => 2500,
            3 => 3500,
            4 => 4500,
            5 => 5500,
            6 => 6500,
            7 => 7500,
            8 => 8500,
            9 => 9500,
            10 => 10000,
            _ => 0
        };
    }

    // ��޺� ���� ����
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    private void SyntheticButtonTextColor()
    {
        SyntheticButtonText.color = new Color32(49, 98, 137, 255);
    }

    private void ProbabilityTexts(int EnhanceLevel)
    {
        ProbabilityText.text = $"Ȯ�� : {Probability(EnhanceLevel)}";
    }

    private void CostMoneyTexts(int EnhanceLevel)
    {
        CostMoneyText.text = $"��� : {Cost(EnhanceLevel)}";
    }
}