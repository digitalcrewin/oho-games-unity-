using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class P_SitNGoDetails : MonoBehaviour
{
    public static P_SitNGoDetails instance;

    public Transform gameTypeContent;

    public Color nonSelectedGameTypeColor;
    public Color selectedGameTypeColor;

    public GameObject detailsItem, entriesItem, prizeItem;

    JsonData roomData;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < gameTypeContent.childCount; i++)
        {
            Button gameTypeBtn = gameTypeContent.GetChild(i).GetComponent<Button>();
            gameTypeBtn.onClick.AddListener(() => {
                GameTypeButtonClickSetImageNColor(gameTypeBtn, gameTypeBtn.gameObject.name);
            });
        }
    }

    public void GameTypeButtonClickSetImageNColor(Button buttonSelected, string gameTypeSelected)
    {
        buttonSelected.transform.GetChild(0).gameObject.SetActive(false);
        buttonSelected.transform.GetChild(1).gameObject.SetActive(true);
        buttonSelected.transform.GetChild(2).GetComponent<Text>().color = selectedGameTypeColor;
        buttonSelected.transform.GetChild(3).gameObject.SetActive(true);

        for (int i = 0; i < gameTypeContent.childCount; i++)
        {
            if (gameTypeContent.GetChild(i).gameObject.name != buttonSelected.gameObject.name)
            {
                Button gameTypeBtnOff = gameTypeContent.GetChild(i).GetComponent<Button>();
                gameTypeBtnOff.transform.GetChild(1).gameObject.SetActive(false);
                gameTypeBtnOff.transform.GetChild(0).gameObject.SetActive(true);
                gameTypeBtnOff.transform.GetChild(2).GetComponent<Text>().color = nonSelectedGameTypeColor;
                gameTypeBtnOff.transform.GetChild(3).gameObject.SetActive(false);
            }
        }

        detailsItem.SetActive(false);
        entriesItem.SetActive(false);
        prizeItem.SetActive(false);

        if (buttonSelected.gameObject.name == "Details")
            detailsItem.SetActive(true);
        else if (buttonSelected.gameObject.name == "Entries")
            entriesItem.SetActive(true);
        else if (buttonSelected.gameObject.name == "Prize")
            prizeItem.SetActive(true);
        //else if (buttonSelected.gameObject.name == "Tables")
    }

    public void OnClickOnButton(string buttonName)
    {
        switch (buttonName)
        {
            case "back":
                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.LobbySecondSitNGo);
                break;

            case "blind_structure":
                P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbySitNGoBlindStructure);
                break;

            case "registerBtn":
                P_SocketController.instance.SendGetTables(roomData["game_id"].ToString());
                break;
        }
    }

    public void OnLoadScrollDetails(JsonData dataOfI)
    {
        if (P_GameConstant.enableLog)
            Debug.Log(JsonMapper.ToJson(dataOfI));

        roomData = dataOfI;
    }

    public void OnSitNGoTableData(string responseData)
    {
        JsonData data = JsonMapper.ToObject(responseData);

        Debug.Log("OnSitNGoTableData: " + data);
    }
}
