using System.Collections.Generic;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{
    [Header("µÂ∑” π›∞Ê")]
    [SerializeField] private float goldDropRadius = 3.0f;
    [SerializeField] private float gachaDropRadius = 5.0f;
    [SerializeField] private float runeDropRadius = 7.0f;

    [Header("¿Á»≠ µÂ∑” µ•¿Ã≈Õ")]
    [SerializeField] private DropGoodsData normalDropData;
    [SerializeField] private DropGoodsData bossDropData;

    [Header("¿Á»≠ «¡∏Æ∆’")]
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject gachaPrefab;

    [Header("∑È «¡∏Æ∆’")]
    [SerializeField] private List<GameObject> runePrefabs;
    [Range(0f, 1f), SerializeField] private float runeDropChance = 0.65f;

    protected override void Initialized()
    {
        if (normalDropData == null)
            normalDropData = Resources.Load<DropGoodsData>(Define.normalDropData);

        if (bossDropData == null)
            bossDropData = Resources.Load<DropGoodsData>(Define.BossDropData);

        if (goldPrefab == null)
            goldPrefab = Resources.Load<GameObject>(Define.GoldPrefab);

        if (gachaPrefab == null)
            gachaPrefab = Resources.Load<GameObject>(Define.DiamondPrefab);

        if (runePrefabs == null || runePrefabs.Count == 0)
            runePrefabs = new List<GameObject>(Resources.LoadAll<GameObject>(Define.RunePrefab_Path));

        PoolManager.Instance.PreloadDropItems(goldPrefab, 50);
        PoolManager.Instance.PreloadDropItems(gachaPrefab, 20);
    }

    public void DropFromEnemy(Vector3 dropPos, bool isBoss)
    {
        DropGoodsData dropData = isBoss ? bossDropData : normalDropData;

        // ∞ÒµÂ µÂ∑”
        DropItems(goldPrefab, dropPos, dropData.GetGoldAmount(), goldDropRadius, GoodsType.Gold, 10);

        if (isBoss)
        {
            // ∞°√≠ µÂ∑”
            DropItems(gachaPrefab, dropPos, dropData.GetGachaAmount(), gachaDropRadius, GoodsType.Gp, 5);

            // ∑È µÂ∑” (»Æ∑¸¿˚)
            TryDropRune(dropPos);
        }
    }

    private void DropItems(GameObject prefab, Vector3 dropOrigin, int count, float radius, GoodsType type, int baseValue)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[DropManager] Prefab for {type} is null. Check assignment.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = dropOrigin + Random.insideUnitSphere * radius;
            offset.y = 1.0f;

            GameObject drop = PoolManager.Instance.ActivateObj(prefab, offset, Quaternion.identity);
            if (drop.TryGetComponent<GoodsItem>(out var item))
            {
                item.Initialize(baseValue, type);
            }
            else
            {
                Debug.LogError($"[DropManager] Missing GoodsItem component on {prefab.name}");
            }
        }
    }

    private void TryDropRune(Vector3 dropOrigin)
    {
        if (runePrefabs == null || runePrefabs.Count == 0) return;

        float rand = Random.value;
        if (rand > runeDropChance) return;

        int index = Random.Range(0, runePrefabs.Count);
        GameObject rune = runePrefabs[index];
        if (rune == null)
        {
            Debug.LogWarning("[DropManager] Selected rune prefab is null.");
            return;
        }

        Vector3 offset = dropOrigin + Random.insideUnitSphere * runeDropRadius;
        offset.y = 1.0f;

        PoolManager.Instance.ActivateObj(rune, offset, Quaternion.identity);
        Debug.Log($"[DropManager] ∑È µÂ∑”µ : {rune.name} (Chance: {runeDropChance * 100}%)");
    }
}
