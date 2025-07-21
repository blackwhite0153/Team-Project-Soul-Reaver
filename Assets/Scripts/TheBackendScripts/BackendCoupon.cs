using BackEnd;
using UnityEngine;
using System.Collections.Generic;

public class BackendCoupon : Singleton<BackendCoupon>
{
    public void ReceiveCoupon(string couponCode, System.Action onFailed = null, System.Action onSuccess = null)
    {
        Backend.Coupon.UseCoupon(couponCode, callback =>
        {
            if (!callback.IsSuccess())
            {
                FailedToReceive(callback);
                onFailed?.Invoke();
                return;
            }

            Debug.Log("\uCFE0\uD3F0 \uC0AC\uC6A9\uC5D0 \uC131\uACF5\uD588\uC2B5\uB2C8\uB2E4. : " + callback);

            try
            {
                LitJson.JsonData itemObject = callback.GetFlattenJSON()["itemObject"];

                if (itemObject.Count <= 0)
                {
                    Debug.LogWarning("\uCFE0\uD3F0\uC5D0 \uC544\uC774\uD15C\uC774 \uC5C6\uC2B5\uB2C8\uB2E4.");
                }
                else
                {
                    foreach (LitJson.JsonData item in itemObject)
                    {
                        if (item["item"] == null)
                        {
                            Debug.LogWarning("item[\"item\"] \uAC12\uC774 null\uC785\uB2C8\uB2E4.");
                            continue;
                        }

                        // itemType\uC774 \uC788\uB294 \uAD6C\uC870 (\uCC44\uD2B8 \uAE30\uBC18)
                        if (item["item"].ContainsKey("itemType"))
                        {
                            int itemId = int.Parse(item["item"]["itemId"].ToString());
                            string itemType = item["item"]["itemType"].ToString();
                            string itemName = item["item"]["itemName"].ToString();
                            int itemCount = int.Parse(item["itemCount"].ToString());

                            ApplyReward(itemName, itemCount);
                        }
                        // ������ ���� �Ǵ� ��Ʈ ��� ���� ����
                        else if (item["item"].IsObject)
                        {
                            foreach (string key in item["item"].Keys)
                            {
                                string itemName = key;
                                string rawCount = item["item"][key].ToString();

                                if (int.TryParse(rawCount, out int itemCount))
                                {
                                    ApplyReward(itemName, itemCount);
                                }
                                else
                                {
                                    Debug.LogWarning($"\uCC28\uD2B8 \uAE30\uBC18 \uC544\uC774\uD15C \uD30C\uC2F1 \uC2E4\uD328: {itemName} = {rawCount}");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("\uC9C0\uC6D0\uD558\uC9C0 \uC54A\uB294 \uC544\uC774\uD15C\uC785\uB2C8\uB2E4. itemType \uC5C6\uC74C");
                        }
                    }

                    Debug.Log("\uCFE0\uD3F0 \uC544\uC774\uD15C \uC801\uC6A9 \uC644\uB8CC");
                    BackendGameData.Instance.GameDataUpdate();
                }

                onSuccess?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError("\uCFE0\uD3F0 \uCC98\uB9AC \uC911 \uC608\uC81C \uBC1C\uC0DD: " + e);
                onFailed?.Invoke();
            }
        });
    }

    private void ApplyReward(string itemName, int itemCount)
    {
        if (itemName == "Gold")
        {
            GameManager.Instance.GoldNum += itemCount;
        }
        else if (itemName == "Gem")
        {
            GameManager.Instance.GpNum += itemCount;
        }

        if (BackendGameData.UserData == null)
        {
            Debug.LogWarning("UserData�� null ���¿��� ���� �����մϴ�.");
            BackendGameData.UserData = new UserData();
        }

        if (BackendGameData.UserData.Inventory == null)
        {
            Debug.LogWarning("Inventory�� null�Դϴ�. ���� �����մϴ�.");
            BackendGameData.UserData.Inventory = new Dictionary<string, int>();
        }

        if (BackendGameData.UserData.Inventory.ContainsKey(itemName))
        {
            BackendGameData.UserData.Inventory[itemName] += itemCount;
        }
        else
        {
            BackendGameData.UserData.Inventory.Add(itemName, itemCount);
        }

        Debug.Log($"�κ��丮�� �߰��� : {itemName} - {itemCount}��");
    }

    private void FailedToReceive(BackendReturnObject callback)
    {
        if (callback.GetMessage().Contains("���� ����"))
        {
            Debug.Log("���� ���� ������ �����Ǿ��ų� �Ⱓ�� ����� �����Դϴ�.");
        }
        else if (callback.GetMessage().Contains("�̹� ����Ͻ� ����"))
        {
            Debug.Log("�ش� ������ �̹� ����ϼ̽��ϴ�.");
        }
        else
        {
            Debug.Log("���� �ڵ尡 �߸��Ǿ��ų� �̹� ����� �����Դϴ�.");
        }

        Debug.LogError($"���� ��� �� ������ �߻��߽��ϴ� : {callback}");
    }
}