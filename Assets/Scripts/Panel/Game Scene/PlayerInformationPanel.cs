using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class UI_PlayerInformation : MonoBehaviour
{
    [SerializeField] private TMP_Text _nickNameText;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private Slider _hpSliderBar;
    [SerializeField] private Slider _mpSliderBar;

    private PlayerStats _playerStats;

    private float _displayedHp;
    private float _displayedMp;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        _playerStats = player.GetComponent<PlayerStats>();
        if (_playerStats == null) return;

        Setting();
    }

    private void Setting()
    {

        BackendReturnObject bro = Backend.BMember.GetUserInfo();

        if (bro.IsSuccess())
        {
            string nickname = bro.GetReturnValuetoJSON()["row"]["nickname"].ToString();
            _nickNameText.text = nickname;
            _characterNameText.text = nickname;
        }
        else
        {
            Debug.LogError("�г��� �������� ����: " + bro.GetMessage());
            _nickNameText.text = "���K �̸��� �����!";
        }

        _hpSliderBar.maxValue = PlayerStats.Instance.MaxHp;
        _hpSliderBar.minValue = 0.0f;

        _mpSliderBar.maxValue = PlayerStats.Instance.MaxMp;
        _mpSliderBar.minValue = 0.0f;

        _displayedHp = PlayerStats.Instance.CurrentHp;
        _displayedMp = PlayerStats.Instance.CurrentMp;
    }

    private void Update()
    {
        PassiveRegen();   // ü��/���� ȸ�� ó��
        UpdateDisplay();  // �ε巯�� �����̴� ó��
    }

    private void PassiveRegen()
    {
        float delta = Time.deltaTime;

        PlayerStats.Instance.CurrentHp = Mathf.Min(
            PlayerStats.Instance.CurrentHp + PlayerStats.Instance.HpRegen * delta,
            PlayerStats.Instance.MaxHp);

        PlayerStats.Instance.CurrentMp = Mathf.Min(
            PlayerStats.Instance.CurrentMp + PlayerStats.Instance.MpRegen * delta,
            PlayerStats.Instance.MaxMp);
    }

    private void UpdateDisplay()
    {
        _displayedHp = Mathf.Lerp(_displayedHp, PlayerStats.Instance.CurrentHp, Time.deltaTime * 10f);
        _displayedMp = Mathf.Lerp(_displayedMp, PlayerStats.Instance.CurrentMp, Time.deltaTime * 10f);

        _hpSliderBar.value = _displayedHp;
        _mpSliderBar.value = _displayedMp;
    }

}