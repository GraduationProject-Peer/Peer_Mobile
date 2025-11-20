using UnityEngine;
using UnityEngine.SceneManagement;

public class SignupEmail : MonoBehaviour
{
    public void GoToSignup()
    {
        SceneManager.LoadScene("SignupEmailScene"); // ← 실제 회원가입 씬 이름!
    }
}

