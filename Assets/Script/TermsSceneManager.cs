using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TermsSceneManager : MonoBehaviour
{
    public Toggle agreeToggle;
    public Button nextButton;

    void Start()
    {
        // 시작할 때 버튼 비활성화
        nextButton.interactable = false;

        // 토글 상태가 바뀔 때 → 함수 연결
        agreeToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        // 체크되면 버튼 활성화
        nextButton.interactable = isOn;
    }

    public void OnClickNext()
    {
        if (agreeToggle.isOn)
        {
            SceneManager.LoadScene("SignupEmailScene"); 
        }
    }
}
