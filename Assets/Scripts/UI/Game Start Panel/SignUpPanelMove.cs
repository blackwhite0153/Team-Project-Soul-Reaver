using UnityEngine;
using DG.Tweening;

// GameStart ���� �ִ� SignUpPanel 
public class SignUpPanelMove : MonoBehaviour
{
    public void UpMove()
    {
        SoundManager.Instance.PlaySFX("Button");

        RectTransform rect = transform as RectTransform;

        rect.DOKill();

        // Y�� -48���� 3�ʰ� �ε巴�� �̵�
        rect.DOAnchorPosY(-48f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Debug.Log("UI �г� �̵� �Ϸ�! ���� Y: " + rect.anchoredPosition.y);
        });
    }

    public void DownMove()
    {
        SoundManager.Instance.PlaySFX("Button");

        RectTransform rect = transform as RectTransform;

        rect.DOKill();

        // Y�� -48���� 3�ʰ� �ε巴�� �̵�
        rect.DOAnchorPosY(-870f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Debug.Log("UI �г� �̵� �Ϸ�! ���� Y: " + rect.anchoredPosition.y);
        });
    }
}
