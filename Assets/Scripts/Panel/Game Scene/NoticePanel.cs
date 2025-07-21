using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NoticePanel : MonoBehaviour
{
    [Header("Scroll View Content Parent")]
    [SerializeField] private Transform _noticeListTransform;
    [SerializeField] private Transform _noticeContentTransform;

    [Header("Prefab")]
    [SerializeField] private GameObject _noticePrefab;

    [Header("List")]
    [SerializeField] private List<GameObject> _noticeLists;

    [Header("Notice Content")]
    [SerializeField] private TMP_Text _noticeTitleText;
    [SerializeField] private Image _noticeImage;
    [SerializeField] private TMP_Text _noticeContentText;

    [Header("Select Notice Data")]
    [SerializeField] private int _selectedNoticeIndex;

    private void OnEnable()
    {
        LoadNotices();
    }

    private void Awake()
    {
        ResourceAllLoad();

        Setting();
    }

    private void ResourceAllLoad()
    {
        _noticeListTransform = transform.Find("Notice List/Viewport/Content");
        _noticeContentTransform = FindAnyObjectByType<NoticeContent>().gameObject.transform.Find("Viewport/Content");

        _noticePrefab = Resources.Load<GameObject>(Define.NoticePrefab_Path);

        _noticeTitleText = _noticeContentTransform.transform.Find("Notice Title Text").GetComponent<TMP_Text>();
        _noticeImage = _noticeContentTransform.transform.Find("Notice Image").GetComponent<Image>();
        _noticeContentText = _noticeContentTransform.transform.Find("Notice Content Text").GetComponent<TMP_Text>();
    }

    private void Setting()
    {
        _noticeLists = new List<GameObject>();
    }

    // ���� ���� �� ���� �ʱ�ȭ
    private void ResetContent()
    {
        _noticeTitleText.text = string.Empty;
        _noticeImage.sprite = null;
        _noticeContentText.text = string.Empty;
    }

    private void LoadNotices()
    {
        // ���� ���� ������Ʈ ����
        foreach (var notice in _noticeLists)
        {
            Destroy(notice);
        }
        _noticeLists.Clear();
        ResetContent();

        // NoticeList�� �޾ƿ��� ���� UI ����
        BackendNotice.Instance.NoticeListGet();

        List<Notice> notices = BackendNotice.Instance.GetNoticeList();  // _noticeList ���ٿ� �޼��� �ʿ�

        for (int i = 0; i < notices.Count; i++)
        {
            var notice = notices[i];

            GameObject noticeObject = Instantiate(_noticePrefab, _noticeListTransform);
            noticeObject.name = $"Notice List {i}";

            var titleText = noticeObject.transform.Find("Title Text").GetComponent<TMP_Text>();

            titleText.text = notice.Title;

            int index = i; // Ŭ���� ĸó ����

            noticeObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                _selectedNoticeIndex = index;

                DisplayNoticeDetail(notice);
            });

            _noticeLists.Add(noticeObject);
        }
    }

    private IEnumerator LoadImageFromURL(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("�������� �̹��� �ε� ����: " + request.error);
            _noticeImage.sprite = null; // �⺻ �̹��� �Ǵ� ���� ó��
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _noticeImage.sprite = sprite;
            _noticeImage.SetNativeSize();   // ���� �̹��� ������ ����
        }
    }

    private void DisplayNoticeDetail(Notice notice)
    {
        _noticeTitleText.text = notice.Title;
        _noticeImage.sprite = null;
        _noticeContentText.text = notice.Content;

        // �̹��� �ε�
        if (!string.IsNullOrEmpty(notice.ImageKey))
        {
            StartCoroutine(LoadImageFromURL(notice.ImageKey));
        }
    }
}