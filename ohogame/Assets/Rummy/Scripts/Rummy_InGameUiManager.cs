using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;

public class Rummy_InGameUiManager : MonoBehaviour
{
    public static Rummy_InGameUiManager instance;

    [SerializeField]
    private GameObject[] screens; // All screens prefab

    [SerializeField]
    private Transform[] screenLayers; // screen spawn parent


    [SerializeField]
    private Text tableText, tableInfoText;

    public Text resultTimerText;

    [SerializeField]
    private GameObject chatButton;
    [SerializeField]
    public GameObject LoadingImage;

    private Rummy_SuggestionActions selectedSuggestionButton = Rummy_SuggestionActions.Null;


    private float availableCallAmount = 0, selectedRaiseAmount = 0;
    private List<Rummy_InGameActiveScreens> Rummy_InGameActiveScreens = new List<Rummy_InGameActiveScreens>();
    private bool useRaisePotWise = false;
    private int suggestionCallAmount = 0;

    public GameObject players;
    public string tableId;

    public Camera cameraObj;
    [HideInInspector]
    public float height, width;
    public GameObject inGamePopUp;

    public List<GameObject> TableImages = new List<GameObject>();

    public Animator actionPanelAnimator;
    public Button exitButton;

    // game info
    public Text minChipsInfoOnTop;

    // game info menu buttons
    public GameObject discardBtn;
    public GameObject lastHandBtn;
    public GameObject scoreBoardBtn;

    // game info menu button sprites
    public Sprite selectedBtnSprite;
    public Sprite unSelectedBtnSprite;
    public Color unSelectedBtnTxtColor;

    // discard history
    public Transform discHistUserBtnContent;
    public Transform discHistCardContent;
    public GameObject discHistUserBtnObj;
    public GameObject discHistCardObj;

    // last hand game history
    public Transform lastHandScrollContent;
    public GameObject gameHistoryRow;
    public GameObject gameHistoryRowBlank;
    public GameObject gameHistoryGroup;
    public GameObject gameHistoryCard;
    public Button resultLeaveTableBtn;

    // last score board history
    public Transform scoreBoardContent;
    public GameObject scoreBoardRow;

    // chat
    public Transform chatContent;
    public GameObject chatItem;
    public InputField chatInput;

    //settings
    public Toggle soundToggle;
    public Toggle vibrationToggle;

    private void Awake()
    {
        instance = this;

        //cameraObj = GameObject.Find("VideoRecordingCamera").GetComponent<Camera>();

        // Canvas canvas = gameObject.GetComponent<Canvas>();
        // canvas.renderMode = RenderMode.ScreenSpaceCamera;
        // canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        if (Screen.orientation != ScreenOrientation.LandscapeLeft)
            Screen.orientation = ScreenOrientation.LandscapeLeft;

        // CanvasScaler canvasScaler = gameObject.GetComponent<CanvasScaler>();
        // canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        // canvasScaler.matchWidthOrHeight = 0f;

        height = gameObject.GetComponent<RectTransform>().rect.height;
        width = gameObject.GetComponent<RectTransform>().rect.width;

        // Debug.Log("inGameUiManager height=" + height +", width="+ width);
        // Debug.Log("inGameUiManager screen height=" + Screen.height +", screen width="+ Screen.width);
        // Screen.SetResolution(Screen.width, Screen.height, true);

        // if (GlobalGameManager.instance!=null && GlobalGameManager.instance.GetRoomData().isLobbyRoom)
        // {
        //     clubTableOject.SetActive(false);
        //     lobbyTableObject.SetActive(true);
        // }
        // else
        // {
        //     //clubTableOject.SetActive(true);
        //     //lobbyTableObject.SetActive(false);
        // }

        // ToggleActionButton(false);
        // ToggleSuggestionButton(false);

        players.transform.GetChild(0).GetComponent<R_PlayerScript>().nameText.text = R_PlayerManager.instance.GetPlayerGameData().userId + "_" + R_PlayerManager.instance.GetPlayerGameData().userName; // PlayerManager.instance.
        players.transform.GetChild(0).GetComponent<R_PlayerScript>().balanceText.text = R_PlayerManager.instance.GetPlayerGameData().coins.ToString();

        // for static check
        // SetGameHistory("[\"GAME_HISTORY\",["+
        //     "[{\"playerId\":\"500\",\"score\":11,\"cards\":[[\"2D\",\"3D\",\"4D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"playerId\":\"501\",\"score\":0,\"cards\":[[\"6D\",\"7D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"playerId\":\"502\",\"score\":32,\"cards\":[[\"7D\",\"8D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"winnerId\":\"501\"}],"+
        //     "[{\"playerId\":\"501\",\"score\":0,\"cards\":[[\"6D\",\"7D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"playerId\":\"502\",\"score\":32,\"cards\":[[\"7D\",\"8D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"winnerId\":\"501\"}],"+
        //     "[{\"playerId\":\"501\",\"score\":0,\"cards\":[[\"6D\",\"7D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"playerId\":\"502\",\"score\":32,\"cards\":[[\"7D\",\"8D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"winnerId\":\"501\"}],"+
        //     "[{\"playerId\":\"501\",\"score\":0,\"cards\":[[\"6D\",\"7D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"playerId\":\"502\",\"score\":32,\"cards\":[[\"7D\",\"8D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"winnerId\":\"501\"}]]]");

        // SetScoreBoard("[\"SCORE_BOARD\",[{\"score\":42,\"result\":\"LOST\",\"chips\":\"-10\",\"gameId\":1665667051318},{\"score\":0,\"result\":\"WON\",\"chips\":\"+470\",\"gameId\":1665667051318},{\"score\":80,\"result\":\"LOST\",\"chips\":\"-480\",\"gameId\":1665667051318},{\"score\":80,\"result\":\"LOST\",\"chips\":\"-480\",\"gameId\":1665667051318}]]");

        string soundsCheck = PlayerPrefs.GetString("issound");
        Debug.Log("issound="+soundsCheck);
        if (soundsCheck == "1")
        {
            soundToggle.isOn = true;
        }
        else if (soundsCheck == "0")
        {
            soundToggle.isOn = false;
        }

        if (R_SocketController.instance != null)
        {
            if (R_SocketController.instance.selectedRow != null)
            {
                //minChipsInfoOnTop.text = R_SocketController.instance.selectedRow["minChips"].ToString();
            }
        }
    }

    public void ShowScreen(Rummy_InGameScreens screenName, object[] parameter = null)
    {
        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((ScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            Rummy_InGameActiveScreens mainMenuScreen = new Rummy_InGameActiveScreens();
            mainMenuScreen.screenName = screenName;
            mainMenuScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;
            Rummy_InGameActiveScreens.Add(mainMenuScreen);


            switch (screenName)
            {
                case Rummy_InGameScreens.GameSettings:
                    {
                        //gm.GetComponent<TopUpScript>().Init((float)parameter[0]);
                    }
                    break;
                default:
                    break;
            }

        }
    }

    public void DestroyScreen(Rummy_InGameScreens screenName)
    {
        for (int i = 0; i < Rummy_InGameActiveScreens.Count; i++)
        {
            if (Rummy_InGameActiveScreens[i].screenName == screenName)
            {
                Destroy(Rummy_InGameActiveScreens[i].screenObject);
                Rummy_InGameActiveScreens.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(ScreenLayer layerName)
    {
        for (int i = 0; i < Rummy_InGameActiveScreens.Count; i++)
        {
            if (Rummy_InGameActiveScreens[i].screenLayer == layerName)
            {
                Destroy(Rummy_InGameActiveScreens[i].screenObject);
                Rummy_InGameActiveScreens.RemoveAt(i);
            }
        }
    }

    private bool IsScreenActive(Rummy_InGameScreens screenName)
    {
        for (int i = 0; i < Rummy_InGameActiveScreens.Count; i++)
        {
            if (Rummy_InGameActiveScreens[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }

    private ScreenLayer GetScreenLayer(Rummy_InGameScreens screenName)
    {
        /*switch (screenName)
        {
            case Rummy_InGameScreens.Message:
                return ScreenLayer.LAYER2;

            case Rummy_InGameScreens.Reconnecting:
                return ScreenLayer.LAYER3;

            case Rummy_InGameScreens.Loading:
                return ScreenLayer.LAYER4;

            default:
                return ScreenLayer.LAYER1;
        }*/
        return ScreenLayer.LAYER1;
    }

    public void OnClickOnButton(string eventName)
    {
        // Debug.Log("Event Name => " + eventName);

        // SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "tableInfoBtn":
                R_SocketController.instance.SendDiscardedHistory();
                break;

            case "discardInfoBtn":
                discardBtn.GetComponent<Image>().sprite = selectedBtnSprite;
                lastHandBtn.GetComponent<Image>().sprite = unSelectedBtnSprite;
                scoreBoardBtn.GetComponent<Image>().sprite = unSelectedBtnSprite;
                discardBtn.GetComponentInChildren<Text>().color = new Color(0f, 0.007843138f, 0.227451f);
                lastHandBtn.GetComponentInChildren<Text>().color = unSelectedBtnTxtColor;
                scoreBoardBtn.GetComponentInChildren<Text>().color = unSelectedBtnTxtColor;
                R_SocketController.instance.SendDiscardedHistory();
                break;

            case "lastHandInfoBtn":
                lastHandBtn.GetComponent<Image>().sprite = selectedBtnSprite;
                discardBtn.GetComponent<Image>().sprite = unSelectedBtnSprite;
                scoreBoardBtn.GetComponent<Image>().sprite = unSelectedBtnSprite;
                lastHandBtn.GetComponentInChildren<Text>().color = new Color(0f, 0.007843138f, 0.227451f);
                discardBtn.GetComponentInChildren<Text>().color = unSelectedBtnTxtColor;
                scoreBoardBtn.GetComponentInChildren<Text>().color = unSelectedBtnTxtColor;
                R_SocketController.instance.SendGameHistory();
                break;

            case "scoreBoardInfoBtn":
                scoreBoardBtn.GetComponent<Image>().sprite = selectedBtnSprite;
                discardBtn.GetComponent<Image>().sprite = unSelectedBtnSprite;
                lastHandBtn.GetComponent<Image>().sprite = unSelectedBtnSprite;
                scoreBoardBtn.GetComponentInChildren<Text>().color = new Color(0f, 0.007843138f, 0.227451f);
                discardBtn.GetComponentInChildren<Text>().color = unSelectedBtnTxtColor;
                lastHandBtn.GetComponentInChildren<Text>().color = unSelectedBtnTxtColor;
                R_SocketController.instance.SendScoreBoard();
                break;

            case "chatPanelOnBtn":

                break;

            case "chatSendBtn":
                if (!string.IsNullOrEmpty(chatInput.text))
                {
                    R_SocketController.instance.SendChatMessage(chatInput.text);
                    GameObject NewChatItem = Instantiate(chatItem, chatContent);
                    NewChatItem.GetComponent<Text>().text = "<b>You:</b> " + chatInput.text;
                    chatInput.text = "";
                }
                break;

            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in Rummy_InGameUiManager = " + eventName);
#endif
                break;
        }
    }

    public void OnClickLeaveTable()
    {
        resultTimerText.text = "Game exit in few seconds";
        //if (!Rummy_InGameManager.instance.lastGameToggle.GetComponent<Toggle>().isOn)
        {
            R_SocketController.instance.SendExitGame();
        }
        resultLeaveTableBtn.interactable = false;
        /*StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(5f, () => {
            OnClickOnBack();
        }));*/
    }

    public void OnClickOnBack()
    {
        Debug.Log(Rummy_InGameManager.instance.resultAfterWait + " - " + Rummy_InGameManager.instance.isSeatingReceived);
        if (Rummy_InGameManager.instance.resultAfterWait != null)
            Rummy_InGameManager.instance.resultAfterWait.Stop();

        if (!Rummy_InGameManager.instance.isSeatingReceived)
            R_SocketController.instance.SendExitGame();
        else
            R_SocketController.instance.SendLeaveTable();
    }

    public void ResetSocketAndLoadMenu()
    {
        if (R_SocketController.instance != null)
        {
            R_SocketController.instance.ResetConnection();
            R_SocketController.instance.rummyUserId = string.Empty;
            R_SocketController.instance.rummyGameType = string.Empty;
            R_SocketController.instance.discardedCardData = string.Empty;
            R_SocketController.instance = null;
            R_GlobalGameManager.instance.mainSocketController.GetComponent<R_SocketController>().enabled = false;
        }
        // R_GlobalGameManager.instance.mainSocketController.SetActive(false);
        R_GlobalGameManager.instance.IsJoiningPreviousGame = false;
        Rummy_InGameManager.instance.isSeatingReceived = false;

        R_GlobalGameManager.instance.DestroyScene(R_Scenes.InGame);
        R_GlobalGameManager.instance.LoadScene(R_Scenes.MainMenuScene);
        // Rummy_InGameManager.instance.LoadMainMenu();
    }

    public void SetDiscardUsers()
    {
        for (int j = 0; j < discHistUserBtnContent.childCount; j++)
        {
            Destroy(discHistUserBtnContent.GetChild(j).gameObject);
        }
        for (int i = 0; i < Rummy_InGameManager.instance.allPlayersScript.Length; i++)
        {
            if (!string.IsNullOrEmpty(Rummy_InGameManager.instance.allPlayersScript[i].playerData.userId))
            {
                GameObject user = Instantiate(discHistUserBtnObj, discHistUserBtnContent);
                string userId = Rummy_InGameManager.instance.allPlayersScript[i].playerData.userId;
                string userName = Rummy_InGameManager.instance.allPlayersScript[i].playerData.userName;
                user.GetComponent<Button>().onClick.AddListener(() => {R_SocketController.instance.SendDiscardedHistory(userId);});
                // if (userId.Length > 5){ userId = userId.Substring(0,5) + "..."; }
                if (userName.Length > 5){ userName = userName.Substring(0,5) + "..."; }
                user.transform.GetChild(0).GetComponent<Text>().text = userName;
            }
        }
    }

    public void SetDiscardedHistory(string socketPacket)
    {
        for (int j = 0; j < discHistCardContent.childCount; j++)
        {
            Destroy(discHistCardContent.GetChild(j).gameObject);
        }

        //["DISCARDED_HISTORY",["6D","10S","KH","4D"]]
        JsonData data = JsonMapper.ToObject(socketPacket);
        int dataCount = data[1].Count;
        if (dataCount > 0)
        {
            for (int i = 0; i < dataCount; i++)
            {
                GameObject card = Instantiate(discHistCardObj, discHistCardContent);
                card.GetComponent<Image>().sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(data[1][i].ToString());
            }
        }
    }

    public void SetGameHistory(string socketPacket)
    {
        for (int g = 0; g < lastHandScrollContent.childCount; g++)
        {
            Destroy(lastHandScrollContent.GetChild(g).gameObject);
        }

        //[[{"playerId":"501","score":0,"cards":[["6D","7D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"playerId":"502","score":32,"cards":[["7D","8D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"winnerId":"501"}],[{"playerId":"501","score":0,"cards":[["6D","7D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"playerId":"502","score":32,"cards":[["7D","8D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"winnerId":"501"}],[{"playerId":"501","score":0,"cards":[["6D","7D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"playerId":"502","score":32,"cards":[["7D","8D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"winnerId":"501"}]]

        ////["GAME_HISTORY",[[{"playerId":"472","score":300,"status":"DROP","cards":[["2S","4S","6S","7S","9S","10S"],["AH","8H"],["3D","10D"],["3C","9C","JC"]],"playerName":"v2"},{"playerId":"513","playerName":"v3.ab","status":"WINNER","score":0,"cards":[["7S","JS"],["5H","8H","10H"],["AD","5D","6D"],["8D","4S","AC","3C","8C","9C"]]},{"winnerId":"513"}]]]
	
	    // ["GAME_HISTORY",[[{"playerId":"472","score":300,"status":"DROP","cards":[["2S","4S","6S","7S","9S","10S"],["AH","8H"],["3D","10D"],["3C","9C","JC"]],"playerName":"v2"},{"playerId":"513","playerName":"v3.ab","status":"WINNER","score":0,"cards":[["7S","JS"],["5H","8H","10H"],["AD","5D","6D"],["8D","4S","AC","3C","8C","9C"]]},{"winnerId":"513"}],[{"playerId":"513","score":80,"status":"WRONG SHOW","cards":[["JJ","10C","QC","KC"],["AS"],["5D","6D","8D","9D"],["AD","2D","3S","4C"]],"playerName":"v3.ab"},{"playerId":"472","playerName":"v2","status":"WINNER","score":0,"cards":[["7H","7S","7C"],["3S"],["2D","JD","QD"],["QH","AC","2C","4C","JC","QC"]]},{"winnerId":"472"}]]]

        JsonData data = JsonMapper.ToObject(socketPacket);
        int outerDataCount = data[1].Count;
        for (int k = 0; k < outerDataCount; k++)
        {
            int innerDataCount = data[1][k].Count;
            Debug.Log("rowDataCount="+innerDataCount);
            for (int i = 0; i < innerDataCount; i++)
            {
                IDictionary iResult = data[1][k][i] as IDictionary;
                if (iResult.Contains("playerId"))
                {
                    // Debug.Log("playerId="+data[1][k][i]["playerId"]);
                    GameObject resultRow = Instantiate(gameHistoryRow, lastHandScrollContent);
                    string playerId = data[1][k][i]["playerId"].ToString();
                    resultRow.name = playerId;
                    // if (playerId == PlayerManager.instance.GetPlayerGameData().userId)
                    // {
                    //     string forTrimUsername = PlayerManager.instance.GetPlayerGameData().userName;
                    //     if (forTrimUsername.Length > 8){ forTrimUsername = forTrimUsername.Substring(0,8) + "..."; }
                    //     resultRow.transform.GetChild(0).GetComponent<Text>().text = forTrimUsername;
                    // }
                    // else
                    // {
                    //     resultRow.transform.GetChild(0).GetComponent<Text>().text = playerId; //"Player " + i;
                    // }
                    string forTrimUsername = data[1][k][i]["playerName"].ToString();
                    if (forTrimUsername.Length > 8){ forTrimUsername = forTrimUsername.Substring(0,8) + "..."; }
                    resultRow.transform.GetChild(0).GetComponent<Text>().text = forTrimUsername;

                    try
                    {
                        // if (string.Equals(playerId, data[1][k][innerDataCount-1]["winnerId"].ToString()))
                        // {
                        //     resultRow.transform.GetChild(1).GetComponent<Text>().text = "Winner";
                        // }
                        // else
                        // {
                        //     resultRow.transform.GetChild(1).GetComponent<Text>().text = "Lost";
                        // }
                        resultRow.transform.GetChild(1).GetComponent<Text>().text = data[1][k][i]["status"].ToString();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("winnerId exception="+e.Message);
                    }

                    for (int j = 0; j < data[1][k][i]["cards"].Count; j++)
                    {
                        // Debug.Log("card J: "+data[1][k][i]["cards"][j]);
                        // Debug.Log("card J: "+data[1][k][i]["cards"][j].Count);
                         group = GameObject.Instantiate(gameHistoryGroup, resultRow.transform.GetChild(2));
                        for (int l = 0; l < data[1][k][i]["cards"][j].Count; l++)
                        {
                            // Debug.Log("card J: "+data[1][k][i]["cards"][j][l]);
                            GameObject card = GameObject.Instantiate(gameHistoryCard, group.transform);
                            card.GetComponent<Image>().sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(data[1][k][i]["cards"][j][l].ToString());
                        }
                    }
                    resultRow.transform.GetChild(3).GetComponent<Text>().text = data[1][k][i]["score"].ToString();
                }
            }
            GameObject resultRowBlank = Instantiate(gameHistoryRowBlank, lastHandScrollContent);
        } 
        // LayoutRebuilder.ForceRebuildLayoutImmediate(lastHandScrollContent.GetComponent<RectTransform>());
        // Canvas.ForceUpdateCanvases();
    }

    public void SetScoreBoard(string socketPacket)
    {
        for (int g = 0; g < scoreBoardContent.childCount; g++)
        {
            Destroy(scoreBoardContent.GetChild(g).gameObject);
        }
        JsonData data = JsonMapper.ToObject(socketPacket);
        int outerDataCount = data[1].Count;
        for (int k = 0; k < outerDataCount; k++)
        {
            GameObject scoreBoardObj = Instantiate(scoreBoardRow, scoreBoardContent);
            int srNo = k + 1;
            scoreBoardObj.transform.GetChild(0).GetComponent<Text>().text = srNo.ToString();
            scoreBoardObj.transform.GetChild(1).GetComponent<Text>().text = data[1][k]["gameId"].ToString();
            scoreBoardObj.transform.GetChild(2).GetComponent<Text>().text = data[1][k]["result"].ToString();
            scoreBoardObj.transform.GetChild(3).GetComponent<Text>().text = data[1][k]["score"].ToString();
            scoreBoardObj.transform.GetChild(4).GetComponent<Text>().text = data[1][k]["chips"].ToString();
        }
    }

    public void SetChat(string socketPacket)
    {
        //["CHAT_MESSAGE", {"playerId": 502,"message": "hello"}]
        JsonData data = JsonMapper.ToObject(socketPacket);
        string playerId = data[1]["playerId"].ToString();
        string msg = data[1]["message"].ToString();
        GameObject NewChatItem = Instantiate(chatItem, chatContent);
        // NewChatItem.GetComponent<Text>().text = msg; //"<b>" + playerId + ":</b> " + 
        
        string trimmedMsg = (msg.Length > 20) ? msg.Substring(0,20) + "..." : msg;
        int countScript = Rummy_InGameManager.instance.allPlayersScript.Length;
        for (int i = 0; i < countScript; i++)
        {
            if (playerId.Equals(Rummy_InGameManager.instance.allPlayersScript[i].playerData.userId))
            {
                string userName = Rummy_InGameManager.instance.allPlayersScript[i].playerData.userName;
                if (userName.Length > 5){ userName = userName.Substring(0,5) + "..."; }
                NewChatItem.GetComponent<Text>().text = "<b>" + userName + ":</b> " + msg;
            }
            if ( (!playerId.Equals(R_PlayerManager.instance.GetPlayerGameData().userId)) &&
                (Rummy_InGameManager.instance.allPlayersScript[i].playerData.userId == playerId)
            )
            {
                Rummy_InGameManager.instance.allPlayersScript[i].chatText.gameObject.SetActive(true);
                Rummy_InGameManager.instance.allPlayersScript[i].chatText.transform.parent.gameObject.SetActive(true);
                Rummy_InGameManager.instance.allPlayersScript[i].chatText.text = trimmedMsg;
                 StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(2f, () => {
                                Rummy_InGameManager.instance.allPlayersScript[i].chatText.transform.parent.gameObject.SetActive(false);
                                Rummy_InGameManager.instance.allPlayersScript[i].chatText.gameObject.SetActive(false);
                                Rummy_InGameManager.instance.allPlayersScript[i].chatText.text = string.Empty;
                            }));
                break;
            }
        }
    }

// IEnumerator RefreshLayout(GameObject layout)
//      {
//          /*HorizontalLayoutGroup vlg = layout.GetComponent<HorizontalLayoutGroup>();
//          vlg.enabled = false;
//          vlg.enabled = true;*/
//          lastHandScrollContent.gameObject.SetActive(false);
//       Debug.Log("%%%%%%%%%%%%%%555555555");
//          yield return new WaitForFixedUpdate();
//          lastHandScrollContent.gameObject.SetActive(true);
//          lastHandScrollContent.gameObject.SetActive(false);
      
//          lastHandScrollContent.gameObject.SetActive(true);
//      }
        GameObject group ;
    public void OnServerResponseFound(R_RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
               //ShowMessage(errorMessage);
            }

            return;
        }
    }

    public void OnSoundToggleChanged()
    {
        Debug.Log("soundToggle="+soundToggle.isOn);
        if (soundToggle.isOn)
        {
            PlayerPrefs.SetString("issound", "1");
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetString("issound", "0");
            PlayerPrefs.Save();
        }
        R_SoundManager.instance.SoundCheck();
        Debug.Log("issound="+PlayerPrefs.GetString("issound"));
    }

    IEnumerator ShowPopUp(string msg, float delay)
    {
        inGamePopUp.SetActive(true);
        inGamePopUp.transform.GetChild(0).GetComponent<Text>().text = msg;
        yield return new WaitForSeconds(delay);
        inGamePopUp.SetActive(false);
    }

    public void ShowTableMessage(string messageToShow)
    {
        tableText.text = messageToShow;
    }

    public IEnumerator SetTableText(string textStr, float waitFor = 1.5f, Action callBackMethod = null)
    {
        tableText.text = textStr;
        yield return new WaitForSeconds(waitFor);
        tableText.text = string.Empty;
        if (callBackMethod!=null)
        {
            callBackMethod();
        }
    }

    public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "Ok")
    {
        // if (!IsScreenActive(Rummy_InGameScreens.Message))
        // {
        //     Rummy_InGameActiveScreens mainMenuScreen = new Rummy_InGameActiveScreens();
        //     mainMenuScreen.screenName = Rummy_InGameScreens.Message;
        //     mainMenuScreen.screenLayer = GetScreenLayer(Rummy_InGameScreens.Message);

        //     GameObject gm = Instantiate(screens[(int)Rummy_InGameScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
        //     mainMenuScreen.screenObject = gm;

        //     Rummy_InGameActiveScreens.Add(mainMenuScreen);

        //     gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
        // }
    }

    public void ShowMessage(string messageToShow, Action yesButtonCallBack, Action noButtonCallBack, string yesButtonText = "Yes", string noButtonText = "No")
    {

    }

    public void ToggleSuggestionButton(bool isShow, bool isCheckAvailable = false, int callAmount = 0, float availableBalance = 0)
    {

    }

    //public void ToggleActionButton(bool isShow, PlayerScript playerObject = null, bool isCheckAvailable = false, int lastBetAmount = 0, float availableBalance = 0)
    //{

    //}

    private void OnDestroy()
    {
        // R_SocketController.instance.ResetConnection();
    }

}


public class Rummy_InGameActiveScreens
{
    public GameObject screenObject;
    public Rummy_InGameScreens screenName;
    public ScreenLayer screenLayer;
}

public enum Rummy_InGameScreens
{
    GameSettings,
    Message
}

public enum Rummy_PlayerAction
{
    Call,
    Raise,
    Fold,
    Check,
    AllIn
}

public enum Rummy_SuggestionActions
{
    Call,
    Call_Any,
    Fold,
    Check,
    Null
}

public enum Rummy_Emoji
{
    BeerCheers,
    Bluffing,
    Cherees,
    DangerBomb,
    Donkey,
    Dung,
    Fish,
    Gun,
    Kiss,
    Murgi,
    Oscar,
    Rocket,
    ThumbsUp,
    YouRaPro,
    Rose,
    Perfume,
    Ring,
    Car
}

