using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignupPasswordManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField passwordInput;        // 비밀번호 입력
    public TMP_InputField rePasswordInput;      // 비밀번호 재확인 입력
    public TextMeshProUGUI ErrorText_Password;  // 오류/안내 메시지
    public Button nextButton;                   // 다음/회원가입 버튼

    private const int MinPasswordLength = 6;

    private void Start()
    {
        // 처음엔 메시지 지우기
        if (ErrorText_Password != null)
            ErrorText_Password.text = "";

        // 필요하면 시작할 때 버튼 비활성화 하고,
        // 조건 만족했을 때만 활성화하는 방식도 가능
        if (nextButton != null)
            nextButton.interactable = true;
    }

    // NextButton의 OnClick에 연결할 함수
    public void OnClickNext()
    {
        string pw = passwordInput.text.Trim();
        string confirm = rePasswordInput.text.Trim();   // ← 변수 이름 맞게 수정

        // 1) 둘 중 하나라도 비어있을 때
        if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(confirm))
        {
            SetMessage("비밀번호와 비밀번호 확인을 모두 입력해주세요.");
            return;
        }

        // 2) 길이 체크 (6자리 이상)
        if (pw.Length < MinPasswordLength)
        {
            SetMessage($"비밀번호는 최소 {MinPasswordLength}자리 이상이어야 합니다.");
            return;
        }

        // 3) 비밀번호 / 재확인 일치 여부
        if (pw != confirm)
        {
            SetMessage("비밀번호가 일치하지 않습니다.");
            return;
        }

        // 여기까지 왔으면 통과!
        SetMessage(""); // 메시지 지우기 (원하면 "사용 가능한 비밀번호입니다." 로 바꿔도 됨)

        Debug.Log("비밀번호 설정 완료");

        // TODO: 실제로는 다음 씬으로 이동하거나, 회원가입 API 호출
        SceneManager.LoadScene("SignupFinishScene");
    }

    // 메시지 표시용 헬퍼 함수
    private void SetMessage(string msg)
    {
        if (ErrorText_Password != null)
        {
            ErrorText_Password.gameObject.SetActive(true);
            ErrorText_Password.text = msg;
        }
    }

    // 입력 도중에 에러 문구 지우고 싶으면
    // 두 InputField의 OnValueChanged에 이 함수 연결해도 됨
    public void OnPasswordValueChanged(string _)
    {
        if (ErrorText_Password != null)
            ErrorText_Password.text = "";
    }
}

