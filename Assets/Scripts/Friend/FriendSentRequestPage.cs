using UnityEngine;
using TMPro;

public class FriendSentRequestPage : FriendPageBase
{ 
    [Header("Send Request Friend")]
    [SerializeField] private TMP_InputField _inputFieldNickname;
    [SerializeField] private FadeEffect_TMP _textResult;

    private void OnEnable()
    {
        // [친구 요청 대기] 목록 불러오기
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
            _textResult.FadeOut("친구 요청을 보낼 닉네임을 입력해주세요.");
            return;
        }

        _inputFieldNickname.text = "";

        BackEndFriend.Instance.SendRequestFriend(nickname);
    }
}