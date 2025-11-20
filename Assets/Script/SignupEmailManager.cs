using System.Collections;
using System.Text.RegularExpressions;   // ← 이메일 형식 체크용
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SignupEmailManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public Button recheckButton;        // ⬅ 중복확인 버튼 (RecheckButten 오브젝트 드래그)
    public Button nextButton;           // ⬅ 다음 버튼 (기존 Button)
    public TextMeshProUGUI emailCheckMessage;

    // 중복확인 성공 여부
    private bool isEmailAvailable = false;

    private void Start()
    {
        // 처음에는 다음 버튼 비활성 + 안 보이게
        if (nextButton != null)
        {
            nextButton.interactable = false;
            nextButton.gameObject.SetActive(false);
        }

        // 중복확인 버튼은 처음에 보이게
        if (recheckButton != null)
            recheckButton.gameObject.SetActive(true);

        if (emailCheckMessage != null)
            emailCheckMessage.text = "";
    }

    // ✅ 이메일 형식 검사 함수
    private bool IsValidEmail(string email)
    {
        // 너무 빡세지 않은 기본 정규식
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    // 중복확인 버튼에 연결할 함수
    public void OnClickCheckEmail()
    {
        string email = emailInput.text.Trim();

        // 1) 비어있을 때
        if (string.IsNullOrEmpty(email))
        {
            emailCheckMessage.text = "이메일을 먼저 입력해주세요.";
            SetEmailInvalidState();
            return;
        }

        // 2) 형식이 잘못됐을 때
        if (!IsValidEmail(email))
        {
            emailCheckMessage.text = "이메일 형식을 확인해주세요.";
            SetEmailInvalidState();
            return;
        }

        // 3) 형식 OK → 서버에 중복확인 요청
        emailCheckMessage.text = "이메일 확인 중...";
        StartCoroutine(CheckEmailDuplicateRoutine(email));
    }

    private IEnumerator CheckEmailDuplicateRoutine(string email)
    {
        // 👉 여기를 너네 백엔드 주소로 바꿔야 해!
        string url = "https://your-api-domain.com/auth/check-email?email="
                     + UnityWebRequest.EscapeURL(email);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // 위에서 이미 "이메일 확인 중..." 출력했으니 여기선 생략
            // emailCheckMessage.text = "이메일 확인 중...";

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Email check error: " + www.error);
                emailCheckMessage.text = "서버 오류가 발생했습니다. 다시 시도해주세요.";
                SetEmailInvalidState();
                yield break;
            }

            // ★ 서버에서 내려주는 형식을 맞춰야 함
            // 예시: { "available": true } or { "available": false }
            string json = www.downloadHandler.text;
            Debug.Log("Email check response: " + json);

            // bool available = json.Contains("true"); // 임시 파싱
            bool available = true;

            if (available)
            {
                isEmailAvailable = true;
                emailCheckMessage.text = "사용 가능한 이메일입니다.";
                SetEmailValidState();   // ✅ 여기서 버튼 전환
            }
            else
            {
                isEmailAvailable = false;
                emailCheckMessage.text = "이미 사용 중인 이메일입니다.";
                SetEmailInvalidState();
            }
        }
    }

    // ✅ 이메일 사용 가능 상태(성공)일 때 버튼/플래그 설정
    private void SetEmailValidState()
    {
        isEmailAvailable = true;

        // 중복확인 버튼 숨기기
        if (recheckButton != null)
            recheckButton.gameObject.SetActive(false);

        // 다음 버튼 보이게 + 활성화
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.interactable = true;
        }
    }

    // ❌ 이메일 잘못됐거나 사용 불가/에러일 때 설정
    private void SetEmailInvalidState()
    {
        isEmailAvailable = false;

        // 중복확인 버튼 다시 보이게
        if (recheckButton != null)
            recheckButton.gameObject.SetActive(true);

        // 다음 버튼 숨기고 비활성화
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.interactable = false;
        }
    }

    // 실제 회원가입/다음 버튼 눌렀을 때 확인용
    public void OnClickNext()
    {
        if (!isEmailAvailable)
        {
            emailCheckMessage.text = "이메일 중복확인을 먼저 완료해주세요.";
            return;
        }

        // 여기서 비밀번호 검증(길이, 특수문자 등) 하고
        // 회원가입 API 호출 or 다음 씬 이동하면 됨
        Debug.Log("이메일 중복확인 완료됨, 다음 단계 진행!");
    }
}


