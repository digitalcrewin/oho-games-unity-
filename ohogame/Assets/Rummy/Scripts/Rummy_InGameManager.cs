using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using System;
using BestHTTP.SocketIO;
using System.Security.Cryptography;
using System.Linq;
//using Newtonsoft.Json.Linq;
using DG.Tweening;

public class Rummy_InGameManager : MonoBehaviour
{
    public static Rummy_InGameManager instance;

    public GameObject[] allPlayersObject;

    [SerializeField]
    public R_PlayerScript[] allPlayersScript;

    [SerializeField]
    private Transform[] allPlayerPos;


    [SerializeField]
    private GameObject cardAnimationPrefab, betAnimationPrefab;
    [SerializeField]
    private Transform animationLayer;

    [SerializeField]
    private GameObject winningPrefab, chipscoine;

    public bool isGameStart;

    private R_PlayerScript[] onlinePlayersScript = null;
    private R_PlayerScript myPlayerObject = null, currentPlayer = null;
    private int MATCH_ROUND = 0, LAST_BET_AMOUNT = 0;
    private CardData[] openCards = null;

    private string lastPlayerAction = "";
    private List<GameObject> winnersObject = new List<GameObject>();
    private int communityCardsAniamtionShowedUpToRound = 0;
    private int currentRoundTotalBets = 0;

    private bool isRematchRequestSent = false, isTopUpDone = false;
    private float availableBalance = 0;

    public GameObject WinnAnimationpos;

    //DEV_CODE (Created By Nitin)
    public string currentClickedSeatNum = "";

    //DEV_CODE

    //Variables to store values regarding match winning cards and to highlight them
    public bool isHighlightCard;
    public CardData[] highlightCards;
    public string[] highlightCardString;

    //For Recording Game Play
    Texture2D screenshot;
    public int videoWidth /* = 1280*/;
    public int videoHeight /*= 720*/;
    public bool isRecording = false;


    //To Store Player Data
    public string cardValue = "";          //To Store Card Number with Card Icon
    string tableValue = "";         //To Store table blinds values
    string userID = "";

    //To Store Date and Time
    string date = "";
    string time = "";

    //To Store total player bet value
    string balance = "";

    bool isCardValueSet = false;
    bool isScreenshotCaptured = false;
    string myPlayerSeat;

    int startPlayerTimerReceived = 0;

    public Image thunderPointBar;
    [HideInInspector]
    public bool userWinner = false, isSeatRotation = false;


    public GameObject sortingBtn;
    public GameObject dropBtn;
    public GameObject submitBtn;
    public GameObject lastGameToggle;
    public GameObject autoDropToggle;
    public GameObject autoDropMsgPanel;
    public Button exitBtn; //top-right (Leave Table)
    public R_Task coPlayerTimer, resultAfterWait;
    public bool isMyTurn;
    public bool isMyFirstTurnTaked;
    public bool isCardPulledByMe;
    public bool isCardDicardByMe;
    public bool isFinishSend;
    public bool isWrongShowByMe;
    public bool isLeaveTableByMe;
    public bool isSeatingReceived;
    public bool isAutoDropByMe;
    public bool isDropByMe;
    public bool isMyTurnSkipped;
    public bool isPlayerSubmitTimerReceived;
    public Sprite defaultCardSprite;
    public GameObject cardPulledByMe;

    public int sittingCount;

    // timer
    public float turnTimer, finishTimer;

    // Animation
    public Transform closeCardForAnimOtherPicked;
    public Transform openCardForAnimOtherPicked;

    // for Drop Or SkipTurn
    public GameObject forDropOrSkipTurn;

    // Result
    public Transform resultScrollContent;
    public GameObject resultDataRow;
    public GameObject resultDataRowGroup;
    public GameObject resultDataRowGroupCard;
    public GameObject resultPanel;
    public GameObject confirmationPanel;
    public Text nextGameTimerText;
    public Image resultCutJoker;
    public Text resultMinChipsInfoOnTop;
    public Button yesConfBtn, reCheckConfBtn;

    // FINISH_GAME_TIMER
    public Sprite greenTimerSprite, redTimerSprite;

    // chat btn
    public Button chatBtn;

    // for player sitting position
    int[] Pos_3Player;
    int[] Pos_4Player;
    int[] Pos_5Player;
    int[] Pos_6Player;

    // to prevent Submit button visible again & again
    bool isClickedSubmit = false;

    // public R_Task coTossedCards, coFirstPlayer;

    private void Awake()
    {
        instance = this;
        //ClubInGameManager.instance = null;
        //ClubInGameUIManager.instance = null;
        if (Application.isEditor)
            Application.runInBackground = false;
        //Debug.Log("Time: " + System.DateTime.Now.Hour + System.DateTime.Now.Minute);
    }

    public Text TableName;
    public GameObject RabbitButton;

    private void Start()
    {
        isSeatRotation = false;
        //DEV_CODE
        highlightCardString = new string[5];
        highlightCards = new CardData[5];

        // gameExitCalled = false;

        // UpdatePot("");
        // Pot.SetActive(false);
        // DeactivateAllPots();
        onlinePlayersScript = new R_PlayerScript[0];

        TableName.text = "";// GlobalGameManager.instance.GetRoomData().title;

        // for player sitting position
        Pos_3Player = new int[] { 0, 2, 4 };
        Pos_4Player = new int[] { 0, 2, 3, 4 };
        Pos_5Player = new int[] { 0, 1, 2, 3, 4 };
        Pos_6Player = new int[] { 0, 1, 2, 3, 4, 5 };
        // string tossedcarddataTest = "[\"TOSSED_CARDS\",[{\"playerId\":\"502\",\"card\":\"JJ\",\"rank\":0},{\"playerId\":\"503\",\"card\":\"AH\",\"rank\":1},{\"playerId\":\"504\",\"card\":\"JS\",\"rank\":0}]]";
        // SetTossedCards(tossedcarddataTest);
        // SetResult("[\"RESULT\",[{\"playerId\":\"459\",\"score\":97,\"cards\":[[\"9H\",\"9H\",\"JD\",\"9S\",\"JS\",\"AC\",\"10C\"],[\"JJ\",\"3H\",\"4H\"],[\"6S\",\"7S\",\"KS\"]]},{\"playerId\":\"501\",\"score\":0,\"cards\":[[\"6D\",\"7D\",\"8D\",\"9D\"],[\"4D\",\"4S\",\"4C\"],[\"3S\",\"4S\",\"5S\"],[\"JJ\",\"4H\",\"5H\"]]},{\"winnerId\":\"501\"}]]");
        // OnSetSeating("[\"SEATING\",[\"479\",\"501\",\"502\",\"503\"]]");
        // OnSetSeating("[\"SEATING\",{\"seats\":[\"501\",\"472\",\"502\",\"503\",\"504\",\"505\"],\"turnTimer\":30,\"finishTimer\":15}]");
        // OnSetSeating("[\"SEATING\",{\"seats\":[\"501\",\"472\",\"502\",\"503\"],\"turnTimer\":30,\"finishTimer\":15}]");
        // OnSetSeating("[\"SEATING\",{\"seats\":[\"501\",\"472\"],\"turnTimer\":30,\"finishTimer\":15}]");
        // AnimOpponentCardPicked("[\"OPPONENT_CARD_PICKED\",\"479\"]");
        // StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(1f, () => {
        // SetTossedCardsMultiPlayer("[\"SetTossedCards\",[{\"playerId\":\"472\",\"card\":\"JJ\",\"rank\":0},{\"playerId\":\"501\",\"card\":\"AH\",\"rank\":1},{\"playerId\":\"502\",\"card\":\"2H\",\"rank\":2},{\"playerId\":\"503\",\"card\":\"3H\",\"rank\":3},{\"playerId\":\"504\",\"card\":\"4H\",\"rank\":4},{\"playerId\":\"505\",\"card\":\"5H\",\"rank\":5}]]");
        // SetTossedCardsMultiPlayer("[\"SetTossedCards\",[{\"playerId\":\"472\",\"card\":\"JJ\",\"rank\":0},{\"playerId\":\"501\",\"card\":\"AH\",\"rank\":1},{\"playerId\":\"502\",\"card\":\"2H\",\"rank\":2},{\"playerId\":\"503\",\"card\":\"3H\",\"rank\":3}]]");
        // SetTossedCardsMultiPlayer("[\"SetTossedCards\",[{\"playerId\":\"472\",\"card\":\"JJ\",\"rank\":0},{\"playerId\":\"501\",\"card\":\"AH\",\"rank\":1}]]");
        // }));

        allPlayersScript[0].nameText.text = R_PlayerManager.instance.GetPlayerGameData().userName; //PlayerManager.instance
        allPlayersScript[0].balanceText.text = R_PlayerManager.instance.GetPlayerGameData().coins.ToString();
        if (R_PlayerManager.instance.GetPlayerGameData().avatarURL != null)
        {
            StartCoroutine(R_WebServices.instance.LoadImageFromUrl(R_PlayerManager.instance.GetPlayerGameData().avatarURL, allPlayersScript[0].avtar));
            RectTransform allPlayer0Rt = allPlayersScript[0].avtar.GetComponent<RectTransform>();
            allPlayer0Rt.offsetMin = new Vector2(0f, allPlayer0Rt.offsetMin.y);  //left
            allPlayer0Rt.offsetMax = new Vector2(0f, allPlayer0Rt.offsetMax.y);  //-right
            allPlayer0Rt.offsetMax = new Vector2(allPlayer0Rt.offsetMax.x, 0f);  //-top
            allPlayer0Rt.offsetMin = new Vector2(allPlayer0Rt.offsetMin.x, 0f);  //bottom
        };
        Rummy_InGameUiManager.instance.ShowTableMessage("Please wait while game starts");
    }

    public void OnClickOnButton(string eventName)
    {
        // Debug.Log("Event Name => " + eventName);

        // SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "fromDeck":  //R_CardManager GetNewCard()
                // Debug.Log("isMyTurn="+isMyTurn);
                // Debug.Log("isCardPulledByMe="+isCardPulledByMe);
                if (isMyTurn == true && R_SocketController.instance != null)
                {
                    if (!isCardPulledByMe)
                    {
                        R_SoundManager.instance.PlaySound(R_SoundType.CARD_PICKED);
                        R_SocketController.instance.SendFromDeck();
                        R_CardManager.Instance.SendArrangedCard();
                    }
                    isCardPulledByMe = true;
                    R_CardManager.Instance.showArea.SetActive(true);
                    dropBtn.SetActive(false);
                }
                break;

            case "fromDiscarded":  //R_CardManager onClickDropBtn()
                // Debug.Log("isMyTurn="+isCardPulledByMe);
                // Debug.Log("isCardPulledByMe="+isCardPulledByMe);

                // string currentCardFunc = R_CardManager.Instance.dropArea.GetComponent<Image>().sprite.name;
                // char[] cardFunc = currentCardFunc.ToCharArray();
                // string numStr = currentCardFunc.Substring(0, cardFunc.Length - 1);

                // string jokerCardInitial = R_CardManager.Instance.jokerImg.sprite.name;
                // char[] jokerCardFunc = jokerCardInitial.ToCharArray();
                // string jokerNumStr = jokerCardInitial.Substring(0, jokerCardFunc.Length - 1);

                // // if (R_CardManager.Instance.dropArea.GetComponent<Image>().sprite.name.Equals(R_CardManager.Instance.jokerImg.sprite.name))
                // if (numStr.Equals(jokerNumStr) && !R_CardManager.Instance.dropArea.GetComponent<Image>().sprite.name.Equals("JJ"))
                // {
                //     // StartCoroutine(R_CardManager.Instance.ShowNotification("Cut Joker card selection is not allowed"));
                //     if (isMyTurn==true)
                //     {
                //         if (R_CardManager.Instance.jokerImg.sprite.name == "JJ" && (currentCardFunc=="JS" || currentCardFunc=="JC" || currentCardFunc=="JH" || currentCardFunc=="JD"))
                //         {
                //             Debug.Log("cut-joker=Joker & Drop-Area=JS||JC||JH||JD");
                //         }
                //         else
                //         {
                //             StartCoroutine(Rummy_InGameUiManager.instance.SetTableText("Cut Joker card selection is not allowed", 2f));
                //         }
                //     }
                // }
                // else 
                if (R_CardManager.Instance.dropArea.GetComponent<Image>().sprite.name.Equals("JJ"))
                {
                    if (isMyTurn == true)
                    {
                        StartCoroutine(Rummy_InGameUiManager.instance.SetTableText("Joker card selection is not allowed", 2f));
                    }
                }
                else if (isMyTurn == true && R_SocketController.instance != null)
                {
                    if (!isCardPulledByMe)
                    {
                        R_SoundManager.instance.PlaySound(R_SoundType.CARD_PICKED);
                        R_SocketController.instance.SendFromDiscarded();
                        R_CardManager.Instance.SendArrangedCard();
                    }
                    isCardPulledByMe = true;
                    R_CardManager.Instance.showArea.SetActive(true);
                    dropBtn.SetActive(false);
                }
                break;

            case "dropBtn":
                if (R_SocketController.instance != null)
                {
                    R_CardManager.Instance.SendArrangedCard();
                    R_SocketController.instance.SendDrop();
                    // R_SocketController.instance.ResetConnection();
                    // LoadMainMenu();
                    isDropByMe = true;
                    DisplayDropMsg();
                    DisableControlsAndHideHandCards();
                }
                break;

            case "submitBtn":
                // if (!isFinishSend)
                // {
                R_CardManager.Instance.DragOnShowCard();
                // }
                // else
                // {
                //     R_CardManager.Instance.SendArrangedCard();
                // }
                sortingBtn.SetActive(false);
                submitBtn.SetActive(false);
                dropBtn.SetActive(false);
                Rummy_InGameUiManager.instance.exitButton.interactable = false;
                timerFinishRing.fillAmount = 0f;
                //StartCoroutine(R_CardManager.Instance.ShowNotification("Waiting for Result"));

                if ((sittingCount == 2) || (sittingCount > 2 && !isWrongShowByMe))
                {
                    Rummy_InGameUiManager.instance.ShowTableMessage("Waiting for Result");
                }
                isClickedSubmit = true;
                break;

            case "yesAutoDropBtn":
                R_SocketController.instance.SendAutoDrop();
                isAutoDropByMe = true;
                DisplayDropMsg();
                DisableControlsAndHideHandCards();
                break;

            case "setToggleOff":
                autoDropToggle.GetComponent<Toggle>().isOn = false;
                break;

            case "forDropOrSkipTurn":
                forDropOrSkipTurn.SetActive(false);
                R_CardManager.Instance.handCards.gameObject.SetActive(true);
                break;

            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in Rummy_InGameUiManager = " + eventName);
#endif
                break;
        }
    }

    public void OnLeaveTableToggle(Toggle mToggle)
    {
        // Debug.Log("is on="+mToggle.isOn);
        if (mToggle.isOn)
        {
            R_SocketController.instance.SendExitNextRound("false");
            isLeaveTableByMe = true;
            mToggle.interactable = false;
        }
    }

    public void OnAutoDropToggle(Toggle mToggle)
    {
        // Debug.Log("is on="+mToggle.isOn);
        if (mToggle.isOn)
        {
            autoDropMsgPanel.SetActive(true);
        }
    }

    public void DisplayDropMsg()
    {
        autoDropMsgPanel.SetActive(true);
        autoDropMsgPanel.transform.Find("Confirmation").gameObject.SetActive(false);
        autoDropMsgPanel.transform.Find("MsgText").gameObject.SetActive(true);
        autoDropMsgPanel.transform.Find("CloseBtn").GetComponent<Button>().enabled = false;
    }

    void DisableControlsAndHideHandCards()
    {
        if (sittingCount > 2)
        {
            float delayTime = 0.5f;
            if (isAutoDropByMe || isDropByMe)
            {
                delayTime = 3f;
            }
            R_Task hideCardsButtonsForMultiplayer = new R_Task(R_GlobalGameManager.instance.WaitForDelay(delayTime));
            hideCardsButtonsForMultiplayer.Finished += delegate (bool manual)
            {
                if (!manual)
                {
                    for (int i = 0; i < allPlayersScript.Length; i++)
                    {
                        if (allPlayersScript[i].playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            allPlayersScript[i].timerBarImg.fillAmount = 0f;
                        }
                    }
                    DisableAllGameControlButtons();
                    R_CardManager.Instance.handCards.gameObject.AddComponent<Image>();
                    R_CardManager.Instance.handCards.gameObject.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
                    R_CardManager.Instance.handCards.gameObject.AddComponent<Button>().onClick.AddListener(() =>
                    {
                        R_CardManager.Instance.handCards.gameObject.SetActive(false);
                        forDropOrSkipTurn.SetActive(true);
                    });
                    R_CardManager.Instance.handCards.gameObject.SetActive(false);
                    autoDropMsgPanel.SetActive(false);
                    forDropOrSkipTurn.SetActive(true);
                    R_CardManager.Instance.SetCardNotInteractable();
                    hideCardsButtonsForMultiplayer.Finished -= delegate (bool manual) { };
                }
            };
        }
    }



    #region socket_response
    public void OnSetSeating(string socketPacket)
    {
        //["SEATING",{"seats":["501","459"],"turnTimer":30,"finishTimer":15}]
        //["SEATING",{"seats":["461","547"],"turnTimer":30,"finishTimer":40,"playerDetails":[{"userId":"461","userName":"sam","chips":400000},{"userId":"547","userName":"1vnhup","chips":500000}]}]
        // ["SEATING",{"seats":["472","513"],"turnTimer":30,"finishTimer":40,"playerDetails":[{"userId":"472","userName":"v2","profileImage":"","chips":9999000,"isFirstGame":true},{"userId":"513","userName":"v3.ab","profileImage":"https://dcmediaapifiles.s3.amazonaws.com/41b5fefc-ae45-4577-ba66-0ab062f73774.jpg","chips":495000,"isFirstGame":true}]}]
        Debug.Log("SetSeating = " + socketPacket);
        JsonData data = JsonMapper.ToObject(socketPacket);

        // Debug.Log("SetSeating = seats =" + data[1]["seats"]);
        // Debug.Log("SetSeating = turnTimer =" + data[1]["turnTimer"]);
        // Debug.Log("SetSeating = finishTimer =" + data[1]["finishTimer"]);

        if (!float.TryParse(data[1]["turnTimer"].ToString(), out turnTimer)) { } else { turnTimer -= 1; }
        if (!float.TryParse(data[1]["finishTimer"].ToString(), out finishTimer)) { }

        // Debug.Log("turnTimer="+turnTimer);
        // Debug.Log("finishTimer="+finishTimer);

        sittingCount = data[1]["seats"].Count;
        int playerIndex = -1;
        if (sittingCount > 0)
        {
            isSeatingReceived = true;
            for (int i = 0; i < sittingCount; i++)
            {
                R_PlayerData playerData = new R_PlayerData();
                playerData.userId = data[1]["seats"][i].ToString();
                if (sittingCount == 2)
                {
                    if (playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        print("IFFFFFFFFFFFF ME : " + playerData.userId);
                        playerIndex = i;
                        allPlayersScript[0].InitPlayerData(playerData);
                        // allPlayersScript[0].nameText.text = playerData.userId;
                    }
                    else
                    {
                        allPlayersScript[3].gameObject.SetActive(true);
                        allPlayersScript[3].InitPlayerData(playerData);
                        // allPlayersScript[3].nameText.text = playerData.userId;
                    }
                }
                else if (sittingCount == 3)
                {
                    if (playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        allPlayersScript[0].InitPlayerData(playerData);
                        // allPlayersScript[0].nameText.text = playerData.userId;
                        print("IFFFFFFFFFFFF ME : " + playerData.userId);
                        playerIndex = i;
                        // setSitting(Pos_3Player, data[1], playerIndex);
                        setSitting_1(Pos_3Player, data[1]["seats"], playerIndex);
                    }
                }
                else if (sittingCount == 4)
                {
                    print(playerData.userId + " " + R_PlayerManager.instance.GetPlayerGameData().userId);
                    if (playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        print("HHHHHHHHHHHHHHHHIIIIIIIIIIIIIIII");
                        allPlayersScript[0].InitPlayerData(playerData);
                        // allPlayersScript[0].nameText.text = playerData.userId;
                        print("IFFFFFFFFFFFF ME : " + playerData.userId);
                        playerIndex = i;
                        // setSitting(Pos_4Player, data[1], playerIndex);
                        setSitting_1(Pos_4Player, data[1]["seats"], playerIndex);
                    }
                }
                else if (sittingCount == 5)
                {
                    if (playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        allPlayersScript[0].InitPlayerData(playerData);
                        // allPlayersScript[0].nameText.text = playerData.userId;
                        print("IFFFFFFFFFFFF ME : " + playerData.userId);
                        playerIndex = i;
                        // setSitting(Pos_5Player, data[1], playerIndex);
                        setSitting_1(Pos_5Player, data[1]["seats"], playerIndex);
                    }
                }
                else if (sittingCount == 6)
                {
                    if (playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        allPlayersScript[0].InitPlayerData(playerData);
                        // allPlayersScript[0].nameText.text = playerData.userId;
                        print("IFFFFFFFFFFFF ME : " + playerData.userId);
                        playerIndex = i;
                        // setSitting(Pos_6Player, data[1], playerIndex);
                        setSitting_1(Pos_6Player, data[1]["seats"], playerIndex);
                    }
                }
            }
            R_GlobalGameManager.instance.IsJoiningPreviousGame = true;
        }
        int playerDetailsCount = data[1]["playerDetails"].Count;
        if (playerDetailsCount > 0)
        {
            //{"userId":"461","userName":"sam","chips":400000},{"userId":"547","userName":"1vnhup","chips":500000}
            for (int i = 0; i < playerDetailsCount; i++)
            {
                int countScript = allPlayersScript.Count();
                for (int j = 0; j < countScript; j++)
                {
                    if (!string.IsNullOrEmpty(allPlayersScript[j].playerData.userId) && allPlayersScript[j].playerData.userId.Equals(data[1]["playerDetails"][i]["userId"].ToString()))
                    {
                        allPlayersScript[j].playerData.userName = data[1]["playerDetails"][i]["userName"].ToString();
                        allPlayersScript[j].playerData.balance = float.Parse(data[1]["playerDetails"][i]["chips"].ToString());
                        allPlayersScript[j].nameText.text = allPlayersScript[j].playerData.userName;
                        allPlayersScript[j].balanceText.text = allPlayersScript[j].playerData.balance.ToString();
                        int currentIndex = j;
                        try
                        {
                            allPlayersScript[currentIndex].playerData.profileImage = data[1]["playerDetails"][i]["profileImage"].ToString();
                            RectTransform allPlayer0Rt = allPlayersScript[currentIndex].avtar.GetComponent<RectTransform>();
                            allPlayer0Rt.offsetMin = new Vector2(0f, allPlayer0Rt.offsetMin.y);  //left
                            allPlayer0Rt.offsetMax = new Vector2(0f, allPlayer0Rt.offsetMax.y);  //-right
                            allPlayer0Rt.offsetMax = new Vector2(allPlayer0Rt.offsetMax.x, 0f);  //-top
                            allPlayer0Rt.offsetMin = new Vector2(allPlayer0Rt.offsetMin.x, 0f);  //bottom
                            SetAvatarImages(allPlayersScript[currentIndex].playerData.profileImage, allPlayersScript[currentIndex].avtar, currentIndex);
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log("profileImage error");
                        }
                        break;
                    }
                }
            }
            Rummy_InGameUiManager.instance.SetDiscardUsers();
        }
    }

    // copied from GetFirstTurnOfPlayer()
    public void SetTossedCardsMultiPlayer(string socketPacket)
    {
        // Debug.Log("SetTossedCards");
        // Debug.Log("socketPacket="+socketPacket);
        JsonData data = JsonMapper.ToObject(socketPacket);
        int userCount = data[1].Count;
        int playerIndex = -1;
        if (userCount > 0)
        {
            Rummy_InGameUiManager.instance.ShowTableMessage(string.Empty);
            List<GameObject> firstCardForAnimation = new List<GameObject>();
            if (userCount == 2)
            {
                for (int i = 0; i < userCount; i++)
                {
                    Debug.Log("i=" + i);
                    Debug.Log("playerId=" + data[1][i]["playerId"].ToString());
                    Debug.Log("card=" + data[1][i]["card"].ToString());
                    Debug.Log("rank=" + data[1][i]["rank"].ToString());

                    string currentCard = data[1][i]["card"].ToString();
                    // R_PlayerData playerData = new R_PlayerData();
                    // playerData.userId = data[1][i]["playerId"].ToString();

                    // if (userCount == 2)
                    // {
                    if (data[1][i]["playerId"].ToString() == R_PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        print("IFFFFFFFFFFFF ME : " + data[1][i]["playerId"].ToString());
                        playerIndex = i;
                        //Debug.Log("dataCount timer img FOUND ", allPlayersScript[playerIndex].timerBarImg);
                        allPlayersScript[0].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                        //allPlayersScript[0].firstCardImage.gameObject.SetActive(true);
                        firstCardForAnimation.Add(allPlayersScript[0].firstCardImage.gameObject);
                        // allPlayersScript[0].InitPlayerData(playerData);
                    }
                    else
                    {
                        allPlayersScript[3].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                        //allPlayersScript[3].firstCardImage.gameObject.SetActive(true);
                        firstCardForAnimation.Add(allPlayersScript[3].firstCardImage.gameObject);
                        allPlayersScript[3].gameObject.SetActive(true);
                    }
                    // allPlayersScript[3].InitPlayerData(playerData);
                    // }
                    // else
                    // {
                    //     FirstCardShow(data);
                    // }
                }
                ShowFirstCardAnimation(firstCardForAnimation);
            }
            else
            {
                FirstCardShow(data);
            }
            // else
            // {
            //     R_Task coTossedCards = new R_Task(R_GlobalGameManager.instance.WaitForDelay(2f)); //HideFirstCardsFromTossedCards()
            //     coTossedCards.Finished += delegate(bool manual) {
            //         if (!manual)
            //         {
            //             // if (!string.IsNullOrEmpty(R_SocketController.instance.firstPlayerData))
            //             // {
            //             //     SetFirstPlayer(R_SocketController.instance.firstPlayerData);
            //             // }
            //             HideFirstCards();
            //             coTossedCards.Finished -= delegate(bool manual){};
            //         }
            //     };
            // }
        }
    }

    void ShowFirstCardAnimation(List<GameObject> firstCardForAnimation)
    {
        Vector3 animStartPos = allPlayersScript[0].transform.parent.position;
        bool isFirstPlayerSend = false;
        int loopCount = firstCardForAnimation.Count;
        Debug.Log("firstCardForAnimation " + loopCount);
        for (int i = 0; i < loopCount; i++)
        {
            Vector3 endPosition = firstCardForAnimation[i].transform.localPosition;
            Debug.Log("endPosition " + i + ", " + endPosition);

            firstCardForAnimation[i].transform.position = animStartPos;

            firstCardForAnimation[i].SetActive(true);

            int index = i;

            var animSequence = firstCardForAnimation[i].transform.DOLocalMove(endPosition, 0.5f)
            .OnComplete(() =>
            {

                if (index == (loopCount - 1))
                {
                    Debug.Log("isFirstPlayerSend=" + isFirstPlayerSend);
                    isFirstPlayerSend = true;
                    if (isFirstPlayerSend)
                    {
                        // SetFirstPlayer("[\"FIRST_PLAYER\",{\"playerId\":\"472\",\"card\":\"JJ\",\"rank\":0}]");
                        if (!string.IsNullOrEmpty(R_SocketController.instance.firstPlayerData))
                        {
                            StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(1.5f, () =>
                            {
                                SetFirstPlayer(R_SocketController.instance.firstPlayerData);
                            }));
                            StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(5f, () =>
                            {
                                HideFirstCards();
                            }));
                        }
                    }
                    Debug.Log("isFirstPlayerSend2=" + isFirstPlayerSend);
                }
            });

        }
    }

    void FirstCardShow(JsonData data)
    {
        //[    "504",    "503",    "502",    "501"]
        //["TOSSED_CARDS",[{"playerId":"451","card":"JJ","rank":0},{"playerId":"472","card":"AH","rank":1},{"playerId":"478","card":"2H","rank":2},{"playerId":"479","card":"3H","rank":3}]]
        // Vector2 []endPos = new Vector2[6];
        // Vector3 startPos = allPlayersScript[0].transform.parent.position;
        List<GameObject> firstCardForAnimationMulti = new List<GameObject>();
        for (int i = 0; i < allPlayersScript.Length; i++)
        {
            // endPos[i] = allPlayersScript[i].firstCardImage.GetComponent<RectTransform>().anchoredPosition;
            // allPlayersScript[i].firstCardImage.transform.position = startPos;
            // int index = i;

            for (int j = 0; j < data[1].Count; j++)
            {
                if (data[1][j]["playerId"].ToString() == allPlayersScript[i].playerData.userId)
                {
                    string currentCard = data[1][j]["card"].ToString();
                    allPlayersScript[i].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                    allPlayersScript[i].firstCardImage.gameObject.SetActive(true);
                    firstCardForAnimationMulti.Add(allPlayersScript[i].firstCardImage.gameObject);
                }
            }
            // allPlayersScript[i].firstCardImage.transform.DOLocalMove(endPos[i].normalized, 1f);
        }
        ShowFirstCardAnimation(firstCardForAnimationMulti);
    }

    public void setSitting_1(int[] playerPos, JsonData data, int playerIndex)
    {
        //string currentCard = "";
        if (playerIndex == 0)
        {
            for (int p = 1; p < playerPos.Length; p++)
            {
                R_PlayerData d = new R_PlayerData();
                //currentCard = data[1][p]["card"].ToString();
                string playerId = data[p].ToString();
                d.userId = playerId;
                allPlayersScript[playerPos[p]].nameText.text = d.userId;
                //allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                //allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].InitPlayerData(d);
                d = null;
            }
        }
        else if (playerIndex == playerPos.Length - 1)
        {
            for (int p = 1; p < playerPos.Length; p++)
            {
                R_PlayerData d = new R_PlayerData();
                string playerId = data[p - 1] + "";
                //currentCard = data[1][p-1]["card"].ToString();
                d.userId = playerId;
                allPlayersScript[playerPos[p]].nameText.text = d.userId;
                //allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                //allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].InitPlayerData(d);
                d = null;
            }
        }
        else
        {
            for (int p = 1; p < playerPos.Length; p++)
            {
                string playerId = "";
                R_PlayerData d = new R_PlayerData();

                print("Modulo : " + ((p + playerIndex) % data.Count));
                int pos = ((p + playerIndex) % data.Count);
                playerId = data[pos] + "";
                d.userId = playerId;
                allPlayersScript[playerPos[p]].nameText.text = d.userId;
                /*if (p <= playerIndex)
                {
                    //currentCard = data[1][p - 1]["card"].ToString();
                    //playerId = data[p - 1] + "";
                    
                    playerId = data[p + 1] + "";
                    d.userId = playerId;
                }
                else
                {
                    //currentCard = data[1][p]["card"].ToString();
                    playerId = data[p] + "";
                    d.userId = playerId;
                }Old Code*/
                //allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                //allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].InitPlayerData(d);
                d = null;
            }
        }
    }

    // arrange from right-side
    public void setSitting(int[] playerPos, JsonData data, int playerIndex)
    {
        string currentCard = "";
        if (playerIndex == 0)
        {
            for (int p = (playerPos.Length - 1); p > 0; p--)
            {
                R_PlayerData d = new R_PlayerData();
                d.userId = data[p].ToString();
                allPlayersScript[playerPos[p]].nameText.text = d.userId;
                // currentCard = data[1][p]["card"].ToString();
                // allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                // allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].InitPlayerData(d);
                d = null;
            }
        }
        else if (playerIndex == playerPos.Length)
        {
            for (int p = (playerPos.Length - 1); p > 0; p--)
            {
                R_PlayerData d = new R_PlayerData();
                d.userId = data[p - 1].ToString();
                allPlayersScript[playerPos[p]].nameText.text = d.userId;
                // currentCard = data[1][p - 1]["card"].ToString();
                // allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                // allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                allPlayersScript[playerPos[p]].InitPlayerData(d);
                d = null;
            }
        }
        else
        {
            for (int p = (playerPos.Length - 1); p > 0; p--)
            {
                R_PlayerData d = new R_PlayerData();
                if (p <= playerIndex)
                {
                    // currentCard = data[1][p-1]["card"].ToString();
                    d.userId = data[p - 1].ToString();
                    Debug.Log("setsetting 1: " + p);
                    allPlayersScript[playerPos[p]].nameText.text = d.userId;
                    // allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                    // allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                    allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                    allPlayersScript[playerPos[p]].InitPlayerData(d);
                    d = null;
                }
                else
                {
                    // currentCard = data[1][p]["card"].ToString();
                    d.userId = data[p].ToString();
                    Debug.Log("setsetting 2: " + p);
                    allPlayersScript[playerPos[p]].nameText.text = d.userId;
                    // allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                    // allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                    allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                    allPlayersScript[playerPos[p]].InitPlayerData(d);
                    d = null;
                }
                Debug.Log("setsetting 3: " + p);
                // allPlayersScript[playerPos[p]].nameText.text = d.userId;
                // // allPlayersScript[playerPos[p]].firstCardImage.sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(currentCard);
                // // allPlayersScript[playerPos[p]].firstCardImage.gameObject.SetActive(true);
                // allPlayersScript[playerPos[p]].gameObject.SetActive(true);
                // allPlayersScript[playerPos[p]].InitPlayerData(d);
                // d = null;
            }
        }
    }

    // IEnumerator HideFirstCardsFromTossedCards()
    // {
    //     // yield return new WaitForSeconds(1f);
    //     // Playerselect1.SetActive(true);
    //     // Playerselect2.SetActive(true);
    //     yield return new WaitForSeconds(2f);
    //     HideFirstCards();

    //     // to test manually
    //     //["FIRST_PLAYER",{"playerId":"31","card":"JJ","rank":0}]
    //     // SetFirstPlayer("[\"FIRST_PLAYER\",{\"playerId\":\"31\",\"card\":\"JJ\",\"rank\":0}]");
    // }

    public void SetFirstPlayer(string socketPacket)
    {
        //["FIRST_PLAYER",{"playerId":"31","card":"JJ","rank":0}]
        // ["FIRST_PLAYER",{"playerId":"471","playerName":"v4.ab","card":"QH","rank":12}]
        JsonData data = JsonMapper.ToObject(socketPacket);
        int cardCount = data[1].Count;
        if (cardCount > 0)
        {
            // Debug.Log("[playerId]="+data[1]["playerId"].ToString());
            // Debug.Log("[card]="+data[1]["card"].ToString());
            // Debug.Log("[rank]="+data[1]["rank"].ToString());

            // for (int i = 0; i < allPlayersScript.Length; i++)
            // {
            //     if (allPlayersScript[i].playerData.userId == data[1]["playerId"].ToString())
            //     {
            //         int tempIndex = i;
            //         Debug.Log("WINNER ID="+allPlayersScript[i].playerData.userId);
            //         Sequence firstCardWinSequence = DOTween.Sequence();
            //         firstCardWinSequence.Append(allPlayersScript[tempIndex].firstCardImage.transform.DOPunchScale(new Vector3 (.5f, .5f, 1), .5f));
            //         firstCardWinSequence.Append(allPlayersScript[tempIndex].firstCardImage.transform.DOPunchScale(new Vector3 (.5f, .5f, 1), .5f));
            //         firstCardWinSequence.OnComplete(() => {HideFirstCards();});
            //     }
            // }

            R_Task coFirstPlayer;
            //             if (R_SocketController.instance.rummyUserId==data[1]["playerId"].ToString())
            //             {
            // // Debug.Log("Self TURN");
            //                 coFirstPlayer = new R_Task(Rummy_InGameUiManager.instance.SetTableText("Your Turn", 1.5f, CallbackAfterFirstPlayerMsg));
            //             }
            //             else
            //             {
            // // Debug.Log("Opponent TURN");
            //                 coFirstPlayer = new R_Task(Rummy_InGameUiManager.instance.SetTableText("Opponent Turn", 1.5f, CallbackAfterFirstPlayerMsg));
            //             }
            coFirstPlayer = new R_Task(Rummy_InGameUiManager.instance.SetTableText(data[1]["playerName"].ToString() + " has won Cut for Seat, and will start the hand", 3.5f, CallbackAfterFirstPlayerMsg));
            coFirstPlayer.Finished += delegate (bool manual)
            {
                if (!manual)
                {
                    // if (!string.IsNullOrEmpty(R_SocketController.instance.handCardsData))
                    // {
                    //     R_CardManager.Instance.SetHandCards(R_SocketController.instance.handCardsData);
                    // }
                    // if (!string.IsNullOrEmpty(R_SocketController.instance.deckInfoData))
                    // {
                    //     R_CardManager.Instance.SetDeckInfo(R_SocketController.instance.deckInfoData);
                    // }
                    Rummy_InGameUiManager.instance.ShowTableMessage("Waiting for Hand Cards");
                    coFirstPlayer.Finished -= delegate (bool manual) { };
                }
            };
        }
    }

    void CallbackAfterFirstPlayerMsg()
    {
        // SetHandCards("[\"HAND_CARDS\",[\"4H\",\"JH\",\"6S\",\"QS\",\"9D\",\"AS\",\"JC\",\"5S\",\"4H\",\"9C\",\"5D\",\"3D\",\"JS\"] ]");
    }

    string tempTimerPlayerId = string.Empty;
    // string tempTimerSec = string.Empty;
    Image timerRing = null;
    public void StartPlayerTimer(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        int dataCount = data[1].Count;
        if ((dataCount > 0))
        {
            if (startPlayerTimerReceived < 10)
            {
                startPlayerTimerReceived++;
            }
            else if (startPlayerTimerReceived == 10)
            {
                Debug.Log("startPlayerTimerReceived=10, " + startPlayerTimerReceived);
                startPlayerTimerReceived++;
                Debug.Log("startPlayerTimerReceived=" + startPlayerTimerReceived);
                exitBtn.interactable = true;
            }

            int playerIndex = -1;
            string timerPlayerId = data[1]["playerId"].ToString();
            string timerSec = data[1]["timmer"].ToString();

            // sound for last 10 seconds
            if ((timerPlayerId == R_PlayerManager.instance.GetPlayerGameData().userId) && (!isAutoDropByMe && !isDropByMe && !isMyTurnSkipped))
            {
                int timerSecInt = 30;
                if (Int32.TryParse(timerSec, out timerSecInt))
                {
                    if (timerSecInt <= 10)
                    {
                        R_SoundManager.instance.PlaySound(R_SoundType.TIMER_LAST_10_SEC);
                    }
                }
                if (timerSec == turnTimer.ToString())
                {
                    R_SoundManager.instance.PlaySound(R_SoundType.TIMER_ON);
                }
            }

            //poker's player timer logic flow for reference:
            //isTurn = true, playerId wise
            //set currentPlayer
            //PlayerTimerReset();
            //R_PlayerScript ShowRemainingTime

            // if (string.IsNullOrEmpty(tempTimerPlayerId))
            // {
            //     // Debug.Log("temp player id is empty");
            //     tempTimerSec = timerSec;
            // }
            // else if (!timerPlayerId.Equals(tempTimerPlayerId))
            // {
            //     // Debug.Log("timerPlayerId is not equal to tempPlayerId");
            //     tempTimerSec = timerSec;
            // }

            // Debug.Log("timerSec: " + timerSec+", float.Parse(timerSec): " + float.Parse(tempTimerSec));
            // Debug.Log("isTossedCardReceived="+R_SocketController.instance.isTossedCardReceived);
            if ((timerSec == turnTimer.ToString()) || !R_SocketController.instance.isTossedCardReceived)
            {
                // Debug.Log("from 44");
                tempTimerPlayerId = timerPlayerId;
                int countScript = allPlayersScript.Count();
                for (int i = 0; i < countScript; i++)
                {
                    R_PlayerData playerData = allPlayersScript[i].playerData;
                    if (playerData.userId == timerPlayerId)
                    {
                        playerIndex = i;
                        // Debug.Log("dataCount timer img FOUND ", allPlayersScript[playerIndex].timerBarImg);
                        timerRing = allPlayersScript[playerIndex].timerBarImg;
                        allPlayersScript[playerIndex].timerBarImg.sprite = greenTimerSprite;
                        playerData.isTurn = true;
                        // break;
                    }
                    else
                    {
                        playerData.isTurn = false;
                    }
                }
                if ((tempTimerPlayerId == R_PlayerManager.instance.GetPlayerGameData().userId) && (!isAutoDropByMe && !isDropByMe && !isMyTurnSkipped))
                {
                    // R_SoundManager.instance.PlaySound(R_SoundType.TIMER_ON);
                    isMyTurn = true;
                    if (!isCardPulledByMe)
                    { dropBtn.SetActive(true); }
                    else { dropBtn.SetActive(false); }

                    if (autoDropToggle.activeSelf)
                    {
                        autoDropToggle.SetActive(false);
                    }
                    isMyFirstTurnTaked = true;
                }
                else
                {
                    isMyTurn = false;
                    isCardPulledByMe = false;
                    dropBtn.SetActive(false);

                    // auto-discard logic
                    if (!isCardDicardByMe && cardPulledByMe != null)
                    {
                        R_CardManager.Instance.Player_1_Card.Remove(cardPulledByMe);
                        Destroy(cardPulledByMe);
                        R_CardManager.Instance.ResetSelectedCard();

                        R_Task runAfterWait = new R_Task(R_GlobalGameManager.instance.WaitForDelay(0.1f));
                        runAfterWait.Finished += delegate (bool manual)
                        {
                            R_CardManager.Instance.SendArrangedCard();
                            runAfterWait.Finished -= delegate (bool manual) { };
                        };
                    }
                    cardPulledByMe = null;
                    isCardDicardByMe = false;
                    confirmationPanel.SetActive(false);

                    if ((!isMyFirstTurnTaked && !autoDropToggle.activeSelf && !isClickedSubmit) && (!isAutoDropByMe && !isDropByMe && !isMyTurnSkipped))
                    {
                        autoDropToggle.SetActive(true);
                    }
                }
                PlayerTimerReset();
                if (!R_SocketController.instance.isTossedCardReceived)
                {
                    float divideAmtR = (1f / (turnTimer + 0.5f));//1/29 == 0.034
                                                                 // Debug.Log("turnTimer="+turnTimer);
                                                                 // Debug.Log("divideAmtRR="+divideAmtR);
                    timerRing.fillAmount = (float.Parse(timerSec) * divideAmtR);//divideAmt;  //1-0.51=0.49
                    // R_SocketController.instance.isReconnectForTimer = false;
                }
                else
                {
                    timerRing.fillAmount = 1f;
                }
            }

            if (timerSec == "1")
            {
                // for auto-discard mismatch
                R_Task runAfterWait2 = new R_Task(R_GlobalGameManager.instance.WaitForDelay(0.5f));
                runAfterWait2.Finished += delegate (bool manual)
                {
                    R_CardManager.Instance.discard_Btn.SetActive(false);
                    R_CardManager.Instance.showArea.SetActive(false);
                    runAfterWait2.Finished -= delegate (bool manual) { };
                };
            }

            if (timerSec == "0")
            {
                // Debug.Log("from 0");
                PlayerTimerReset();
                // R_CardManager.Instance.discard_Btn.SetActive(false);

                // if (autoDropToggle.GetComponent<Toggle>().isOn)
                // {
                //     StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(3f, () => {
                //         Rummy_InGameUiManager.instance.OnClickOnBack();
                //     }));
                // }


                // when turn skip
                if (!isCardPulledByMe && !isCardDicardByMe && !isMyTurnSkipped && (timerPlayerId == R_PlayerManager.instance.GetPlayerGameData().userId))
                {
                    isMyTurnSkipped = true;
                    Debug.Log("1st. You SKIP your TURN");
                    DisableControlsAndHideHandCards();
                }

            }

            // if ((timerRing!=null) && (((float.Parse(tempTimerSec))-1f))>=float.Parse(timerSec))//13 >= 14
            if ((timerRing != null) && (((turnTimer) - 1f) >= float.Parse(timerSec)))//28 >= 29
            {
                float divideAmt = (1f / (turnTimer));
                // Debug.Log("divideAmt: "+divideAmt);
                timerRing.fillAmount = timerRing.fillAmount - divideAmt;
                // Debug.Log("timerRing.fillAmount: "+timerRing.fillAmount);
                // Debug.Log("Time.deltaTime: "+Time.deltaTime);
            }
        }
    }

    public void AnimOpponentCardPicked(string socketPacket)
    {
        // ["OPPONENT_CARD_PICKED",501]
        //["OPPONENT_CARD_PICKED",{    "playerId": "502",    "from": "discardedPile"}]
        //["OPPONENT_CARD_PICKED",{    "playerId": "502",    "from": "deck"}]
        //["OPPONENT_CARD_PICKED",{"playerId":"459","from":"discardedPile","nextCardOnTop":"QD"}]

        JsonData data = JsonMapper.ToObject(socketPacket);
        string playerId = data[1]["playerId"].ToString();
        string from = data[1]["from"].ToString();
        Debug.Log("playerId=" + playerId);
        Debug.Log("from=" + from);
        Sprite oldOpenDeckSprite = R_CardManager.Instance.dropArea.GetComponent<Image>().sprite;

        IDictionary iResult = data[1] as IDictionary;
        if (iResult.Contains("nextCardOnTop"))
        {
            string nextCardOnTop = data[1]["nextCardOnTop"].ToString();
            Debug.Log("nextCardOnTop=" + nextCardOnTop);
            R_CardManager.Instance.dropArea.GetComponent<Image>().sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(data[1]["nextCardOnTop"].ToString());
        }

        if (!string.IsNullOrEmpty(playerId) && !string.IsNullOrEmpty(from))
        {
            if (from.Equals("deck"))
            {
                Vector2 oldPosition = closeCardForAnimOtherPicked.position;
                for (int i = 0; i < allPlayersScript.Count(); i++)
                {
                    if ((playerId != R_PlayerManager.instance.GetPlayerGameData().userId) && (allPlayersScript[i].playerData.userId == playerId))
                    {
                        closeCardForAnimOtherPicked.gameObject.SetActive(true);
                        closeCardForAnimOtherPicked.DOMove(allPlayersScript[i].transform.GetChild(0).GetChild(1).GetChild(6).position, 0.5f);
                        closeCardForAnimOtherPicked.DOScale(new Vector2(0.37f, 0.37f), 0.5f).OnComplete(() =>
                        {
                            closeCardForAnimOtherPicked.gameObject.SetActive(false);
                            closeCardForAnimOtherPicked.position = oldPosition;
                            closeCardForAnimOtherPicked.localScale = Vector3.one;
                        });
                    }
                }
            }
            else if (from.Equals("discardedPile"))
            {
                Vector2 oldPosition = closeCardForAnimOtherPicked.position;
                Sprite oldSprite = closeCardForAnimOtherPicked.GetComponent<Image>().sprite;
                for (int i = 0; i < allPlayersScript.Count(); i++)
                {
                    if ((playerId != R_PlayerManager.instance.GetPlayerGameData().userId) && (allPlayersScript[i].playerData.userId == playerId))
                    {
                        closeCardForAnimOtherPicked.position = R_CardManager.Instance.dropArea.GetComponent<RectTransform>().position;
                        closeCardForAnimOtherPicked.GetComponent<Image>().sprite = oldOpenDeckSprite;
                        closeCardForAnimOtherPicked.gameObject.SetActive(true);
                        closeCardForAnimOtherPicked.DOMove(allPlayersScript[i].transform.GetChild(0).GetChild(1).GetChild(6).position, 0.5f);
                        closeCardForAnimOtherPicked.DOScale(new Vector2(0.37f, 0.37f), 0.5f).OnComplete(() =>
                        {
                            closeCardForAnimOtherPicked.gameObject.SetActive(false);
                            closeCardForAnimOtherPicked.position = oldPosition;
                            closeCardForAnimOtherPicked.GetComponent<Image>().sprite = oldSprite;
                            closeCardForAnimOtherPicked.localScale = Vector3.one;
                        });
                    }
                }
            }
        }
    }

    public void AnimDiscardCard(JsonData data, Sprite spriteFromData)
    {
        //GetSpriteFromSpriteArray(data[1]["cardValue"].ToString());
        //GetSpriteFromSpriteArray(data[1]["playerId"].ToString());

        string playerId = data[1]["playerId"].ToString();

        Vector2 oldPosition = closeCardForAnimOtherPicked.position;
        Sprite oldSprite = closeCardForAnimOtherPicked.GetComponent<Image>().sprite;
        Vector2 oldSizeDelta = closeCardForAnimOtherPicked.GetComponent<RectTransform>().sizeDelta;    //width, height

        if (playerId == R_PlayerManager.instance.GetPlayerGameData().userId)
        {
            R_CardManager.Instance.dropArea.GetComponent<Image>().sprite = spriteFromData;
        }
        else
        {
            for (int i = 0; i < allPlayersScript.Count(); i++)
            {
                if ((playerId != R_PlayerManager.instance.GetPlayerGameData().userId) && (allPlayersScript[i].playerData.userId == playerId))
                {
                    closeCardForAnimOtherPicked.position = allPlayersScript[i].transform.GetChild(0).GetChild(1).GetChild(6).position;
                    closeCardForAnimOtherPicked.GetComponent<Image>().sprite = spriteFromData;
                    closeCardForAnimOtherPicked.GetComponent<RectTransform>().sizeDelta = allPlayersScript[i].transform.GetChild(0).GetChild(1).GetChild(6).GetComponent<RectTransform>().sizeDelta;
                    closeCardForAnimOtherPicked.gameObject.SetActive(true);

                    closeCardForAnimOtherPicked.DOMove(R_CardManager.Instance.dropArea.GetComponent<RectTransform>().position, 0.5f);
                    closeCardForAnimOtherPicked.DOScale(new Vector2(2.37f, 2.37f), 0.5f).OnComplete(() =>
                    {
                        closeCardForAnimOtherPicked.gameObject.SetActive(false);
                        R_CardManager.Instance.dropArea.GetComponent<Image>().sprite = spriteFromData;
                        closeCardForAnimOtherPicked.position = oldPosition;
                        closeCardForAnimOtherPicked.GetComponent<Image>().sprite = oldSprite;
                        closeCardForAnimOtherPicked.GetComponent<RectTransform>().sizeDelta = oldSizeDelta;
                        closeCardForAnimOtherPicked.localScale = Vector3.one;
                    });
                }
            }
        }
    }

    Image timerFinishRing = null;

    public void PlayerSubmitTimer(string socketPacket)
    {
        if (!isPlayerSubmitTimerReceived)
            isPlayerSubmitTimerReceived = true;

        JsonData data = JsonMapper.ToObject(socketPacket);
        //["PLAYER_SUBMIT_TIMMER",{"timmer":29,"playerId":"592"}]
        Debug.Log("PlayerSubmitTimer=" + data[1]);
        string timerSec = data[1]["timmer"].ToString();
        string playerId = data[1]["playerId"].ToString();
        int timerSecInt = int.Parse(timerSec);
        if ((playerId == R_PlayerManager.instance.GetPlayerGameData().userId) && (timerSecInt >= 0) && (!isAutoDropByMe && !isDropByMe && !isMyTurnSkipped))
        {
            if (exitBtn.interactable)
            {
                exitBtn.interactable = false;
            }
            if (!string.IsNullOrEmpty(timerSec))
            {
                int playerIndex = -1;
                int countScript = allPlayersScript.Count();

                if (timerSec == "29")  // || !R_SocketController.instance.isTossedCardReceived  //for reconnect
                {
                    for (int i = 0; i < countScript; i++)
                    {
                        R_PlayerData playerData = allPlayersScript[i].playerData;
                        if (playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            playerIndex = i;
                            Debug.Log("dataCount timer img FOUND playerIndex=" + playerIndex, allPlayersScript[playerIndex].timerBarImg);
                            allPlayersScript[playerIndex].timerBarImg.sprite = redTimerSprite;
                            timerFinishRing = allPlayersScript[playerIndex].timerBarImg;
                        }
                    }

                    PlayerTimerReset();

                    if (!R_SocketController.instance.isTossedCardReceived) //for reconnect
                    {
                        float divideAmtR = (1f / (finishTimer + 0.5f));//1/29 == 0.034
                        timerFinishRing.fillAmount = (float.Parse(timerSec) * divideAmtR);//divideAmt;  //1-0.51=0.49
                    }
                    else
                    {
                        timerFinishRing.fillAmount = 1f;
                    }

                    if (!isClickedSubmit)
                    {
                        submitBtn.SetActive(true);
                        dropBtn.SetActive(false);
                        autoDropToggle.SetActive(false);
                    }
                    Rummy_InGameUiManager.instance.exitButton.interactable = false;
                }
                if (timerSec == "0")
                {
                    PlayerTimerReset();
                }

                if (timerFinishRing != null && (((finishTimer) - 1f) >= float.Parse(timerSec)))//14 >= 15)
                {
                    float divideAmt = (1f / (finishTimer));
                    timerFinishRing.fillAmount = timerFinishRing.fillAmount - divideAmt;
                }
            }
        }
        else
        {
            Debug.Log("PlayerSubmitTimer main else");
        }
    }

    string tempFinishTimerPlayerId = string.Empty;
    // string tempFinishTimerSec = string.Empty;
    public void StartFinishGameTimer(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        string timerSec = data[1].ToString();

        int timerSecInt = int.Parse(timerSec);
        if ((timerSecInt >= 0) && (!isAutoDropByMe && !isDropByMe && !isMyTurnSkipped && ((sittingCount == 2) || (sittingCount > 2 && !isWrongShowByMe))))
        {
            if (exitBtn.interactable)
            {
                exitBtn.interactable = false;
            }

            if (!string.IsNullOrEmpty(timerSec)) // && !isFinishSend)
            {
                int playerIndex = -1;
                // Debug.Log("timerSec: " + timerSec);
                int countScript = allPlayersScript.Count();

                // sound for last 10 seconds
                int timerSecIntForSound = 30;
                if (Int32.TryParse(timerSec, out timerSecIntForSound))
                {
                    if (timerSecIntForSound <= 10)
                    {
                        R_SoundManager.instance.PlaySound(R_SoundType.TIMER_LAST_10_SEC);
                    }
                }
                if (timerSec == finishTimer.ToString())
                {
                    R_SoundManager.instance.PlaySound(R_SoundType.TIMER_ON);
                }

                // if (isWrongShowByMe)
                // {
                //     float divideAmt = (1f / (finishTimer));
                //     float divideAmtR = (1f / (finishTimer+0.5f));//1/29 == 0.034

                //     if ((timerSec==finishTimer.ToString()) || R_SocketController.instance.isReconnectForTimer)
                //     {
                //         PlayerTimerReset();
                //         submitBtn.SetActive(false);
                //         dropBtn.SetActive(false);
                //         Rummy_InGameUiManager.instance.exitButton.interactable = false;
                //     }
                //     // for (int i = 0; i < countScript; i++)
                //     // {
                //     //     R_PlayerData playerData = allPlayersScript[i].playerData;
                //     //     if (playerData.userId!=PlayerManager.instance.GetPlayerGameData().userId)
                //     //     {
                //     //         playerIndex = i;
                //     //         // Debug.Log("dataCount timer img FOUND playerIndex=" + playerIndex, allPlayersScript[playerIndex].timerBarImg);
                //     //         allPlayersScript[playerIndex].timerBarImg.sprite = redTimerSprite;
                //     //         timerFinishRing = allPlayersScript[playerIndex].timerBarImg;

                //     //         if ((timerSec==finishTimer.ToString()) || R_SocketController.instance.isReconnectForTimer)
                //     //         {
                //     //             if (R_SocketController.instance.isReconnectForTimer)
                //     //             {
                //     //                 timerFinishRing.fillAmount = (float.Parse(timerSec) * divideAmtR);//divideAmt;  //1-0.51=0.49
                //     //                 R_SocketController.instance.isReconnectForTimer = false;
                //     //             }
                //     //             else
                //     //             {
                //     //                 timerFinishRing.fillAmount = 1f;
                //     //             }
                //     //         }
                //     //         if (timerFinishRing!=null && (((finishTimer)-1f)>=float.Parse(timerSec)))//14 >= 15)
                //     //         {
                //     //             // Debug.Log("timerRing!=null");

                //     //             // Debug.Log("divideAmt: "+divideAmt);
                //     //             timerFinishRing.fillAmount = timerFinishRing.fillAmount - divideAmt;
                //     //             // Debug.Log("timerRing.fillAmount: "+timerFinishRing.fillAmount);
                //     //             // Debug.Log("Time.deltaTime: "+Time.deltaTime);
                //     //         }
                //     //     }
                //     // }
                //     if (timerSec=="0")
                //     {
                //         // Debug.Log("from 0");
                //         PlayerTimerReset();
                //     }
                // }
                // else
                // {
                if ((timerSec == finishTimer.ToString()) || !R_SocketController.instance.isTossedCardReceived)
                {
                    // Debug.Log("from 15");
                    // tempFinishTimerSec = timerSec;

                    for (int i = 0; i < countScript; i++)
                    {
                        R_PlayerData playerData = allPlayersScript[i].playerData;
                        if (playerData.userId == R_PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            // R_SoundManager.instance.PlaySound(R_SoundType.TIMER_ON);
                            playerIndex = i;
                            // Debug.Log("dataCount timer img FOUND playerIndex=" + playerIndex, allPlayersScript[playerIndex].timerBarImg);
                            allPlayersScript[playerIndex].timerBarImg.sprite = redTimerSprite;
                            timerFinishRing = allPlayersScript[playerIndex].timerBarImg;
                        }
                    }

                    PlayerTimerReset();
                    if (!R_SocketController.instance.isTossedCardReceived)
                    {
                        float divideAmtR = (1f / (finishTimer + 0.5f));//1/29 == 0.034
                        timerFinishRing.fillAmount = (float.Parse(timerSec) * divideAmtR);//divideAmt;  //1-0.51=0.49
                                                                                          // R_SocketController.instance.isReconnectForTimer = false;
                    }
                    else
                    {
                        timerFinishRing.fillAmount = 1f;
                    }
                    Debug.Log(isClickedSubmit + " - " + sittingCount + " - " + isPlayerSubmitTimerReceived);
                    //if (!isClickedSubmit || (isClickedSubmit == true && sittingCount > 2 && isPlayerSubmitTimerReceived))
                    if (!isClickedSubmit)
                    {
                        Debug.Log("SHOW SUBMIT BUTTON");
                        submitBtn.SetActive(true);
                        dropBtn.SetActive(false);
                        autoDropToggle.SetActive(false);
                    }
                    Rummy_InGameUiManager.instance.exitButton.interactable = false;
                }

                if (timerSec == "0")
                {
                    // Debug.Log("from 0");
                    PlayerTimerReset();
                }

                if (timerFinishRing != null && (((finishTimer) - 1f) >= float.Parse(timerSec)))//14 >= 15)
                {
                    // Debug.Log("timerRing!=null");
                    float divideAmt = (1f / (finishTimer));
                    // Debug.Log("divideAmt: "+divideAmt);
                    timerFinishRing.fillAmount = timerFinishRing.fillAmount - divideAmt;
                    // Debug.Log("timerRing.fillAmount: "+timerFinishRing.fillAmount);
                    // Debug.Log("Time.deltaTime: "+Time.deltaTime);
                }
                // }
            }
        }
        else if (timerSecInt < 0)
        {
            int nextDisplaySec = Math.Abs(timerSecInt);
            nextGameTimerText.text = "Next game starts in " + nextDisplaySec + " seconds.";
        }
    }

    public void SetSubmissionMsg(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        Debug.Log("isMyTurnSkipped=" + isMyTurnSkipped);
        Debug.Log("isAutoDropByMe=" + isAutoDropByMe);
        Debug.Log("isDropByMe=" + isDropByMe);
        if (!isAutoDropByMe && !isDropByMe && !isMyTurnSkipped)
        {
            //Rummy_InGameUiManager.instance.ShowTableMessage(data[1]["message"].ToString());
            StartCoroutine(Rummy_InGameUiManager.instance.SetTableText(data[1]["message"].ToString(), 10f));
        }
    }

    public void SetWrongShowMsg(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        // Debug.Log("SetWrongShowMsg: "+data[1]["message"].ToString());
        // Rummy_InGameUiManager.instance.ShowTableMessage(data[1]["message"].ToString());
        //Rummy_InGameUiManager.instance.SetTableText(data[1]["message"].ToString(), 5f);
        Rummy_InGameUiManager.instance.ShowTableMessage(data[1]["message"].ToString());
        // R_CardManager.Instance.SetCardNotInteractable();
        // DisableAllGameControlButtons();
        isWrongShowByMe = true;
        // PlayerTimerReset();
        if (sittingCount > 2)
        {
            DisableControlsAndHideHandCards();
        }
    }

    public void SetResult(string socketPacket)
    {
        Rummy_InGameUiManager.instance.ShowTableMessage(string.Empty);
        JsonData data = JsonMapper.ToObject(socketPacket);
        Debug.Log("SetResult: " + data[1]);
        // string result = data[1].ToString(); // {"459":91,"503":0,"winnerId":"503"}
        int dataCount = data[1].Count; //data[1]["result"].Count;
        // Debug.Log("dataCount: "+dataCount);
        if (dataCount > 0)
        {
            for (int i = 0; i < dataCount; i++)
            {
                IDictionary iResult = data[1][i] as IDictionary;
                if (iResult.Contains("playerId"))
                {
                    Debug.Log("data playerId=" + data[1][i]["playerId"] + ", score=" + data[1][i]["score"]);

                    GameObject resultRow = Instantiate(resultDataRow, resultScrollContent);
                    //resultScrollContent;
                    //resultDataRow;
                    //resultDataRowGroup;
                    //resultDataRowGroupCard;
                    string playerId = data[1][i]["playerId"].ToString();
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
                    string forTrimUsername = data[1][i]["playerName"].ToString();
                    if (forTrimUsername.Length > 8) { forTrimUsername = forTrimUsername.Substring(0, 8) + "..."; }
                    resultRow.transform.GetChild(0).GetComponent<Text>().text = forTrimUsername;
                    // }

                    resultRow.transform.GetChild(1).GetComponent<Text>().text = data[1][i]["status"].ToString();

                    // if (data[1]["winnerId"].ToString() == playerId)
                    // {
                    //     resultRow.transform.GetChild(1).GetComponent<Text>().text = "Winner";
                    // }
                    // else
                    // {
                    //     resultRow.transform.GetChild(1).GetComponent<Text>().text = "Lost";
                    // }
                    /*
                    public Transform resultScrollContent;
                    public GameObject resultDataRow;
                    public GameObject resultDataRowGroup;
                    public GameObject resultDataRowGroupCard;
                    public GameObject resultPanel;
                    public Text nextGameTimerText;
                    [ ["6D","JD","2C","KC"],["2H","4S","AC"],["4D","3S","8S"],["4H","5H","2C"] ]
                    */
                    // Debug.Log("cards: "+data[1][i]["cards"].ToString());
                    // Debug.Log("cards count: "+data[1][i]["cards"].Count);
                    // GameObject row = GameObject.Instantiate(resultDataRowGroup, resultRow.transform.GetChild(2));
                    for (int j = 0; j < data[1][i]["cards"].Count; j++)
                    {
                        if (data[1][i]["cards"].Count == 13)
                        {
                            GameObject group = GameObject.Instantiate(resultDataRowGroup, resultRow.transform.GetChild(2));
                            GameObject card = GameObject.Instantiate(resultDataRowGroupCard, group.transform);
                            card.GetComponent<Image>().sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(data[1][i]["cards"][j].ToString());
                        }
                        else
                        {
                            // Debug.Log("card J: "+data[1][i]["cards"][j]);
                            // Debug.Log("card J: "+data[1][i]["cards"][j].Count);
                            GameObject group = GameObject.Instantiate(resultDataRowGroup, resultRow.transform.GetChild(2));
                            for (int k = 0; k < data[1][i]["cards"][j].Count; k++)
                            {
                                // Debug.Log("card J: "+data[1][i]["cards"][j][k]);
                                GameObject card = GameObject.Instantiate(resultDataRowGroupCard, group.transform);
                                card.GetComponent<Image>().sprite = R_CardManager.Instance.GetSpriteFromSpriteArray(data[1][i]["cards"][j][k].ToString());
                            }
                        }
                    }


                    // resultRow.transform.GetChild(3).GetComponent<Text>().text = data[1][i]["score"].ToString();
                    // Debug.Log("Score = "+data[1][i]["score"].ToString(), resultRow.transform.GetChild(3).GetComponent<Text>());
                    resultRow.transform.GetChild(3).GetComponent<Text>().text = data[1][i]["score"].ToString();
                    resultRow.transform.GetChild(4).GetComponent<Text>().text = data[1][i]["chips"].ToString();
                }
                else if (iResult.Contains("winnerId"))
                {
                    try
                    {
                        if (data[1][i]["winnerId"].ToString() == R_PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            R_SoundManager.instance.PlaySound(R_SoundType.WINNER);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("error in getting winnerId=" + e.Message);
                    }
                    //     for (int j = 0; j < resultScrollContent.childCount; j++)
                    //     {
                    //         if (resultScrollContent.GetChild(j).name == data[1][i]["winnerId"].ToString())
                    //         {
                    //             resultScrollContent.GetChild(j).GetChild(1).GetComponent<Text>().text = "Winner";
                    //         }
                    //         else
                    //         {
                    //             resultScrollContent.GetChild(j).GetChild(1).GetComponent<Text>().text = "Lost";
                    //         }
                    //     }
                }
            }
            resultCutJoker.sprite = R_CardManager.Instance.jokerImg.sprite;
            if (autoDropMsgPanel.activeSelf)
            {
                autoDropMsgPanel.SetActive(false);
            }
            resultPanel.SetActive(true);
            resultAfterWait = new R_Task(R_GlobalGameManager.instance.WaitForDelay(20f));
            resultAfterWait.Finished += delegate (bool manual)
            {
                if (!manual && !isLeaveTableByMe)
                {
                    R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);
                }
                resultAfterWait.Finished -= delegate (bool manual) { };
            };
            if (Rummy_InGameManager.instance.lastGameToggle.GetComponent<Toggle>().isOn)
            {
                Rummy_InGameUiManager.instance.resultTimerText.text = "Game exit in few seconds";
                StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(10f, () =>
                {
                    Rummy_InGameUiManager.instance.OnClickOnBack();
                }));
            }
            //resultMinChipsInfoOnTop.text = R_SocketController.instance.selectedRow["minChips"].ToString();
            resultMinChipsInfoOnTop.text = R_SocketController.instance.selectedRow["entry_fee"].ToString();
        }
    }

    public void SetGameStartTimer(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        Debug.Log("SetResult: " + data[1]);
        if (resultPanel.activeInHierarchy)
        {
            Rummy_InGameUiManager.instance.resultTimerText.text = "Next game starts in " + data[1].ToString() + " seconds.";
        }
        else
        {
            if (data[1].ToString() == "1") //data[1].ToString() == "0" || 
            {
                StartCoroutine(Rummy_InGameUiManager.instance.SetTableText("Game starts in " + data[1].ToString() + " second.", 1f, () =>
                {
                    Rummy_InGameUiManager.instance.ShowTableMessage("Please wait while game starts");
                }));
                StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(5f, () =>
                {
                    if (!Rummy_InGameManager.instance.isSeatingReceived)
                    {
                        Rummy_InGameUiManager.instance.exitButton.interactable = true;
                    }
                }));
            }
            else
            {
                Rummy_InGameUiManager.instance.ShowTableMessage("Game starts in " + data[1].ToString() + " seconds.");
            }
        }
    }
    #endregion //socket_response


    void SetAvatarImages(string url, Image img, int index)
    {
        // R_WebServices public IEnumerator LoadImageFromUrl(string URL, Image image) 
        Debug.Log("SetAvatarImages coroutine starts");
        R_Task coSetPlayerImage;

        coSetPlayerImage = new R_Task(R_WebServices.instance.LoadImageFromUrl(url, img));
        coSetPlayerImage.Finished += delegate (bool manual)
        {
            if (!manual)
            {
                Debug.Log("Image set for playerIndex=" + index);
                coSetPlayerImage.Finished -= delegate (bool manual) { };
            }
        };
    }


    public void ShowConfirmationPanel(GameObject card)
    {
        confirmationPanel.SetActive(true);

        yesConfBtn.onClick.RemoveAllListeners();
        reCheckConfBtn.onClick.RemoveAllListeners();

        yesConfBtn.onClick.AddListener(() => { OnYesConfBtnClick(card); });
        reCheckConfBtn.onClick.AddListener(() => { OnReCheckConfBtnClick(card); });
    }

    void OnYesConfBtnClick(GameObject card)
    {
        card.GetComponent<R_CardView>().FinishConfirm();
        confirmationPanel.SetActive(false);
    }

    void OnReCheckConfBtnClick(GameObject card)
    {
        // if(R_CardManager.Instance.handCards.GetChild(card.GetComponent<R_CardView>().groupIndexForConfirm).childCount==2)
        // {
        //     Transform groupNew = R_CardManager.Instance.AddGroup();
        //     groupNew.SetSiblingIndex(card.GetComponent<R_CardView>().groupIndexForConfirm);
        //     card.transform.SetParent(groupNew);
        // }
        // else
        // {
        card.transform.SetParent(R_CardManager.Instance.handCards.GetChild(card.GetComponent<R_CardView>().groupIndexForConfirm));
        card.transform.SetSiblingIndex(card.GetComponent<R_CardView>().cardIndexForConfirm);
        // }
        confirmationPanel.SetActive(false);
    }

    void HideFirstCards()
    {
        int countScript = allPlayersScript.Count();
        for (int k = 0; k < countScript; k++)
        {
            allPlayersScript[k].firstCardImage.gameObject.SetActive(false);
        }
    }

    void PlayerTimerReset()
    {
        for (int i = 0; i < allPlayersScript.Length; i++)
        {
            allPlayersScript[i].timerBarImg.fillAmount = 0f;
        }
    }

    void DisableAllGameControlButtons()
    {
        sortingBtn.SetActive(false);
        dropBtn.SetActive(false);
        submitBtn.SetActive(false);
        lastGameToggle.SetActive(false);
        autoDropToggle.SetActive(false);
        R_CardManager.Instance.group_Btn.SetActive(false);
        R_CardManager.Instance.discard_Btn.SetActive(false);
        R_CardManager.Instance.showArea.GetComponent<BoxCollider2D>().enabled = false;
        R_CardManager.Instance.dropArea.GetComponent<Button>().enabled = false;
        R_CardManager.Instance.takecardbtn.GetComponent<Button>().enabled = false;
    }

    public void LoadMainMenu()
    {
        R_GlobalGameManager.instance.LoadScene(R_Scenes.MainMenuScene);
    }

    void OnApplicationQuit()
    {
        Debug.LogError("OnApplicationQuitOnApplicationQuitOnApplicationQuit");
        // R_SocketController.instance.SendLeaveMatchRequest();
        // R_SocketController.instance.SendDrop();
        R_SocketController.instance.ResetConnection();
    }

    void SaveScreenshot()
    {
        //Taking Screenshot
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Screenshots")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Screenshots"));

        byte[] byteArray = screenshot.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "Screenshots", "Image_" + tableValue + "_" + cardValue + date + "_" + time + ".png"), byteArray);

        Debug.Log("Saved Screenshot successfully...");
        isScreenshotCaptured = false;

        //Delete Extra Files
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] fileInfo = dirInfo.GetFiles("*.png");
        for (int j = 0; j < fileInfo.Length; j++)
        {
            File.Delete(fileInfo[j].FullName);
        }
    }
}

public enum R_SUITS
{
    SPAIDS,
    CLUBS,
    HEARTS,
    DIAMONDS,
    JOKER
}
public enum R_ShiftDirectionEnum
{
    LEFT,
    RIGHT,
    NONE
}

public class Rummy_MatchMakingPlayerData
{
    public R_PlayerData playerData;
    public bool isTurn;
    public bool isCheckAvailable;
    public string playerType;
}

//[System.Serializable]
//public class Rummy_TableSeats
//{
//    public bool status;
//    public string message;
//    public Seat[] data;
//}

//[System.Serializable]
//public class Rummy_Seat
//{
//    public int userId;
//    public string seatNo;
//}

//[System.Serializable]
//public class Rummy_MyBetData
//{
//    public int userId;
//    public int bet;
//    public int lastBet;
//    public int pot;
//    public List<Pot> sidePot = new List<Pot>();
//}

//[System.Serializable]
//public class Rummy_Pot
//{
//    public int amount;
//}

//[System.Serializable]
//public class Rummy_WinnerObject
//{
//    public int userId;
//    public string userName;
//    public int winAmount;
//    public bool isWin;
//    public int betAmount;
//    public string name;
//}

//[System.Serializable]
//public class Rummy_ShowdownSidePot
//{
//    public int amount;
//    public List<WinnerObject> users = new List<WinnerObject>();
//    public List<WinnerObject> winners = new List<WinnerObject>();
//}

//[System.Serializable]
//public class Rummy_AllShowdownSidePots
//{
//    public List<ShowdownSidePot> sidePot = new List<ShowdownSidePot>();
//}





//[System.Serializable]
//public class Seat
//{
//    public int userId;
//    public string seatNo;
//}

//[System.Serializable]
//public class Pot
//{
//    public int amount;
//}

//[System.Serializable]
//public class WinnerObject
//{
//    public int userId;
//    public string userName;
//    public int winAmount;
//    public bool isWin;
//    public int betAmount;
//    public string name;
//}

//[System.Serializable]
//public class ShowdownSidePot
//{
//    public int amount;
//    public List<WinnerObject> users = new List<WinnerObject>();
//    public List<WinnerObject> winners = new List<WinnerObject>();
//}
