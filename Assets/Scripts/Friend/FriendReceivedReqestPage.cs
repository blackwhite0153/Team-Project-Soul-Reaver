
public class FriendReceivedRequestPage : FriendPageBase
{
    private void OnEnable()
    {
        // [ģ�� ���� ���] ��� �ҷ����� 
        BackEndFriend.Instance.GetReceivedRequestList();
    }

    private void OnDisable()
    {
        DeactivateAll();
    }
}