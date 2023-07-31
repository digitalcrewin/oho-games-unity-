using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using System.Collections;
using BestHTTP.JSON;
using LitJson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using PlatformSupport.Collections.ObjectModel;

public class R_SocketController : MonoBehaviour
{
    public static R_SocketController instance;

    private const float RESPONSE_READ_DELAY = 0.2f, REQUEST_SEND_DELAY = 0.1f;
    private SocketManager socketManager;

    private List<R_SocketResponse> socketResponse = new List<R_SocketResponse>();
    private List<R_SocketRequest> socketRequest = new List<R_SocketRequest>();

    [SerializeField]
    private R_SocketState socketState;

    [SerializeField]
    private string TABLE_ID = "";

    public string rummyUserId = string.Empty;
    public string rummyGameType = string.Empty;
    public JsonData selectedRow = null;

    public bool isTossedCardReceived = false;

    void OnEnable()
    {
        if (instance==null)
            instance = this;
    }

    void Awake()
    {
        // instance = this;
        SetSocketState(R_SocketState.NULL);
    }

    public void ReStart()
    {
        SetSocketState(R_SocketState.NULL);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
Debug.Log("SOCKET ReStart()");
        Connect();
        StartCoroutine(WaitAndCheckInternetConnection());
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        if (GlobalGameManager.IsJoiningPreviousGame)
        {
            TABLE_ID = GlobalGameManager.instance.GetRoomData().socketTableId;
        }
Debug.Log("SOCKET START");
        Connect();
        StartCoroutine(WaitAndCheckInternetConnection());

        R_GlobalGameManager.instance.isReJoinGame = true;
    }

    public void Connect(bool isReconnecting = false)
    {
        ResetConnection(isReconnecting);
        SetSocketState(R_SocketState.Connecting);

        if (!IsInvoking("HandleSocketResponse"))
        {
            InvokeRepeating("HandleSocketResponse", RESPONSE_READ_DELAY, RESPONSE_READ_DELAY);
        }

        if (!IsInvoking("SendSocketRequest"))
        {
            InvokeRepeating("SendSocketRequest", REQUEST_SEND_DELAY, REQUEST_SEND_DELAY);
        }

        ReConnect();
    }


    private void ReConnect()
    {
        socketManager = null;
        SocketOptions socketOptions = new SocketOptions();
        socketOptions.Timeout = new TimeSpan(0, 0, 4);
        socketOptions.Reconnection = false;
        socketOptions.AutoConnect = false;
        socketOptions.ReconnectionDelayMax = new TimeSpan(0, 0, 4);
        socketOptions.AdditionalQueryParams = new ObservableDictionary<string, string>();
        //socketOptions.AdditionalQueryParams.Add("userId", "4");
        socketOptions.AdditionalQueryParams.Add("id", rummyUserId);

        // if (R_GameConstants.isSeparateGame)
        //     socketManager = new SocketManager(new Uri(R_GameConstants.SOCKET_URL + "/socket.io/?id=" + R_PlayerManager.instance.GetPlayerGameData().userId), socketOptions);
        // else
        //     socketManager = new SocketManager(new Uri(R_GameConstants.SOCKET_URL + "/socket.io/?id=" + PlayerManager.instance.GetPlayerGameData().userId), socketOptions);
        socketManager = new SocketManager(new Uri(R_GameConstants.SOCKET_URL + "/socket.io/"), socketOptions);

Debug.LogError("Socket URL IS " + socketManager.Uri);

        socketManager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
        socketManager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
        socketManager.Socket.On(SocketIOEventTypes.Error, OnError);

        //Default Events
        socketManager.Socket.On("reconnect", OnReconnect);
        socketManager.Socket.On("reconnecting", OnReconnecting);
        socketManager.Socket.On("reconnect_attempt", OnReconnectAttempt);
        socketManager.Socket.On("reconnect_failed", OnReconnectFailed);

        socketManager.Socket.On(R_SocketEvetns.personal.ToString(), OnPersonal);
        socketManager.Socket.On(R_SocketEvetns.time.ToString(), OnTime);
        // socketManager.Socket.On(R_SocketEvetns.player_timer.ToString(), OnPlayerTimer);
        // socketManager.Socket.On(R_SocketEvetns.RANDOM_CARDS.ToString(), OnRandomCards);
        socketManager.Socket.On(R_SocketEvetns.SEATING.ToString(), OnSeating);
        socketManager.Socket.On(R_SocketEvetns.TOSSED_CARDS.ToString(), OnTossedCards);
        socketManager.Socket.On(R_SocketEvetns.FIRST_PLAYER.ToString(), OnFirstPlayer);
        socketManager.Socket.On(R_SocketEvetns.HAND_CARDS.ToString(), OnHandCards);
        socketManager.Socket.On(R_SocketEvetns.DECK_INFO.ToString(), OnDeckInfo);
        socketManager.Socket.On(R_SocketEvetns.DISCARDED_CARD.ToString(), OnDiscardedCard);
        socketManager.Socket.On(R_SocketEvetns.PLAYER_TIMMER.ToString(), OnPlayerTimmer);
        socketManager.Socket.On(R_SocketEvetns.CARD_PICKED.ToString(), OnCardPicked);
        socketManager.Socket.On(R_SocketEvetns.OPPONENT_CARD_PICKED.ToString(), OnOpponentCardPicked);
        socketManager.Socket.On(R_SocketEvetns.ARRANGED_CARDS.ToString(), OnArrangedCards);
        socketManager.Socket.On(R_SocketEvetns.PLAYER_SUBMIT_TIMMER.ToString(), OnPlayerSubmitTimer);
        // socketManager.Socket.On(R_SocketEvetns.FINISH_GAME_TIMER.ToString(), OnFinishGameTimer);
        socketManager.Socket.On("FINISH_GAME_TIMER", OnFinishGameTimer);
        socketManager.Socket.On(R_SocketEvetns.SUBMISSION.ToString(), OnSubmission);
        socketManager.Socket.On(R_SocketEvetns.WRONG_SHOW.ToString(), OnWrongShow);
        socketManager.Socket.On(R_SocketEvetns.RESULT.ToString(), OnResult);
        socketManager.Socket.On(R_SocketEvetns.GAME_EXIT.ToString(), OnGameExit);
        socketManager.Socket.On(R_SocketEvetns.DISCARDED_HISTORY.ToString(), OnDiscardedHistory);
        socketManager.Socket.On(R_SocketEvetns.GAME_HISTORY.ToString(), OnGameHistory);
        socketManager.Socket.On(R_SocketEvetns.CHAT_MESSAGE.ToString(), OnChatMessage);
        socketManager.Socket.On(R_SocketEvetns.GAME_START_TIMER.ToString(), OnGameStartTimer);  //after result event
        socketManager.Socket.On(R_SocketEvetns.SCORE_BOARD.ToString(), OnScoreBoard);  //after result event

        //New Events
        socketManager.Socket.On(R_SocketEvetns.STARTING_GAME.ToString(), OnStartingGame);
        socketManager.Socket.On(R_SocketEvetns.PLAYER_DETAILS.ToString(), OnPlayerDetails);
        socketManager.Socket.On(R_SocketEvetns.INSUFFICIENT_CHIPS.ToString(), OnInsufficientChips);
        socketManager.Socket.On(R_SocketEvetns.EXITED_NEXT_ROUND.ToString(), OnExitedNextRound);
        socketManager.Socket.On(R_SocketEvetns.PLAYER_EXIT.ToString(), OnPlayerExit);

        socketManager.Open();
    }

    private void HandleSocketResponse()
    {
        if (socketResponse.Count > 0)
        {
            R_SocketResponse responseObject = socketResponse[0];
            socketResponse.RemoveAt(0);

#if DEBUG

#if UNITY_EDITOR
            //if (GlobalGameManager.instance.CanDebugThis(responseObject.eventType))
            //{
            //Debug.LogError("---------------Handling ServerResponse = " + responseObject.eventType + "  socketState = " + socketState + "   data = " + responseObject.data);
            //}
#else
            Debug.LogError("Handling ServerResponse = " + responseObject.eventType + "  socketState = " + socketState + "   data = " + responseObject.data);
#endif

#endif
            Debug.Log(gameObject.transform.parent.name + " <color=yellow> " + responseObject.eventType + ",</color> " + responseObject.data);
            switch (responseObject.eventType)
            {
                case R_SocketEvetns.CONNECT:
                    {
                        switch (GetSocketState())
                        {
                            case R_SocketState.Connecting:
                                SetSocketState(R_SocketState.WaitingForOpponent);
                                Debug.Log(gameObject.transform.parent.name + "<color=yellow>IsJoiningPreviousGame " + R_GlobalGameManager.instance.IsJoiningPreviousGame + "</color>");
                                if (R_GlobalGameManager.instance.IsJoiningPreviousGame)
                                {
                                    RequestForMatchStatus();
                                }
                                else
                                {
                                    SendGameJoinRequest();
                                }
                                break;

                            case R_SocketState.ReConnecting:
                                RequestForMatchStatus();
                                break;

                            default:
                                break;

                        }
                    }
                    break;

                case R_SocketEvetns.DISCONNECT:
                    StartReconnectProcedure();
                    break;

                case R_SocketEvetns.RECONNECT_ATTEMPT:
                    break;


                case R_SocketEvetns.personal:
                    Debug.Log("HandleSocketResponse: personal");
                    break;

                // case R_SocketEvetns.time:
                //     Debug.Log("HandleSocketResponse: time");
                //     break;

                // case R_SocketEvetns.player_timer:
                //     Debug.Log("HandleSocketResponse: player_timer");
                //     break;

                // case R_SocketEvetns.RANDOM_CARDS:
                //     Debug.Log("HandleSocketResponse: RANDOM_CARDS");
                //     break;

                case R_SocketEvetns.TOSSED_CARDS:
                    Debug.Log("HandleSocketResponse: TOSSED_CARDS");
                    break;

                case R_SocketEvetns.FIRST_PLAYER:
                    Debug.Log("HandleSocketResponse: FIRST_PLAYER");
                    break;

                case R_SocketEvetns.HAND_CARDS:
                    Debug.Log("HandleSocketResponse: HAND_CARDS");
                    break;


                default:
                    Debug.LogError("UnHandlled EventType Found in response eventType = " + responseObject.eventType + "   responseStructure = " + responseObject.data);
                    break;
            }
         //Debug.LogError("responseObject.data***********" + responseObject.data);
        }
    }

    private void SendSocketRequest()
    {
        if (socketRequest.Count > 0)
        {
            R_SocketRequest request = socketRequest[0];
            socketRequest.RemoveAt(0);

            if (request.plainDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.plainDataToBeSend);

#if DEBUG
                Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color> :  sending Plain request " + request.requestDataStructure + "   Time = " + System.DateTime.Now);
#endif
            }
            else if (request.jsonDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.jsonDataToBeSend);

#if DEBUG
                Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color> :  Send Socket Request " + request.requestDataStructure + "  Time = " + System.DateTime.Now);
#endif
            }
        }
    }










// LISTNER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------
 #region LISTNER_METHODS
    void OnServerConnect(Socket socket, Packet packet, params object[] args)
    {

#if DEBUG

#if UNITY_EDITOR
        if (R_GlobalGameManager.instance.CanDebugThis(R_SocketEvetns.CONNECT))
        {
            Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
#endif

#endif

        R_SocketResponse response = new R_SocketResponse();
        response.eventType = R_SocketEvetns.CONNECT;
        socketResponse.Add(response);
    }

    void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
    {
        //string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (R_GlobalGameManager.instance.CanDebugThis(R_SocketEvetns.DISCONNECT))
        {
            //Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
#endif
#endif
        R_SocketResponse response = new R_SocketResponse();
        response.eventType = R_SocketEvetns.DISCONNECT;
        socketResponse.Add(response);
        // isTossedCardReceived = false;
    }

    void OnError(Socket socket, Packet packet, params object[] args)
    {
        // if (string.IsNullOrEmpty(TABLE_ID)) //first time connecting
        // {
        //     ReConnect();
        // }

#if DEBUG

        Error error = args[0] as Error;
        switch (error.Code)
        {
            case SocketIOErrors.User:
                Debug.LogError("Exception in an event handler! Time = " + System.DateTime.Now);
                break;
            case SocketIOErrors.Internal:
                Debug.LogError("Internal error! Time = " + System.DateTime.Now);
                break;
            default:
                Debug.LogError("server error! Time = " + System.DateTime.Now);
                break;
        }
#endif

    }

    void OnReconnect(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG
        Debug.Log("Reconnected Time = " + System.DateTime.Now);
#endif
    }

    void OnReconnecting(Socket socket, Packet packet, params object[] args)
    {

#if DEBUG
        Debug.Log("Reconnecting Time = " + System.DateTime.Now);
#endif
    }

    void OnReconnectAttempt(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG

#if UNITY_EDITOR
        if (R_GlobalGameManager.instance.CanDebugThis(R_SocketEvetns.RECONNECT_ATTEMPT))
        {
            Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
#endif

#endif

        R_SocketResponse response = new R_SocketResponse();
        response.eventType = R_SocketEvetns.RECONNECT_ATTEMPT;
        socketResponse.Add(response);
    }

    void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG
        Debug.Log("ReconnectFailed Time = " + System.DateTime.Now);
#endif
    }

    void OnPersonal(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">personal</color> packet = " + packet);
        // ["personal","room Joined 1652440585435"]
    }

    void OnTime(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnTime packet = " + packet);
    }

    // void OnPlayerTimer(Socket socket, Packet packet, params object[] args)
    // {
    //     Debug.Log("OnPlayerTimer packet = " + packet);
    // }

    // void OnRandomCards(Socket socket, Packet packet, params object[] args)
    // {
    //     Debug.Log("OnRandomCards packet = " + packet);
    // }

    void OnSeating(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">SEATING</color> packet = " + packet);
        // ["SEATING",{"seats":["501","459"],"turnTimer":30,"finishTimer":15}]
        // if (R_GlobalGameManager.instance.IsJoiningPreviousGame)
        // {
        //     R_GlobalGameManager.instance.ResetGame(packet.ToString());
        // }
        // else
        // {
            Rummy_InGameManager.instance.OnSetSeating(packet.ToString());
        // }
    }

    void OnTossedCards(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">TOSSED_CARDS</color> packet = " + packet);
        //["TOSSED_CARDS",[{"playerId":"502","card":"JJ","rank":0},{"playerId":"503","card":"AH","rank":1}]]
        
        // set for 2 players only
        // if (R_CardManager.Instance != null)
        // {
        //     R_CardManager.Instance.SetTossedCards(packet.ToString());
        // }
        // set for multiplayer
    
        Rummy_InGameManager.instance.SetTossedCardsMultiPlayer(packet.ToString());
        isTossedCardReceived = true;
    }

public string firstPlayerData = string.Empty;
    void OnFirstPlayer(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">FIRST_PLAYER</color> packet = " + packet);
        //["FIRST_PLAYER",{"playerId":"502","card":"JJ","rank":0}]
         Debug.Log(packet);
         firstPlayerData = packet.ToString();
        //Rummy_InGameManager.instance.SetFirstPlayer(packet.ToString());
    }

    // public void SetFirstPlayerAfterTossedCards()
    // {
    //     if ((R_CardManager.Instance != null) && (!string.IsNullOrEmpty(firstPlayerData)))
    //     {
    //         R_CardManager.Instance.SetFirstPlayer(firstPlayerData);
    //     }
    // }

 //public string handCardsData = string.Empty;
    void OnHandCards(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">HAND_CARDS</color> packet = " + packet);
        //["HAND_CARDS",["3H","5D","5C","QH","7S","9S","6C","4D","AD","JH","KH","9C","7D"]]
        // handCardsData = packet.ToString();
        R_CardManager.Instance.SetHandCards(packet.ToString());
    }

    // public void SetHandCardsAfterFirstPlayer()
    // {
    //     if ((R_CardManager.Instance != null) && (!string.IsNullOrEmpty(handCardsData)))
    //     {
    //         R_CardManager.Instance.SetHandCards(handCardsData);
    //     }
    // }

// public string deckInfoData = string.Empty;
    void OnDeckInfo(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">DECK_INFO</color> packet = " + packet);
        // side joker card
        // ["DECK_INFO",["2S"]]
        // deckInfoData = packet.ToString();
        R_CardManager.Instance.SetDeckInfo(packet.ToString());
    }

public string discardedCardData = string.Empty;
    void OnDiscardedCard(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">DISCARDED_CARD</color> packet = " + packet);
        // for open card deck
        //["DISCARDED_CARD","8H"]
        //["DISCARDED_CARD",{"cardValue":"9D","playerId":"503"}]
        discardedCardData = packet.ToString();
        R_CardManager.Instance.SetDiscardedCardInfo(discardedCardData);
    }

    void OnPlayerTimmer(Socket socket, Packet packet, params object[] args)
    {
        Debug.LogWarning("<color=\"#F45B06\">PLAYER_TIMMER</color> packet = " + packet);
        // ["PLAYER_TIMMER",{"timmer":9,"playerId":"501"}]
        // timmer 45 to 0
        Rummy_InGameManager.instance.StartPlayerTimer(packet.ToString());
    }

    void OnCardPicked(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">CARD_PICKED</color> packet = " + packet);
        // ["CARD_PICKED",["QC"]]
        if (R_CardManager.Instance != null)
        {
            R_CardManager.Instance.SetCardPicked(packet.ToString());
        }
    }
    void OnOpponentCardPicked(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OPPONENT_CARD_PICKED</color> packet = " + packet);
        // ["OPPONENT_CARD_PICKED",501]
        //["OPPONENT_CARD_PICKED",{    "playerId": "502",    "from": "discardedPile"}]
        //["OPPONENT_CARD_PICKED",{    "playerId": "502",    "from": "deck"}]
        //["OPPONENT_CARD_PICKED",{"playerId":"459","from":"discardedPile","nextCardOnTop":"QD"}]
        Rummy_InGameManager.instance.AnimOpponentCardPicked(packet.ToString());
    }

    void OnArrangedCards(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">ARRANGED_CARDS</color> packet = " + packet);
        // 
        
    }

    void OnPlayerSubmitTimer(Socket socket, Packet packet, params object[] args)  
    {
        Debug.Log("<color=\"#F45B06\">PLAYER_SUBMIT_TIMMER</color> packet = " + packet);
        // ["PLAYER_SUBMIT_TIMMER",{"timmer":29,"playerId":"592"}]
        // timmer 29 to 0
        Rummy_InGameManager.instance.PlayerSubmitTimer(packet.ToString());
    }
    void OnFinishGameTimer(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#F45B06\">FINISH_GAME_TIMER</color> packet = " + packet);
        // ["FINISH_GAME_TIMER",15]
        // timmer 15 to 0
        Rummy_InGameManager.instance.StartFinishGameTimer(packet.ToString());
    }

    void OnSubmission(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnSubmission</color> packet = " + packet);
        // ["WRONG_SHOW",{"message": "2 has submitted the cards, please group you cards and submit"}]
        Rummy_InGameManager.instance.SetSubmissionMsg(packet.ToString());
    }

    void OnWrongShow(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnWrongShow</color> packet = " + packet);
        // ["WRONG_SHOW",{"message":"you have placed a wrong show 80 points will be deducted"}]
        Rummy_InGameManager.instance.SetWrongShowMsg(packet.ToString());
    }

    void OnResult(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnResult</color> packet = " + packet);
        // ["RESULT",{"459":91,"503":0,"winnerId":"503"}]
        //["RESULT",{"result": [{"playerId": "503","score": 70},{"playerId": "504","score": 70}],"winnerId": "503"}]
        //[{"playerId":"501","status":"WINNER","score":0,"cards":[["2D","3D","4D","5D"],["2S","2C","2C"],["8H","9H","10H"],["10S","JS","QS"]]},{"playerId":"502","status":"LOSER","score":0,"cards":[["6D","7D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"winnerId":"501"}]
        //["RESULT",[{"playerId":"600","playerName":"v3.ab","status":"WINNER","score":0,"chips":800,"cards":[["4C","5C","6C"],["6D","7D","8D"],["JJ","QD","QC"],["JH","JD","JS","JC"]]},{"playerId":"616","playerName":"v4.ab9561","status":"LOSER","score":80,"chips":10,"cards":[["5D"],["9S","QS","KS"],["4H","5H"],["3D"],["3H","2D","10D","7C","8C","9C"]]},{"winnerId":"600"}]]
        //"status":"WINNER" "status":"WRONG SHOW" "status":"LOSER"
        Rummy_InGameManager.instance.SetResult(packet.ToString());
    }

    void OnGameExit(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnGameExit</color> packet = " + packet);
        // ["GAME_EXIT"]
        ResetConnection();

        if (Rummy_InGameManager.instance.resultAfterWait!=null)
            Rummy_InGameManager.instance.resultAfterWait.Stop();

        R_SocketController.instance.rummyUserId = string.Empty;
        R_SocketController.instance.rummyGameType = string.Empty;
        R_SocketController.instance.discardedCardData = string.Empty;
        R_SocketController.instance = null;
        R_GlobalGameManager.instance.mainSocketController.GetComponent<R_SocketController>().enabled = false;
        R_GlobalGameManager.instance.IsJoiningPreviousGame = false;
        R_GlobalGameManager.instance.DestroyScene(R_Scenes.InGame);
        R_GlobalGameManager.instance.isReJoinGame = true;

        Rummy_InGameManager.instance.LoadMainMenu();
    }

    void OnDiscardedHistory(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnDiscardedHistory</color> packet = " + packet);
        //["DISCARDED_HISTORY",["6D","10S","KH","4D"]]
        Rummy_InGameUiManager.instance.SetDiscardedHistory(packet.ToString());
    }

    void OnGameHistory(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnGameHistory</color> packet = " + packet);
        //[[{"playerId":"501","score":0,"cards":[["6D","7D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"playerId":"502","score":32,"cards":[["7D","8D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"winnerId":"501"}],[{"playerId":"501","score":0,"cards":[["6D","7D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"playerId":"502","score":32,"cards":[["7D","8D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"winnerId":"501"}],[{"playerId":"501","score":0,"cards":[["6D","7D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"playerId":"502","score":32,"cards":[["7D","8D","8D","9D"],["4D","4S","4C"],["3S","4S","5S"],["JJ","4H","5H"]]},{"winnerId":"501"}]]
        Rummy_InGameUiManager.instance.SetGameHistory(packet.ToString());
    }

    void OnChatMessage(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnChatMessage</color> packet = " + packet);
        Rummy_InGameUiManager.instance.SetChat(packet.ToString());
    }

    void OnGameStartTimer(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnGameStartTimer</color> packet = " + packet);
        Rummy_InGameManager.instance.SetGameStartTimer(packet.ToString());
    }

    void OnScoreBoard(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnScoreBoard</color> packet = " + packet);
        Rummy_InGameUiManager.instance.SetScoreBoard(packet.ToString());
    }

    //New Methods
    void OnStartingGame(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnStartingGame</color> packet = " + packet);
        JsonData data = JsonMapper.ToObject(packet.ToString());
        Debug.Log(data[0] + "<color=\"#FFFF00\">OnStartingGame</color> packet = " + data[1]);
        //Rummy_InGameUiManager.instance.ShowTableMessage("Game starts in " + data[1].ToString() + " second.");
    }

    void OnPlayerDetails(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnPlayerDetails</color> packet = " + packet);
    }
    
    void OnInsufficientChips(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnInsufficientChips</color> packet = " + packet);
    }
    
    void OnExitedNextRound(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnExitedNextRound</color> packet = " + packet);
        Rummy_InGameUiManager.instance.ResetSocketAndLoadMenu();
    }
    
    void OnPlayerExit(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("<color=\"#FFFF00\">OnPlayerExit</color> packet = " + packet);
        Rummy_InGameUiManager.instance.ResetSocketAndLoadMenu();
    }

    #endregion










    // EMIT_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------
    #region EMIT_METHODS
    public void SendGameJoinRequest(int seatNo = 0)
    {
        string requestStringData = string.Empty;
        // if (R_GameConstants.isSeparateGame)
        //     requestStringData = R_PlayerManager.instance.GetPlayerGameData().userId;
        // else
        //     requestStringData = PlayerManager.instance.GetPlayerGameData().userId;=
        requestStringData = "{\"gameId\":\"" + rummyGameType + "\"}"; //rummyUserId; //\"id\":"+rummyUserId+",
        Debug.Log("<color=\"#52da59\">"+R_SocketEvetns.join.ToString() + "</color> : " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        socketManager.Socket.Emit(R_SocketEvetns.join.ToString(), requestObjectData);
        isTossedCardReceived = false;

        StartCoroutine(R_GlobalGameManager.instance.RunAfterDelay(10f, () => {
            if(!Rummy_InGameManager.instance.isSeatingReceived)
            {
                Rummy_InGameUiManager.instance.exitButton.interactable = true;
            }
        }));

        // R_SocketRequest request = new R_SocketRequest();
        // request.emitEvent = R_SocketEvetns.join.ToString();
        // Debug.LogError("<color=\"#52da59\">"+request.emitEvent + "</color> : " + requestStringData);
        // request.plainDataToBeSend = requestStringData;
        // request.jsonDataToBeSend = null;
        // request.requestDataStructure = requestStringData;
        // socketRequest.Add(request);
    }

    public void SendFromDeck()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.FROM_DECK.ToString();
        request.plainDataToBeSend = string.Empty;
        request.jsonDataToBeSend = null;
        request.requestDataStructure = string.Empty;
        socketRequest.Add(request);
    }

    public void SendFromDiscarded()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.FROM_DISCARDED.ToString();
        request.plainDataToBeSend = string.Empty;
        request.jsonDataToBeSend = null;
        request.requestDataStructure = string.Empty;
        socketRequest.Add(request);
    }

    public void SendDiscard(string cardName)
    {
        string requestStringData = "{\"cardValue\":\""+cardName+"\"}";
        object requestObjectData = Json.Decode(requestStringData);
        
        Debug.Log("<color=\"#52da59\">"+R_SocketEvetns.DISCARD.ToString() + "</color> "+requestStringData);

        // socketManager.Socket.Emit(R_SocketEvetns.DISCARD.ToString(), OnSendDiscardCallback, requestObjectData);
        socketManager.Socket.Emit(R_SocketEvetns.DISCARD.ToString(), requestObjectData);
    }
    void OnSendDiscardCallback(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnAckCallback! socket=" + socket);
        Debug.Log("OnAckCallback! originalPacket=" + packet);
        //Debug.Log(string.Format("OnAckCallback from {0}", args[0]));
        // JsonData userData = JsonMapper.ToObject(packet.ToString());
        // Debug.Log(userData[0]["status"]);
    }

    public void SendDrop()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.DROP.ToString();

        Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color>");

        // request.plainDataToBeSend = string.Empty;
        // request.jsonDataToBeSend = null;
        // request.requestDataStructure = string.Empty;
        // socketRequest.Add(request);
        socketManager.Socket.Emit(R_SocketEvetns.DROP.ToString());
    }

    public void SendFinishCards(string data)
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.FINISH.ToString();
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = data;
        socketRequest.Add(request);
    }

    public void SendArrangedCards(string data)
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.ARRANGEMENT.ToString();
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = data;
        socketRequest.Add(request);
    }

    public void SendDiscardedHistory(string userId = null)
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.DISCARDED_HISTORY.ToString();

        string data;
        if (!string.IsNullOrEmpty(userId))
        {
            data = "{\"id\":"+userId+"}";
        }
        else
        {
            data = "{\"id\":"+R_PlayerManager.instance.GetPlayerGameData().userId+"}";
        }
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = data;
        socketRequest.Add(request);
    }

    public void SendGameHistory()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.GAME_HISTORY.ToString();
        Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color>");
        string data = "{}";
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = data;
        socketRequest.Add(request);
    }

    public void SendScoreBoard()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.SCORE_BOARD.ToString();
        Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color>");
        string data = "{\"id\":"+R_PlayerManager.instance.GetPlayerGameData().userId+"}";
        object requestObjectData = Json.Decode(data);
        Debug.Log("<color=\"#52da59\">"+R_SocketEvetns.SCORE_BOARD.ToString() + "</color> "+requestObjectData);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = data;
        socketRequest.Add(request);
    }

    public void SendChatMessage(string msg)
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.CHAT_MESSAGE.ToString();
        string data = "{\"playerId\": "+R_PlayerManager.instance.GetPlayerGameData().userId+",\"message\":\""+msg+"\"}";
        Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color> "+data);
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = data;
        socketRequest.Add(request);
    }

    public void SendExitNextRound(string status)
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.EXIT_NEXT_ROUND.ToString();
        string data = "{\"IsConnected\":"+status+"}";
        Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color> "+data);
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = data;
        socketRequest.Add(request);
    }

    public void SendAutoDrop()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.AUTO_DROP.ToString();
        string data = "{}";
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = null;
        socketRequest.Add(request);
    }

    public void SendExitGame()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.EXIT_GAME.ToString();
        //R_SocketController.instance.rummyUserId = string.Empty;
        //R_SocketController.instance.rummyGameType
        //string data = "{\"roomId\":\"" + rummyUserId + "\", \"gameType\":\"" + rummyGameType + "\"}";
        //Debug.Log("<color=\"#52da59\">"+request.emitEvent + "</color> "+data);
        string data = "{}";
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = null;
        socketRequest.Add(request);
    }

    public void SendLeaveTable()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.leave_Table.ToString();
        //R_SocketController.instance.rummyUserId = string.Empty;
        //R_SocketController.instance.rummyGameType
        //string data = "{\"roomId\":\"" + rummyUserId + "\", \"gameType\":\"" + rummyGameType + "\"}";
        //Debug.Log("<color=\"#52da59\">" + request.emitEvent + "</color> " + data);
        string data = "{}";
        object requestObjectData = Json.Decode(data);
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = null;
        socketRequest.Add(request);
    }

    public void SendDisconnect()
    {
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.DISCONNECT.ToString();
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = null;
        request.requestDataStructure = null;
        socketRequest.Add(request);
    }

    // public bool SendLeaveMatchRequest()
    // {
    //     if (string.IsNullOrEmpty(TABLE_ID))
    //     {
    //         return false;
    //     }

    //     FoldData requestData = new FoldData();

    //     requestData.userId = "" + R_PlayerManager.instance.GetPlayerGameData().userId;
    //     requestData.tableId = TABLE_ID;
    //     requestData.gameType = "lobby";

    //     string requestStringData = JsonMapper.ToJson(requestData);
    //     object requestObjectData = Json.Decode(requestStringData);

    //     R_SocketRequest request = new R_SocketRequest();
    //     request.emitEvent = "leaveMatch";
    //     request.plainDataToBeSend = null;
    //     request.jsonDataToBeSend = requestObjectData;
    //     request.requestDataStructure = requestStringData;
    //     Debug.LogError("[SOCKET EVENT] - leaveMatch" + "[Params]  " + requestStringData);
    //     socketManager.Socket.Emit(request.emitEvent, request.jsonDataToBeSend);

    //     return true;
    // }
#endregion









// OTHER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------

#region OTHER_METHODS
    public R_SocketState GetSocketState()
    {
        return socketState;
    }

    public void SetSocketState(R_SocketState state)
    {
        Debug.Log("Set Socket State " + state, this.gameObject);
        socketState = state;
    }

    public void SetTableId(string tableIdToAssign)
    {
        R_GlobalGameManager.instance.GetRoomData().socketTableId = tableIdToAssign;
        TABLE_ID = tableIdToAssign;

        PrefsManager.SetData(PrefsKey.RoomData, JsonUtility.ToJson(GlobalGameManager.instance.GetRoomData()));
        //Debug.Log("Table ID Is :" + TABLE_ID);
    }

    public string GetTableID()
    {
        return TABLE_ID;
    }

    public void ResetConnection(bool isReconnecting = false)
    {
        if (socketManager != null && socketManager.Socket.IsOpen)
        {
            socketManager.Close();
        }

        socketRequest.Clear();
        socketResponse.Clear();

        if (!isReconnecting)
        {
            if (IsInvoking("HandleSocketResponse"))
            {
                CancelInvoke("HandleSocketResponse");
            }

            if (IsInvoking("SendSocketRequest"))
            {
                CancelInvoke("SendSocketRequest");
            }

            SetSocketState(R_SocketState.NULL);
        }
        // if (Rummy_InGameManager.instance.gameExitCalled)
        //     R_GlobalGameManager.instance.LoadScene(R_Scenes.MainMenuScene);
    }
#endregion









#region ReconnectProtocols

    private void StartReconnectProcedure()
    {
        if (!isPreocedureRunning)
        {
            StartCoroutine(WaitForReconnect());
        }
    }

    private bool isPreocedureRunning = false;
    private IEnumerator WaitForReconnect()
    {
        isPreocedureRunning = true;
        // Rummy_InGameUiManager.instance.ShowScreen(Rummy_InGameScreens.Reconnecting);
        SetSocketState(R_SocketState.ReConnecting);
        socketManager = null;
        ReConnect();

        int counter = 0, maxCount = 30, reInitialisationCount = 3;

        while (counter < maxCount)
        {
            yield return new WaitForSeconds(1f);

            if (socketManager.Socket.IsOpen)
            {
                counter = maxCount;
            }
            else
            {
                if (counter % reInitialisationCount == 0)
                {
                    ReConnect();
                }
            }

            ++counter;
        }

        if (!socketManager.Socket.IsOpen)
        {
            // Rummy_InGameUiManager.instance.DestroyScreen(Rummy_InGameScreens.Reconnecting);
            Rummy_InGameUiManager.instance.ShowMessage("Connection error, check your network connection and try again.", () =>
            {
                StartCoroutine(WaitForReconnect());
            },

             () =>
             {
                // Rummy_InGameManager.instance.LoadMainMenu();
             }, "Retry", "Cancel"
            );
        }

        isPreocedureRunning = false;
    }

    private void RequestForMatchStatus()
    {
        string requestStringData = string.Empty;
        requestStringData = "{\"gameType\":\""+rummyGameType+"\"}"; //\"id\":"+rummyUserId+",
        object requestObjectData = Json.Decode(requestStringData);
        // socketManager.Socket.Emit(R_SocketEvetns.join.ToString(), requestObjectData);
        R_SocketRequest request = new R_SocketRequest();
        request.emitEvent = R_SocketEvetns.join.ToString();
         request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
        R_CardManager.Instance.isDiscardedCardFirstTime = false;


        // R_ReconnectData data = new R_ReconnectData();
        // data.userId = "" + R_PlayerManager.instance.GetPlayerGameData().userId;
        // data.tableId = TABLE_ID;
        // data.isYesOrNo = "Yes";

        // string requestdeta = JsonMapper.ToJson(data);
        // object requestfordeta = Json.Decode(requestdeta);

        // R_SocketRequest request = new R_SocketRequest();
        // request.emitEvent = "userReconnect";
        // request.plainDataToBeSend = null;
        // request.jsonDataToBeSend = requestfordeta;
        // request.requestDataStructure = requestdeta;
        // socketRequest.Add(request);
    }

    private IEnumerator WaitAndCheckInternetConnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(R_GameConstants.NETWORK_CHECK_DELAY);

            if (GetSocketState() == R_SocketState.Game_Running)
            {
                if (!R_WebServices.instance.IsInternetAvailable())
                {
                    if (socketManager != null)
                    {
                        socketManager.Close();
                    }
                }
            }
        }
    }

#endregion
}



public enum R_SocketEvetns
{
    CONNECT,
    DISCONNECT,
    RECONNECT_ATTEMPT,

    join, //emit
    personal,
    time,
    // player_timer,
    // RANDOM_CARDS,
    SEATING,
    TOSSED_CARDS,
    FIRST_PLAYER,
    HAND_CARDS,
    DECK_INFO,
    DISCARDED_CARD,
    PLAYER_TIMMER,
    FROM_DECK, //emit
    FROM_DISCARDED, //emit
    DISCARD, //emit
    CARD_PICKED,
    OPPONENT_CARD_PICKED,
    DROP, //emit
    FINISH, //emit
    ARRANGEMENT, //emit
    ARRANGED_CARDS,
    FINISH_GAME_TIMER,
    PLAYER_SUBMIT_TIMMER,
    SUBMISSION,
    WRONG_SHOW,
    RESULT,
    GAME_EXIT, //not working
    DISCARDED_HISTORY, //emit & on
    GAME_HISTORY, //emit & on
    SCORE_BOARD, //emit & on
    CHAT_MESSAGE, //emit & on
    EXIT_NEXT_ROUND, //emit
    AUTO_DROP, //emit
    GAME_START_TIMER,   //after result event
    EXIT_GAME, //emit

    // New Events
    STARTING_GAME,
    PLAYER_DETAILS,
    INSUFFICIENT_CHIPS,
    leave_Table,
    EXITED_NEXT_ROUND,
    PLAYER_EXIT

}

// [System.Serializable]
// public class R_RabitData
// {
//     public string tableId;
//     public string userId;
// }

// [System.Serializable]
// public class R_StandUpdata
// {
//     public string tableId;
//     public string userId;
//     public int isStatndOut;
// }

[System.Serializable]
public class R_SocketRequest
{
    public object jsonDataToBeSend;
    public string plainDataToBeSend;
    public string emitEvent;
    public string requestDataStructure;
}

[System.Serializable]
public class R_SocketResponse
{
    public R_SocketEvetns eventType;
    public string data;
}


// [System.Serializable]
// public class R_FoldData
// {
//     public string tableId;
//     public string userId;
//     public string gameType;
//     public UserBetData userData;
// }

// [System.Serializable]
// public class R_MinEvent
// {
//     public string tableId;
//     public string userId;
//     public string appStatus;
// }

// [System.Serializable]
// public class R_BetData
// {
//     public string tableId;
//     public string userId;
//     public string bet;
//     public UserBetData userData;
//     public string userAction;

// }

[System.Serializable]
public class R_ReconnectData
{
    public string tableId;
    public string userId;
    public string isYesOrNo;
}


// [System.Serializable]
// public class R_SendMessageData
// {
//     public string userId;
//     public string tableId;

//     public string from;
//     public string to;
//     public string title;
//     public string desc;
// }

// [System.Serializable]
// public class R_CoinUpdateData
// {
//     public string email;
//     public string coins;
// }

// [System.Serializable]
// public class R_UserBetData
// {
//     public int betData, roundNo;
//     public string playerAction;
// }

// [System.Serializable]
// public class R_RematchData
// {
//     public string isBuyIn, userId, tableId, coins;
// }

// [System.Serializable]
// public class R_RoomCreateData
// {
//     public string roomId;
//     public string players;
//     public string isPrivate;
//     public string isFree;
// }


// [System.Serializable]
// public class R_PrivateRoomJoinData
// {
//     public string userId;
//     public string roomId;
//     public string players;
//     public string playerType;

//     public string isPrivate;
//     public string isFree;
//     public string tableId;
// }

[System.Serializable]
public enum R_SocketState
{
    Connecting,
    WaitingForOpponent,
    Game_Running,
    InitializingCards,
    ReConnecting,
    NULL
}