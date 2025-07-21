using UnityEngine;
using System;
using System.Collections.Generic;
using BackEnd;

public class BackEndFriend : Singleton<BackEndFriend>
{
    [SerializeField] private FriendSentRequestPage sentRequestPage;
    [SerializeField] private FriendReceivedRequestPage receivedRequestPage;
    [SerializeField] private FriendPage friendPage;

    private void Start()
    {
        if (friendPage == null)
            friendPage = FindAnyObjectByType<FriendPage>();

        if (sentRequestPage == null)
            sentRequestPage = FindAnyObjectByType<FriendSentRequestPage>();

        if (receivedRequestPage == null)
            receivedRequestPage = FindAnyObjectByType<FriendReceivedRequestPage>();
    }

    private string GetUserInfoBy(string nickname)
    {
        // 해당 닉네임의 유저가 존재하는지 여부는 동기로 진행
        var bro = Backend.Social.GetUserInfoByNickName(nickname);
        string inDate = string.Empty;

        if (!bro.IsSuccess())
        {
            Debug.LogError($"유저 검색 도중 에러가 발생했습니다. :{bro}");
            return inDate;
        }

        // JSON 데이터 피싱 성공
        try
        {
            LitJson.JsonData jsonData = bro.GetFlattenJSON()["row"];

            // 받아온 데이터의 개수가 0이면 데이터가 없는 것
            if (jsonData == null || jsonData.Count <= 0)
            {
                Debug.LogError("유저의 inDate 데이터가 없습니다.");
                return inDate;
            }

            inDate = jsonData["inDate"].ToString();

            Debug.Log($"{nickname}의 inDate 값은 {inDate} 입니다.");
        }
        // JSON 데이터 파싱 실패
        catch (Exception e)
        {
            // try-catch 에러 출력
            Debug.LogError(e);
        }

        return inDate;
    }

    public void SendRequestFriend(string nickname)
    {
        // RequestFriend() 메소드를 이용해 친구 추가 요청을 할 때 해당 친구의 inDate 정보가 필요
        string inDate = GetUserInfoBy(nickname);
        if (string.IsNullOrEmpty(inDate))
            return;

        // 먼저 보낸 요청 대기 목록을 가져와 중복 검사
        Backend.Friend.GetSentRequestList(sentCallback =>
        {
            if (!sentCallback.IsSuccess())
            {
                Debug.LogError($"친구 요청 대기 목록 조회 도중 에러가 발생했습니다. : {sentCallback}");
                return;
            }

            // 이미 보낸 inDate들을 수집
            var rows = sentCallback.GetFlattenJSON()["rows"];
            var sentInDates = new HashSet<string>();
            foreach (LitJson.JsonData item in rows)
                sentInDates.Add(item["inDate"].ToString());

            if (sentInDates.Contains(inDate))
            {
                Debug.LogWarning($"{nickname}님께 이미 친구 요청을 보냈습니다.");
                return;
            }

            // 중복이 아니면 실제 친구 요청
            Backend.Friend.RequestFriend(inDate, callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError($"{nickname} 친구 요청 도중 에러가 발생했습니다. : {callback}");
                    return;
                }

                Debug.Log($"친구 요청에 성공했습니다. : {callback}");

                // 친구 요청에 성공하면 친구 요청 대기 목록 불러오기
                GetSentRequestList();
            });
        });
    }

    public void GetSentRequestList()
    {
        Backend.Friend.GetSentRequestList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 요청 대기 목록 조회 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            // JSON 데이터 파싱 성공
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("친구 요청 대기 목록 데이터가 없습니다.");
                    return;
                }

                // 친구 요청 대기 목록에 있는 모든 UI 비활성화
                sentRequestPage.DeactivateAll();

                foreach (LitJson.JsonData item in jsonData)
                {
                    FriendData friendData = new FriendData();

                    //friend.nickname        = item.ContainsKey("nickname") == true ? item["nickname"].ToString() : "NONAME";
                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";
                    friendData.inDate = item["inDate"].ToString();
                    friendData.createdAt = item["createdAt"].ToString();

                    // [친구요청]을 보낸 시간으로부터 일정 기간이 지났다면 자동으로 친구 요청 취소
                    if (IsExpirationDate(friendData.createdAt))
                    {
                        RevokeSentRequest(friendData.inDate);
                        continue;
                    }

                    // 현재 friend 정보를 바탕으로 친구 요청 대기 UI활성화
                    sentRequestPage.Activate(friendData);
                }
            }
            // JSON 데이터 파싱 실패
            catch (Exception e)
            {
                // try - catch 에러 출력
                Debug.LogError(e);
            }
        });
    }

    public void RevokeSentRequest(string inDate)
    {
        Backend.Friend.RevokeSentRequest(inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 요청 취소 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            Debug.Log($"친구 요청 취소에 성공했습니다. : {callback}");

            // 친구 요청 취소 후 목록 갱신
            GetSentRequestList();
        });
    }

    public void GetReceivedRequestList()
    {
        Backend.Friend.GetReceivedRequestList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 수락 대기 목록 조회 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            // JSON 데이터 파싱 성공
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // 받아온 데이터의 개수가 0이면 데이터가 없는것
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("친구 수락 대기 목록 데이터가 없습니다.");
                    return;
                }

                receivedRequestPage.DeactivateAll();

                foreach (LitJson.JsonData item in jsonData)
                {
                    FriendData friendData = new FriendData();

                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";
                    friendData.inDate = item["inDate"].ToString();
                    friendData.createdAt = item["createdAt"].ToString();

                    if (IsExpirationDate(friendData.createdAt))
                    {
                        RejectFriend(friendData);
                        continue;
                    }

                    receivedRequestPage.Activate(friendData);
                }
            }
            // JSON 데이터 파싱 실패
            catch (Exception e)
            {
                // try-catch 에러 출력
                Debug.LogError(e);
            }
        });
    }

    public void AcceptFriend(FriendData friendData)
    {
        Backend.Friend.AcceptFriend(friendData.inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 수락 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            Debug.Log($"{friendData.nickname}님과 친구가 되었습니다. : {callback}");

            // 수락 후 받은 요청 갱신
            GetReceivedRequestList();
            GetFriendList(); // 친구 목록 새로고침
        });
    }

    public void RejectFriend(FriendData friendData)
    {
        Backend.Friend.RejectFriend(friendData.inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 거절 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            Debug.Log($"{friendData.nickname}님 친구 요청을 거절했습니다. : {callback}");

            // 거절 후 받은 요청 갱신
            GetReceivedRequestList();
        });
    }

    public void GetFriendList()
    {
        Backend.Friend.GetFriendList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 목록 조회 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            // JSON 데이터 파싱 성공
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("친구 목록 데이터가 없습니다.");
                    return;
                }

                // 친구 목록에 있는 모든 UI 비활성화
                friendPage.DeactivateAll();

                List<TransactionValue> transactionList = new List<TransactionValue>();
                List<FriendData> friendDataList = new List<FriendData>();

                foreach (LitJson.JsonData item in jsonData)
                {
                    FriendData friendData = new FriendData();

                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";
                    friendData.inDate = item["inDate"].ToString();
                    friendData.createdAt = item["createdAt"].ToString();
                    friendData.lastLogin = item["lastLogin"].ToString();

                    friendDataList.Add(friendData);

                    // friendData.inDate의 UserGameData 조회를 위해 Transaction 요청 구성
                    Where where = new Where();
                    where.Equal("owner_inDate", friendData.inDate);
                    transactionList.Add(TransactionValue.SetGet(Define.User_Data_Table, where));
                }

                // 최신 뒤끝서버 5.18.0 기준 TransactionReadV2 사용
                Backend.GameData.TransactionReadV2(transactionList, transactionCallback =>
                {
                    if (!transactionCallback.IsSuccess())
                    {
                        Debug.LogError($"Transaction Error : {transactionCallback}");
                        return;
                    }

                    // GetReturnValuetoJSON() 사용: 원본 JSON 접근
                    LitJson.JsonData fullJson = transactionCallback.GetReturnValuetoJSON();

                    if (!fullJson.ContainsKey("Responses"))
                    {
                        Debug.LogWarning("Transaction 응답에 'Responses' 키가 없습니다.");
                        return;
                    }

                    LitJson.JsonData responses = fullJson["Responses"];

                    for (int i = 0; i < friendDataList.Count; i++)
                    {
                        var resp = responses[i];
                        if (!resp.ContainsKey("Get") || !resp["Get"].ContainsKey(Define.User_Data_Table))
                        {
                            Debug.LogWarning($"[{i}] 친구의 유저데이터가 존재하지 않음.");
                            friendPage.Activate(friendDataList[i]);
                            continue;
                        }

                        var rows = resp["Get"][Define.User_Data_Table]["rows"];
                        if (rows != null && rows.Count > 0)
                        {
                            var row = rows[0];
                            if (row.ContainsKey("nickname"))
                            {
                                friendDataList[i].nickname = row["nickname"].ToString();
                            }
                            // TODO: 레벨, 프로필 등 추가 필드 반영
                        }
                    }

                    friendPage.ActivateAll(friendDataList);
                });
            }
            // JSON 데이터 파싱 실패
            catch (Exception e)
            {
                // try-catch 에러 출력
                Debug.LogError(e);
            }
        });
    }

    public void BreakFriend(FriendData friendData)
    {
        Backend.Friend.BreakFriend(friendData.inDate, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("친구 삭제 성공");
                // 친구 리스트 재갱신
                GetFriendList();
            }
            else
            {
                Debug.LogError($"친구 삭제 도중 에러가 발생했습니다. : {callback}");

                // NotFound 예외 처리 (이미 삭제된 상태 등)
                if (callback.GetStatusCode() == "404" && callback.GetErrorCode() == "NotFoundException")
                {
                    Debug.LogWarning("이미 삭제된 친구입니다. UI에서 제거만 처리합니다.");
                    // UI에서 제거만 진행
                    GetFriendList(); // 전체 갱신 추천
                }
            }
        });
    }

    private bool IsExpirationDate(string createdAt)
    {
        // GetServerTime() - 서버 시간 불러오기
        var bro = Backend.Utils.GetServerTime();

        if (!bro.IsSuccess())
        {
            Debug.LogError($"서버 시간 불러오기에 실패했습니다. : {bro}");
            return false;
        }

        // JSON 데이터 파싱 성공
        try
        {
            // createdAt 시간으로부터 3일 뒤의 시간
            DateTime after3Days = DateTime.Parse(createdAt).AddDays(Define.Expiration_Days);

            // 현재 서버 시간
            string serverTime = bro.GetFlattenJSON()["utcTime"]?.ToString();
            if (string.IsNullOrEmpty(serverTime))
            {
                Debug.LogError("서버 시간이 비어 있습니다.");
                return false;
            }

            // 만료까지 남은 시간 = 만료 시간 - 현재 서버 시간
            TimeSpan timeSpan = after3Days - DateTime.Parse(serverTime);

            if (timeSpan.TotalHours < 0)
            {
                return true;
            }
        }
        // JSON 파싱 실패
        catch (Exception e)
        {
            // try-catch 에러 출력
            Debug.LogError(e);
        }

        return false;
    }
}