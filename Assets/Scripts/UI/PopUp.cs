using UnityEngine;
using DG.Tweening;

public class PopUp : MonoBehaviour
{
    // DOTween ��ġ�ؼ� ������� �ȵǸ� ���¿��� �˻� �� ���� �ٿ�
    private float Duration = 0.2f;   // Ŀ���� �ð�

    public void PlayGrow(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);

        gameObject.transform.DOScale(Vector3.one, Duration)
            .SetEase(Ease.OutBack);
    }

    // SetEase�� ����
    // Ease.Linear(������ �ӵ�)
    // Ease.InOutQuad(�ε巯�� ���۰� ��)
    // Ease.OutBack(���� �� ƨ��� Ȯ��)
}