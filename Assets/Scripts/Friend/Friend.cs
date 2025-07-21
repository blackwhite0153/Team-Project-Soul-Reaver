
public class Friend : FriendBase
{
    public override void Setup(BackEndFriend friendSystem, FriendPageBase friendPage, FriendData friendData)
    {
        base.Setup(friendSystem, friendPage, friendData);

        textTime.text = System.DateTime.Parse(friendData.lastLogin).ToString();
    }

    public void OnClickDeleteFriend()
    {
        // 模备 UI 坷宏璃飘 昏力
        friendPage.Deactivate(gameObject);
        // 模备 昏力 (Backend Console)
        backendFriendSystem.BreakFriend(friendData);

        SoundManager.Instance.PlaySFX("Button");
    }
}