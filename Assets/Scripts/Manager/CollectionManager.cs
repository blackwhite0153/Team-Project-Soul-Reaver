using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : Singleton<CollectionManager>
{
    public ItemBase[] Items;                       // ��� ������ ���� �迭 (�ܺο��� ����)
    public GameObject IconPrefab;                  // ������ ������ UI ������
    public Transform ContentParent;                // �����ܵ��� ���� ��ũ�Ѻ� ������ �θ� ��ü
    public Button CloseButton;                     // ������ ����â �ݱ� ��ư

    [Header("Info Panel")]
    public GameObject ItemInfoPanel;                // ������ �� ����â �г�
    public TMP_Text NameText;                       // ������ �̸� �ؽ�Ʈ
    public TMP_Text GradeText;                      // ������ ��� �ؽ�Ʈ
    public TMP_Text ExplanationText;                // ������ ���� �ؽ�Ʈ

    [Header("Reward")]
    public Button[] RewardButtons;                  // ���� ���� ��ư�� (�ν����Ϳ� 4�� ����)
    public TMP_Text[] RewardButtonTexts;            // ���� ��ư �� �ؽ�Ʈ��
    private bool[] _rewardClaimed;                   // �� ���� �ܰ� ���� ���� üũ �迭

    // �ڿ� �迭 �����ϸ��
    //[Header("Reward Data")]
    private int[] _rewardGoldAmounts = {1000, 1500, 2000, 2500, 3000};                 // �� ���� �ܰ躰 ���� ��差

    private int _unlockedCount = 0;                  // �رݵ�(ȹ����) ������ ���� ī��Ʈ
    private int[] _rewardMilestones = { 5, 10, 15, 20, 25 };  // ���� ���� ������ ���� ����
    private List<GameObject> _iconObjects = new List<GameObject>(); // ������ ������ ������Ʈ ����Ʈ

    // �ڳ�
    private const string COLLECTION_TABLE = "USER_COLLECTION";
    private string collectionRowInDate = string.Empty;

    // ��޺� ���� �켱���� (���ڰ� �������� �켱)
    private Dictionary<Grade, int> _gradeOrder = new Dictionary<Grade, int>()
    {
        { Grade.Legendary, 0 },
        { Grade.Rare, 1 },
        { Grade.Uncommon, 2 },
        { Grade.Common, 3 }
    };

    // ��޺� ���� ����
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    void Start()
    {
        LoadCollectionDataFromBackend(); // ���ο��� UI �ʱ�ȭ���� �ϵ��� ó����
    }

    private void Update()
    {
        //Debug.Log(_unlockedCount);
    }

    // ������ �� ���� UI�� ���� ä��� �г� Ȱ��ȭ
    private void ShowItemInfo(ItemBase item)
    {
        SoundManager.Instance.PlaySFX("Button");

        NameText.text = item.Name;
        GradeText.text = item.grade.ToString();
        ExplanationText.text = item.Explanation;
        ItemInfoPanel.SetActive(true);
    }

    private void InitializeUI()
    {
        // ������ ���� �� ����
        var sortedItems = Items
            .OrderBy(item => _gradeOrder.ContainsKey(item.grade) ? _gradeOrder[item.grade] : int.MaxValue)
            .ToList();

        foreach (var item in sortedItems)
        {
            GameObject iconObj = Instantiate(IconPrefab, ContentParent);
            Image iconImage = iconObj.transform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = item.ItemImage;

            Image background = iconObj.transform.Find("Background").GetComponent<Image>();
            Color bgColor = _gradeColors.ContainsKey(item.grade) ? _gradeColors[item.grade] : Color.white;

            UpdateIconLockState(iconObj, item.IsUnlocked, bgColor);

            Button btn = iconObj.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowItemInfo(item));

            _iconObjects.Add(iconObj);
        }

        // ���� ��ư ����
        for (int i = 0; i < RewardButtons.Length; i++)
        {
            int index = i;
            RewardButtons[i].onClick.AddListener(() => OnRewardButtonClicked(index));
            RewardButtons[i].gameObject.SetActive(false);
        }

        CloseButton.onClick.AddListener(OnCloseButtonClick);
        ItemInfoPanel.SetActive(false);

        CheckRewardAvailability();
    }


    // �ܺο��� ������ �ر� �� ȣ���ϴ� �Լ�
    public void UnlockItem(ItemBase item)
    {
        if (!item.IsUnlocked)
        {
            item.IsUnlocked = true;
            _unlockedCount++;

            // ���� ������ ���� ����
            foreach (var icon in _iconObjects)
            {
                Destroy(icon);
            }
            _iconObjects.Clear();

            // UI �ٽ� �ʱ�ȭ (������ ���� ���� �� ����)
            InitializeUI();

            CheckRewardAvailability();
        }

        if (string.IsNullOrEmpty(collectionRowInDate))
            SaveCollectionDataToBackend();
        else
            UpdateCollectionDataToBackend();
    }

    // �������� ��� ���¿� ���� �̹��� ���� ����
    private void UpdateIconLockState(GameObject iconObj, bool unlocked, Color bgColor)
    {
        var iconImage = iconObj.transform.Find("Icon").GetComponent<Image>();
        var background = iconObj.transform.Find("Background").GetComponent<Image>();

        if (unlocked)
        {
            iconImage.color = Color.white;           // ������ ������
            background.color = bgColor;               // ��޿� �´� ����
        }
        else
        {
            iconImage.color = new Color(1, 1, 1, 0.3f);   // ������ ������ (��� ����)
            background.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0.3f);  // ������ ���
        }
    }

    // ���� ���� �� �ִ��� üũ�ϰ� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ ó��
    private void CheckRewardAvailability()
    {
        for (int i = 0; i < _rewardMilestones.Length; i++)
        {
            // ���� ���� �̼��� && �ر� ������ ���� ���� �̻��̸� ��ư Ȱ��ȭ
            if (!_rewardClaimed[i] && _unlockedCount >= _rewardMilestones[i])
            {
                RewardButtons[i].gameObject.SetActive(true);
                RewardButtons[i].interactable = true;
                RewardButtonTexts[i].text = $"���� �ޱ�! ({_rewardMilestones[i]}��)";
            }
            // �̹� ���� ������ ��� ��ư Ȱ��ȭ + ��Ȱ��ȭ ó��
            else if (_rewardClaimed[i])
            {
                RewardButtons[i].gameObject.SetActive(true);
                RewardButtons[i].interactable = false;
                RewardButtonTexts[i].text = "���� �Ϸ�";
            }
            // ���� ���� �� ������ ��ư �����
            else
            {
                RewardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // ���� ��ư Ŭ�� �� ȣ��, ���� ���� ó��
    private void OnRewardButtonClicked(int index)
    {
        if (_rewardClaimed[index]) return;  // �̹� ���� ������ ����

        SoundManager.Instance.PlaySFX("Button");

        Debug.Log($"{_rewardMilestones[index]}�� ���� ����!");

        // ��� ���� ���� ó�� (����)
        if (_rewardGoldAmounts != null && index < _rewardGoldAmounts.Length)
        {
            int gold = _rewardGoldAmounts[index];
            Debug.Log($"��� +{gold}");

            GameManager.Instance.GetGp(gold); // ���� ��� ���� �Լ� ȣ��
        }

        // ���� ���� �Ϸ� ó��
        _rewardClaimed[index] = true;
        RewardButtons[index].interactable = false;
        RewardButtonTexts[index].text = "���� �Ϸ�";

        // ����� ���� ���� ������ ������Ʈ
        if (!string.IsNullOrEmpty(collectionRowInDate))
            UpdateCollectionDataToBackend();
    }

    private void OnCloseButtonClick()
    {
        SoundManager.Instance.PlaySFX("Button");

        ItemInfoPanel.SetActive(false);
    }

    // �ڳ� ������ ������ ������
    [System.Serializable]
    public class CollectionSaveData
    {
        public List<string> unlockedItemIDs = new List<string>();  // �رݵ� ������ ID ���
        public List<bool> rewardClaimed = new List<bool>();         // ���� ���� ����
    }

    #region ����

    public void SaveCollectionDataToBackend()
    {
        CollectionSaveData data = new CollectionSaveData();

        foreach (var item in Items)
        {
            if (item.IsUnlocked)
                data.unlockedItemIDs.Add(item.ID);
        }

        foreach (var claimed in _rewardClaimed)
        {
            data.rewardClaimed.Add(claimed);
        }

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.Insert(COLLECTION_TABLE, param);

        if (bro.IsSuccess())
        {
            Debug.Log("���� ������ ���� ����");
            collectionRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("���� ������ ���� ����: " + bro);
        }
    }


    #endregion

    #region �����ϱ�
    public void UpdateCollectionDataToBackend()
    {
        if (string.IsNullOrEmpty(collectionRowInDate))
        {
            SaveCollectionDataToBackend();
            Debug.LogError("���� �����Ͱ� ���� ���ԵǾ�� ������ �����մϴ�. SaveCollectionDataToBackend()�� ���� ȣ���ϼ���.");
            return;
        }

        CollectionSaveData data = new CollectionSaveData();

        foreach (var item in Items)
        {
            if (item.IsUnlocked)
                data.unlockedItemIDs.Add(item.ID);
        }

        foreach (var claimed in _rewardClaimed)
        {
            data.rewardClaimed.Add(claimed);
        }

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2(COLLECTION_TABLE, collectionRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
        {
            Debug.Log("���� ������ ���� ����");
        }
        else
        {
            Debug.LogError("���� ������ ���� ����: " + bro);
        }
    }

    #endregion

    #region �ҷ�����

    public void LoadCollectionDataFromBackend()
    {
        foreach (var item in Items)
            item.IsUnlocked = false;

        var bro = Backend.GameData.GetMyData(COLLECTION_TABLE, new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("���� ������ ��ȸ ����: " + bro);
            InitializeUI();
            return;
        }

        JsonData rows = bro.FlattenRows();
        if (rows.Count <= 0)
        {
            Debug.Log("���� ������ ���� (���� ����)");
            _rewardClaimed = new bool[_rewardMilestones.Length];
            InitializeUI();
            return;
        }

        collectionRowInDate = rows[0]["inDate"].ToString();
        string json = rows[0]["StatData"].ToString();

        CollectionSaveData data = JsonUtility.FromJson<CollectionSaveData>(json);

        foreach (var item in Items)
            item.IsUnlocked = data.unlockedItemIDs.Contains(item.ID);

        _unlockedCount = data.unlockedItemIDs.Count;

        _rewardClaimed = new bool[_rewardMilestones.Length];
        for (int i = 0; i < _rewardClaimed.Length && i < data.rewardClaimed.Count; i++)
            _rewardClaimed[i] = data.rewardClaimed[i];

        InitializeUI(); // �� �� Ÿ�̹��� �߿�!
    }

    #endregion
}
