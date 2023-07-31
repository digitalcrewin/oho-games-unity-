using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class R_PrefsManager : MonoBehaviour
{
    public static void SetData(R_PrefsKey prefsKey,string dataToAssign)
    {
        Debug.Log(prefsKey + " AAA " + dataToAssign);
        PlayerPrefs.SetString("" + prefsKey, dataToAssign);
        Debug.Log("AAA " + PlayerPrefs.GetString("" + prefsKey, ""));
    }

    public static void SetPlayerGameData(R_PlayerGameDetails dataToAssign)
    {
        PlayerPrefs.SetString("" + R_PrefsKey.R_PlayerGameData, JsonUtility.ToJson(dataToAssign));
    }

    public static void DeletePlayerData()
    {
        PlayerPrefs.DeleteKey("" + R_PrefsKey.R_PlayerGameData);
    }

    public static R_PlayerGameDetails GetPlayerData()
    {
        R_PlayerGameDetails playerData = new R_PlayerGameDetails();
       
        string prefsData = PlayerPrefs.GetString(""+ R_PrefsKey.R_PlayerGameData,"");

        Debug.Log(prefsData);
        if (!string.IsNullOrEmpty(prefsData))
        {
            Debug.Log(prefsData);
            JsonData data = JsonMapper.ToObject(prefsData);

            // for separate login
            playerData.userId = data["userId"].ToString();
            //playerData.name = data["name"].ToString();
            playerData.userName = data["userName"].ToString();
            playerData.email = data["email"].ToString();
            playerData.token = data["token"].ToString();
            //playerData.password = data["password"].ToString();
            playerData.avatarURL = data["avatarURL"].ToString();

            // playerData.userName = data["userName"].ToString();
            // playerData.registrationType = data["registrationType"].ToString();
        }
        else
        {
            playerData.userName = playerData.password = playerData.userId = "";
        }

        return playerData;
    }
}

public enum R_PrefsKey
{
    R_PlayerGameData,
}