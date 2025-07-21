using UnityEngine;
using UnityEngine.UI;

public class KillCountCardFlipButton : MonoBehaviour
{
    public Image fillImage;  // UI Image (Fill Ÿ������ ����)
    public int maxCount = 10; // �� ���� ������ �� ������
    private int killCount = 0;

    public bool OpenButton = false;

    public void OnMonsterKilled()
    {
        killCount++;
        if (killCount > maxCount) killCount = maxCount;

        fillImage.fillAmount = (float)killCount / maxCount;

        if(killCount >= 10)
        {
            OpenButton = true;
        }
    }

    public void ResetCount()
    {
        OpenButton = false;
        killCount = 0;
        fillImage.fillAmount = 0f;
    }
}
