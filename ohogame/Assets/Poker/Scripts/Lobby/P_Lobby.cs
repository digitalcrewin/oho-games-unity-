using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

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

    public Text errorText;

    public string currentCategory = "TEXAS";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetGameTypeInScrollView();
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

            for (int i = 0; i < data["data"].Count; i++)
            {
                int tempI = i;
                string categoryData = data["data"][i]["game_type"]["name"].ToString();

                if (currentCategory.Contains(categoryData) || categoryData.Contains(currentCategory))
                {
                    IDictionary iDataI = data["data"][i] as IDictionary;

                    if (currentCategory.Equals("SIT N GO"))
                    {
                        GameObject sitNGoObj = Instantiate(sitNGoPrefab, mainScrollViewContent);
                        P_Lobby_SitnGo pLobbySitNGo = sitNGoObj.GetComponent<P_Lobby_SitnGo>();

                        if (iDataI.Contains("game_json_data"))
                        {
                            if (iDataI.Contains("game_json_data"))
                            {
                                IDictionary iDataIgame = data["data"][i]["game_json_data"] as IDictionary;
                                string smallBlindData = "0", bigBlindData = "0", minimumBuyin = "0";
                                if (iDataIgame.Contains("small_blind"))
                                    smallBlindData = data["data"][i]["game_json_data"]["small_blind"].ToString();

                                if (iDataIgame.Contains("big_blind"))
                                    bigBlindData = data["data"][i]["game_json_data"]["big_blind"].ToString();

                                pLobbySitNGo.titleText.text = data["data"][i]["game_json_data"]["room_name"].ToString(); //categoryData;

                                pLobbySitNGo.registerStatusBtn.onClick.AddListener(() =>
                                {
                                    //SecondPrefab("SIT N GO", data["data"][tempI]);
                                    SecondPrefabSitNGo("SIT N GO", data["data"][tempI]);
                                });

                                if (iDataIgame.Contains("minimum_buyin"))
                                    minimumBuyin = data["data"][i]["game_json_data"]["minimum_buyin"].ToString();

                                pLobbySitNGo.bagAmountText.text = minimumBuyin;

                                pLobbySitNGo.trophyAmountText.text = data["data"][i]["game_json_data"]["prize_money"].ToString();
                                pLobbySitNGo.startsText.text = "Starts when " + data["data"][i]["game_json_data"]["minimum_player"].ToString() + " player joins";
                                pLobbySitNGo.playersText.text = data["data"][i]["totalPlayers"].ToString() + "/" + data["data"][i]["game_json_data"]["maximum_player"].ToString();

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
                                    Debug.Log("Division error in players line image");
                                    pLobbySitNGo.playerLineImage.fillAmount = 0f;
                                }
                            }
                        }
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
            }
        }
        else
        {
            errorText.text = "Data not found from server";
            errorText.gameObject.SetActive(true);
        }
    }

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
                                        SecondPrefabSitNGo("SIT N GO", data["data"][tempI]);
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

    void SecondPrefabSitNGo(string gameType, JsonData dataOfI)
    {
        if (P_GameConstant.enableLog)
            Debug.Log(JsonMapper.ToJson(dataOfI));

        P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbySecondSitNGo);

        if (P_SitNGoDetails.instance != null)
        {
            P_SitNGoDetails.instance.OnLoadScrollDetails(dataOfI);
        }

        P_SocketController.instance.lobbySelectedGameType = gameType;
    }

    void ClearMainScrollView()
    {
        for (int i = 0; i < mainScrollViewContent.childCount; i++)
        {
            Destroy(mainScrollViewContent.GetChild(i).gameObject);
        }
    }
}
