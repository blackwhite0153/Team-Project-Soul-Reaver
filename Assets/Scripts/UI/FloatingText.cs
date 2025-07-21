using UnityEngine;
using TMPro;

// ���� �ð� ���� ���� �������� ���� ������� �ؽ�Ʈ�� ǥ���ϴ� Ŭ����

public class FloatingText : MonoBehaviour
{
    private float _moveSpeed = 50f;         // ���� �̵��ϴ� �ӵ�
    private float _fadeDuration = 0.5f;     // ���������� �ð�
    private float _startDelay = 0.1f;       // ���� �� ��� �ð�

    private TMP_Text _text;
    private RectTransform _rect;
    private CanvasGroup _canvasGroup;

    private float _timer;
    private bool _iswaiting;

    private Vector2 _startPosition;         // ó�� ��ġ ����

    // ������Ʈ�� �����ǰų� ���� �� �����
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();                        // �ؽ�Ʈ ������Ʈ ��������
        _rect = GetComponent<RectTransform>();                   // ��ġ ������ ���� RectTransform
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();   // ���� ������ ���� CanvasGroup �߰�

        _startPosition = _rect.anchoredPosition;                 // ó����ġ ����
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // �ܺο��� ȣ���Ͽ� �ؽ�Ʈ�� ǥ���ϴ� �Լ�
    public void Show(string message)
    {
        _text.text = message;          // ǥ���� �޽��� ����
        _timer = 0f;                   // Ÿ�̸� �ʱ�ȭ
        _iswaiting = true;
        _canvasGroup.alpha = 1f;       // ���� �ʱ�ȭ (���� ���̰�)
        _rect.anchoredPosition = _startPosition;    // ��ġ �ʱ�ȭ
        gameObject.SetActive(true);   // ������Ʈ Ȱ��ȭ
    }

    // �� �����Ӹ��� ȣ��Ǵ� �Լ�
    private void Update()
    {
        _timer += Time.deltaTime;      // Ÿ�̸� ����

        if(_iswaiting)
        {
            // ��� �ð� �� ������ ��
            if (_timer < _startDelay)
                return;
            // ��� �ð� ����, �ִϸ��̼� ����
            _iswaiting = false;
            _timer = 0f; // �ִϸ��̼� Ÿ�̸� �ʱ�ȭ
        }

        // �ؽ�Ʈ�� ���� �̵�
        _rect.anchoredPosition += Vector2.up * _moveSpeed * Time.deltaTime;

        // ���� �����ϰ� (�ð��� ���� ���İ� ����)
        _canvasGroup.alpha = Mathf.Lerp(1f, 0f, _timer / _fadeDuration);

        // ���� �ð� ������ ��Ȱ��ȭ (�ٽ� ����� �� �ֵ���)
        if (_timer > _fadeDuration)
        {
            gameObject.SetActive(false);
        }
    }
}