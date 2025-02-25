using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientInfo
{
    public static string Username
    {
        get => PlayerPrefs.GetString("C_Username", string.Empty);
        set => PlayerPrefs.SetString("C_Username", value);
    }

    public static int ClientId
    {
        get => PlayerPrefs.GetInt("C_ClientId", 0);
        set => PlayerPrefs.SetInt("C_ClientId", value);
    }

    public static string LobbyName
    {
        get => PlayerPrefs.GetString("C_LastLobbyName", "");
        set => PlayerPrefs.SetString("C_LastLobbyName", value);
    }
}
