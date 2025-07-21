using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMesh;
    [SerializeField] private Animator _animator;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private MeshRenderer _meshRenderer;

    private void Awake()
    {
        if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
        if (_textMesh == null) _textMesh = GetComponent<TextMeshPro>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

        _meshRenderer.sortingLayerName = "UI";     // Canvas�� Sorting Layer �̸�
        _meshRenderer.sortingOrder = 50;           // ���� ��ġ�� UI���� ���� ���
    }

    public void Show(float damage, Color color, Vector3 targetWorldPosition)
    {
        _textMesh.text = ((int)damage).ToString();
        _textMesh.color = color;

        // World Space������ ��ġ�� ���� �����ϸ� ��
        _rectTransform.position = targetWorldPosition;

        _animator.Play("DamagePopup", 0, 0.0f);
        StartCoroutine(DisableAfterAnimation());
    }

    private IEnumerator DisableAfterAnimation()
    {
        float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        PoolManager.Instance.DeactivateObj(gameObject);
    }
}
