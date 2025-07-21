
public class Friend : FriendBase
{
    public override void Setup(BackEndFriend friendSystem, FriendPageBase friendPage, FriendData friendData)
    {
        base.Setup(friendSystem, friendPage, friendData);

        textTime.text = System.DateTime.Parse(friendData.lastLogin).ToString();
    }

    public void OnClickDeleteFriend()
    {
        // ģ�� UI ������Ʈ ����
        friendPage.Deactivate(gameObject);
        // ģ�� ���� (Backend Console)
        backendFriendSystem.BreakFriend(friendData);

        SoundManager.Instance.PlaySFX("Button");
    }
}