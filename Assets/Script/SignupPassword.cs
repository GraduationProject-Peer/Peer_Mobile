using UnityEngine;
using UnityEngine.SceneManagement;

public class SignupPassword : MonoBehaviour
{
    public void GoToSignupPassword()
    {
        SceneManager.LoadScene("SignupPasswordScene"); // ← 실제 회원가입 씬 이름!
    }
}

