using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

public class P_SitNGoDetails : MonoBehaviour
{
    public static P_SitNGoDetails instance;

    [SerializeField] Transform gameTypeContent;

    [SerializeField] Color nonSelectedGameTypeColor;
    [SerializeField] Color selectedGameTypeColor;

    [SerializeField] GameObject detailsItem, entriesItem, prizeItem;

    [Space(10)]

    [SerializeField] Text titleText;

    [Space(10)]

    // details
    [SerializeField] Text detailsStartsWhenTxt;
    [SerializeField] Text detailsPlayerCountTxt;
    [SerializeField] Image detailsPlayersLineImg;
    [SerializeField] Text detailsBuyInTxt;
    [SerializeField] Text detailsPrizeTxt;


    [Space(10)]

    [SerializeField] Text prizePrizePoolTxt;
    [SerializeField] Text prizePlacesPaidTxt;
    [SerializeField] Transform prizeScrollContent;
    [SerializeField] GameObject rankPrizeItemPrefab;
    [SerializeField] GameObject rankPrizeNoData;


    [Space(10)]

    // entries (players)
    [SerializeField] Transform entriesScrollContent;
    [SerializeField] GameObject entriesItemPrefab;
    [SerializeField] GameObject entriesNoData;
    [SerializeField] GameObject entriesSelfEntry;
    [SerializeField] Text entriesSelfEntryName;
    [SerializeField] TMP_Text entriesSelfEntryStack;
    [SerializeField] Text entriesRegisterBtnText;
    [SerializeField] Image entriesRegisterBtnImage;

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

        P_SocketController.instance.SendGetTables(roomData["game_id"].ToString());


        float maxPlayers = 0f, totalPlayers = 0f;
        if (float.TryParse(roomData["game_json_data"]["maximum_player"].ToString(), out maxPlayers)) { }
        if (float.TryParse(roomData["totalPlayers"].ToString(), out totalPlayers)) { }
        detailsStartsWhenTxt.text = "Starts when " + maxPlayers + " player joins";
        detailsPlayerCountTxt.text = totalPlayers + "/" + maxPlayers;
        titleText.text = roomData["game_json_data"]["room_name"].ToString();

        detailsBuyInTxt.text = roomData["game_json_data"]["minimum_buyin"].ToString();
        detailsPrizeTxt.text = roomData["game_json_data"]["prize_money"].ToString();

        try
        {
            detailsPlayersLineImg.fillAmount = (totalPlayers / maxPlayers);
        }
        catch (System.Exception e)
        {
            // for division error
            Debug.Log("Division error in players line image");
            detailsPlayersLineImg.fillAmount = 0f;
        }
    }

    void GameTypeButtonClickSetImageNColor(Button buttonSelected, string gameTypeSelected)
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
        {
            detailsItem.SetActive(true);
        }
        else if (buttonSelected.gameObject.name == "Entries")
        {
            entriesItem.SetActive(true);
            if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
                StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/players", PlayersResponse));
        }
        else if (buttonSelected.gameObject.name == "Prize")
        {
            prizeItem.SetActive(true);
            if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
                StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/prize-data", PrizeDataResponse));
        }
        //else if (buttonSelected.gameObject.name == "Tables")
    }

    void PlayersResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        if (P_GameConstant.enableErrorLog)
            Debug.Log("players data Response : " + serverResponse);

        JsonData data = JsonMapper.ToObject(serverResponse);

        //string testData = "{\"message\":\"Players\",\"statusCode\":200,\"status\":true,\"data\":[{\"userId\":\"97\",\"userName\":\"aditya\",\"stack\":10000},{\"userId\":\"26\",\"userName\":\"NDEWZ\",\"stack\":10000}]}";
        //JsonData data = JsonMapper.ToObject(testData);

        if (data["statusCode"].ToString() == "200")
        {
            if (data["data"].Count > 0)
            {
                DestroyAllPlayersDataItemPrefab();

                entriesNoData.SetActive(false);
                entriesSelfEntry.SetActive(false);

                for (int i = 0; i < data["data"].Count; i++)
                {
                    GameObject go = Instantiate(entriesItemPrefab, entriesScrollContent);
                    go.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = data["data"][i]["userName"].ToString();
                    go.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "<sup><size=30><sprite=0></size></sup>" + data["data"][i]["stack"].ToString();

                    if (data["data"][i]["userId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        entriesSelfEntry.SetActive(true);
                        entriesSelfEntryName.text = data["data"][i]["userName"].ToString();
                        entriesSelfEntryStack.text = "<sup><size=30><sprite=0></size></sup>" + data["data"][i]["stack"].ToString();

                        //entriesRegisterBtnText.text = "Join";
                        //Image entriesRegisterBtnImage
                    }
                }
            }
            else
            {
                entriesNoData.SetActive(true);
            }
        }
        else
        {
            entriesNoData.SetActive(true);
        }
    }

    void PrizeDataResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        if (P_GameConstant.enableErrorLog)
            Debug.Log("prize data Response : " + serverResponse);

        JsonData data = JsonMapper.ToObject(serverResponse);

        if (data["statusCode"].ToString() == "200")
        {
            if (data["data"].Count > 0)
            {
                DestroyAllPrizeDataItemPrefab();

                rankPrizeNoData.SetActive(false);

                for (int i = 0; i < data["data"]["prizeData"].Count; i++)
                {
                    GameObject go = Instantiate(rankPrizeItemPrefab, prizeScrollContent);
                    go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = data["data"]["prizeData"][i]["rank"].ToString();
                    go.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "<sup><size=30><sprite=0></size></sup>" + data["data"]["prizeData"][i]["price"].ToString();
                }
            }
            else
            {
                rankPrizeNoData.SetActive(true);
            }
        }
        else
        {
            rankPrizeNoData.SetActive(true);
        }
    }

    void DestroyAllPlayersDataItemPrefab()
    {
        for (int i = 0; i < entriesScrollContent.childCount; i++)
        {
            Destroy(entriesScrollContent.transform.GetChild(i).gameObject);
        }
    }

    void DestroyAllPrizeDataItemPrefab()
    {
        for (int i = 0; i < prizeScrollContent.childCount; i++)
        {
            if (prizeScrollContent.transform.GetChild(i).name != "PrizePlacesBG" && prizeScrollContent.transform.GetChild(i).name != "RankPrizeLblBG")
            Destroy(prizeScrollContent.transform.GetChild(i).gameObject);
        }
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
                if (P_SitNGoBlindStructure.instance != null)
                    P_SitNGoBlindStructure.instance.GameId = roomData["game_id"].ToString();
                break;

            case "registerBtn":
                
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

        Debug.Log("OnSitNGoTableData: " + data.ToJson());

        IDictionary iData = data as IDictionary;

        if (iData.Contains("data"))
        {
            //for (int i = 0; i < data["data"].Count; i++)
            //{
            Debug.Log("players count: " + data["data"][0]["table_attributes"]["players"].Count);
            Debug.Log("maxPlayers " + data["data"][0]["table_attributes"]["maxPlayers"]);

            // GET_TABLES_BY_GAME_ID_RES's data not matched from GET_GAMES_RES, so set according to GET_GAMES_RES
            //float maxPlayers = 0f, totalPlayers = 0f;
            //if (float.TryParse(data["data"][0]["table_attributes"]["maxPlayers"].ToString(), out maxPlayers)) { }
            //if (float.TryParse(data["data"][0]["table_attributes"]["players"].Count.ToString(), out totalPlayers)) { }
            //detailsStartsWhenTxt.text = "Starts when " + maxPlayers + " player joins";
            //detailsPlayerCountTxt.text = totalPlayers + "/" + maxPlayers;
            //titleText.text = data["data"][0]["table_name"].ToString();

            //detailsBuyInTxt.text = data["data"][0]["buyIn"].ToString();
            //detailsPrizeTxt.text = data["data"][0]["prizeMoney"].ToString();

            //try
            //{
            //    detailsPlayersLineImg.fillAmount = (totalPlayers / maxPlayers);
            //}
            //catch (System.Exception e)
            //{
            //    // for division error
            //    Debug.Log("Division error in players line image");
            //    detailsPlayersLineImg.fillAmount = 0f;
            //}
            //}
        }
    }
}
