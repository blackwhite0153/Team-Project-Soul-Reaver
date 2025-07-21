using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlidCard : MonoBehaviour
{
    public GameObject CardBack;
    public bool CardBackIsActive;

    private float _flipDuration = 0.1f;

    public bool IsFlipping { get; private set; }

    private BuffData currentBuff;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnClick);
    }

    public void SetupCard(BuffData buff)
    {
        currentBuff = buff;
        if (_button != null)
            _button.interactable = true; // �ٽ� Ŭ�� �����ϵ���
    }

    public void OnClick()
    {
        if (currentBuff != null)
        {
            BuffManager.Instance.ApplyBuff(currentBuff);
            Debug.Log($"���� �����: {currentBuff.buffName}");

            // �� �� Ŭ���ϸ� ��Ȱ��ȭ
            if (_button != null)
                _button.interactable = false;

            CardFlipManager.Instance.EndFlipCards();
        }
        else
        {
            Debug.LogWarning("BuffData�� �������� �ʾҽ��ϴ�.");
        }
    }

    public IEnumerator FlipCard()
    {
        IsFlipping = true;

        float elapsedTime = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, CardBackIsActive ? 0 : 180, 0);

        while (elapsedTime < _flipDuration)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / _flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;

        // �յ� ��ȯ
        CardBackIsActive = !CardBackIsActive;
        CardBack.SetActive(CardBackIsActive);

        IsFlipping = false;
    }
}
