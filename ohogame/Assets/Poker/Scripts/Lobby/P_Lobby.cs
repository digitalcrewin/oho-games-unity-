using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;

public class P_Lobby : MonoBehaviour
{
    public static P_Lobby instance;

    public string[] gameTypeStr = { "TEXAS", "PLO", "SIT N GO", "ANONYMOUS", "PRACTICE", "TOURNAMENT" };

    public Transform gameTypeContent;
    public Transform mainScrollViewContent;
    public GameObject gameTypePrefab;
    public GameObject texasPrefab;
    public GameObject holdemPrefab;
    public GameObject PLOPrefab;
    public GameObject sitNGoPrefab;
    public GameObject practicePrefab;
    public GameObject tournamentPrefab;
    public GameObject secondTexasPrefab;

    public Color nonSelectedGameTypeColor;
    public Color selectedGameTypeColor;

    [SerializeField] Text titleText;
    public Text errorText;

    public string currentCategory = "TEXAS";


    List<Coroutine> sitNGoTimerCoList = new List<Coroutine>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
        SetGameTypeInScrollView();

        //code to show "TEXAS" only
        //P_SocketController.instance.SendGetRooms();
        //titleText.text = "TEXAS";
    }

    public void OnClickOnButton(string buttonName)
    {
        switch(buttonName)
        {
            case "back":
                P_MainSceneManager.instance.ScreenDestroy();
                GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);

                //P_SocketController.instance.ExitGamePlay();
                P_SocketController.instance.lobbySelectedGameType = "";
                break;

            case "wallet":
                //P_MainSceneManager.instance.DestroyScreen(P_MainScenes.LobbyScene);
                P_MainSceneManager.instance.DestroyCurrentScreen();
                GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);
                if (MainDashboardScreen.instance != null)
                    MainDashboardScreen.instance.MenuSelection(1);
                break;
        }
    }

    void SetGameTypeInScrollView()
    {
        for (int i = 0; i < gameTypeStr.Length; i++)
        {
            int tempI = i;
            string tempName = gameTypeStr[i];
            GameObject go = Instantiate(gameTypePrefab, gameTypeContent);
            go.name = i + "_" + gameTypeStr[i];
            Text txt = go.transform.GetChild(0).GetComponent<Text>();
            txt.text = gameTypeStr[i];
            if (i == 0)
            {
                go.transform.GetChild(1).gameObject.SetActive(true);
                txt.color = selectedGameTypeColor;
            }
            Button btn = go.transform.GetComponent<Button>();
            btn.onClick.AddListener(() => OnGameTypeButtonClick(tempI, tempName));

        }

        P_SocketController.instance.SendGetRooms();
    }

    public void OnGameTypeButtonClick(int clickedI, string tempName)
    {
        for (int i = 0; i < gameTypeContent.childCount; i++)
        {
            gameTypeContent.GetChild(i).GetChild(0).GetComponent<Text>().color = nonSelectedGameTypeColor;
            gameTypeContent.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        gameTypeContent.GetChild(clickedI).GetChild(0).GetComponent<Text>().color = selectedGameTypeColor;
        gameTypeContent.GetChild(clickedI).GetChild(1).gameObject.SetActive(true);

        ClearMainScrollView();

        if (tempName.Equals("TEXAS"))
        {
            currentCategory = "TEXAS";
        }
        else if (tempName.Equals("HOLDEM"))
        {
            currentCategory = "HOLDEM";
        }
        else if (tempName.Equals("PLO"))
        {
            currentCategory = "PLO";
        }
        else if (tempName.Equals("SIT N GO"))
        {
            currentCategory = "SIT N GO";
        }
        else if (tempName.Equals("ANONYMOUS"))
        {
            currentCategory = "ANONYMOUS";
        }
        else if (tempName.Equals("PRACTICE"))
        {
            currentCategory = "PRACTICE";
        }
        else if (tempName.Equals("TOURNAMENT"))
        {
            currentCategory = "TOURNAMENT";
        }
        P_SocketController.instance.SendGetRooms();
        StopSitNGoClockCoList();
    }

    public void CreateLobby1Data(string responseData)
    {
        JsonData data = JsonMapper.ToObject(responseData);
        if (data["data"].Count > 0)
        {
            for (int i = 0; i < mainScrollViewContent.childCount; i++)
            {
                Destroy(mainScrollViewContent.GetChild(i).gameObject);
            }

            int tempCounterForTournamentDesign = 0;

            for (int i = 0; i < data["data"].Count; i++)
            {
                int tempI = i;
                string categoryData = data["data"][i]["game_type"]["name"].ToString();

                //Debug.Log("P_Lobby i: " + i + ", categoryData: " + categoryData + ", currentCategory: " + currentCategory);
                //if (currentCategory.Contains(categoryData) || categoryData.Contains(currentCategory))
                if (categoryData.StartsWith(currentCategory))
                {
                    IDictionary iDataI = data["data"][i] as IDictionary;

                    if (currentCategory.Equals("SIT N GO"))
                    {
                        GameObject sitNGoObj = Instantiate(sitNGoPrefab, mainScrollViewContent);
                        P_Lobby_SitnGo pLobbySitNGo = sitNGoObj.GetComponent<P_Lobby_SitnGo>();

                        if (iDataI.Contains("game_json_data"))
                        {
                            IDictionary iDataIgame = data["data"][i]["game_json_data"] as IDictionary;
                            string smallBlindData = "0", bigBlindData = "0", minimumBuyin = "0";
                            if (iDataIgame.Contains("small_blind"))
                                smallBlindData = data["data"][i]["game_json_data"]["small_blind"].ToString();

                            if (iDataIgame.Contains("big_blind"))
                                bigBlindData = data["data"][i]["game_json_data"]["big_blind"].ToString();

                            pLobbySitNGo.titleText.text = data["data"][i]["game_json_data"]["room_name"].ToString(); //categoryData;



                            if (iDataIgame.Contains("minimum_buyin"))
                                minimumBuyin = data["data"][i]["game_json_data"]["minimum_buyin"].ToString();

                            pLobbySitNGo.bagAmountText.text = minimumBuyin;

                            pLobbySitNGo.trophyAmountText.text = data["data"][i]["game_json_data"]["prize_money"].ToString();
                            //pLobbySitNGo.startsText.text = "Starts when " + data["data"][i]["game_json_data"]["maximum_player"].ToString() + " player joins"; //minimum_player
                            pLobbySitNGo.playersText.text = data["data"][i]["totalPlayers"].ToString() + "/" + data["data"][i]["game_json_data"]["maximum_player"].ToString();
                            pLobbySitNGo.firstAmountText.text = data["data"][i]["table"]["prize_money_first"].ToString();

                            float maxPlayers = 0f, totalPlayers = 0f;
                            if (float.TryParse(data["data"][i]["game_json_data"]["maximum_player"].ToString(), out maxPlayers)) { }
                            if (float.TryParse(data["data"][i]["totalPlayers"].ToString(), out totalPlayers)) { }

                            try
                            {
                                pLobbySitNGo.playerLineImage.fillAmount = (totalPlayers / maxPlayers);
                            }
                            catch (System.Exception e)
                            {
                                // for division error
                                if (P_GameConstant.enableErrorLog)
                                    Debug.Log("Division error in players line image");
                                pLobbySitNGo.playerLineImage.fillAmount = 0f;
                            }


                            bool isGameStarted = bool.Parse(data["data"][i]["table"]["isGameStarted"].ToString());
                            bool isGameEnded = bool.Parse(data["data"][i]["table"]["isGameEnded"].ToString());
                            string registrationStatus = "";
                            bool isMyPlayerFind = false;
                            //if (data["table"]["table_attributes"]["players"].Count == int.Parse(data["table"]["table_attributes"]["maxPlayers"].ToString()))
                            //{
                            for (int j = 0; j < data["data"][i]["table"]["table_attributes"]["players"].Count; j++)
                            {
                                if (data["data"][i]["table"]["table_attributes"]["players"][j]["userId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                                {
                                    isMyPlayerFind = true;
                                }
                            }
                            //}

                            if (isGameStarted && !isGameEnded)
                            {
                                pLobbySitNGo.registerStatusBtn.GetComponent<Image>().sprite = pLobbySitNGo.startedSprite;
                                pLobbySitNGo.registerStatusBtn.transform.GetChild(0).GetComponent<Text>().text = "Started";
                                registrationStatus = "Started";
                            }
                            else if (!isGameStarted && isGameEnded)
                            {
                                pLobbySitNGo.registerStatusBtn.GetComponent<Image>().sprite = pLobbySitNGo.finishedSprite;
                                pLobbySitNGo.registerStatusBtn.transform.GetChild(0).GetComponent<Text>().text = "Finished";
                                registrationStatus = "Finished";
                            }
                            else if (isMyPlayerFind)
                            {
                                // Registered
                                pLobbySitNGo.registerStatusBtn.GetComponent<Image>().sprite = pLobbySitNGo.registeringSprite;
                                pLobbySitNGo.registerStatusBtn.transform.GetChild(0).GetComponent<Text>().text = "Registered";
                                registrationStatus = "Registered";
                            }
                            else
                            {
                                // Not Register
                                pLobbySitNGo.registerStatusBtn.GetComponent<Image>().sprite = pLobbySitNGo.registeringSprite;
                                pLobbySitNGo.registerStatusBtn.transform.GetChild(0).GetComponent<Text>().text = "Registering";
                                registrationStatus = "Registering";
                            }

                            //else if (!isGameStarted && !isGameEnded)
                            //{
                            //    pLobbySitNGo.registerStatusBtn.GetComponent<Image>().sprite = pLobbySitNGo.registeringSprite;
                            //    pLobbySitNGo.registerStatusBtn.transform.GetChild(0).GetComponent<Text>().text = "Registering";
                            //    registrationStatus = "Registering";
                            //}

                            
                            Text sitNGoTimerText = pLobbySitNGo.startsText;
                            //Debug.Log("uptime: " + data["data"][i]["table"]["uptime"]);
                            if (totalPlayers == maxPlayers)
                            {
                                //Debug.Log("uptime if");
                                string upTime = data["data"][i]["table"]["uptime"].ToString();
                                TimeSpan t = TimeSpan.FromSeconds(Double.Parse(upTime));
                                //string answer = string.Format("{0:D1}h:{1:D1}m:{2:D1}s", t.Hours, t.Minutes, t.Seconds); //Debug.Log("answer: " + answer);
                                sitNGoTimerCoList.Add(StartCoroutine(StartSitNGoClock(t, sitNGoTimerText)));
                                //Debug.Log("Sit n Go coroutine added to list");
                            }
                            else
                            {
                                //Debug.Log("uptime else");
                                pLobbySitNGo.startsText.text = "Starts when " + data["data"][i]["game_json_data"]["maximum_player"].ToString() + " player joins"; //minimum_player
                            }    

                            pLobbySitNGo.registerStatusBtn.onClick.AddListener(() =>
                            {
                                //SecondPrefab("SIT N GO", data["data"][tempI]);
                                SecondPrefabSitNGo("SIT N GO", data["data"][tempI], registrationStatus);
                            });
                        }
                    }
                    else if (currentCategory.Equals("TOURNAMENT"))
                    {
                        Debug.Log("TOURNAMENT");
                    }
                    else
                    {
                        GameObject texas1 = Instantiate(texasPrefab, mainScrollViewContent);
                        P_Lobby_Texas pLobbyTexas1 = texas1.GetComponent<P_Lobby_Texas>();

                        if (iDataI.Contains("game_json_data"))
                        {
                            IDictionary iDataIgame = data["data"][i]["game_json_data"] as IDictionary;
                            string smallBlindData = "0", bigBlindData = "0", minimumBuyin = "0";
                            if (iDataIgame.Contains("small_blind"))
                                smallBlindData = data["data"][i]["game_json_data"]["small_blind"].ToString();

                            if (iDataIgame.Contains("big_blind"))
                                bigBlindData = data["data"][i]["game_json_data"]["big_blind"].ToString();

                            pLobbyTexas1.blindsText.text = smallBlindData + "/" + bigBlindData;

                            if (iDataIgame.Contains("minimum_buyin"))
                                minimumBuyin = data["data"][i]["game_json_data"]["minimum_buyin"].ToString();

                            pLobbyTexas1.minBuyInText.text = minimumBuyin;
                        }

                        pLobbyTexas1.playerCountText.text = data["data"][i]["totalPlayers"].ToString();

                        if (currentCategory.Equals("TEXAS"))
                        {
                            pLobbyTexas1.heading.text = "HOLD'EM";
                            pLobbyTexas1.bgButton.onClick.AddListener(() =>
                            {
                                SecondPrefab("TEXAS", data["data"][tempI]);
                            });
                        }
                        else if (currentCategory.Equals("PLO"))
                        {
                            pLobbyTexas1.heading.text = categoryData;
                            pLobbyTexas1.bgButton.onClick.AddListener(() =>
                            {
                                SecondPrefab("PLO", data["data"][tempI]);
                            });
                        }
                        //else if (currentCategory.Equals("SIT N GO"))
                        //{
                        //    pLobbyTexas1.heading.text = categoryData;
                        //    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                        //    {
                        //        SecondPrefab("SIT N GO", data["data"][tempI]);
                        //    });
                        //}
                        else if (currentCategory.Equals("ANONYMOUS"))
                        {
                            pLobbyTexas1.heading.text = categoryData;
                            pLobbyTexas1.bgButton.onClick.AddListener(() =>
                            {
                                SecondPrefab("ANONYMOUS", data["data"][tempI]);
                            });
                        }
                        else if (currentCategory.Equals("PRACTICE"))
                        {
                            pLobbyTexas1.heading.text = categoryData;
                            pLobbyTexas1.bgButton.onClick.AddListener(() =>
                            {
                                SecondPrefab("PRACTICE", data["data"][tempI]);
                            });
                        }
                    }
                }
                else
                {
                    // temporary here for static TOURNAMENT
                    if (currentCategory.Equals("TOURNAMENT"))
                    {
                        if (tempCounterForTournamentDesign == 0)
                        {
                            GameObject tournamentObj = Instantiate(tournamentPrefab, mainScrollViewContent);
                            P_Lobby_Tournament pLobbyTournament = tournamentObj.GetComponent<P_Lobby_Tournament>();

                            pLobbyTournament.registeringImg.sprite = pLobbyTournament.registeringBG;
                            pLobbyTournament.registeringTxt.text = "Registering";

                            pLobbyTournament.titleTxt.text = "Delhi Tournament";
                            pLobbyTournament.dateMonthTxt.text = "11 Aug";
                            pLobbyTournament.winAmountTxt.text = "5K";
                            pLobbyTournament.activePlayersTxt.text = "2";
                            pLobbyTournament.timerTxt.text = "07:00";
                            pLobbyTournament.buyInTxt.text = "20";

                            pLobbyTournament.registeringBtn.onClick.AddListener(() =>
                            {
                                SecondPrefabTournament(data["data"][tempI], "Registering");
                            });
                        }
                        else if (tempCounterForTournamentDesign == 1)
                        {
                            GameObject tournamentObj = Instantiate(tournamentPrefab, mainScrollViewContent);
                            P_Lobby_Tournament pLobbyTournament = tournamentObj.GetComponent<P_Lobby_Tournament>();

                            pLobbyTournament.registeringImg.sprite = pLobbyTournament.startedNLateBG;
                            pLobbyTournament.registeringTxt.text = "Late Reg";

                            pLobbyTournament.titleTxt.text = "Hyper 1500 GTD (RE)";
                            pLobbyTournament.dateMonthTxt.text = "10 Aug";
                            pLobbyTournament.winAmountTxt.text = "2K";
                            pLobbyTournament.activePlayersTxt.text = "4";
                            pLobbyTournament.timerTxt.text = "05:00";
                            pLobbyTournament.buyInTxt.text = "100";

                            pLobbyTournament.registeringBtn.onClick.AddListener(() =>
                            {
                                SecondPrefabTournament(data["data"][tempI], "Late Reg");
                            });
                        }
                        else if (tempCounterForTournamentDesign == 2)
                        {
                            GameObject tournamentObj = Instantiate(tournamentPrefab, mainScrollViewContent);
                            P_Lobby_Tournament pLobbyTournament = tournamentObj.GetComponent<P_Lobby_Tournament>();

                            pLobbyTournament.registeringImg.sprite = pLobbyTournament.finishedBG;
                            pLobbyTournament.registeringTxt.text = "Finished";

                            pLobbyTournament.titleTxt.text = "After Dark Hyper 1500 GTD (RE)";
                            pLobbyTournament.dateMonthTxt.text = "8 Aug";
                            pLobbyTournament.winAmountTxt.text = "5K";
                            pLobbyTournament.activePlayersTxt.text = "83";
                            pLobbyTournament.timerTxt.text = "03:00";
                            pLobbyTournament.buyInTxt.text = "50";

                            pLobbyTournament.registeringBtn.onClick.AddListener(() =>
                            {
                                SecondPrefabTournament(data["data"][tempI], "Finished");
                            });
                        }
                        tempCounterForTournamentDesign++;
                    }
                }
            }
        }
        else
        {
            errorText.text = "Data not found from server";
            errorText.gameObject.SetActive(true);
        }
    }

    IEnumerator StartSitNGoClock(TimeSpan t, Text sitNGoTimerText)
    {
        while (t.TotalSeconds > 0)
        {
            t = t.Add(TimeSpan.FromSeconds(1));
            //Debug.Log(t.ToString());
            if (sitNGoTimerText != null)
            {
                if (t.Hours > 0)
                {
                    sitNGoTimerText.text = "Running Since " + t.Hours + "h:" + t.Minutes + "m:" + t.Seconds + "s";
                }
                else
                {
                    if (t.Minutes > 0)
                        sitNGoTimerText.text = "Running Since " + t.Minutes + "m:" + t.Seconds + "s";
                    else
                        sitNGoTimerText.text = "Running Since " + t.Seconds + "s";
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    void StopSitNGoClockCoList()
    {
        for (int i = 0; i < sitNGoTimerCoList.Count; i++)
        {
            if (sitNGoTimerCoList[i] != null)
            {
                StopCoroutine(sitNGoTimerCoList[i]);
            }
        }
        sitNGoTimerCoList.Clear();
    }

    // old method: when used API
    public void GetLobby1Data(string selectedCategoryStr)
    {
        if (!P_WebServices.instance.IsInternetAvailable())
        {
            errorText.text = "No Internet";
            errorText.gameObject.SetActive(true);
        }
        else
        {
            errorText.text = "";
            errorText.gameObject.SetActive(false);

            P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.Loading);
            StartCoroutine(P_WebServices.instance.GETRequestData(P_RequestType.PokerGameList, (string downloadText, bool isError, string errorString) =>
            {
                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.Loading);
                if (isError)
                {
                    if (P_GameConstant.enableErrorLog)
                        Debug.Log("Get game list error: " + errorString);

                    errorText.text = "Error from server";
                    errorText.gameObject.SetActive(true);
                }
                else
                {
                    if (P_GameConstant.enableLog)
                        Debug.Log("Get game list: " + downloadText);

                    JsonData data = JsonMapper.ToObject(downloadText);
                    if (data["data"].Count > 0)
                    {
                        for (int i = 0; i < data["data"].Count; i++)
                        {
                            int tempI = i;
                            string categoryData = data["data"][i]["game_type"]["name"].ToString();

                            if (selectedCategoryStr.Contains(categoryData) || categoryData.Contains(selectedCategoryStr))
                            {
                                GameObject texas1 = Instantiate(texasPrefab, mainScrollViewContent);
                                P_Lobby_Texas pLobbyTexas1 = texas1.GetComponent<P_Lobby_Texas>();

                                IDictionary iDataI = data["data"][i] as IDictionary;
                                if (iDataI.Contains("game_json_data"))
                                {
                                    IDictionary iDataIgame = data["data"][i]["game_json_data"] as IDictionary;
                                    string smallBlindData = "0", bigBlindData = "0", minimumBuyin = "0";
                                    if (iDataIgame.Contains("small_blind"))
                                        smallBlindData = data["data"][i]["game_json_data"]["small_blind"].ToString();

                                    if (iDataIgame.Contains("big_blind"))
                                        bigBlindData = data["data"][i]["game_json_data"]["big_blind"].ToString();

                                    pLobbyTexas1.blindsText.text = smallBlindData + "/" + bigBlindData;

                                    if (iDataIgame.Contains("minimum_buyin"))
                                        minimumBuyin = data["data"][i]["game_json_data"]["minimum_buyin"].ToString();

                                    pLobbyTexas1.minBuyInText.text = minimumBuyin;
                                }

                                pLobbyTexas1.playerCountText.text = data["data"][i]["totalPlayers"].ToString();

                                if (selectedCategoryStr.Equals("TEXAS"))
                                {
                                    pLobbyTexas1.heading.text = "HOLD'EM";
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("TEXAS", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("PLO"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("PLO", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("SIT N GO"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        //SecondPrefab("SIT N GO", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("ANONYMOUS"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("ANONYMOUS", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("PRACTICE"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("PRACTICE", data["data"][tempI]);
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        errorText.text = "Data not found from server";
                        errorText.gameObject.SetActive(true);
                    }
                }
            }));
        }
    }

    void SecondPrefab(string gameType, JsonData dataOfI)
    {
        if (P_GameConstant.enableLog)
            Debug.Log(JsonMapper.ToJson(dataOfI));

        P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbySecond);

        if (P_Lobby_Second.instance != null)
        {
            P_Lobby_Second.instance.OnLoadScrollDetails(gameType, dataOfI);
        }

        P_SocketController.instance.lobbySelectedGameType = gameType;
    }

    void SecondPrefabSitNGo(string gameType, JsonData dataOfI, string registrationStatus)
    {
        if (P_GameConstant.enableLog)
            Debug.Log(JsonMapper.ToJson(dataOfI));

        //bool isGameStart = false;
        //if (dataOfI["table"]["table_attributes"]["players"].Count == int.Parse(dataOfI["table"]["table_attributes"]["maxPlayers"].ToString()))
        //{
        //    for (int i = 0; i < dataOfI["table"]["table_attributes"]["players"].Count; i++)
        //    {
        //        if (dataOfI["table"]["table_attributes"]["players"][i]["userId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
        //        {
        //            // start gameplay
        //            P_SocketController.instance.TABLE_ID = dataOfI["table"]["tableId"].ToString();
        //            P_SocketController.instance.SendJoin(P_SocketController.instance.TABLE_ID, dataOfI["game_json_data"]["minimum_buyin"].ToString());
        //            isGameStart = true;



        //            P_MainSceneManager.instance.ScreenDestroy();
        //            P_MainSceneManager.instance.LoadScene(P_MainScenes.InGame);

        //            P_SocketController.instance.gameId = dataOfI["game_id"].ToString();
        //            P_SocketController.instance.SendJoinViewer();

        //            //P_SocketController.instance.tableData = roomData;
        //            P_SocketController.instance.gameTableData = dataOfI;
        //            P_SocketController.instance.gameTypeName = dataOfI["game_type"]["name"].ToString();
        //            if (P_InGameManager.instance != null)
        //            {
        //                if (P_SocketController.instance.gameTypeName == "SIT N GO") //for SIT N GO rule: game start ho to join nahi karwana
        //                {
        //                    if (dataOfI["table_attributes"]["players"].Count < int.Parse(dataOfI["table_attributes"]["maxPlayers"].ToString()))
        //                    {
        //                        //SIT N GO Table have empty seat
        //                        P_InGameUiManager.instance.AllPlayerPosPlusOn();
        //                    }
        //                    else
        //                    {
        //                        //SIT N GO Table is full
        //                        P_InGameUiManager.instance.AllPlayerPosPlusOff(true);
        //                    }
        //                }
        //                else
        //                {
        //                    P_InGameUiManager.instance.AllPlayerPosPlusOn();
        //                }
        //            }
        //            if (P_InGameUiManager.instance != null)
        //                P_InGameUiManager.instance.tableInfoText.text = dataOfI["table_name"].ToString();
        //            if (P_GameConstant.enableLog)
        //                Debug.Log("Get game table click: " + JsonMapper.ToJson(dataOfI));

        //            P_SocketController.instance.gameTableMaxPlayers = Int32.Parse(P_SocketController.instance.gameTableData["maxPlayers"].ToString());
        //        }
        //    }
        //}

        //if (!isGameStart)
        //{
            P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbySecondSitNGo);

            if (P_SitNGoDetails.instance != null)
            {
                P_SitNGoDetails.instance.OnLoadScrollDetails(dataOfI, registrationStatus);
            }
        //}

        P_SocketController.instance.lobbySelectedGameType = gameType;
    }

    void SecondPrefabTournament(JsonData dataOfI, string registrationStatus)
    {
        string gameType = "TOURNAMENT";
        Debug.Log("registrationStatus: " + registrationStatus);
        P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbyTournaments);
    }

    void ClearMainScrollView()
    {
        for (int i = 0; i < mainScrollViewContent.childCount; i++)
        {
            Destroy(mainScrollViewContent.GetChild(i).gameObject);
        }
    }
}
