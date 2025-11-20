using UnityEngine;
using UnityEngine.SceneManagement;

public class SignupFinish : MonoBehaviour
{
    public void GoToSignupFinish()
    {
        SceneManager.LoadScene("SignupFinishScene"); // ← 실제 회원가입 씬 이름!
    }
}

