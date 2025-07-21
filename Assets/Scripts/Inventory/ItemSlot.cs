using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ItemSlot : MonoBehaviour
{
    public Image BackGroundImage;           // ������ ��� ����
    public Image ItemImage;                 // ������ �̹���
    public GameObject Equipped;             // ������ ������Ʈ
    public TextMeshProUGUI EnhanceText;     // ������ ��ȭ

    public ItemInstance ItemInstance { get; private set; }
    public RuneInstance RuneInstance { get; private set; }

    private Dictionary<Grade, Color> _gradeColors = new Dictionary<Grade, Color>()
    {
        { Grade.Legendary, new Color32(237, 171, 95, 255) },
        { Grade.Rare, new Color32(190, 119, 231, 255) },
        { Grade.Uncommon, new Color32(87, 152, 241, 255) },
        { Grade.Common, new Color32(114, 143, 115, 255) }
    };

    public void Setup(ItemInstance item)
    {
        ItemInstance = item;    // �� �ʿ�!

        ItemImage.sprite = item.ItemSo.ItemImage;
        EnhanceText.text = item.EnhanceLevel > 0 ? $"Lv {item.EnhanceLevel}" : "Lv 0";

        if (_gradeColors.TryGetValue(item.ItemSo.grade, out var color))
            BackGroundImage.color = color;
        else
            BackGroundImage.color = Color.white;
    }

    public void RuneSetup(RuneInstance item)
    {
        RuneInstance = item;    // �� �ʿ�!

        ItemImage.sprite = item.RuneSo.ItemImage;

        BackGroundImage.color = new Color32(221, 160, 221, 255);
    }

    public void SetUpEquipped(bool active)
    {
        // �⺻ ������ �̹��� ��Ȱ��ȭ
        Equipped.SetActive(active);
    }
}