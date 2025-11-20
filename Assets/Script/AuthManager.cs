using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject signupPanel;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goSignupButton;     // 선택: 하단 전환 버튼
    [SerializeField] private TMP_Text errorTextLogin;

    [Header("Signup UI")]
    [SerializeField] private TMP_InputField emailInputSignup;
    [SerializeField] private TMP_InputField passwordInputSignup;
    [SerializeField] private TMP_InputField confirmPasswordInput;
    [SerializeField] private Button signupButton;
    [SerializeField] private Button goLoginButton;
    [SerializeField] private TMP_Text errorTextSignup;

    [Header("Options")]
    [SerializeField] private string homeSceneName = "HomeScene";
    [SerializeField] private bool mockMode = true;  // 서버 없으면 true로 테스트

    private void Start()
    {
        // 기본 진입은 로그인 탭
        ShowLogin();
        HideAllErrors();
        ValidateBindings(); // 인스펙터 누락 잡아주기(에디터 로그)
    }

    /* ------------------------- 화면 전환 ------------------------- */
    public void ShowLogin()
    {
        if (loginPanel)  loginPanel.SetActive(true);
        if (signupPanel) signupPanel.SetActive(false);
        HideAllErrors();
    }

    public void ShowSignup()
    {
        if (loginPanel)  loginPanel.SetActive(false);
        if (signupPanel) signupPanel.SetActive(true);
        HideAllErrors();
    }

    private void HideAllErrors()
    {
        if (errorTextLogin)  errorTextLogin.gameObject.SetActive(false);
        if (errorTextSignup) errorTextSignup.gameObject.SetActive(false);
    }

    /* -------------------- 게스트(둘러보기) 진입 ------------------- */
    public void OnClickGuestExplore()
    {
        PlayerPrefs.SetInt("is_guest", 1);
        PlayerPrefs.DeleteKey("access_token");
        PlayerPrefs.Save();
        SceneManager.LoadScene(homeSceneName);
    }

    /* --------------------------- 검증 ---------------------------- */
    private bool IsEmailValid(string email)
        => !string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains(".");

    private bool IsPasswordValid(string pwd)
        => !string.IsNullOrEmpty(pwd) && pwd.Length >= 6;

    /* --------------------------- 로그인 -------------------------- */
    public void OnClickLogin()
    {
        // 널 가드: 인스펙터 누락 시 사용자에게 에러 띄우고 종료
        if (!emailInput || !passwordInput)
        {
            Debug.LogError("[AuthManager] Login inputs are not assigned.");
            ShowLoginError("입력 필드가 설정되지 않았어.");
            return;
        }
        if (!loginButton)
        {
            Debug.LogWarning("[AuthManager] loginButton is not assigned.");
        }

        var email = emailInput.text?.Trim();
        var pwd   = passwordInput.text;

        if (!IsEmailValid(email))   { ShowLoginError("이메일 형식을 확인해주세요."); return; }
        if (!IsPasswordValid(pwd))  { ShowLoginError("비밀번호는 6자 이상이어야 합니다."); return; }

        if (loginButton) loginButton.interactable = false;
        HideAllErrors();

        if (mockMode) StartCoroutine(MockLogin(email, pwd));
        else          StartCoroutine(CallLogin(email, pwd));
    }

    private IEnumerator MockLogin(string email, string pwd)
    {
        yield return new WaitForSeconds(0.4f);
        if (loginButton) loginButton.interactable = true;

        // 성공 가정
        PlayerPrefs.SetString("access_token", "MOCK_TOKEN");
        PlayerPrefs.SetInt("is_guest", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(homeSceneName);
    }

    private IEnumerator CallLogin(string email, string pwd)
    {
        // 실제 API 연결(백엔드 준비되면 활성화)
        string url  = ApiClient.BaseUrl + "/auth/login";
        string body = JsonUtility.ToJson(new LoginBody { email = email, password = pwd });

        yield return ApiClient.PostJson(url, body, (code, res) =>
        {
            if (loginButton) loginButton.interactable = true;

            if (code >= 200 && code < 300)
            {
                PlayerPrefs.SetString("access_token", ExtractValue(res, "access_token"));
                PlayerPrefs.SetInt("is_guest", 0);
                PlayerPrefs.Save();
                SceneManager.LoadScene(homeSceneName);
            }
            else
            {
                ShowLoginError($"로그인 실패({code}).");
            }
        });
    }

    private void ShowLoginError(string msg)
    {
        Debug.Log($"[AuthManager] ShowLoginError: {msg}");
        if (errorTextLogin)
        {
            errorTextLogin.text = msg;
            errorTextLogin.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("[AuthManager] errorTextLogin is not assigned.");
        }
    }

    /* --------------------------- 회원가입 ------------------------ */
    public void OnClickSignup()
    {
        if (!emailInputSignup || !passwordInputSignup || !confirmPasswordInput)
        {
            Debug.LogError("[AuthManager] Signup inputs are not assigned.");
            ShowSignupError("입력 필드가 설정되지 않았어.");
            return;
        }
        if (!signupButton)
        {
            Debug.LogWarning("[AuthManager] signupButton is not assigned.");
        }

        var email = emailInputSignup.text?.Trim();
        var pwd   = passwordInputSignup.text;
        var pwd2  = confirmPasswordInput.text;

        if (!IsEmailValid(email))  { ShowSignupError("이메일 형식을 확인해줘."); return; }
        if (!IsPasswordValid(pwd)) { ShowSignupError("비밀번호는 6자 이상이야."); return; }
        if (pwd != pwd2)           { ShowSignupError("비밀번호가 서로 달라."); return; }

        if (signupButton) signupButton.interactable = false;
        HideAllErrors();

        if (mockMode) StartCoroutine(MockSignup());
        else          StartCoroutine(CallSignup(email, pwd));
    }

    private IEnumerator MockSignup()
    {
        yield return new WaitForSeconds(0.4f);
        if (signupButton) signupButton.interactable = true;
        ShowLogin(); // 가입 성공 가정 → 로그인 화면으로
    }

    private IEnumerator CallSignup(string email, string pwd)
    {
        string url  = ApiClient.BaseUrl + "/auth/signup";
        string body = JsonUtility.ToJson(new SignupBody { email = email, password = pwd });

        yield return ApiClient.PostJson(url, body, (code, res) =>
        {
            if (signupButton) signupButton.interactable = true;

            if (code >= 200 && code < 300)
            {
                ShowLogin();
            }
            else
            {
                ShowSignupError($"회원가입 실패({code}).");
            }
        });
    }

    private void ShowSignupError(string msg)
    {
        Debug.Log($"[AuthManager] ShowSignupError: {msg}");
        if (errorTextSignup)
        {
            errorTextSignup.text = msg;
            errorTextSignup.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("[AuthManager] errorTextSignup is not assigned.");
        }
    }

    /* ------------------------- 유틸리티 -------------------------- */
    private string ExtractValue(string json, string key)
    {
        var k = $"\"{key}\":";
        int i = json.IndexOf(k);
        if (i < 0) return "";
        i += k.Length;
        int q1 = json.IndexOf('"', i);
        int q2 = json.IndexOf('"', q1 + 1);
        if (q1 >= 0 && q2 > q1) return json.Substring(q1 + 1, q2 - q1 - 1);
        return "";
    }

#if UNITY_EDITOR
    [ContextMenu("Validate Bindings (AuthManager)")]
    private void ValidateBindings()
    {
        Debug.Log(
            "[AuthManager Validate]\n" +
            $"loginPanel={(loginPanel ? "OK" : "NULL")}, signupPanel={(signupPanel ? "OK" : "NULL")}\n" +
            $"emailInput={(emailInput ? "OK" : "NULL")}, passwordInput={(passwordInput ? "OK" : "NULL")}, loginButton={(loginButton ? "OK" : "NULL")}, errorTextLogin={(errorTextLogin ? "OK" : "NULL")}\n" +
            $"emailInputSignup={(emailInputSignup ? "OK" : "NULL")}, passwordInputSignup={(passwordInputSignup ? "OK" : "NULL")}, confirmPasswordInput={(confirmPasswordInput ? "OK" : "NULL")}, signupButton={(signupButton ? "OK" : "NULL")}, errorTextSignup={(errorTextSignup ? "OK" : "NULL")}"
        );
    }
#endif
}

[System.Serializable] public class LoginBody  { public string email; public string password; }
[System.Serializable] public class SignupBody { public string email; public string password; }
