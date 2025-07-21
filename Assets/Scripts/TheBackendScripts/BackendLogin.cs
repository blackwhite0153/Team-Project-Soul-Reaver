// �ڳ�
using BackEnd;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackendLogin : Singleton<BackendLogin>
{
    public SignUpPanelMove SignUpPanelMove;

    [Header("Login Input Fields")]
    public TMP_InputField IDInputField;
    public TMP_InputField PwInputField;

    [Header("SingUp Input Fields")]
    public TMP_InputField SignupIDField;
    public TMP_InputField SignupPwField;
    public TMP_InputField SignupNewNickNameInputField;

    [Header("Login Buttons")]
    public Button loginButton;
    public Button SingnUpButton1;

    [Header("SingUp Buttons")]
    public Button SingnUpButton2;
    public Button SingnUpCloseButton;

    [Header("Login Erorr Text")]
    public TMP_Text IDorPwErorrText;

    [Header("Sing up Erorr Text")]
    public TMP_Text SignupIDorPwErorrText;
    public TMP_Text SignupNewNickNameErorrText;

    bool Singup = false;
    bool Nickname = false;

    private void Start()
    {
        loginButton.onClick.AddListener(CustomLogin);
        SingnUpButton1.onClick.AddListener(SignUpPanelMove.UpMove);
        SingnUpButton2.onClick.AddListener(OnSignUpButtonClicked);
        SingnUpCloseButton.onClick.AddListener(SignUpPanelMove.DownMove);
    }

    // ȸ������
    public void CustomSignUp()
    {
        string id = SignupIDField.text;
        string pw = SignupPwField.text;

        Debug.Log("ȸ�������� ��û�մϴ�.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("ȸ�����Կ� �����߽��ϴ�. :" + bro);
            SignupIDorPwErorrText.color = Color.green;
            SignupIDorPwErorrText.text = "ȸ�����Կ� �����޽��ϴ�.";
            Singup = true;
        }
        else
        {
            Debug.LogError("ȸ�����Կ� �����߽��ϴ�. : " + bro);
            SignupIDorPwErorrText.text = "ȸ�����Կ� �����޽��ϴ�.";
        }
    }

    // �α���
    private void CustomLogin()
    {
        SoundManager.Instance.PlaySFX("Button");

        // �α��� �ϱ�
        string id = IDInputField.text;
        string pw = PwInputField.text;

        Debug.Log("�α����� ��û�մϴ�.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("�α����� �����޽��ϴ�. : " + bro);

            SoundManager.Instance.StopBGM();

            MySceneManager.Instance.FadePanel.gameObject.SetActive(true);

            MySceneManager.Instance.LoadSceneWithFade("Game");
        }
        else
        {
            Debug.LogError("�α����� �����߽��ϴ�. : " + bro);
            IDorPwErorrText.text = "���̵� Ȥ�� ��й�ȣ�� ���� �ʽ��ϴ�.";
        }
    }

    // �г���
    public void UpdateNickname()
    {
        // �г���
        string nickname = SignupNewNickNameInputField.text;

        Debug.Log("�г����� ��û�մϴ�.");
        var bro = Backend.BMember.CreateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("�г��� ������ �����߽��ϴ�. : " + bro);
            SignupNewNickNameErorrText.color = Color.green;
            SignupNewNickNameErorrText.text = "�г��� ������ �����߽��ϴ�.";
            Nickname = true;
        }
        else
        {
            Debug.LogError("�г��� ������ �����߽��ϴ�. : "+ bro) ;
            SignupNewNickNameErorrText.text = "�г��� ������ �����߽��ϴ�.";
        }
    }

    private void NickName()
    {
        string name = SignupNewNickNameInputField.text;

        if (string.IsNullOrEmpty(name))
        {
            SignupNewNickNameErorrText.color = Color.red;
            SignupNewNickNameErorrText.text = "�г����� �Է����ּ���.";
            return;
        }

        if (name.Length > 10)
        {
            SignupNewNickNameErorrText.color = Color.red;
            SignupNewNickNameErorrText.text = "�г����� �ִ� 10�����Դϴ�.";
            return;
        }

        // ȸ������ �õ�
        CustomSignUp();

        // �г��� �ߺ� �˻�
        UpdateNickname();

        // ��� �Ǵ��ؼ� ���� �ÿ��� �ڷ�ƾ ����
        if (Singup && Nickname)
        {
            StartCoroutine(InvokeSingObj());
        }
    }

    private IEnumerator InvokeSingObj()
    {
        yield return new WaitForSeconds(2f);

        SignupIDorPwErorrText.color = Color.red;
        SignupNewNickNameErorrText.color = Color.red;

        SignUpPanelMove.DownMove();
    }

    private void OnSignUpButtonClicked()
    {
        SoundManager.Instance.PlaySFX("Button");

        NickName();
    }
}