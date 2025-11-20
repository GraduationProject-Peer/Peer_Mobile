using UnityEngine;

public static class SessionUtil
{
    public static bool IsGuest() => PlayerPrefs.GetInt("is_guest", 0) == 1;
    public static void SetGuest(bool isGuest)
    {
        PlayerPrefs.SetInt("is_guest", isGuest ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static string GetToken() => PlayerPrefs.GetString("access_token", "");
    public static void SetToken(string token)
    {
        if (string.IsNullOrEmpty(token)) PlayerPrefs.DeleteKey("access_token");
        else PlayerPrefs.SetString("access_token", token);
        PlayerPrefs.Save();
    }
}

