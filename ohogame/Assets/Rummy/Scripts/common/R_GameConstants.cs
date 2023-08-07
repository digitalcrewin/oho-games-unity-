using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_GameConstants : MonoBehaviour
{
    public static bool enableLog = true;
    public static bool enableErrorLog = true;
    public static bool isSeparateGame = true;
    
    public static int TURN_TIME = 20;
    public static int timerStart = 0;
    public static int playerbetAmount = 0;

#region WEB
    public const float NETWORK_CHECK_DELAY = 2f;
    public const int API_RETRY_LIMIT = 5;
    public const int API_TIME_OUT_LIMIT = 50;

    //public const string BASE_URL = "http://15.206.57.137"; //Production
    public const string BASE_URL = "http://3.111.178.138"; //Dev

    public const string URL = BASE_URL + ":4000/api/v1";
    public const string API_URL = URL;

    public const string SOCKET_URL = "http://3.111.178.138"+ ":5001";
    public const string NEW_API_URL = "http://3.111.178.138:4000/api/v1/rummy";

    public static string[] GAME_URLS =
    {
        API_URL +"/auth/register",
        API_URL +"/auth/login",
        API_URL +"/user/profile",
        API_URL +"/rooms",
        NEW_API_URL+"/player/type",
        NEW_API_URL+"/entryFee/Name/info",
        NEW_API_URL+"/get/gameId",
        NEW_API_URL+"/games"
    };
#endregion

#region ANIMATIONS
    public const float CARD_ANIMATION_DURATION = 0.27f;
    public const float BET_PLACE_ANIMATION_DURATION = 0.5f;
    public const float LOCAL_BET_ANIMATION_DURATION = 1f;

#endregion
}

[System.Serializable]
public enum R_RequestType
{
    Register,
    Login,
    Profile,
    Rooms,
    Type,
    Info,
    GameId,
    Games
}
