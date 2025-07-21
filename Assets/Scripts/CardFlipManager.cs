using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipManager : Singleton<CardFlipManager>
{
    private KillCountCardFlipButton KillCountCardFlipButton;

    public List<FlidCard> CardsToFlip = new List<FlidCard>();
    //�ؽ�Ʈ�� 3���� ��������
    public List<TMP_Text> BuffText = new List<TMP_Text>();
    public List<BuffData> BuffData = new List<BuffData>();

    private int _flipCount = 6;

    private bool isFlipping = false;

    public GameObject FlipCardUI;
    public Button StartFlipButton;
    public Button EndFlipButton;

    private void Start()
    {
        KillCountCardFlipButton = FindAnyObjectByType<KillCountCardFlipButton>();

        FlipCardUI.SetActive(false);

        StartFlipButton.onClick.AddListener(GoFlipCards);

        EndFlipButton.onClick.AddListener(EndFlipCards);
    }

    void GoFlipCards()
    {
        if (KillCountCardFlipButton == null || !KillCountCardFlipButton.OpenButton)
        {
            Debug.Log("���� ����. ��ư �۵�����.");
            return;
        }
        KillCountCardFlipButton.ResetCount();
        FlipCardUI.SetActive(true);
        StartCoroutine(FlipAllCardsMultipleTimes());
    }

    public void EndFlipCards()
    {
        //���⿡ EndFlipCard() �ְ����
        KillCountCardFlipButton.ResetCount();
        FlipCardUI.SetActive(false);
    }

    IEnumerator FlipAllCardsMultipleTimes()
    {
        isFlipping = true;

        for (int i = 0; i < _flipCount; i++)
        {
            List<Coroutine> flipRoutines = new List<Coroutine>();

            foreach (var card in CardsToFlip)
            {
                flipRoutines.Add(StartCoroutine(card.FlipCard()));
            }

            // ��� ī�� ȸ���� ���� ������ ���
            yield return new WaitUntil(() => AllCardsDoneFlipping());

            yield return new WaitForSeconds(0.05f); // ��¦ �� �ֱ� ���ϸ�
        }

        isFlipping = false;

        AssignRandomBuffDescriptions();
    }

    private bool AllCardsDoneFlipping()
    {
        foreach (var card in CardsToFlip)
        {
            if (card.IsFlipping) return false;
        }
        return true;
    }

    void AssignRandomBuffDescriptions()
    {
        List<BuffData> copyList = new List<BuffData>(BuffData);

        for (int i = 0; i < BuffText.Count; i++)
        {
            if (copyList.Count == 0) break;

            int rand = Random.Range(0, copyList.Count);
            BuffData selected = copyList[rand];
            copyList.RemoveAt(rand);

            BuffText[i].text = selected.description;

            if (i < CardsToFlip.Count)
            {
                CardsToFlip[i].SetupCard(selected); // ���⼭ BuffData �Ҵ��
            }
        }
    }



}
