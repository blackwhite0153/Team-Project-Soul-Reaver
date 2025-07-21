using UnityEngine;
using BackEnd;

// �������忡 ����ϴ� Manager
public class RankingManager : Singleton<RankingManager>
{
    public void UpdateUserLeaderboardScore(int stageWave)
    {
        string leaderboardName = "0197ce1b-cdd0-706f-b1be-c1c33b84a626";

        string tableName = "USER_STAGEWAVE";
        string rowInDate = string.Empty;

        Debug.Log("������ ��ȸ �õ�");

        var bro = Backend.GameData.GetMyData(tableName, new Where());

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("������ ��ȸ �� ������ �߻��߽��ϴ�. : " + bro);
            return;
        }

        Debug.Log("������ ��ȸ ����" + bro);

        if (bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.Log("�����Ͱ� �������� �ʽ��ϴ�. ������ ������ �õ��մϴ�.");
            var bro2 = Backend.GameData.Insert(tableName);

            if (bro2.IsSuccess() == false)
            {
                Debug.LogError("������ ���� �� ������ �߻��߽��ϴ�. : " + bro2);
                return;
            }
            Debug.Log("������ ���Կ� �����߽��ϴ�. : " + bro2);

            rowInDate = bro2.GetInDate();
        }
        Debug.Log("�� ���� ������ rowInDate : " + rowInDate); // ����� rowIndate�� ���� ������ �����ϴ�.  

        Param param = new Param();
        param.Add("StatWave", stageWave);
        param.Add("score", stageWave);

        Debug.Log("��ŷ(��������) ���� �õ�");
        var rankBro = Backend.Leaderboard.User.UpdateMyDataAndRefreshLeaderboard(leaderboardName, tableName, rowInDate, param);

        if (rankBro.IsSuccess() == false)
        {
            Debug.LogError("��ŷ ��� �� ������ �߻��߽��ϴ�. : " + rankBro);
            return;
        }

        Debug.Log("��ŷ ���Կ� �����߽��ϴ�. : " + rankBro);
    }
}