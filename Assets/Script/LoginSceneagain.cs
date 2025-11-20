using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginSceneagain : MonoBehaviour
{
    public void GoToLogin()
    {
        SceneManager.LoadScene("LoginScene"); // ← 실제 회원가입 씬 이름!
    }
}


