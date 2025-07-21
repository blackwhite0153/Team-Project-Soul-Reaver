using BackEnd;
using System;
using System.Collections.Generic;
using UnityEngine;

// �ڳ� ������ ���� ����(Notice) �ý����� �����ϴ� Ŭ����
public class BackendNotice : Singleton<BackendNotice>
{
    [SerializeField] private List<Notice> _noticeList = new List<Notice>();    // ���� ���ÿ� ����� ���� ����Ʈ

    public List<Notice> GetNoticeList()
    {
        return _noticeList;
    }

    public void NoticeListGet()
    {
        var bro = Backend.Notice.NoticeList();  // ���� ���� ��� ��û

        if (!bro.IsSuccess())
        {
            Debug.LogError("���� ���� �ҷ����� �� ������ �߻��߽��ϴ�.");
            return;
        }

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ����Ʈ �ҷ����� ��û�� �����߽��ϴ�. : " + bro);

            LitJson.JsonData jsonList = bro.FlattenRows();

            for (int i = 0; i < jsonList.Count; i++)
            {
                string inDate = jsonList[i]["inDate"].ToString();

                // �ߺ� ����: inDate�� �̹� �ִ��� Ȯ��
                bool alreadyExists = _noticeList.Exists(n => n.InDate == inDate);

                if (alreadyExists)
                {
                    continue; // �̹� �߰��� ������� �ǳʶ�
                }

                Notice notice = new Notice();

                notice.Title = jsonList[i]["title"].ToString();
                notice.Content = jsonList[i]["content"].ToString();
                notice.PostingDate = DateTime.Parse(jsonList[i]["postingDate"].ToString());
                notice.InDate = jsonList[i]["inDate"].ToString();
                notice.UUid = jsonList[i]["uuid"].ToString();
                notice.IsPublic = jsonList[i]["isPublic"].ToString() == "y" ? true : false;
                notice.Author = jsonList[i]["author"].ToString();

                if (jsonList[i].ContainsKey("imageKey"))
                {
                    notice.ImageKey = "https://s3.ap-northeast-2.amazonaws.com/upload-console.thebackend.io" + jsonList[i]["imageKey"].ToString();
                }
                if (jsonList[i].ContainsKey("linkUrl"))
                {
                    notice.LinkUrl = jsonList[i]["linkUrl"].ToString();
                }
                if (jsonList[i].ContainsKey("linkButtonName"))
                {
                    notice.LinkButtonName = jsonList[i]["linkButtonName"].ToString();
                }

                _noticeList.Add(notice);
            }
        }
    }
}