using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillWindowPanel : MonoBehaviour
{
    [Header("��ų ���� �θ� ������Ʈ")]
    [SerializeField] private GameObject _skillDescriptionParentObject;

    [Header("��ų ���� ����")]
    [SerializeField] private GameObject _skillDescriptionIconObject;
    [SerializeField] private TMP_Text _skillDescriptionLevelText;
    [SerializeField] private TMP_Text _skillDescriptionText;
    [SerializeField] private TMP_Text _skillCoolTimeText;
    [SerializeField] private TMP_Text _skillManaCostText;

    [Header("���� ���õ� ��ų")]
    [SerializeField] private Skill_Base _currentSkill;
    [SerializeField] private int _selectSkillIndex;

    [Header("��ų ��ȭ ����")]
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _skillGoldCostText;
    [SerializeField] private Button _skillUpgradeButton;

    [Header("��ų ��� �θ� ������Ʈ")]
    [SerializeField] private GameObject _skillListParentObject;

    [Header("��ų ��� ����")]
    [SerializeField] private GameObject _SkillIconPrefabs;
    [SerializeField] private List<TMP_Text> _skillLevelText;
    [SerializeField] private List<GameObject> _skillIconListObject;

    [Header("������ ��ų ������ ��ư")]
    [SerializeField] private List<Button> _skillListButton;

    private void Start()
    {
        ResourceAllLoad();

        Setting();

        SkillManager.Instance.LoadSkillDataFromBackend();

        GenerateSkillList();

        SelectSkill(SkillManager.Instance.AllSkills[0]);
    }

    // ���� ������Ʈ ���ҽ��� �ε�
    private void ResourceAllLoad()
    {
        _SkillIconPrefabs = Resources.Load<GameObject>(Define.Skill_Icon_Prefab_Path);

        // ��ų ����
        _skillDescriptionParentObject = GameObject.Find("Skill Description");

        _skillDescriptionIconObject = _skillDescriptionParentObject.transform.Find("Skill Description Image/Skill Description Icon Image").gameObject;
        _skillDescriptionLevelText = _skillDescriptionParentObject.transform.Find("Skill Description Level Text").GetComponent<TMP_Text>();
        _skillDescriptionText = _skillDescriptionParentObject.transform.Find("Skill Description Text").GetComponent<TMP_Text>();

        // �Ҹ� ���� & ��Ÿ��
        _skillCoolTimeText = _skillDescriptionParentObject.transform.Find("Skill CoolTime").GetComponent<TMP_Text>();
        _skillManaCostText = _skillDescriptionParentObject.transform.Find("Skill ManaCost").GetComponent<TMP_Text>();

        // ��ų ���
        _skillListParentObject = GameObject.Find("Skill List/Viewport").transform.Find("Content").gameObject;

        // ��ų ��ȭ
        _goldText = _skillDescriptionParentObject.transform.Find("Gold/Gold Text").GetComponent<TMP_Text>();
        _skillGoldCostText = _skillDescriptionParentObject.transform.Find("Skill Upgrade Button/Gold/Skill Upgrade Gold Cost Text").GetComponent<TMP_Text>();
        _skillUpgradeButton = _skillDescriptionParentObject.transform.Find("Skill Upgrade Button").GetComponent<Button>();
    }

    // �ʱ� ����
    private void Setting()
    {
        _skillLevelText = new List<TMP_Text>();
        _skillIconListObject = new List<GameObject>();

        _skillListButton = new List<Button>();

        _skillUpgradeButton.onClick.AddListener(() => OnUpgradeSkillClicked(_currentSkill));
    }

    // ��ų ����Ʈ �߰�
    private void GenerateSkillList()
    {
        _skillLevelText.Clear();
        _skillIconListObject.Clear();
        _skillListButton.Clear();

        var allSkills = SkillManager.Instance.AllSkills;

        for (int i = 0; i < allSkills.Count; i++)
        {
            // i��° ��ų ������ �޾ƿ���
            var skill = allSkills[i];

            // ������ ������Ʈ ����
            var iconObj = Instantiate(_SkillIconPrefabs, _skillListParentObject.transform);

            // ������ ��ų ������Ʈ ����
            iconObj.name = $"Skill {i + 1}";
            iconObj.transform.localScale = Vector3.one;
            iconObj.transform.localPosition = Vector3.zero;

            // ��ų ������ ��ư �޾ƿ���
            var button = iconObj.GetComponent<Button>();
            button.onClick.AddListener(() => SelectSkill(skill));

            // ������ ���� ������Ʈ �޾ƿ���
            var levelText = iconObj.transform.Find("Skill Level Text").GetComponent<TMP_Text>();
            var iconImage = iconObj.transform.Find("Skill Icon Image").GetComponent<Image>();

            // �޾ƿ� ������Ʈ ����
            levelText.text = $"Lv.{skill.Skill_Level}";
            iconImage.sprite = skill.Skill_Icon;

            // �߰�
            _skillIconListObject.Add(iconObj);
            _skillListButton.Add(button);
            _skillLevelText.Add(levelText);
        }
    }

    // ��ų ����
    private void SelectSkill(Skill_Base skill)
    {
        SoundManager.Instance.PlaySFX("Button");

        var allSkills = SkillManager.Instance.AllSkills;

        _currentSkill = skill;

        for (int i = 0; i < allSkills.Count; i++)
        {
            if (allSkills[i] == skill)
            {
                _selectSkillIndex = i;
                break;
            }
        }

        UpdateSkillDescription(skill);
    }

    // ���� ��ų ���� ������Ʈ
    private void UpdateSkillDescription(Skill_Base skill)
    {
        _goldText.text = $"{GameManager.Instance.GoldNum}G";
        _skillGoldCostText.text = $"{skill.GoldCost}G";

        _skillDescriptionIconObject.GetComponent<Image>().sprite = skill.Skill_Icon;
        _skillDescriptionLevelText.text = $"LV.{skill.Skill_Level}";
        _skillDescriptionText.text = $"{skill.Skill_Explanation}";

        _skillCoolTimeText.text = $"��Ÿ�� : {skill.CooldownTime:F1}";
        _skillManaCostText.text = $"�Ҹ� ���� : {skill.ManaCost:F1}";
    }

    // ��ų ���׷��̵�
    private void OnUpgradeSkillClicked(Skill_Base skill)
    {
        SoundManager.Instance.PlaySFX("Button");

        if (_currentSkill == null) return;

        if (GameManager.Instance.GoldNum < _currentSkill.GoldCost)
            return;

        GameManager.Instance.GoldNum -= skill.GoldCost;

        skill.Skill_Level++;
        skill.GoldCost += 1000;

        skill.RefreshExplanation();

        UpdateSkillDescription(_currentSkill);
        UpdateSkillIconLevelText(_selectSkillIndex, _currentSkill.Skill_Level);

        SkillManager.Instance.UpdateSkillDataToBackend();
    }

    private void UpdateSkillIconLevelText(int index, int level)
    {
        if (index >= 0 && index < _skillLevelText.Count)
            _skillLevelText[index].text = $"Lv.{level}";
    }
}