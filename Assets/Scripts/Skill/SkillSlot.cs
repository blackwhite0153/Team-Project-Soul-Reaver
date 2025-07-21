using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Dictionary<int, Skill_Base> _skillData;
    [SerializeField] private List<Button> _skillSlotButtons; // �ν����Ϳ��� ���� ��ư 8�� ����

    private PlayerController _player;

    private void Start()
    {
        Setting();
    }

    private void Update()
    {
        if (!SkillManager.Instance.AutoCastEnabled) return;

        if (SkillManager.Instance.AllSkills == null) return;

        // �÷��̾ ���ų� �׾����� �ڵ� �ߵ� ����
        if (_player == null || _player.IsDeath) return;

        for (int i = 0; i < SkillManager.Instance.SkillSlots.Count; i++)
        {
            if (SkillManager.Instance.SkillLocks[i].activeSelf) return;

            var skill = SkillManager.Instance.AllSkills[i];

            if (skill.CurrentCooldown <= 0.0f)
            {
                skill.UseSkill(i);
            }
        }
    }

    private void Setting()
    {
        _skillData = new Dictionary<int, Skill_Base>();
        _player = FindAnyObjectByType<PlayerController>();

        // ��ų ���� ���ҽ� �ε�
        SkillManager.Instance.ResourceAllLoad();

        SetSlotButton();
        SetSkillIcons();
        BindSlotEvents();
    }

    private void SetSlotButton()
    {
        for (int i = 0; i < SkillManager.Instance.SkillSlots.Count; i++)
        {
            _skillSlotButtons.Add(SkillManager.Instance.SkillSlots[i].GetComponent<Button>());
        }
    }

    private void SetSkillIcons()
    {
        for (int i = 0; i < _skillSlotButtons.Count; i++)
        {
            if (i < SkillManager.Instance.AllSkills.Count)
            {
                var skill = SkillManager.Instance.AllSkills[i];
                _skillData[i] = skill;

                // ������ ����
                SkillManager.Instance.SkillSlots[i].sprite = skill.Skill_Icon;
            }
        }
    }

    private void BindSlotEvents()
    {
        for (int i = 0; i < _skillSlotButtons.Count; i++)
        {
            int slotIndex = i; // ĸó ����
            _skillSlotButtons[i].onClick.AddListener(() => SelectSkill(slotIndex));
        }
    }

    // ��ų ����
    private void SelectSkill(int index)
    {
        if (_skillData.TryGetValue(index, out var skill))
        {
            Debug.Log($"[SkillSlot] {index + 1}�� ���� Ŭ���� - ��ų�� : {skill.Skill_Name}");

            // ��ų ���
            skill.UseSkill(index);
        }
        else
        {
            Debug.LogWarning($"[SkillSlot] {index + 1}�� ���Կ� ��ϵ� ��ų�� �����ϴ�.");
        }
    }

    private void Clear()
    {
        _skillData.Clear();
    }
}