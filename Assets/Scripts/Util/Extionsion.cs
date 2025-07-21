using System.Collections.Generic;
using UnityEngine;

// Ȯ�� �޼��带 �����ϴ� ���� Ŭ����
public static class Extionsion
{
    // GameObject�� Ư�� Ÿ���� ������Ʈ�� �������ų�, ������ �߰��ϴ� Ȯ�� �޼���
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        // GameObject���� T Ÿ���� ������Ʈ�� �����´�.
        T component = obj.GetComponent<T>();

        // ������Ʈ�� ������ �߰�
        if (component == null)
            obj.AddComponent<T>();

        // �������ų� �߰��� ������Ʈ ��ȯ
        return component;
    }

    // �־��� GameObject�� �������� Ư�� ���̾� �� ���� ����� Ÿ�� GameObject�� ã�� ���
    public static GameObject GetNearestTarget(this GameObject gameObject, string layerName, float radius)
    {
        // ������ ���̾� �̸��� ����� �ش� ���̾ ���� LayerMask�� ������
        LayerMask layerMask = LayerMask.GetMask(layerName);

        // ���� GameObject�� ��ġ�� �߽����� �־��� ������(radius) �ȿ� �ִ� Collider���� ����
        // �ش� ���̾�(layerMask)�� ���Ե� ������Ʈ�鸸 Ž����
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius, layerMask);

        // ���� ����� ������Ʈ�� ������ ����. ó������ null�� �ʱ�ȭ
        Collider shortObject = null;

        // ���� Ž���� Collider�� �ϳ��� �ִٸ�
        if (colliders.Length > 0)
        {
            // ������ �Ǵ� GameObject ��ġ�� ù ��° Collider ���� �Ÿ� ����
            float shortDistance1 = Vector3.Distance(gameObject.transform.position, colliders[0].transform.position);

            // �ϴ� ù ��° Collider�� ���� ����� ������ �ʱ� ����
            shortObject = colliders[0];

            // ������ Collider���� �ݺ��ϸ鼭 �� ����� Collider�� �ִ��� �˻�
            foreach (Collider collider in colliders)
            {
                // ���� �˻� ���� Collider���� �Ÿ� ����
                float shortDistance2 = Vector3.Distance(gameObject.transform.position, collider.transform.position);

                // �� ª�� �Ÿ��� �߰ߵǸ� �װ��� ���� ����� ������Ʈ�� ����
                if (shortDistance1 > shortDistance2)
                {
                    shortDistance1 = shortDistance2;
                    shortObject = collider;
                }
            }
            // ���� ����� Collider�� GameObject�� ��ȯ
            return shortObject.gameObject;
        }
        else
        {
            // Ž���� ������Ʈ�� �ϳ��� ���� ��� null ��ȯ
            return null;
        }
    }

    // �־��� GameObject�� �������� Ư�� ���̾� �� ���� ����� Ÿ�� GameObject�� ã�� ���
    public static List<GameObject> GetRandomTarget(this GameObject gameObject, string layerName, float radius, int minCount = 3, int maxCount = 10)
    {
        // ������ ���̾� �̸��� ����� �ش� ���̾ ���� LayerMask�� ������
        LayerMask layerMask = LayerMask.GetMask(layerName);

        // ���� GameObject�� ��ġ�� �߽����� �־��� ������(radius) �ȿ� �ִ� Collider���� ����
        // �ش� ���̾�(layerMask)�� ���Ե� ������Ʈ�鸸 Ž����
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius, layerMask);

        // ������ Ÿ�� ���
        List<GameObject> targetList = new List<GameObject>();

        // Ÿ�� �ĺ� ����� ����
        foreach (var col in colliders)
        {
            targetList.Add(col.gameObject);
        }

        // ������ Ÿ�� �� ���� (�ּҰ����� ���� ������ ��ü ��ȯ)
        int countToSelect = Mathf.Min(targetList.Count, Random.Range(minCount, maxCount + 1));

        // ���õ� Ÿ�� ���
        List<GameObject> selectedTargets = new List<GameObject>();

        // �������� Ÿ�� ���� (�ߺ� ����)
        for (int i = 0; i < countToSelect; i++)
        {
            int randIndex = Random.Range(0, targetList.Count);
            selectedTargets.Add(targetList[randIndex]);
            targetList.RemoveAt(randIndex); // �ߺ� ����
        }

        return selectedTargets;
    }
}