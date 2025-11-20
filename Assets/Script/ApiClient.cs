using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiClient
{
    // 에뮬레이터면 10.0.2.2, PC 플레이는 127.0.0.1
    public static string BaseUrl = "http://127.0.0.1:8000";

    // 절대 URL 버전 (네 코드와 호환)
    public static IEnumerator PostJson(string url, string jsonBody, System.Action<long, string> onDone)
    {
        var req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        onDone?.Invoke(req.responseCode, req.downloadHandler.text);
    }

    // 경로(path) + 인증 토큰 첨부 버전
    public static IEnumerator PostJsonAuth(string path, string jsonBody, string token, System.Action<long, string> onDone)
    {
        var url = $"{BaseUrl}{path}";
        var req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        if (!string.IsNullOrEmpty(token))
            req.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return req.SendWebRequest();
        onDone?.Invoke(req.responseCode, req.downloadHandler.text);
    }

    public static IEnumerator GetAuth(string path, string token, System.Action<long, string> onDone)
    {
        var url = $"{BaseUrl}{path}";
        var req = UnityWebRequest.Get(url);
        if (!string.IsNullOrEmpty(token))
            req.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return req.SendWebRequest();
        onDone?.Invoke(req.responseCode, req.downloadHandler.text);
    }
}

