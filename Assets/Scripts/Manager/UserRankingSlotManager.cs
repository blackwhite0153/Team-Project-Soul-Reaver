using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRankingSlotManager : Singleton<UserRankingSlotManager>
{
    [SerializeField] private Transform rankingContentParent; // Content ������Ʈ
    [SerializeField] private GameObject rankingSlotPrefab;   // ������
    [SerializeField] private int rankCount = 2;              // �� �� �������
    [SerializeField] private Sprite[] rankIcons;             // 1~3�� �̹��� ��

    private string leaderboardUUID = "0197ce1b-cdd0-706f-b1be-c1c33b84a626";

    public void LoadRanking()
    {
        var bro = Backend.Leaderboard.User.GetLeaderboard(leaderboardUUID, rankCount);

        Backend.Leaderboard.User.GetLeaderboard("0197ce1b-cdd0-706f-b1be-c1c33b84a626");

        if (!bro.IsSuccess())
        {
            Debug.LogError("��ŷ �ҷ����� ����: " + bro);
            return;
        }

        Debug.Log("��ŷ �ҷ����� ����");

        var rows = bro.FlattenRows();
        Debug.Log("rows.Count: " + rows.Count);


        foreach (Transform child in rankingContentParent)
            Destroy(child.gameObject); // ������ �ִ� ��� ����

        for(int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            string nickname = row["nickname"].ToString();
            int rank = int.Parse(row["rank"].ToString());
            int Wave = int.Parse(row["score"].ToString());

            GameObject slot = Instantiate(rankingSlotPrefab, rankingContentParent);
            Debug.Log("������ ������: " + slot.name);

            // ������Ʈ �����ͼ� ����
            TMP_Text rankText = slot.transform.Find("Ranking Obj/Ranking Text").GetComponent<TMP_Text>();
            TMP_Text nicknameText = slot.transform.Find("NickName Obj/NickName Text").GetComponent<TMP_Text>();
            TMP_Text WaveText = slot.transform.Find("Wave Obj/Wave Text").GetComponent<TMP_Text>();
            Image iconImage = slot.transform.Find("Ranking Obj/Ranking Image").GetComponent<Image>();
            Button friendButton = slot.transform.Find("Friend Buttom").GetComponent<Button>();

            rankText.text = $"{rank}";
            nicknameText.text = nickname;
            WaveText.text = $"{Wave}";

            // ������ ���� (1~3����)
            if (rank <= 3 && rankIcons.Length >= rank)
                iconImage.sprite = rankIcons[rank - 1];
            else
                iconImage.gameObject.SetActive(false); // �⺻ ��Ȱ��

            friendButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                string nickname = nicknameText.text;

                BackEndFriend.Instance.SendRequestFriend(nickname);
            });
        }
    }
}
