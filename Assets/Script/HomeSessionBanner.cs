using UnityEngine;
using TMPro;

public class HomeSessionBanner : MonoBehaviour
{
    [Header("Optional UI")]
    public GameObject guestBanner;   // "게스트 모드입니다" 라벨/패널
    public TMP_Text welcomeText;     // "환영합니다" 같은 문구

    void Start()
    {
        bool isGuest = SessionUtil.IsGuest();
        if (guestBanner) guestBanner.SetActive(isGuest);

        if (welcomeText)
        {
            welcomeText.text = isGuest ? "게스트 모드로 둘러보는 중" : "로그인 완료!";
        }
    }
}

