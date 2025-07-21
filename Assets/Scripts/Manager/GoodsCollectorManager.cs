using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsCollectorManager : Singleton<GoodsCollectorManager>
{
    private readonly Queue<GoodsItem> _collectionQueue = new();
    [SerializeField] private int maxCollectPerFrame = 30; // �����Ӵ� �ִ� ���� ó�� ����

    private void Update()
    {
        int processed = 0;

        while (_collectionQueue.Count > 0 && processed < maxCollectPerFrame)
        {
            var item = _collectionQueue.Dequeue();
            item.DoCollect(); // ���� ���� ó��
            processed++;
        }
    }

    public void RequestCollect(GoodsItem item)
    {
        if (!_collectionQueue.Contains(item))
            _collectionQueue.Enqueue(item);
    }
}

