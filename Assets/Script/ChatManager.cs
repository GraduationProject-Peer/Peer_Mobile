using System.Collections;
using UnityEngine;
using TMPro;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField messageInput;
    public TMP_Text responseText;

    public void OnClickAnalyze()
    {
        if (SessionUtil.IsGuest())
        {
            if (responseText) responseText.text = "게스트 모드: 감정 분석은 로그인 후 이용 가능해요.";
            Debug.Log("게스트 모드: 분석 버튼 차단");
            return;
        }
        StartCoroutine(AnalyzeRoutine());
    }

    IEnumerator AnalyzeRoutine()
    {
        if (SessionUtil.IsGuest())
        {
            Debug.Log("게스트 모드: 분석 불가");
            yield break;
        }

        string token = SessionUtil.GetToken();
        string text = messageInput ? messageInput.text : "";
        string body = JsonUtility.ToJson(new AnalyzeBody { content = text });

        yield return ApiClient.PostJsonAuth("/emotion/analyze", body, token, (code, res) =>
        {
            if (responseText)
            {
                responseText.text = (code >= 200 && code < 300)
                    ? $"분석 결과: {res}"
                    : $"분석 실패({code})";
            }
            Debug.Log($"분석 응답({code}): {res}");
        });
    }
}

[System.Serializable] public class AnalyzeBody { public string content; }

