using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_PlayerManager : MonoBehaviour
{
    public static R_PlayerManager instance;

    [SerializeField]
    private R_PlayerGameDetails playerGameData = null; // contains user details


    private void Awake()
    {
        instance = this;
        R_PlayerGameDetails rpd = new R_PlayerGameDetails();
        rpd.userId = PlayerManager.instance.GetPlayerGameData().userId;
        rpd.token = PlayerManager.instance.GetPlayerGameData().token;
        rpd.userName = PlayerManager.instance.GetPlayerGameData().userName;
        R_PrefsManager.SetPlayerGameData(rpd);
    }

    private void Start()
    {
        playerGameData = R_PrefsManager.GetPlayerData();
        GlobalGameManager.token = playerGameData.token;
        Debug.Log("userId=" + playerGameData.userName);
    }

    public void DeletePlayerGameData()
    {
        playerGameData = null;
    }

    public R_PlayerGameDetails GetPlayerGameData()
    {
        return playerGameData;
    }

    public void SetPlayerGameData(R_PlayerGameDetails dataToAssign)
    {
        playerGameData = dataToAssign;
        // R_PrefsManager.SetPlayerGameData(playerGameData);
    }

    public bool IsLogedIn()
    {
        Debug.Log(playerGameData.userId);
        if (playerGameData != null && playerGameData.userId.Length > 0)
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public class R_PlayerGameDetails
{
    public string userId;
    public string userName;
    public string password;
    public string avatarURL,FrameUrl,CountryURL;
    public string referralCode;
    public string userLevel;
    public string countryName;
    public string countryCode;
    public float coins, diamonds, points;
    public int rabit, emoji, time;
    public bool isSilverCardPurchased;
    public bool isBronzeCardPurchased;
    public bool isPlatinumCardPurchased;
    public string registrationType;
    public string gender;
    public string dob;
    public string panNo;
    public string address;
    public string mobileNo;
    public string email;
    public string name;
    public string token;
}
