using BackEnd;
using LitJson;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Vector3 _moveDirection;

    private event Action<Vector3> OnMoveDirectionChanged;

    private string gameRowInDate = string.Empty; // ���������� inDate ����

    [SerializeField] private int _goldNum;
    [SerializeField] private int _gpNum;

    public GameObject Target { get; set; }

    public Vector3 MoveDirection
    {
        get { return _moveDirection; }
        set
        {
            _moveDirection = value;
            OnMoveDirectionChanged?.Invoke(value);
        }
    }

    public int GoldNum
    {
        get { return _goldNum; }
        set { _goldNum = value; }
    }

    public int GpNum
    {
        get { return _gpNum; }
        set { _gpNum = value; }
    }

    protected override void Initialized()
    {
        base.Initialized();

        Setting();
    }

    private void Awake()
    {
        // ������ �ʱ�ȭ (ItemDatabase�� ���� �����ϴ��� �ݵ�� Ȯ��!)
        var db = FindAnyObjectByType<ItemDatabase>();

        if (db == null)
        {
            Debug.LogError("���� ItemDatabase�� �������� �ʽ��ϴ�!");
        }
    }

    private IEnumerator Start()
    {
        yield return null; // �� ������ ����, �ٸ� �̱��� �ʱ�ȭ ��ٸ�

        SoundManager.Instance.PlayBGM("Night Market");

        Backend.GameData.GetMyData("USER_DATA", new Where(), callback => {
            if (callback.IsSuccess() && callback.FlattenRows().Count == 0)
            {
                StageManager.Instance.SaveStageDataToBackend();
                StatUpgradeManager.Instance.SaveStatsToBackend();
                InventoryManager.Instance.SaveInventoryToBackend();
                DrawingPanelManager.Instance.SaveGachaDataToBackend();
                QuestManager.Instance.LoadAllQuests();
                SaveGameDataToBackend();
                Setting();

            }
            else
            {
                StageManager.Instance.LoadStageDataFromBackend();
                StatUpgradeManager.Instance.LoadStatsFromBackend();
                InventoryManager.Instance.LoadInventoryFromBackend(); // �� ������ Instance�� null�̸� �ȵ�
                DrawingPanelManager.Instance.LoadGachaDataFromBackend();
                QuestManager.Instance.LoadAllQuests();

                SkillManager.Instance.LoadSkillDataFromBackend();
                LoadGameDataFromBackend();
            }
        });
    }

    private void Setting()
    {
        _goldNum = 0;
        _gpNum = 0;
    }

    public void LoseMoney(int money)
    {
        _goldNum -= money;
        UpdateGameDataToBackend();
    }

    public void LoseGp(int gp)
    {
        _gpNum -= gp;
        UpdateGameDataToBackend();
    }

    public void GetMoney(int money)
    {
        _goldNum += money;
        UpdateGameDataToBackend();

    }

    public void GetGp(int gp)
    {
        _gpNum += gp;
        UpdateGameDataToBackend();
    }

    // ����
    public void SaveGameDataToBackend()
    {
        GameSaveData data = new GameSaveData
        {
            gold = _goldNum,
            gp = _gpNum
        };

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        BackendReturnObject bro = Backend.GameData.Insert("USER_MONEY", param); // ���̺���� USER_GAME

        if (bro.IsSuccess())
        {
            Debug.Log("���� ������ ���� ����");
            gameRowInDate = bro.GetInDate(); // ������ inDate ����
        }
        else
        {
            Debug.LogError("���� ������ ���� ����: " + bro);
        }
    }

    // ����
    public void UpdateGameDataToBackend()
    {
        if (string.IsNullOrEmpty(gameRowInDate))
        {
            Debug.LogError("���� SaveGameDataToBackend()�� ������ �ʱ� �����͸� �����ؾ� �մϴ�.");
            return;
        }

        GameSaveData data = new GameSaveData
        {
            gold = _goldNum,
            gp = _gpNum
        };

        string json = JsonUtility.ToJson(data);
        Param param = new Param();
        param.Add("StatData", json);

        BackendReturnObject bro = Backend.GameData.UpdateV2("USER_MONEY", gameRowInDate, Backend.UserInDate, param);

        if (bro.IsSuccess())
            Debug.Log("���� ������ ���� ����");
        else
            Debug.LogError("���� ������ ���� ����: " + bro);
    }

    // �ҷ�����
    public void LoadGameDataFromBackend()
    {
        BackendReturnObject bro = Backend.GameData.GetMyData("USER_MONEY", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("���� ������ �ε� ����: " + bro);
            return;
        }

        JsonData rows = bro.FlattenRows();
        if (rows.Count <= 0)
        {
            Debug.Log("���� ������ ����");
            SaveGameDataToBackend();
            return;
        }

        gameRowInDate = rows[0]["inDate"].ToString(); // ������ inDate ����
        string json = rows[0]["StatData"].ToString();

        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        _goldNum = data.gold;
        _gpNum = data.gp;
    }
}

// �ڳ� ���� �� �ҷ�����

[Serializable]
public class GameSaveData
{
    public int gold;
    public int gp;
}