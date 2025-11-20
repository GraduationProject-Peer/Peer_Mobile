using System.Collections;
using UnityEngine;
using TMPro;


public class DiaryManager : MonoBehaviour
{
    public TMP_InputField diaryInput;
    public TMP_Text resultText;
// test change

    public void OnClickSave()
    {
        // UI 클릭 시점 가드 (코루틴 호출 전)
        if (SessionUtil.IsGuest())
        {
            if (resultText) resultText.text = "게스트 모드: 저장은 로그인 후 이용 가능해요.";
            Debug.Log("게스트 모드: 저장 버튼 차단");
            return;
        }
        StartCoroutine(SaveDiaryRoutine());
    }

    IEnumerator SaveDiaryRoutine()
    {
        if (SessionUtil.IsGuest())
        {
            Debug.Log("게스트 모드: 일기 저장 불가");
            yield break;
        }

        string token = SessionUtil.GetToken();
        string content = diaryInput ? diaryInput.text : "";
        string body = JsonUtility.ToJson(new DiaryBody { content = content });

        yield return ApiClient.PostJsonAuth("/diary/save", body, token, (code, res) =>
        {
            if (resultText)
            {
                resultText.text = (code >= 200 && code < 300)
                    ? "저장 완료!"
                    : $"저장 실패({code})";
            }
            Debug.Log($"일기 저장 응답({code}): {res}");
        });
    }
}

[System.Serializable] public class DiaryBody { public string content; }

