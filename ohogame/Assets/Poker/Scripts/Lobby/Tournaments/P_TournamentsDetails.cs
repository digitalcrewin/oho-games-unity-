using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class P_TournamentsDetails : MonoBehaviour
{
    public static P_TournamentsDetails instance;

    [SerializeField] Transform gameTypeContent;

    [SerializeField] Color nonSelectedGameTypeColor;
    [SerializeField] Color selectedGameTypeColor;

    [SerializeField] GameObject detailsItem, entriesItem, prizeItem, tablesItem;

    [Space(10)]

    [SerializeField] Text titleText;

    [Space(10)]

    // details
    //[SerializeField] Text detailsStartsWhenTxt;
    //[SerializeField] Text detailsPlayerCountTxt;
    //[SerializeField] Image detailsPlayersLineImg;
    [SerializeField] Text StartingInLbl;
    [SerializeField] Transform timerParent;
    [SerializeField] Text daysTxt;
    [SerializeField] Text hoursTxt;
    [SerializeField] Text minutesTxt;
    [SerializeField] Text secondsTxt;
    [SerializeField] Text startingLevelTxt;
    [SerializeField] Text lateRegistrationTxt;
    [SerializeField] Text blindsUpTxt;
    [SerializeField] Text detailsBuyInTxt;
    [SerializeField] Text detailsPrizeTxt;
    [SerializeField] Text reEntriesTxt;
    [SerializeField] Text avgStackTxt;


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
    //[SerializeField] Button entriesRegisterBtn;
    //[SerializeField] Text entriesRegisterBtnText;
    //[SerializeField] Image entriesRegisterBtnImage;
    //[SerializeField] Text entriesRegisterErrorText;

    [Space(10)]

    [SerializeField] Transform tablesScrollContent;
    [SerializeField] GameObject tablesItemPrefab;
    [SerializeField] GameObject tablesNoData;

    [Space(10)]

    [SerializeField] Button registerBtn;
    [SerializeField] Text registerBtnText;
    [SerializeField] Image registerBtnImage;
    [SerializeField] Sprite registerBtnBG;
    [SerializeField] Sprite unRegisterBtnBG;
    [SerializeField] Sprite buyChipsBtnBG;

    [Space(10)]

    [SerializeField] GameObject registerConfirmPopUp;
    [SerializeField] Text regCnfErrorTxt;
    [SerializeField] Text regCnfBuyInTxt;
    [SerializeField] Text regCnfMyBalanceTxt;
    [SerializeField] Button regCnfRegBtn;
    [SerializeField] Button regCnfCloseBtn;

    [Space(10)]

    [SerializeField] GameObject registerSuccessPopUp;
    [SerializeField] Text regSuccessTxt;
    [SerializeField] Image regSuccessImg;

    JsonData roomData;
    public JsonData RoomData
    {
        //get { return roomData; }   // get method
        set { roomData = value; }  // set method
    }
    string tournamentStatus = string.Empty;

    public bool isMyIdRegistered = false;
    bool isMyIdRegisteredFromPlayerAPI = false;

    Coroutine tournamentStartTimerCo;
    //Coroutine checkForStartTournamentCo;
    Coroutine tournamentRegisterCo;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetIntialData();
    }

    void SetIntialData()
    {
        for (int i = 0; i < gameTypeContent.childCount; i++)
        {
            Button gameTypeBtn = gameTypeContent.GetChild(i).GetComponent<Button>();
            gameTypeBtn.onClick.AddListener(() =>
            {
                GameTypeButtonClickSetImageNColor(gameTypeBtn, gameTypeBtn.gameObject.name);
            });
        }

        titleText.text = roomData["game_json_data"]["room_name"].ToString();

        startingLevelTxt.text = "-";
        lateRegistrationTxt.text = "-";
        blindsUpTxt.text = "-";
        detailsBuyInTxt.text = roomData["game_json_data"]["minimum_buyin"].ToString();
        regCnfBuyInTxt.text = roomData["game_json_data"]["minimum_buyin"].ToString();
        detailsPrizeTxt.text = roomData["game_json_data"]["prize_money"].ToString();
        reEntriesTxt.text = "-";
        avgStackTxt.text = roomData["game_json_data"]["default_stack"].ToString();

        bool isMyIdRegistered = false;
        
        IDictionary iDataIgame = roomData["game_json_data"] as IDictionary;

        if (iDataIgame.Contains("players"))
        {
            for (int j = 0; j < roomData["game_json_data"]["players"].Count; j++)
            {
                if (roomData["game_json_data"]["players"][j].ToString() == P_SocketController.instance.gamePlayerId)
                    isMyIdRegistered = true;
            }
        }

        DateTime regStartDate = Convert.ToDateTime(roomData["game_json_data"]["registration_start_date"].ToString()).ToLocalTime();
        //DateTime regStartDate = Convert.ToDateTime("2023-08-20T05:30:00.000Z").ToLocalTime();
        DateTime regEndDate = Convert.ToDateTime(roomData["game_json_data"]["registration_end_date"].ToString()).ToLocalTime();
        //DateTime regEndDate = Convert.ToDateTime("2023-08-21T05:30:00.000Z").ToLocalTime();
        DateTime gameStartDate = Convert.ToDateTime(roomData["game_json_data"]["start_date"].ToString()).ToLocalTime();
        //DateTime gameStartDate = Convert.ToDateTime("2023-08-19T20:10:00.000Z").ToLocalTime();
        Debug.Log("regStartDate:" + regStartDate + ", regEndDate:" + regEndDate + ", gameStartDate:" + gameStartDate + ", tournamentStatus:" + tournamentStatus);

        tournamentStatus = roomData["tournament_status"].ToString(); //"REGISTRATION_OPEN";

        if (tournamentStatus == "TOURNAMENT_STARTED")
        {
            StartingInLbl.text = "Started";
            for (int i = 0; i < timerParent.childCount; i++)
            {
                timerParent.GetChild(i).GetComponent<Text>().color = new Color32(255, 255, 255, 70);
            }
        }
        else if (tournamentStatus == "TOURNAMENT_ENDED")
        {
            StartingInLbl.text = "Finished";
            for (int i = 0; i < timerParent.childCount; i++)
            {
                timerParent.GetChild(i).GetComponent<Text>().color = new Color32(255, 255, 255, 70);
            }
        }
        else if (tournamentStatus == "CREATED" || tournamentStatus == "REGISTRATION_OPEN" || tournamentStatus == "REGISTRATION_CLOSED")
        {
            if (tournamentStatus == "CREATED")
                StartingInLbl.text = "Reg. will Start";
            else if (tournamentStatus == "REGISTRATION_OPEN" || tournamentStatus == "REGISTRATION_CLOSED")
                StartingInLbl.text = "Starting in";
        }

        DateTime today = DateTime.Now;

        TimeSpan differenceRegStart = today.Subtract((DateTime)regStartDate);
        int totalSecondsRegStart = (int)differenceRegStart.TotalSeconds;

        TimeSpan differenceGameStart = today.Subtract((DateTime)gameStartDate);
        int totalSecondsGameStart = (int)differenceGameStart.TotalSeconds;

        // registration k phele ka time
        if ((totalSecondsRegStart < 0))
        {
            tournamentRegisterCo = StartCoroutine(TournamentStartTimer((DateTime)regStartDate, totalSecondsRegStart, (myReturnValue) =>
            {
                StopCoroutine(tournamentRegisterCo);
                // recall GET_GAMES event
                P_SocketController.instance.SendGetRooms();
            }));
        }
        // game start k phele ka time
        else if ((totalSecondsGameStart < 0))
        {
            //tournamentStartTimerCo = StartCoroutine(StartDetailsTimer(gameStartDate)); //old
            tournamentStartTimerCo = StartCoroutine(TournamentStartTimer((DateTime)gameStartDate, totalSecondsGameStart, (myReturnValue) =>
            {
                StopCoroutine(tournamentStartTimerCo);
                // recall GET_GAMES event
                P_SocketController.instance.SendGetRooms();
            }));
        }
        


        if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
            StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/players", PlayersResponse));

        //isMyIdRegisteredFromPlayerAPI = isMyIdRegistered;
        //SetRegisterUnregisterBtnInteraction();

        //if (checkForStartTournamentCo != null)
        //    StopCoroutine(checkForStartTournamentCo);
        //checkForStartTournamentCo = StartCoroutine(CheckForStartTournament());
    }

    public void LobbyData(string responseData)
    {
        JsonData data = JsonMapper.ToObject(responseData);
        if (data["data"].Count > 0)
        {
            if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
            {
                for (int i = 0; i < data["data"].Count; i++)
                {
                    int tempI = i;
                    string categoryData = data["data"][i]["game_type"]["name"].ToString();

                    if (roomData["game_id"].ToString() == data["data"][i]["game_id"].ToString())
                    {
                        RoomData = data["data"][tempI];
                        SetIntialData();
                    }
                }
            }
        }
    }

    IEnumerator StartDetailsTimer(DateTime gameStartDt)//TimeSpan t, Text sitNGoTimerText)
    {
        TimeSpan differenceGameStart = DateTime.Now.Subtract((DateTime)gameStartDt);
        //Debug.Log("differenceGameStart.TotalSeconds:" + differenceGameStart.TotalSeconds);
        while (differenceGameStart.TotalSeconds < 0)
        {
            differenceGameStart = differenceGameStart.Add(TimeSpan.FromSeconds(1));
            //Debug.Log("t.Days:" + differenceGameStart.Days + ", t.Hours:" + differenceGameStart.Hours + ", t.Minutes:" + differenceGameStart.Minutes);
            daysTxt.text = ((int)differenceGameStart.Days * -1).ToString("D2");
            hoursTxt.text = ((int)differenceGameStart.Hours * -1).ToString("D2");
            minutesTxt.text = ((int)differenceGameStart.Minutes * -1).ToString("D2");
            secondsTxt.text = ((int)differenceGameStart.Seconds * -1).ToString("D2");
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator TournamentStartTimer(DateTime tournDateTime, int totalSeconds, System.Action<bool> callback)
    {
        for (int i = totalSeconds; i <= 0; i++)
        {
            TimeSpan difference1 = DateTime.Now.Subtract(tournDateTime);
            if (i == 0)
            {
                daysTxt.text = "00";
                hoursTxt.text = "00";
                minutesTxt.text = "00";
                secondsTxt.text = "00";
                yield return null;
                callback(true);
                break;
            }
            else
            {
                try
                {
                    //if (difference1.TotalHours < 0 && ((int)difference1.TotalHours != 0))
                    //{
                        daysTxt.text = ((int)difference1.Days * -1).ToString("D2");
                        hoursTxt.text = ((int)difference1.Hours * -1).ToString("D2");
                        minutesTxt.text = ((int)difference1.Minutes * -1).ToString("D2");
                        secondsTxt.text = ((int)difference1.Seconds * -1).ToString("D2");
                    //}
                }
                catch (Exception e)
                {
                    Debug.Log("error in timerText: " + e.Message);
                }
                yield return new WaitForSecondsRealtime(1f);
            }
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
        tablesItem.SetActive(false);

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
        else if (buttonSelected.gameObject.name == "Tables")
        {
            tablesItem.SetActive(true);
        }    
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
                if (entriesItem.activeInHierarchy)
                {
                    DestroyAllPlayersDataItemPrefab();
                    entriesNoData.SetActive(false);
                    entriesSelfEntry.SetActive(false);
                }
                bool isMyIdRegisteredFromPlayerAPITemp = false;

                for (int i = 0; i < data["data"].Count; i++)
                {
                    if (entriesItem.activeInHierarchy)
                    {
                        GameObject go = Instantiate(entriesItemPrefab, entriesScrollContent);
                        go.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = data["data"][i]["userName"].ToString();
                        go.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "<sup><size=30><sprite=0></size></sup>" + data["data"][i]["stack"].ToString();
                    }

                    if (data["data"][i]["userId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        if (entriesItem.activeInHierarchy)
                        {
                            entriesSelfEntry.SetActive(true);
                            entriesSelfEntryName.text = data["data"][i]["userName"].ToString();
                            entriesSelfEntryStack.text = "<sup><size=30><sprite=0></size></sup>" + data["data"][i]["stack"].ToString();
                        }

                        //entriesRegisterBtn.interactable = false;
                        //entriesRegisterBtnText.text = "Registered";
                        ////Image entriesRegisterBtnImage

                        isMyIdRegisteredFromPlayerAPITemp = true;
                    }
                }
                isMyIdRegisteredFromPlayerAPI = isMyIdRegisteredFromPlayerAPITemp;


                //float maxPlayers = 0f;
                //if (float.TryParse(roomData["game_json_data"]["maximum_player"].ToString(), out maxPlayers)) { }
                //if (data["data"].Count == maxPlayers)
                //{
                //    //entriesRegisterBtn.interactable = false;
                //}
                //else
                //{
                //    //entriesRegisterBtn.interactable = true;
                //    if (P_GameConstant.enableErrorLog)
                //        Debug.Log("entriesRegisterBtn false 2 else");
                //}
            }
            else
            {
                if (entriesItem.activeInHierarchy)
                    entriesNoData.SetActive(true);
                //entriesRegisterBtn.interactable = true;
            }
            SetRegisterUnregisterBtnInteraction();
        }
        else
        {
            if (entriesItem.activeInHierarchy)
                entriesNoData.SetActive(true);
            //entriesRegisterBtn.interactable = true;
        }
    }

    void SetRegisterUnregisterBtnInteraction()
    {
        //Debug.Log("earlier 0");

        if (tournamentStatus == "REGISTRATION_OPEN")
        {
            DateTime today = DateTime.Now;
            DateTime regStartDate = Convert.ToDateTime(roomData["game_json_data"]["registration_start_date"].ToString()).ToLocalTime();
            int regStartDateCompare = DateTime.Compare(regStartDate, today);
            DateTime regEndDate = Convert.ToDateTime(roomData["game_json_data"]["registration_end_date"].ToString()).ToLocalTime();
            int regEndDateCompare = DateTime.Compare(regEndDate, today);
            //Debug.Log("earlier 1 regStartDate:" + regStartDate + ", regEndDate:" + regEndDate);

            if (regStartDateCompare < 0)
            {
                //"is earlier than";

                if (regEndDateCompare > 0)
                {
                    //"is later than";

                    // show register/unregister button
                    registerBtn.interactable = true;
                    //Debug.Log("earlier 2");
                }
                else
                {
                    registerBtn.interactable = false;
                    //Debug.Log("earlier 3");
                }
            }
            else
            {
                registerBtn.interactable = false;
                //Debug.Log("earlier 5");
            }
        }
        else
        {
            registerBtn.interactable = false;
            //Debug.Log("earlier 6");
        }

        //Debug.Log("earlier isMyIdRegisteredFromPlayerAPI");
        if (isMyIdRegisteredFromPlayerAPI) //(isMyIdRegistered)
        {
            registerBtnImage.sprite = unRegisterBtnBG;
            registerBtnText.text = "Unregister";

            // start coroutine that continous check for tournament start
            //if (checkForStartTournamentCo != null)
            //    StopCoroutine(checkForStartTournamentCo);
            //checkForStartTournamentCo = StartCoroutine(CheckForStartTournament());

            //Debug.Log("earlier isMyIdRegisteredFromPlayerAPI IF");
        }
        else
        {
            registerBtnImage.sprite = registerBtnBG;
            registerBtnText.text = "Register";

            //if (checkForStartTournamentCo != null)
            //    StopCoroutine(checkForStartTournamentCo);

            //Debug.Log("earlier isMyIdRegisteredFromPlayerAPI ELSE");
        }
    }

    //IEnumerator CheckForStartTournament()
    //{
    //    DateTime gameStartDate = Convert.ToDateTime(roomData["game_json_data"]["start_date"].ToString()).ToLocalTime();
    //    //DateTime gameStartDate = Convert.ToDateTime("2023-08-21T09:39:00.000Z").ToLocalTime();
    //    Debug.Log("Join Tournament gameStartDate:" + gameStartDate);

    //    TimeSpan differenceGameStart = DateTime.Now.Subtract(gameStartDate);
    //    //Debug.Log("differenceGameStart.TotalSeconds:" + differenceGameStart.TotalSeconds);

    //    while (differenceGameStart.TotalSeconds < 0)
    //    {
    //        differenceGameStart = differenceGameStart.Add(TimeSpan.FromSeconds(1));
    //        //Debug.Log("t.Days:" + differenceGameStart.Days + ", t.Hours:" + differenceGameStart.Hours + ", t.Minutes:" + differenceGameStart.Minutes);
    //        yield return new WaitForSeconds(1);
    //        Debug.Log("Join Tournament while");
    //    }
    //    Debug.Log("Join Tournament while end TotalSeconds:"+ (int) differenceGameStart.TotalSeconds + ", isMyIdRegisteredFromPlayerAPI:"+ isMyIdRegisteredFromPlayerAPI);
    //    if ((int) differenceGameStart.TotalSeconds >= 0)
    //    {
    //        if (isMyIdRegisteredFromPlayerAPI) //(tournamentStatus == "TOURNAMENT_STARTED") && 
    //        {
    //            P_SocketController.instance.gameId = roomData["game_id"].ToString();
    //            //P_SocketController.instance.TABLE_ID = roomData["game_id"].ToString();

    //            IDictionary RoomDataGameJsonData = roomData["game_json_data"] as IDictionary;
    //            if (RoomDataGameJsonData.Contains("maximum_player_in_table"))
    //                P_SocketController.instance.gameTableMaxPlayers = Convert.ToInt32(roomData["game_json_data"]["maximum_player_in_table"].ToString());

    //            P_SocketController.instance.SendJoinTournament();
    //            Debug.Log("Join Tournament Sended");
    //        }
    //        yield return null;
    //        Debug.Log("Join Tournament null");
    //    }
    //}

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

                prizePrizePoolTxt.text = roomData["game_json_data"]["prize_money"].ToString();
                prizePlacesPaidTxt.text = data["data"]["placesPaid"].ToString();

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
                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.LobbyTournaments);
                if (P_Lobby.instance != null)
                {
                    for (int i = 0; i < P_Lobby.instance.gameTypeStr.Length; i++)
                    {
                        int tempI = i;
                        if (P_Lobby.instance.gameTypeStr[i] == P_SocketController.instance.lobbySelectedGameType)
                        {
                            P_Lobby.instance.OnGameTypeButtonClick(tempI, P_SocketController.instance.lobbySelectedGameType);
                        }
                    }
                }
                P_SocketController.instance.lobbySelectedGameType = "";
                break;

            case "blind_structure":
                P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbyTournamentsBlindStructure);
                if (P_TournamentsBlindStructure.instance != null)
                    P_TournamentsBlindStructure.instance.GameId = roomData["game_id"].ToString();
                break;

            case "registerBtn":
                if (registerBtnText.text == "Register")
                {
                    //entriesRegisterBtn.interactable = false;

                    //P_SocketController.instance.SendJoin(P_SocketController.instance.TABLE_ID, roomData["game_json_data"]["minimum_buyin"].ToString());

                    //StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.3f, () =>
                    //{
                    //    if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
                    //        StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/players", PlayersResponse));

                    //    //GameStarted();
                    //}));
                    registerConfirmPopUp.SetActive(true);
                    StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/get-wallet", (string serverResponse, bool isErrorMessage, string errorMessage) =>
                    {
                        JsonData data = JsonMapper.ToObject(serverResponse);
                        if (data["statusCode"].ToString() == "200")
                        {
                            int totalBalance = int.Parse(data["data"]["real_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["bonus_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["win_amount"].ToString().Split('.')[0]);
                            regCnfMyBalanceTxt.text = "" + totalBalance;
                        }
                    }));

                }
                else if (registerBtnText.text == "Unregister")
                {
                    registerBtn.interactable = false;
                    string jsonRequestDeReg = "{\"game_id\":" + roomData["game_id"].ToString() + "}";
                    Debug.Log("register request json:" + jsonRequestDeReg);
                    StartCoroutine(P_WebServices.instance.POSTRequestData(P_RequestType.DeRegisterTournament, jsonRequestDeReg, (string downloadText, bool isError, string errorString) =>
                    {
                        bool isResponseError = false;
                        string unRegErrMsg = "Error in Unregister";
                        if (isError)
                        {
                            if (P_GameConstant.enableErrorLog)
                                Debug.Log("Tournament DeRegister Error: " + errorString);

                            isResponseError = true;
                        }
                        else
                        {
                            if (P_GameConstant.enableLog)
                                Debug.Log("Tournament DeRegister: " + downloadText);

                            JsonData data = JsonMapper.ToObject(downloadText);
                            IDictionary iData = data as IDictionary;
                            if (iData.Contains("statusCode"))
                            {
                                if (data["statusCode"].ToString() == "200")
                                {
                                    registerBtnImage.sprite = registerBtnBG;
                                    registerBtnText.text = "Register";
                                }
                                else
                                {
                                    isResponseError = true;
                                    if (iData.Contains("message"))
                                    {
                                        unRegErrMsg = data["message"].ToString();
                                    }
                                }
                            }
                            else
                            {
                                isResponseError = true;
                            }
                        }
                        if (isResponseError)
                        {
                            P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.Message);
                            if (P_Message.instance != null)
                            {
                                P_Message.instance.HideHeadingCloseButton();
                                P_Message.instance.headingText.text = "Error";
                                P_Message.instance.ShowSingleButtonPopUp(unRegErrMsg, () => {
                                    P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.Message);
                                });
                            }
                            registerBtnImage.sprite = unRegisterBtnBG;
                            registerBtnText.text = "Unregister";
                        }
                        registerBtn.interactable = true;

                        if (entriesItem.activeInHierarchy)
                        {
                            if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
                                StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/players", PlayersResponse));
                        }
                    }));
                }
                break;

            case "regCnfRegisterBtn":

                regCnfRegBtn.interactable = false;
                regCnfCloseBtn.interactable = false;
                string jsonRequest = "{\"game_id\":" + roomData["game_id"].ToString() + "}";
                Debug.Log("register request json:" + jsonRequest);
                StartCoroutine(P_WebServices.instance.POSTRequestData(P_RequestType.RegisterTournament, jsonRequest, (string downloadText, bool isError, string errorString) =>
                {
                    bool isResponseError = false;
                    string regErrMsg = "Error in Register";
                    if (isError)
                    {
                        if (P_GameConstant.enableErrorLog)
                            Debug.Log("Tournament Register Error: " + errorString);

                        isResponseError = true;
                    }
                    else
                    {
                        if (P_GameConstant.enableLog)
                            Debug.Log("Tournament Register: " + downloadText);

                        JsonData data = JsonMapper.ToObject(downloadText);
                        IDictionary iData = data as IDictionary;
                        if (iData.Contains("statusCode"))
                        {
                            if (data["statusCode"].ToString() == "200")
                            {
                                registerBtnImage.sprite = unRegisterBtnBG;
                                registerBtnText.text = "Unregister";
                                registerConfirmPopUp.SetActive(false);
                                regSuccessTxt.text = "Registered Successfully";
                                registerSuccessPopUp.SetActive(true);
                            }
                            else
                            {
                                isResponseError = true;
                                if (iData.Contains("message"))
                                {
                                    regErrMsg = data["message"].ToString();
                                }
                            }
                        }
                        else
                        {
                            isResponseError = true;
                        }
                    }
                    if (isResponseError)
                    {
                        
                        P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.Message);
                        if (P_Message.instance != null)
                        {
                            P_Message.instance.HideHeadingCloseButton();
                            P_Message.instance.headingText.text = "Error";
                            P_Message.instance.ShowSingleButtonPopUp(regErrMsg, () => {
                                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.Message);
                                registerConfirmPopUp.SetActive(false);
                            });
                        }
                    }
                    regCnfRegBtn.interactable = true;
                    regCnfCloseBtn.interactable = true;

                    if (entriesItem.activeInHierarchy)
                    {
                        if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
                            StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/players", PlayersResponse));
                    }
                }));
                
                break;

            case "regCnfClosePopUpBtn":
                registerConfirmPopUp.SetActive(false);
                break;

            case "regSuccessClosePopUpBtn":
                registerSuccessPopUp.SetActive(false);
                //StartCoroutine(GlobalGameManager.instance.RunAfterDelay(3f, () => {
                //    if (registerBtnText.text == "Unregister")
                //    {
                //        P_MainSceneManager.instance.ScreenDestroy();
                //        P_MainSceneManager.instance.LoadScene(P_MainScenes.InGame);
                //        P_InGameUiManager.instance.ShowTournamentReBuyPopUp();
                //    }
                //}));
                break;
        }
    }


    public void OnTournamentGameStartedReceived(string str)
    {
        Debug.Log("P_TournamentDetails TournamentGameStarted: " + str);
        P_SocketController.instance.tableData = roomData;
    }

    void OnDestroy()
    {
        if (tournamentRegisterCo != null)
            StopCoroutine(tournamentRegisterCo);
        if (tournamentStartTimerCo != null)
            StopCoroutine(tournamentStartTimerCo);
    }
}
