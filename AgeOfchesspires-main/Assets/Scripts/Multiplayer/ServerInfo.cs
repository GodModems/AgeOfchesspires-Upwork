using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public static class ServerInfo
{

    public const int UserCapacity = 2; //the actual hard limit

    public static string LobbyName;
    public static string ClientName;

    public static int GameMode
    {
        get => PlayerPrefs.GetInt("S_GameMode", 0);
        set => PlayerPrefs.SetInt("S_GameMode", value);
    }

    public static int ClientId
    {
        get => PlayerPrefs.GetInt("S_ClientId", 0);
        set => PlayerPrefs.SetInt("S_Clientd", value);
    }

   /* public static int MaxUsers
    {
        get => PlayerPrefs.GetInt("S_MaxUsers", 2);
        set => PlayerPrefs.SetInt("S_MaxUsers", Mathf.Clamp(value, 1, UserCapacity));
    }*/
}
