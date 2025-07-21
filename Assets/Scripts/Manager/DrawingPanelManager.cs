using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DrawingPanelManager : Singleton<DrawingPanelManager>
{
    [SerializeField] private Material _sparkleMaterial;

    // Ǯ��
    private Queue<Material> _sparkleMaterialPool = new Queue<Material>();
    private Queue<GameObject> _drawingSlotPool = new Queue<GameObject>();

    public GachaDatabaseSO GachaDatabaseSO;

    [Header("Script")]
    public FloatingText FloatingText;

    [Header("Obj")]
    public GameObject DrawingPanel;
    public GameObject DrawingSlotPrefab;
    public Transform DrawingContent;
    public List<Button> IgnoreButtons;

    [Header("LV")]
    public Image LvLine;
    public TMP_Text LvLineNum;
    public TMP_Text LvText;
    private int _drawLevel = 1;         // ���� �̱� ����
    private int _currentEXP = 0;        // ���� �ʿ��� ����ġ

    private bool _uiTrue = false;

    // ��޺� Ȯ��(�������� Ȯ�� ����)
    private Dictionary<Grade, float> _gradeChances = new();
    private void Start()
    {
        UpdateChancesByLevel(_drawLevel);

        InitMaterialPool(30);
        InitDrawingSlotPool(30);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _uiTrue == false)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (IsClickingOnAnyIgnoreButton())

                    return;
            }

            DrawingPanel.SetActive(false);
        }
    }

    public IEnumerator Draw(int count, int Gp, float delay = 0.5f)     // 1, 10, 30 ���̱�
    {
        // �̱� ���۽� ��ư �� �г� ��Ȱ��ȭ
        _uiTrue = true;
        SetDrawButton(false);

        if (GameManager.Instance.GpNum < Gp)
        {
            FloatingText.Show("-GP�� �����մϴ�.-");
            SetDrawButton(false);
            _uiTrue = false;

            yield break;
        }

        foreach (Transform i in DrawingContent)
        {
            ReturnDrawingSlotToPool(i.gameObject);
        }

        DrawingPanel.SetActive(false);
        DrawingPanel.SetActive(true);

        GameManager.Instance.LoseGp(Gp);

        for (int i = 0; i < count; i++)
        {
            Grade resultGrade = RollGrade(); // Ȯ���� ���� �̱�
            List<ItemBase> items = GachaDatabaseSO.GetItemsByGrade(resultGrade); // ���� ��޿� �ִ� ������ ����Ʈ

            if (items.Count > 0)
            {
                ItemBase selected = items[Random.Range(0, items.Count)]; // ����Ʈ���� ����

                //GameObject drawingIconObj = Instantiate(DrawingSlotPrefab, DrawingContent);
                GameObject drawingIconObj = GetDrawingSlotFromPool();
                drawingIconObj.transform.SetParent(DrawingContent); // �����


                // ������ �̹��� ����
                Image iconImage = drawingIconObj.transform.Find("Icon").GetComponent<Image>();
                iconImage.sprite = selected.ItemImage;

                Image MeterialImage = drawingIconObj.transform.Find("Shader Line").GetComponent<Image>();

                // �ӵ����� ����
                Material sparkleMatInstance = GetMaterialFromPool();
                MeterialImage.material = sparkleMatInstance;

                // ���� �ð��� ���� �ð� ����
                float startTime = Time.timeSinceLevelLoad;
                float duration = 0.3f; // ��¦�� ���� �ð� (1��)
                sparkleMatInstance.SetFloat("_StartTime", startTime);
                sparkleMatInstance.SetFloat("_Duration", duration);

                // ��� �̹����� ���� ����
                Image background = drawingIconObj.transform.Find("Background").GetComponent<Image>();
                Color bgColor = _gradeColors.ContainsKey(selected.grade) ? _gradeColors[selected.grade] : Color.white;
                background.color = bgColor;

                // �κ��丮�� ���� ������ �߰� (������, ����)
                InventoryManager.Instance.AddItem(selected, 0);

                // ó�� ��� �������̸� ������ �ر�
                CollectionManager.Instance.UnlockItem(selected);
            }

            AddExp(5); // �̱� �ѹ��� 5����ġ

            yield return new WaitForSeconds(delay);

            UpdateGachaDataToBackend();
        }

        //�̱� ���� �� ��ư �� �г� Ȱ��ȭ
        _uiTrue = false;
        SetDrawButton(true);
    }

    // ������ �̱� Ȯ��
    private Grade RollGrade()
    {
        float roll = Random.Range(0.0f, 100.0f);
        float total = 0.0f;

        // ���� ������ �´� Ȯ��
        foreach (var pair in _gradeChances)
        {
            total += pair.Value;
            if (roll <= total)
                return pair.Key;
        }

        // Ȥ�� Ȯ�� ���� 100�� �ƴϸ� �⺻��
        return Grade.Common;
    }

    private void AddExp(int amount)
    {
        _currentEXP += amount;
        int expRequired = GetExpToNextLevel(_drawLevel);

        while (_currentEXP >= expRequired && _drawLevel < 10)
        {
            _currentEXP -= expRequired; // �ʰ� ����ġ ������ ���ؼ�
            _drawLevel++;               // ������

            UpdateChancesByLevel(_drawLevel);

            Debug.Log($"������! {_drawLevel}");

            expRequired = GetExpToNextLevel(_drawLevel);    // ���� ������ ����ġ �䱸�� ����
        }

        LvLine.fillAmount = (float)_currentEXP / GetExpToNextLevel(_drawLevel); // ���� �� ����
        LvText.text = _drawLevel.ToString();    //���� �� �ؽ�Ʈ
        LvLineNum.text = $"{GetExpToNextLevel(_drawLevel)}/{_currentEXP}";
    }

    // ��޺� ���� ����
    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    // ������ �� ����ġ
    private int GetExpToNextLevel(int level)
    {
        return level switch
        {
            1 => 100,
            2 => 150,
            3 => 200,
            4 => 300,
            5 => 400,
            6 => 500,
            7 => 600,
            8 => 700,
            9 => 850,
            10 => 1000,
            _ => 99999
        };
    }

    // �̱� ������ �� ���Ȯ�� 
    public void UpdateChancesByLevel(int level)
    {
        if (level == 1)
            _gradeChances = new() { { Grade.Common, 85f }, { Grade.Uncommon, 15 }, { Grade.Rare, 0f }, { Grade.Legendary, 0f } };
        else if (level == 2)
            _gradeChances = new() { { Grade.Common, 83.4f }, { Grade.Uncommon, 16.5f }, { Grade.Rare, 0.1f }, { Grade.Legendary, 0f } };
        else if (level == 3)
            _gradeChances = new() { { Grade.Common, 80.9f }, { Grade.Uncommon, 18.59f }, { Grade.Rare, 0.5f }, { Grade.Legendary, 0.01f } };
        else if (level == 4)
            _gradeChances = new() { { Grade.Common, 78f }, { Grade.Uncommon, 20.95f }, { Grade.Rare, 1f }, { Grade.Legendary, 0.05f } };
        else if (level == 5)
            _gradeChances = new() { { Grade.Common, 74f }, { Grade.Uncommon, 23.9f }, { Grade.Rare, 2f }, { Grade.Legendary, 0.1f } };
        else if (level == 6)
            _gradeChances = new() { { Grade.Common, 72.5f }, { Grade.Uncommon, 24f }, { Grade.Rare, 3f }, { Grade.Legendary, 0.5f } };
        else if (level == 7)
            _gradeChances = new() { { Grade.Common, 68f }, { Grade.Uncommon, 27f }, { Grade.Rare, 4f }, { Grade.Legendary, 1f } };
        else if (level == 8)
            _gradeChances = new() { { Grade.Common, 64.2f }, { Grade.Uncommon, 27.3f }, { Grade.Rare, 6.5f }, { Grade.Legendary, 2f } };
        else if (level == 9)
            _gradeChances = new() { { Grade.Common, 60.8f }, { Grade.Uncommon, 28.6f }, { Grade.Rare, 7.6f }, { Grade.Legendary, 3f } };
        else if (level == 10)
            _gradeChances = new() { { Grade.Common, 58.5f }, { Grade.Uncommon, 29f }, { Grade.Rare, 8.5f }, { Grade.Legendary, 4f } };
    }

    private bool IsClickingOnAnyIgnoreButton()
    {
        // Raycast�� ���콺 ��ġ�� ��� UI ������Ʈ ������
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        // ù ��° ��ư �������� �θ� Canvas�� Raycaster ���
        if (IgnoreButtons.Count == 0 || IgnoreButtons[0] == null)
            return false;

        GraphicRaycaster raycaster = IgnoreButtons[0].GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            foreach (var button in IgnoreButtons)
            {
                if (result.gameObject == button.gameObject)
                    return true;
            }
        }

        return false;
    }

    // ��ư Ȱ�� / ��Ȱ��ȭ
    private void SetDrawButton(bool interactable)
    {
        foreach (var btn in IgnoreButtons)
        {
            btn.interactable = interactable;
        }
    }

    // DrawInfor���� ������
    public int CurrentDrawLevel => _drawLevel;

    public Dictionary<Grade, float> GetcurrentGradeChances(int level)
    {
        Dictionary<Grade, float> chances = level switch
        {
            1 => new() { { Grade.Common, 85f }, { Grade.Uncommon, 15f }, { Grade.Rare, 0f }, { Grade.Legendary, 0f } },
            2 => new() { { Grade.Common, 83.4f }, { Grade.Uncommon, 16.5f }, { Grade.Rare, 0.1f }, { Grade.Legendary, 0f } },
            3 => new() { { Grade.Common, 80.9f }, { Grade.Uncommon, 18.59f }, { Grade.Rare, 0.5f }, { Grade.Legendary, 0.01f } },
            4 => new() { { Grade.Common, 78f }, { Grade.Uncommon, 20.95f }, { Grade.Rare, 1f }, { Grade.Legendary, 0.05f } },
            5 => new() { { Grade.Common, 74f }, { Grade.Uncommon, 23.9f }, { Grade.Rare, 2f }, { Grade.Legendary, 0.1f } },
            6 => new() { { Grade.Common, 72.5f }, { Grade.Uncommon, 24f }, { Grade.Rare, 3f }, { Grade.Legendary, 0.5f } },
            7 => new() { { Grade.Common, 68f }, { Grade.Uncommon, 27f }, { Grade.Rare, 4f }, { Grade.Legendary, 1f } },
            8 => new() { { Grade.Common, 64.2f }, { Grade.Uncommon, 27.3f }, { Grade.Rare, 6.5f }, { Grade.Legendary, 2f } },
            9 => new() { { Grade.Common, 60.8f }, { Grade.Uncommon, 28.6f }, { Grade.Rare, 7.6f }, { Grade.Legendary, 3f } },
            10 => new() { { Grade.Common, 58.5f }, { Grade.Uncommon, 29f }, { Grade.Rare, 8.5f }, { Grade.Legendary, 4f } },
            _ => new() { { Grade.Common, 85f }, { Grade.Uncommon, 15f }, { Grade.Rare, 0f }, { Grade.Legendary, 0f } },
        };

        return chances;
    }

    // Ǯ �ʱ�ȭ��
    
    private void InitMaterialPool(int count)
    {
        _sparkleMaterialPool.Clear();

        for(int i = 0; i<count; i++)
        {
            Material mat = Instantiate(_sparkleMaterial);
            _sparkleMaterialPool.Enqueue(mat);
        }
    }

    private void InitDrawingSlotPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(DrawingSlotPrefab);
            obj.transform.SetParent(DrawingContent);
            obj.SetActive(false);
            _drawingSlotPool.Enqueue(obj);
        }
    }


    private Material GetMaterialFromPool()
    {
        if(_sparkleMaterialPool.Count > 0)
            return _sparkleMaterialPool.Dequeue();
        else
            return Instantiate(_sparkleMaterial);
    }
    private GameObject GetDrawingSlotFromPool()
    {
        if (_drawingSlotPool.Count > 0)
        {
            GameObject slot = _drawingSlotPool.Dequeue();
            slot.SetActive(true);
            return slot;
        }

        GameObject newSlot = Instantiate(DrawingSlotPrefab);
        newSlot.transform.SetParent(DrawingContent);
        return newSlot;
    }
    private void ReturnDrawingSlotToPool(GameObject slot)
    {
        // ���׸��� �ݳ�
        var shaderImg = slot.transform.Find("Shader Line").GetComponent<Image>();
        if (shaderImg.material != null)
        {
            ReturnMaterialToPool(shaderImg.material);
            shaderImg.material = null;
        }

        slot.SetActive(false);
        _drawingSlotPool.Enqueue(slot);
    }

    // Ǯ�� �ٽ� �ִ� �޼��� (�ʿ��� ���)
    private void ReturnMaterialToPool(Material mat)
    {
        _sparkleMaterialPool.Enqueue(mat);
    }

    // �ڳ� ���� �� �ҷ�����
    [System.Serializable]
    public class GachaSaveData
    {
        public int drawLevel;
        public int currentExp;
    }

    // ����
    private string gachaRowInDate = string.Empty; // inDate ����� �ʵ�

    public void SaveGachaDataToBackend()
    {
        GachaSaveData saveData = new GachaSaveData
        {
            drawLevel = _drawLevel,
            currentExp = _currentEXP
        };

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.Insert("USER_GACHA", param); // ���̺���� USER_GACHA

        if (bro.IsSuccess())
        {
            Debug.Log("��í ������ ���� ����");
            gachaRowInDate = bro.GetInDate(); // inDate ����
        }
        else
        {
            Debug.LogError("��í ������ ���� ���� : " + bro);
        }
    }

    // ����
    public void UpdateGachaDataToBackend()
    {
        if (string.IsNullOrEmpty(gachaRowInDate))
        {
            Debug.LogError("���� SaveGachaDataToBackend()�� �����͸� �����ؾ� �մϴ�.");
            return;
        }

        GachaSaveData saveData = new GachaSaveData
        {
            drawLevel = _drawLevel,
            currentExp = _currentEXP
        };

        string json = JsonUtility.ToJson(saveData);
        Param param = new Param();

        param.Add("StatData", json);

        var bro = Backend.GameData.UpdateV2("USER_GACHA", gachaRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
            Debug.Log("��í ������ ���� ����");
        else
            Debug.LogError("��í ������ ���� ����: " + bro);
    }

    // �ҷ�����
    public void LoadGachaDataFromBackend()
    {
        var bro = Backend.GameData.GetMyData("USER_GACHA", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("��í ������ �ε� ����: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();
        if (rows.Count <= 0)
        {
            Debug.Log("��í ���� ������ ����, �ű� ����");
            SaveGachaDataToBackend();
            return;
        }

        gachaRowInDate = rows[0]["inDate"].ToString(); // ������ ����� ����
        string json = rows[0]["StatData"].ToString();
        GachaSaveData saveData = JsonUtility.FromJson<GachaSaveData>(json);

        // ����� �� ����
        _drawLevel = saveData.drawLevel;
        _currentEXP = saveData.currentExp;

        // UI ������Ʈ
        UpdateChancesByLevel(_drawLevel);
        LvLine.fillAmount = (float)_currentEXP / GetExpToNextLevel(_drawLevel);
        LvText.text = _drawLevel.ToString();
        LvLineNum.text = $"{GetExpToNextLevel(_drawLevel)}/{_currentEXP}";
    }
}