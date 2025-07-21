using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFlashEffect : MonoBehaviour
{
    public Image GlowImage;

    public void PlayFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        //���İ� ����
        float duration = 0.15f;
        float time = 0;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / duration);
            GlowImage.color = new Color(1f, 1f, 1f, alpha);
            time += Time.deltaTime;
            yield return null;
        }
        GlowImage.color = new Color(1f, 1f, 1f, 1f);

        // �����ð� (�ణ ��¦�� ä�� ��ٸ���)
        yield return new WaitForSeconds(0.1f);

        // ���İ� ����
        time = 0f;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            GlowImage.color = new Color(1f, 1f, 1f, alpha);
            time += Time.deltaTime;
            yield return null;
        }
        GlowImage.color = new Color(1f, 1f, 1f, 0f);
    }
}
