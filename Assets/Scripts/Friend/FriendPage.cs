

public class FriendPage : FriendPageBase
{
    private void OnEnable()
    {
        // [ģ��] ��� �ҷ�����
        BackEndFriend.Instance.GetFriendList();
    }

    private void OnDisable()
    {
        DeactivateAll();
    }
}