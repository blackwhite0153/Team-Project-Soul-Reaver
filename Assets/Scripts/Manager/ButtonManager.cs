using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button _autoSkillButton;
    private Image _autoButtonSprite;

    // �� �� ��ư�� ���� ������ ���� Ŭ����
    [System.Serializable]
    public class TabButton
    {
        public Button button;                    // ������ ����� ��ư ������Ʈ
        public TextMeshProUGUI text;             // ��ư �� �ؽ�Ʈ (TextMeshPro)
        public GameObject contentPanel;          // �� Ŭ�� �� ������ ������ ��� �г�
    }

    public List<TabButton> tabButtons;           // ������ �� ��ư ����Ʈ

    // ���õ� �� ��ư�� ������
    private Color selectedButtonColor = new Color(0.3529412f, 0.282353f, 0.3411765f, 1f);
    // ���õ� �� ��ư�� �ؽ�Ʈ ����
    private Color selectedTextColor = new Color(0.9490196f, 0.772549f, 0.6509804f, 1f);

    // ���� �� �� �� ��ư ������
    private Color unselectedButtonColor = new Color(0.07843138f, 0.06666667f, 0.1215686f, 0.8784314f);
    // ���� �� �� �� ��ư �ؽ�Ʈ ����
    private Color unselectedTextColor = new Color(0.3301887f, 0.3099413f, 0.3099413f, 1f);

    private TabButton currentSelected;          // ���� ���õ� �� ��ư �����

    // �ʱ�ȭ �Լ�
    private void Start()
    {
        _autoSkillButton.onClick.AddListener(OnAutoSkillButtonClick);

        // ��� �� ��ư�� Ŭ�� �̺�Ʈ ������ ���
        foreach (var tab in tabButtons)
        {
            // ���ٽ� ���, Ŭ�� �� �ش� �� ó�� �Լ� ȣ��
            tab.button.onClick.AddListener(() => OnTabClicked(tab));
        }

        // �ʱ� �� ����: ����Ʈ�� ��ư�� �ϳ��� ������ ù ��° ��ư ����
        if (tabButtons.Count > 0)
            OnTabClicked(tabButtons[0], playSound : false);

        _autoButtonSprite = _autoSkillButton.GetComponent<Image>();
        _autoButtonSprite.color = Color.yellow;
    }

    // �� Ŭ�� �� ����Ǵ� �Լ�
    private void OnTabClicked(TabButton clickedTab, bool playSound = true)
    {
        // ��� ���� ��ȸ�ϸ鼭
        foreach (var tab in tabButtons)
        {
            // Ŭ���� �ǰ� ���� ��ȸ ���� ���� ������ ����
            bool isSelected = (tab == clickedTab);

            if (isSelected && playSound)
            {
                SoundManager.Instance.PlaySFX("Button");
            }

            // ��ư ���� ���� (targetGraphic�� ���� Image ������Ʈ)
            var targetGraphic = tab.button.targetGraphic;

            if (targetGraphic != null)
            {
                targetGraphic.color = isSelected ? selectedButtonColor : unselectedButtonColor;
            }

            // ��ư �ؽ�Ʈ ���� ����
            if (tab.text != null)
            {
                tab.text.color = isSelected ? selectedTextColor : unselectedTextColor;
            }

            // �ش� �ǿ� ����� ������ �г� Ȱ��/��Ȱ�� ����
            if (tab.contentPanel != null)
            {
                tab.contentPanel.SetActive(isSelected);
            }
        }

        // ���� ���õ� �� ������Ʈ
        currentSelected = clickedTab;
    }

    private void OnAutoSkillButtonClick()
    {
        if (SkillManager.Instance.AutoCastEnabled)
        {
            SkillManager.Instance.AutoCastEnabled = false;
            _autoButtonSprite.color = Color.white;
        }
        else
        {
            SkillManager.Instance.AutoCastEnabled = true;
            _autoButtonSprite.color = Color.yellow;
        }
    }

    // Ư�� �г��� �ݴ� �Լ�
    public void ClosePanel(GameObject panelToClose)
    {
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
        }
    }

    // Ư�� �г��� ���� �Լ�
    public void OpenPanel(GameObject panelToOpen)
    {
        if (panelToOpen != null)
        {
            panelToOpen.SetActive(true);
        }
    }
}