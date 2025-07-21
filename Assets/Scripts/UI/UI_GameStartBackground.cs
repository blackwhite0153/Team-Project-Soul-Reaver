using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UI_GameStartBackground : UI_Base
{
    [SerializeField] private TMP_Text _tapToStartText;

    protected override void Initialized()
    {
        base.Initialized();

        NewSetCanvas();

        // �⺻ ������ ����
        _tapToStartText.transform.localScale = Vector3.one;

        // DOTween �ִϸ��̼� : Ŀ���� �۾����� �ݺ�
        _tapToStartText.transform
            .DOScale(1.1f, 0.6f)            // 1.1��� Ŀ���µ� 0.6�� �ɸ�
            .SetEase(Ease.InOutSine)        // �ε巴��
            .SetLoops(-1, LoopType.Yoyo);   // ���� �ݺ� + �ǵ��ƿ���
    }

    private void NewSetCanvas()
    {
        // ���� GameObject�� Canvas ������Ʈ�� �������ų� ������ �߰�
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();

        // Canvas�� �����ϴ� ��� ���� ����
        if (canvas != null)
        {
            // UI�� ȭ�� ��ǥ �������� ������
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
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