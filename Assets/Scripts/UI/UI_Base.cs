using UnityEngine;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour
{
    private void Awake()
    {
        Initialized();
    }

    protected virtual void Initialized()
    {
        SetCanvas();
    }

    private void SetCanvas()
    {
        // ���� GameObject�� Canvas ������Ʈ�� �������ų� ������ �߰�
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();

        // Canvas�� �����ϴ� ��� ���� ����
        if (canvas != null)
        {
            // UI�� ȭ�� ��ǥ �������� ������
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            // ���� �켱 ������ �������� ���� �����ϵ��� ����
            canvas.overrideSorting = true;
        }

        // ���� GameObject�� CanvasScaler ������Ʈ�� �������ų� ������ �߰�
        CanvasScaler canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
        // UI Scale Mode�� Scale With Screen Size�� ����
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        // Reference Resolution�� FHD (1920 x 1080) ������ ����
        canvasScaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        // Screen Match Mode�� Match Width Or Height�� ����
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    }
}