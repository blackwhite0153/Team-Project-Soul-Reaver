using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WindowMenuButton : MonoBehaviour
{
    public Button OpenWindowButton;
    public Button CloseWindowButton;

    public RectTransform CurtainPanel;      // Ŀư ȿ���� �� RectTransform

    public float AnimationDuration = 0.2f;  // �ִϸ��̼� �ð�
    private bool _isOpen = false;           // Ŀư�� ���ȴ��� ����

    private void Start()
    {
        // �ʱ� ���¿��� Ŀư�� �� ���̰� ����
        CurtainPanel.localScale = new Vector3(1.0f, 0.0f, 1.0f);    // Y�� ũ�⸦ 0���� ���� (�� ���̰�)
        CurtainPanel.pivot = new Vector2(0.5f, 1.0f);               // ���� ����

        OpenWindowButton.onClick.AddListener(OpenWindow);
        CloseWindowButton.onClick.AddListener(CloseWindow);
    }

    public void OpenWindow()
    {
        if (!_isOpen)
        {
            _isOpen = true;

            SoundManager.Instance.PlaySFX("Button");

            // Y������ Ŀư�� �Ʒ��� �ø��� �ִϸ��̼� (������ ����, �Ʒ��� Ȯ��)
            CurtainPanel.DOScaleY(1f, AnimationDuration).SetEase(Ease.OutQuad); // localScale.y�� 1�� ����
        }
    }

    public void CloseWindow()
    {
        if(_isOpen)
        {
            _isOpen = false;

            SoundManager.Instance.PlaySFX("Button");

            // Y������ Ŀư�� ���� ���̴� �ִϸ��̼�
            CurtainPanel.DOScaleY(0f, AnimationDuration).SetEase(Ease.InQuad);  // localScale.y�� 1�� ����
        }
    }
}