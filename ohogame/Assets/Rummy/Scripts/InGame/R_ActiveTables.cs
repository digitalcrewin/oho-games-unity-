using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;

public class R_ActiveTables : MonoBehaviour
{
    public static R_ActiveTables instance;

    [Header("for table scrolling")]
    public Transform scrollContent;
    public GameObject scrolling, tableItem, msgText;
    public Text freeChipsText, realMoneyText;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(R_WebServices.instance.GETRequestData(R_RequestType.Rooms, OnServerResponseFound));

        // freeChipsText.text = PlayerManager.instance.GetPlayerGameData().coins.ToString();
        // realMoneyText.text = PlayerManager.instance.GetPlayerGameData().real.ToString();
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);
        R_SoundManager.instance.PlaySound(R_SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    RummyMainMenuController.instance.DestroyScreen(RummyMainMenuScreens.ActiveTable);
                }
                break;
            default:
                if (R_GameConstants.enableErrorLog)
			        Debug.LogError("unhdnled eventName found in LobbyUiManager = " + eventName);
                break;
        }
    }

    public void OnServerResponseFound(string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (isShowErrorMessage)
        {
            if (errorMessage.Length > 0)
            {
                if (R_GameConstants.enableLog)
                    Debug.Log("errorMessage: " + errorMessage);
                msgText.GetComponent<Text>().text = "Error in Server Response!";
            }
            return;
        }

        if (R_GameConstants.enableLog)
            Debug.Log("serverResponse: " + serverResponse);

        if (string.IsNullOrEmpty(serverResponse))
        {
            msgText.GetComponent<Text>().text = "No Server Response Found!";
            return;
        }

        JsonData data = JsonMapper.ToObject(serverResponse);
        if (data["rows"] != null)
        {
            int tableCount = data["rows"].Count;
            if (tableCount > 0)
            {
                for (int i = 0; i < tableCount; i++)
                {
                    GameObject gm = Instantiate(tableItem, scrollContent);
                    gm.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = data["rows"][i]["name"].ToString();
                    gm.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = data["rows"][i]["minPlayers"].ToString() + " / " + data["rows"][i]["maxPlayers"].ToString();
                    gm.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = data["rows"][i]["minChips"].ToString() + " Chips";
                    //R_Utility.GetTrimmedAmount() //to use 1K, 2K, etc.

                    int index = i;
                    gm.name = gm.name + "_" + index;
                    gm.transform.GetComponent<Button>().onClick.AddListener(() => OnClickOnTableItem(index, data["rows"][index]["id"].ToString(), data["rows"][index]));                              
                }
                msgText.SetActive(false);
                scrolling.SetActive(true);
            }
        }
    }

    void OnClickOnTableItem(int index, string gameType, JsonData selectedRow)
    {
        //if (index==0)
        //{
            Debug.Log(gameType+" selected currentUserId="+R_PlayerManager.instance.GetPlayerGameData().userId);
            R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);

            R_GlobalGameManager.instance.mainSocketController.GetComponent<R_SocketController>().enabled = true;
            R_SocketController.instance.rummyUserId = R_PlayerManager.instance.GetPlayerGameData().userId;
            R_SocketController.instance.rummyGameType = gameType;
            R_SocketController.instance.selectedRow = selectedRow;
            if(R_GlobalGameManager.instance.isReJoinGame)
                R_SocketController.instance.ReStart();
        //}
    }
}
