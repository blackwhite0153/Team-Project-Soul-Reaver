using UnityEngine;
using TMPro;

public class FriendSentRequestPage : FriendPageBase
{ 
    [Header("Send Request Friend")]
    [SerializeField] private TMP_InputField _inputFieldNickname;
    [SerializeField] private FadeEffect_TMP _textResult;

    private void OnEnable()
    {
        // [ģ�� ��û ���] ��� �ҷ�����
        BackEndFriend.Instance.GetSentRequestList();
    }

    private void OnDisable()
    {
        DeactivateAll();
    }

    public void OnClickRequestFriend()
    {
        SoundManager.Instance.PlaySFX("Button");

        string nickname = _inputFieldNickname.text;

        if(nickname.Trim().Equals(""))
        {
            _textResult.FadeOut("ģ�� ��û�� ���� �г����� �Է����ּ���.");
            return;
        }

        _inputFieldNickname.text = "";

        BackEndFriend.Instance.SendRequestFriend(nickname);
    }
}