using UnityEngine;
using System.Collections.Generic;

public class MemoryPool
{
    private class PoolItem
    {
        public bool isActive;      
        public GameObject gameObject;
    }

    private int increaseCount = 5;         
    private int maxCount;                   
    private int activeCount;               
    private GameObject poolObject;     
    private Transform poolParent;       
    private List<PoolItem> poolItemList;   

    public int MaxCount => maxCount;    
    public int ActiveCount => activeCount;  

    public MemoryPool(GameObject poolObject, Transform poolParent = null)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;
        this.poolParent = poolParent;

        poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }

    // increaseCount ��ŭ ������Ʈ�� ���� ����
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; ++i)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject);
            poolItem.gameObject.SetActive(false);
            poolItem.gameObject.transform.SetParent(poolParent);

            poolItemList.Add(poolItem);
        }
    }

    // ��� ������Ʈ �ı�
    public void DestroyObjects()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    // Ǯ �ȿ��� ��Ȱ�� ������ ������Ʈ�� ã�� Ȱ��ȭ�ؼ� ��ȯ
    // ���� ��� ������Ʈ�� Ȱ�� ���¶�� ���ο� ������Ʈ�� �߰� ���� �� ��ȯ
    public GameObject ActivatePoolItem()
    {
        if (poolItemList == null) return null;

        if (maxCount == activeCount) InstantiateObjects();

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.isActive == false)
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    // Ư�� ������Ʈ�� ��Ȱ��ȭ ó��
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    // Ȱ�� ������ ������Ʈ �� �ϳ��� ã�� ��Ȱ��ȭ ó��
    public GameObject DeactivatePoolItem()
    {
        if (poolItemList == null) return null;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject.activeSelf == true)
            {
                activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return poolItem.gameObject;
            }
        }

        return null;
    }

   // ��� Ȱ�� ������Ʈ�� ��Ȱ��ȭ ó��
    public void DeactivateAllPoolItems()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}