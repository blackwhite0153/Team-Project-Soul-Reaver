using BackEnd;
using System.Collections.Generic;
using UnityEngine;

// �ڳ� ������ ����(Post) �ý����� �����ϴ� Ŭ����
public class BackendMail : Singleton<BackendMail>
{
    [SerializeField] private List<Mail> _mailList = new List<Mail>();    // ���� ���ÿ� ����� ���� ����Ʈ

    public List<Mail> GetMailList()
    {
        return _mailList;
    }

    /// <summary>
    /// ���� ���� �������� ���� �κ��丮�� ����
    /// </summary>
    /// <param name="item">postItems ������(JsonData)</param>
    public void SaveMailToLocal(LitJson.JsonData items)
    {
        foreach (LitJson.JsonData itemJson in items)
        {
            // ��� �ڵ�: "item" Ű�� ���ų� null�� ��� �ǳʶ�
            if (itemJson == null || !itemJson.ContainsKey("item") || itemJson["item"] == null)
            {
                Debug.LogWarning("itemJson ������ �߸��Ǿ����ϴ� (item Ű ���� �Ǵ� null)");
                continue;
            }

            // ������ ������ ��ȿ���� Ȯ��
            if (itemJson["item"].ContainsKey("itemType"))
            {
                // �ڳ����� ������ ������ ���� ����
                int itemId = int.Parse(itemJson["item"]["itemId"].ToString());
                string itemType = itemJson["item"]["itemType"].ToString();
                string itemName = itemJson["item"]["itemName"].ToString();
                int itemCount = int.Parse(itemJson["itemCount"].ToString());

                // �κ��丮 �ݿ�
                AddToInventory(itemName, itemCount);
            }
            else
            {
                // ������ ���� (��Ʈ ���)
                if (itemJson["item"].IsObject)
                {
                    foreach (var key in itemJson["item"].Keys)
                    {
                        string itemName = key;
                        string rawCount = itemJson["item"][key].ToString();

                        if (int.TryParse(rawCount, out int itemCount))
                        {
                            AddToInventory(itemName, itemCount);
                        }
                        else
                        {
                            Debug.LogWarning($"��Ʈ ��� ������ �Ľ� ����: {itemName} = {rawCount}");
                        }
                    }
                }
                else
                {
                    Debug.LogError("itemJson[\"item\"] ������ ����� �ٸ��ϴ�. Object�� �ƴմϴ�.");
                }
            }
        }
    }

    // ���� ����Ʈ�� �������� �޾ƿ� ���ÿ� ����
    public void MailListGet(PostType mailType)
    {
        var bro = Backend.UPost.GetPostList(mailType);  // ���� ��� ��û

        string chartName = "MailRewardChart";   // ������ ��Ʈ �̸� (�ֿܼ��� ������ �̸��� ��ġ�ؾ� ��)

        if (!bro.IsSuccess())
        {
            Debug.LogError("���� �ҷ����� �� ������ �߻��߽��ϴ�.");
            return;
        }

        Debug.Log("���� ����Ʈ �ҷ����� ��û�� �����߽��ϴ�. : " + bro);

        // ������ ���� ���
        if (bro.GetFlattenJSON()["postList"].Count <= 0)
        {
            Debug.LogWarning("���� ������ �������� �ʽ��ϴ�.");
            return;
        }

        // ���� �ϳ��� ó��
        foreach (LitJson.JsonData mailListJson in bro.GetFlattenJSON()["postList"])
        {
            string inDate = mailListJson["inDate"].ToString();

            // �ߺ� ���� : inDate�� �̹� �ִ��� Ȯ��
            bool alreadyExists = _mailList.Exists(p => p.InDate == inDate);

            if (alreadyExists)
            {
                continue; // �̹� �߰��� ������� �ǳʶ�
            }

            Mail mail = new Mail
            {
                Title = mailListJson["title"].ToString(),
                Content = mailListJson["content"].ToString(),
                InDate = mailListJson["inDate"].ToString()
            };

            // ���� Ÿ�� ���� ó�� (USER_DATA���� ���� �ִ� ���)
            if (mailType == PostType.User)
            {
                if (mailListJson["itemLocation"]["tableName"].ToString() == "USER_DATA")
                {
                    if (mailListJson["itemLocation"]["column"].ToString() == "inventory")
                    {
                        foreach (string itemKey in mailListJson["item"].Keys)
                        {
                            mail.mailReward.Add(itemKey, int.Parse(mailListJson["item"][itemKey].ToString()));
                        }

                        mail.isCanReceive = true;
                    }
                    else
                    {
                        Debug.LogWarning("���� �������� �ʴ� �÷� ���� �Դϴ�. : " +
                                         mailListJson["itemLocation"]["column"].ToString());
                    }
                }
                else
                {
                    Debug.LogWarning("���� �������� �ʴ� ���̺� ���� �Դϴ�. : " +
                                     mailListJson["itemLocation"]["tableName"].ToString());
                }
            }
            // ������ ���� ó�� (��Ʈ ���)
            else
            {
                foreach (LitJson.JsonData itemJson in mailListJson["items"])
                {
                    Debug.Log(mailListJson.ToJson());

                    if (itemJson.ContainsKey("chartName") && itemJson["chartName"].ToString() == chartName)
                    {
                        // item ��ü�� Dictionary<string, string> ���·� �Ǿ� ����
                        foreach (var key in itemJson["item"].Keys)
                        {
                            string itemName = key;
                            string rawCount = itemJson["item"][key].ToString();

                            if (int.TryParse(rawCount, out int itemCount))
                            {
                                if (mail.mailReward.ContainsKey(itemName))
                                    mail.mailReward[itemName] += itemCount;
                                else
                                    mail.mailReward.Add(itemName, itemCount);
                            }
                            else
                            {
                                Debug.LogWarning($"[MailListGet] ���� �Ľ� ����: itemName={itemName}, rawValue={rawCount}");
                            }
                        }

                        mail.isCanReceive = true;
                    }
                    else
                    {
                        Debug.LogWarning("�������� �ʴ� ��Ʈ �����Դϴ� : " +
                            (itemJson.ContainsKey("chartName") ? itemJson["chartName"].ToString() : "��Ʈ Ű ����"));

                        mail.isCanReceive = false;
                    }
                }
            }

            _mailList.Add(mail);    // ���� ����Ʈ�� �߰�
        }

        // ����� ���
        for (int i = 0; i < _mailList.Count; i++)
        {
            Debug.Log($"{i}�� ° ����\n" + _mailList[i].ToString());
        }
    }

    // Ư�� �ε����� ������ ����
    public void MailReceive(PostType mailType, int index)
    {
        if (_mailList.Count <= 0)
        {
            Debug.LogWarning("���� �� �ִ� ������ �������� �ʽ��ϴ�. Ȥ�� ���� ����Ʈ �ҷ����⸦ ���� ȣ�����ּ���.");
            return;
        }

        if (index >= _mailList.Count)
        {
            Debug.LogError($"�ش� ������ �������� �ʽ��ϴ�. : ��û index{index} / ���� �ִ� ���� : {_mailList.Count}");
            return;
        }

        Debug.Log($"{mailType.ToString()}�� {_mailList[index].InDate} ��������� ��û�մϴ�.");

        var bro = Backend.UPost.ReceivePostItem(mailType, _mailList[index].InDate);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"{mailType.ToString()}�� {_mailList[index].InDate} ������� �� ������ �߻��߽��ϴ�. : " + bro);
            return;
        }

        Debug.Log($"{mailType.ToString()}�� {_mailList[index].InDate} ������ɿ� �����߽��ϴ�. : " + bro);

        _mailList.RemoveAt(index);  // ���� ����Ʈ���� ����

        // �������� ������ ��� ���� �κ��丮�� ����
        if (bro.GetFlattenJSON()["postItems"].Count > 0)
        {
            SaveMailToLocal(bro.GetFlattenJSON()["postItems"]);
        }
        else
        {
            Debug.LogWarning("���� ������ ���� �������� �������� �ʽ��ϴ�.");
        }

        BackendGameData.Instance.GameDataUpdate();  // ���� ������ ������Ʈ ȣ��
    }

    // ��� ���� �ϰ� ����
    public void MailReceiveAll(PostType mailType)
    {
        if (_mailList.Count <= 0)
        {
            Debug.LogWarning("���� �� �ִ� ������ �������� �ʽ��ϴ�. Ȥ�� ���� ����Ʈ �ҷ����⸦ ���� ȣ�����ּ���.");
            return;
        }

        Debug.Log($"{mailType.ToString()} ���� ��� ������ ��û�մϴ�.");

        var bro = Backend.UPost.ReceivePostItemAll(mailType);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"{mailType.ToString()} ���� ��� ���� �� ������ �߻��߽��ϴ� : " + bro);
            return;
        }

        Debug.Log("���� ��� ���ɿ� �����߽��ϴ�. : " + bro);

        _mailList.Clear();  // ���� ����Ʈ �ʱ�ȭ

        foreach (LitJson.JsonData postItemsJson in bro.GetFlattenJSON()["postItems"])
        {
            SaveMailToLocal(postItemsJson); // ������ ������ ���� ����
        }

        BackendGameData.Instance.GameDataUpdate();  // ���� ������ ����
    }

    private void AddToInventory(string itemName, int itemCount)
    {
        // ��� �ڵ�: UserData�� null�̸� �ʱ�ȭ
        if (BackendGameData.UserData == null)
        {
            Debug.LogWarning("UserData�� null ���¿��� ���� �����մϴ�.");
            BackendGameData.UserData = new UserData();
        }
        // ��� �ڵ�: UserData.Inventory�� null�̸� �ʱ�ȭ
        if (BackendGameData.UserData.Inventory == null)
        {
            Debug.LogWarning("Inventory�� null�Դϴ�. ���� �����մϴ�.");
            BackendGameData.UserData.Inventory = new Dictionary<string, int>();
        }

        // ��� �Ǵ� ���� ���� �������� �ݿ�
        if (itemName == "Gold")
        {
            GameManager.Instance.GoldNum += itemCount;
        }
        else if (itemName == "Gem")
        {
            GameManager.Instance.GpNum += itemCount;
        }

        // �κ��丮 ����
        if (BackendGameData.UserData.Inventory.ContainsKey(itemName))
            BackendGameData.UserData.Inventory[itemName] += itemCount;
        else
            BackendGameData.UserData.Inventory.Add(itemName, itemCount);

        Debug.Log($"�κ��丮�� �߰���: {itemName} - {itemCount}��");
    }
}