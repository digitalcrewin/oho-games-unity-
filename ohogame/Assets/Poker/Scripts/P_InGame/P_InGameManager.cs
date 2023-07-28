using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Linq;

public class P_InGameManager : MonoBehaviour
{
    public static P_InGameManager instance;

    //public GameObject[] players;
    public List<GameObject> players;

    //public P_Players[] playersScript;
    public List<P_Players> playersScript;
    public P_Players[] onlinePlayersScripts = null;

    public Image[] communityCards;
    //public Transform[] allPlayerPos;
    public List<Transform> allPlayerPos;
    public GameObject[] allPots;

    private P_Players myPlayerObject = null;
    public float onBetAmount = 0;  //"BET", OnBet data["bet"]
    public float potAmount = 0; //"POT", OnPot data["amount"]
    private P_SuggestionActions selectedSuggestionButton = P_SuggestionActions.Null;

    public GameObject[] actionButtons, suggestionButtons, suggestionButtonsActiveImage;
    public GameObject actionBtnParent, suggestionBtnParent, raisePopUp;
    public Text callAmountText, suggestionCallText;
    public Text sliderText;
    public Slider slider;
    private float selectedRaiseAmount = 0; //raise slider.value
    private float availableCallAmount = 0;

    private int suggestionCallAmount = 0;
    float minRaise, maxRaise;
    public Text raiseErrorText;

    public int playerTurnIndex = 0;

    public GameObject cardAnimationPrefab, betAnimationPrefab;
    public Transform animationLayer;

    public GameObject WinAnimationpos;
    private List<GameObject> winnersObject = new List<GameObject>();
    public Text WinnersNameText;
    private bool winnerAnimationFound = false;
    public int holeCardCount = -1;

    [SerializeField]
    private GameObject winningPrefab, chipscoin;

    [Space(10)]
    public Button menuBtn;
    public Button handHistoryBtn;
    public Button chatBtn;
    public Button realTimeResultBtn;
    public Button walletBtn;

    [Space(10)]
    public Animator actionPanelAnimator;
    public Animator suggestionPanelAnimator;

    [Space(10)]
    [SerializeField]
    private Sprite[] holeCardsTemp;

    public bool isSeatRotation = false;
    [SerializeField]
    Sprite chatIconTopBottom, chatIconLeftRight;

    private void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        switch (buttonName)
        {
            case "Fold":
                P_SocketController.instance.SendFold(P_SocketController.instance.TABLE_ID);

                actionButtons[0].GetComponent<Button>().interactable = false;
                actionBtnParent.SetActive(false);
                break;

            case "Check":
                P_SocketController.instance.SendCheck(P_SocketController.instance.TABLE_ID);

                actionButtons[1].GetComponent<Button>().interactable = false;
                actionBtnParent.SetActive(false);
                break;

            //case "Raise":
            //    break;

            case "Call":
                P_SocketController.instance.SendCall(P_SocketController.instance.TABLE_ID);

                actionButtons[3].GetComponent<Button>().interactable = false;
                actionBtnParent.SetActive(false);
                break;



            case "Bet":
                raisePopUp.SetActive(false);
                actionBtnParent.SetActive(true);


                if (sliderText.text == "All In")
                {
                    P_Players playerForBet = GetMyPlayerObject();

                    if (playerForBet != null)
                    {
                        playerForBet.ResetTurn();
                        OnPlayerActionCompleted(P_PlayerAction.Raise, (int)playerForBet.GetPlayerData().balance, "AllIn");
                    }
                    else
                    {
#if ERROR_LOG
                            Debug.LogError("Null Reference exception found playerObject is null in InGameUiManager.RaiseOpen");
#endif
                    }
                }
                else
                {
                    if (P_SocketController.instance.gameTypeName == "PLO 4" || P_SocketController.instance.gameTypeName == "PLO 5")
                    {
                        OnPlayerActionCompleted(P_PlayerAction.Raise, (int)selectedRaiseAmount, potAmount.ToString()); //potAmount
                    }
                    else
                    {
                        OnPlayerActionCompleted(P_PlayerAction.Bet, (int)selectedRaiseAmount, "Bet"); //(P_PlayerAction.Raise, (int)selectedRaiseAmount, "Raise");
                    }
                }
                break;

            
        }

        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].SetActive(false);
        }
    }

    public void OnClickRaise()
    {
        P_Players player = GetMyPlayerObject();
        minRaise = player.GetPlayerData().minRaise;
        maxRaise = player.GetPlayerData().maxRaise;

        if (player != null)
        {
            float localSmallBlind = P_SocketController.instance.firstSeatSmallBlind;
            if (availableCallAmount < localSmallBlind)
                availableCallAmount = localSmallBlind;
            ToggleRaisePopUp(true, minRaise, maxRaise, potAmount);
        }
        else
        {
#if ERROR_LOG
                        Debug.LogError("Null Reference exception found playerObject is null in InGameUiManager.RaiseOpen");
#endif
        }
    }

    public void OnClickRaiseOptions(string buttonName)
    {
        switch (buttonName)
        {
            case "pot": //X4  //PLO: Pot
                {
                    if (P_SocketController.instance.gameTypeName == "PLO 4" || P_SocketController.instance.gameTypeName == "PLO 5")
                    {
                        if (potAmount < minRaise)
                        {
                            slider.value = ((minRaise / 2) * 4);
                            OnSliderValueChange();
                        }
                        else
                        {
                            slider.value = potAmount;
                            OnSliderValueChange();
                        }
                    }
                    else
                    {
                        float halfPot = potAmount / 2;

                        // PotWise Calculation
                        if (halfPot >= minRaise)
                        {
                            slider.value = potAmount;
                            OnSliderValueChange();
                        }
                        else // Call Amount wise calculation
                        {
                            slider.value = ((minRaise / 2) * 4);
                            OnSliderValueChange();
                        }
                    }
                }
                break;

            case "halfPot": //X3
                {
                    float halfPot = potAmount / 2;

                    // PotWise Calculation
                    if (halfPot >= minRaise)
                    {
                        slider.value = (potAmount / 2f);
                        OnSliderValueChange();
                    }
                    else // Call Amount wise calculation
                    {
                        slider.value = ((minRaise / 2) * 3);
                        OnSliderValueChange();
                    }
                }
                break;

            case "thirdPot": //X2
                {
                    float halfPot = potAmount / 2;

                    // PotWise Calculation
                    if (halfPot >= minRaise)
                    {
                        slider.value = ((potAmount * 2f) / 3f);
                        OnSliderValueChange();
                    }
                    else // Call Amount wise calculation
                    {
                        slider.value = minRaise;
                        OnSliderValueChange();
                    }
                }
                break;

            case "allIn":
                break;

            case "raiseClose":
                ToggleRaisePopUp(false);
                break;
        }
    }


    public void OnClickOnSuggestionButton(string buttonName)
    {
        switch (buttonName)
        {
            case "sCall":
                {

                    if (selectedSuggestionButton == P_SuggestionActions.Call)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = P_SuggestionActions.Null;
                    }
                    else
                    {
                        ResetSuggestionButtonsActiveImage();
                        suggestionCallAmount = (int)availableCallAmount;
                        selectedSuggestionButton = P_SuggestionActions.Call;

                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }
                }
                break;


            case "sCallAny":
                {
                    if (selectedSuggestionButton == P_SuggestionActions.Call_Any)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = P_SuggestionActions.Null;
                    }
                    else
                    {
                        ResetSuggestionButtonsActiveImage();
                        selectedSuggestionButton = P_SuggestionActions.Call_Any;
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }
                }
                break;

            case "sCheck":
                {
                    if (selectedSuggestionButton == P_SuggestionActions.Check)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = P_SuggestionActions.Null;
                    }
                    else
                    {
                        ResetSuggestionButtonsActiveImage();
                        selectedSuggestionButton = P_SuggestionActions.Check;
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }
                }
                break;

            case "sFold":
                {
                    if (selectedSuggestionButton == P_SuggestionActions.Fold)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = P_SuggestionActions.Null;
                    }
                    else
                    {
                        ResetSuggestionButtonsActiveImage();
                        selectedSuggestionButton = P_SuggestionActions.Fold;
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }
                }
                break;
        }
    }

    public void ResetSuggestionButtonsActiveImage()
    {
        for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
        {
            suggestionButtonsActiveImage[i].gameObject.SetActive(false);
        }
    }


    public P_Players GetMyPlayerObject()
    {
        if (myPlayerObject == null)
        {
            myPlayerObject = GetPlayerObject(PlayerManager.instance.GetPlayerGameData().userId);
        }

        return myPlayerObject;
    }


    public P_Players GetPlayerObject(string userId)
    {
        for (int i = 0; i < playersScript.Count; i++)
        {
            if (playersScript[i].GetPlayerData().userId == userId)
            {
                return playersScript[i];
            }
        }

        return null;
    }

    public void PlayerTimerReset()
    {
        for (int i = 0; i < playersScript.Count; i++)
        {
            playersScript[i].ResetTurn();
        }
    }

    public void OnPlayerActionCompleted(P_PlayerAction actionType, int betAmount, string playerAction)
    {
        ToggleActionButton(false);

        if (actionType == P_PlayerAction.Fold)
        {
            //SoundManager.instance.PlaySound(SoundType.Fold);
            P_SocketController.instance.SendFold(P_SocketController.instance.TABLE_ID);
        }
        else
        {
            if (actionType == P_PlayerAction.Check)
            {
                //SoundManager.instance.PlaySound(SoundType.Check);
            }

            P_SocketController.instance.SendRaise(betAmount);
        }
    }

    public void ToggleActionButton(bool isShow, P_Players playerObject = null, bool isCheckAvailable = false, int lastBetAmount = 0, float availableBalance = 0)
    {
        if (isShow)
        {
            ResetSuggetionAction();
            int callAmount = lastBetAmount - (int)playerObject.GetPlayerData().totalBet;

            if (callAmount > 0)
            {
                isCheckAvailable = false;
            }


            if (callAmount > 0) // amount available to bet
            {
                if (lastBetAmount > availableBalance)
                {
                    actionButtons[(int)P_PlayerAction.Check].SetActive(false);
                    actionButtons[(int)P_PlayerAction.Call].SetActive(false);
                    actionButtons[(int)P_PlayerAction.AllIn].SetActive(true);
                }
                else
                {
                    callAmountText.text = "" + callAmount; //ScoreShow(callAmount);
                    actionButtons[(int)P_PlayerAction.Check].SetActive(false);
                    actionButtons[(int)P_PlayerAction.AllIn].SetActive(false);
                    actionButtons[(int)P_PlayerAction.Call].SetActive(true);
                }
            }
            else // dont have amount to bet hence show only fold and all-in
            {
                actionButtons[(int)P_PlayerAction.Call].SetActive(false);
                actionButtons[(int)P_PlayerAction.AllIn].SetActive(false);
                actionButtons[(int)P_PlayerAction.Raise].SetActive(true);
                actionButtons[(int)P_PlayerAction.Check].SetActive(true);
                actionButtons[(int)P_PlayerAction.Fold].SetActive(true);
            }

            if (callAmount == 0)
            {
                callAmountText.text = "";
            }

            availableCallAmount = callAmount;
        }
        actionBtnParent.SetActive(isShow);
    }

    public void ToggleSuggestionButton(bool isShow, bool isCheckAvailable = false, int callAmount = 0, float availableBalance = 0)
    {
        suggestionBtnParent.SetActive(isShow);
        if (isShow)
        {
            if (callAmount <= 0)
            {
                isCheckAvailable = true;
            }

            for (int i = 0; i < suggestionButtons.Length; i++)
            {
                suggestionButtons[i].SetActive(true);
                suggestionButtonsActiveImage[i].SetActive(false);
            }

            if (isCheckAvailable)
            {
                suggestionButtons[(int)P_SuggestionActions.Call].SetActive(false);
                suggestionButtons[(int)P_SuggestionActions.Check].SetActive(true);
            }
            else
            {
                availableCallAmount = callAmount;
                if (callAmount < availableBalance)
                {
                    suggestionButtons[(int)P_SuggestionActions.Call].SetActive(true);
                    suggestionButtons[(int)P_SuggestionActions.Check].SetActive(false);
                    if (callAmount != 0)
                    {
                        suggestionCallText.text = "" + callAmount;
                    }
                    else
                    {
                        suggestionCallText.text = "";
                    }
                }
                else
                {
                    for (int i = 0; i < suggestionButtons.Length; i++)
                    {
                        suggestionButtons[i].SetActive(false);
                        suggestionButtonsActiveImage[i].SetActive(false);
                    }
                    suggestionButtons[(int)P_SuggestionActions.Fold].SetActive(true);
                }
            }


            if (selectedSuggestionButton == P_SuggestionActions.Call && callAmount != suggestionCallAmount)
            {
                selectedSuggestionButton = P_SuggestionActions.Null;
            }
            else if (selectedSuggestionButton != P_SuggestionActions.Null && suggestionButtons[(int)selectedSuggestionButton].activeInHierarchy)
            {
                suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
            }
            else
            {
                selectedSuggestionButton = P_SuggestionActions.Null;
            }
        }
    }

    public void OnPlayerActionCompleted(P_SuggestionActions actionType, int betAmount, string playerAction)
    {
        P_InGameManager.instance.ToggleActionButton(false);

        if (actionType == P_SuggestionActions.Fold)
        {
            //SoundManager.instance.PlaySound(SoundType.Fold);
            P_SocketController.instance.SendFold(P_SocketController.instance.TABLE_ID);
        }
        else if (actionType == P_SuggestionActions.Check)
        {
            //SoundManager.instance.PlaySound(SoundType.Check);
            P_SocketController.instance.SendCheck(P_SocketController.instance.TABLE_ID);
        }
        else if (actionType == P_SuggestionActions.Call)
        {
            //SoundManager.instance.PlaySound(SoundType.Check);
            P_SocketController.instance.SendCall(P_SocketController.instance.TABLE_ID);
        }
    }

    public P_SuggestionActions GetSelectedSuggestionAction()
    {
        return selectedSuggestionButton;
    }

    public void ResetSuggetionAction()
    {
        selectedSuggestionButton = P_SuggestionActions.Null;
    }





    public void OnSliderValueChange()
    {
        if (slider.value >= slider.maxValue)
        {
            if (P_SocketController.instance.gameTypeName == "PLO 4" || P_SocketController.instance.gameTypeName == "PLO 5")
                sliderText.text = slider.maxValue.ToString();
            else
                sliderText.text = "All In";
        }
        else
        {
            sliderText.text = "" + (int)slider.value;
        }

        selectedRaiseAmount = slider.value;
    }

    private void ToggleRaisePopUp(bool isShow, float minBet = 0, float maxBet = 0, float potAmount = 0)
    {
        raisePopUp.SetActive(isShow);

        if (isShow)
        {
            slider.minValue = minBet;
            slider.maxValue = maxBet;

            float halfPot = potAmount / 2;

            if (halfPot >= minBet) // Pot Wise Raise Amount
            {
                slider.value = potAmount;

                raisePopUp.transform.Find("Pot/Text").gameObject.GetComponent<Text>().text = "Pot";
                raisePopUp.transform.Find("HalfPot/Text").gameObject.GetComponent<Text>().text = "1/2 Pot";
                raisePopUp.transform.Find("ThirdPart/Text").gameObject.GetComponent<Text>().text = "2/3 Pot";

                if (maxBet >= potAmount)
                {
                    raisePopUp.transform.Find("Pot").gameObject.SetActive(true);
                    raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                    raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                }
                else
                {
                    raisePopUp.transform.Find("Pot").gameObject.SetActive(false);

                    if (slider.maxValue >= (potAmount / 2))
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                        raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                    }
                    else
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(false);

                        float twoThirdAmount = (potAmount * 2) / 3;

                        if (slider.maxValue >= twoThirdAmount)
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                        }
                        else
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(false);
                        }
                    }
                }
            }
            else // Call Wise Raise Amount
            {
                slider.value = minBet;

                if (P_SocketController.instance.gameTypeName == "PLO 4" || P_SocketController.instance.gameTypeName == "PLO 5")
                {
                    if (potAmount < minRaise)
                    {
                        raisePopUp.transform.Find("Pot/Text").gameObject.GetComponent<Text>().text = "X4";
                    }
                    else
                    {
                        raisePopUp.transform.Find("Pot/Text").gameObject.GetComponent<Text>().text = "Pot";
                    }
                }
                else
                    raisePopUp.transform.Find("Pot/Text").gameObject.GetComponent<Text>().text = "X4";

                raisePopUp.transform.Find("HalfPot/Text").gameObject.GetComponent<Text>().text = "X3";
                raisePopUp.transform.Find("ThirdPart/Text").gameObject.GetComponent<Text>().text = "X2";


                if (maxBet >= (availableCallAmount * 4))
                {
                    if (((minRaise / 2) * 4) <= maxRaise)
                    {
                        raisePopUp.transform.Find("Pot").gameObject.SetActive(true);
                    }
                    else
                    {
                        raisePopUp.transform.Find("Pot").gameObject.SetActive(false);
                    }
                    raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                    raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                }
                else
                {
                    raisePopUp.transform.Find("Pot").gameObject.SetActive(false);

                    if (slider.maxValue >= (availableCallAmount * 4))
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                        raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                    }
                    else
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(false);


                        if (slider.maxValue >= (availableCallAmount * 2))
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                        }
                        else
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(false);
                        }
                    }
                }
            }

            OnSliderValueChange();
        }
    }

    public void SliderPlusButton()
    {
        if (slider.value < slider.maxValue)
            slider.value = slider.value + 1;
    }

    public void SliderMinusButton()
    {
        if (slider.value > slider.minValue)
            slider.value = slider.value - 1;
    }










    #region HANDLE_SOCKET_RESPONSE
    public int myPlayerSeatIndex = 0;
    public void OnSeatReceiveSet(string str)
    {
        if (P_InGameUiManager.instance.gameStartInText.gameObject.activeSelf)
        {
            P_InGameUiManager.instance.gameStartInText.text = "Game starts in ";
            P_InGameUiManager.instance.gameStartInText.gameObject.SetActive(false);
        }

        P_InGameUiManager.instance.ResetPlayerAllData();
        if (P_SocketController.instance.isLeaveSeatSended && P_InGameUiManager.instance.IsScreenActive(P_InGameScreens.Loading))
        {
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Loading);
            P_SocketController.instance.isLeaveSeatSended = false;
            Debug.Log("LEAVE SEAT 6 " + System.DateTime.Now.ToString("hh.mm.ss.ffffff"));
        }

        JsonData data = JsonMapper.ToObject(str);


        bool myPlayerFind = false;
        bool myPlayerIsPlaying = false;

        if (data["players"].Count > 0)
        {
            List<MatchMakingPlayerData> matchMakingPlayerData = new List<MatchMakingPlayerData>();

            for (int i = 0; i < data["players"].Count; i++)
            {
                if (data["players"][i] == null)
                {
                    MatchMakingPlayerData playerData = new MatchMakingPlayerData();
                    playerData.isNull = true;
                    matchMakingPlayerData.Add(playerData);
                }
                else if (data["players"][i] != null)
                {
                    MatchMakingPlayerData playerData = new MatchMakingPlayerData();
                    P_PlayerData pl = new P_PlayerData();

                    if (string.IsNullOrEmpty(P_SocketController.instance.idleTimeout))
                        P_SocketController.instance.idleTimeout = data["players"][i]["turnTimer"].ToString();

                    pl.seatNo = data["players"][i]["seat"].ToString();
                    pl.userName = data["players"][i]["id"].ToString();
                    pl.userId = data["players"][i]["userId"].ToString();
                    pl.tableId = data["players"][i]["tableId"].ToString();
                    pl.balance = float.Parse(data["players"][i]["stackSize"].ToString());

                    pl.isPlaying = ((bool)data["players"][i]["isPlaying"] == true) ? true : false;
                    pl.doesHaveCards = ((bool)data["players"][i]["doesHaveCards"] == true) ? true : false;

                    if (data["players"][i]["userId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        myPlayerFind = true;

                        try
                        {
                            if (data["players"][i]["isMuckEnabled"].ToString().ToLower() == "true")
                            {
                                PlayerPrefs.SetInt("isMuckHand", 1);
                            }
                            else
                            {
                                PlayerPrefs.SetInt("isMuckHand", 0);
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.Log("Exception in Muck playerpref set " + e.Message);
                        }
                    }

                    playerData.playerData = pl;
                    matchMakingPlayerData.Add(playerData);
                }
            }

            SeatRotation(matchMakingPlayerData);
        }


        // buy-in or top-up
        if (P_SocketController.instance.isJoinSended)
        {
            if (P_SocketController.instance.isMyBalanceZero)
            {
                if (P_InGameUiManager.instance.buyInPopUp.activeSelf)
                {
                    if (!myPlayerFind)
                    {
                        if (P_GameConstant.enableLog)
                            Debug.Log("Buy-in popup remains on");

                        P_InGameUiManager.instance.buyInPopUp.SetActive(false);
                        P_InGameUiManager.instance.isCallFromMenu = false;
                    }
                }
            }
            else if (!P_SocketController.instance.isMyBalanceZero)
            {
                if (P_InGameUiManager.instance.buyInPopUp.activeSelf)
                {
                    if (P_GameConstant.enableLog)
                        Debug.Log("Buy-in popup Auto close");
                    //P_InGameUiManager.instance.buyInErrorText.text = "";
                    P_InGameUiManager.instance.buyInPopUp.SetActive(false);
                    P_InGameUiManager.instance.isCallFromMenu = false;
                }
            }
            if (P_BuyinPopup.instance != null)
            {
                P_BuyinPopup.instance.buyInButton.interactable = true;
                P_BuyinPopup.instance.buyInCloseButton.interactable = true;
            }
        }

        if (P_SocketController.instance.gameTypeName == "SIT N GO")
        {
            if (!myPlayerFind) //(mySeatIndexTemp == -1)
            {
                if (P_SocketController.instance.gameTableData["table_attributes"]["players"].Count < int.Parse(P_SocketController.instance.gameTableData["table_attributes"]["maxPlayers"].ToString()))
                {
                    //SIT N GO SEAT Table have empty seat
                    P_InGameUiManager.instance.AllPlayerPosPlusOff(false); //plus icon on
                }
                else
                {
                    //SIT N GO SEAT Table is full
                    P_InGameUiManager.instance.AllPlayerPosPlusOff(true); //plus icon off
                }
                P_SocketController.instance.isViewer = true;
                P_SocketController.instance.isJoinSended = false;
            }
            else
            {
                P_InGameUiManager.instance.AllPlayerPosPlusOff(true);
                P_SocketController.instance.isViewer = false;
                P_SocketController.instance.isJoinSended = true;
            }
        }
        else
        {
            if (!myPlayerFind) //(mySeatIndexTemp == -1)
            {
                P_InGameUiManager.instance.AllPlayerPosPlusOff(false);
                P_SocketController.instance.isViewer = true;
                P_SocketController.instance.isJoinSended = false;
            }
            else
            {
                P_InGameUiManager.instance.AllPlayerPosPlusOff(true);
                P_SocketController.instance.isViewer = false;
                P_SocketController.instance.isJoinSended = true;
            }
        }


        P_SocketController.instance.firstSeatSmallBlind = float.Parse(data["smallBlind"].ToString());
    }


    public int mySeatIndex = 0;
    public int mySeatIndexTemp = -1;

    void SeatRotation(List<MatchMakingPlayerData> newMatchMakingPlayerData)
    {
        onlinePlayersScripts = new P_Players[newMatchMakingPlayerData.Count];

        for (int i = 0; i < newMatchMakingPlayerData.Count; i++)
        {
            if (newMatchMakingPlayerData[i].isNull == false)
            {
                int seat = int.Parse(newMatchMakingPlayerData[i].playerData.seatNo);
                if (seat < 0)
                    seat = 0;

                playersScript[i].userName.text = newMatchMakingPlayerData[i].playerData.userName;
                playersScript[i].balance.text = newMatchMakingPlayerData[i].playerData.balance.ToString();
                playersScript[i].gameObject.SetActive(true);

                playersScript[seat].seat = newMatchMakingPlayerData[i].playerData.seatNo;

                newMatchMakingPlayerData[i].playerData.twoCards = new Image[2];
                newMatchMakingPlayerData[i].playerData.sixCards = new Image[6];
                newMatchMakingPlayerData[i].playerData.twoCards[0] = playersScript[i].playerData.twoCards[0];
                newMatchMakingPlayerData[i].playerData.twoCards[1] = playersScript[i].playerData.twoCards[1];

                newMatchMakingPlayerData[i].playerData.sixCards[0] = playersScript[i].playerData.sixCards[0];
                newMatchMakingPlayerData[i].playerData.sixCards[1] = playersScript[i].playerData.sixCards[1];
                newMatchMakingPlayerData[i].playerData.sixCards[2] = playersScript[i].playerData.sixCards[2];
                newMatchMakingPlayerData[i].playerData.sixCards[3] = playersScript[i].playerData.sixCards[3];
                newMatchMakingPlayerData[i].playerData.sixCards[4] = playersScript[i].playerData.sixCards[4];
                newMatchMakingPlayerData[i].playerData.sixCards[5] = playersScript[i].playerData.sixCards[5];

                playersScript[i].Init(newMatchMakingPlayerData[i]);

                if (playersScript[i].playerData.doesHaveCards) //(pl.playerData.isPlaying)
                {
                    if (P_SocketController.instance.isViewer)
                    {
                        if (P_SocketController.instance.gameTypeName == "PLO 4")
                        {
                            for (int j = 0; j < playersScript[i].playerData.sixCards.Length; j++)
                            {
                                if (j < 4)
                                {
                                    playersScript[i].playerData.sixCards[j].sprite = P_CardsManager.instance.cardBackSprite;
                                    playersScript[i].playerData.sixCards[j].gameObject.SetActive(true);
                                }
                            }
                            playersScript[i].playerData.sixCards[0].transform.parent.gameObject.SetActive(true);
                        }
                        else if (P_SocketController.instance.gameTypeName == "PLO 5")
                        {
                            for (int j = 0; j < playersScript[i].playerData.sixCards.Length; j++)
                            {
                                if (j < 5)
                                {
                                    playersScript[i].playerData.sixCards[j].sprite = P_CardsManager.instance.cardBackSprite;
                                    playersScript[i].playerData.sixCards[j].gameObject.SetActive(true);
                                }
                            }
                            playersScript[i].playerData.sixCards[0].transform.parent.gameObject.SetActive(true);
                        }
                        else
                        {
                            playersScript[i].playerData.sixCards[0].sprite = P_CardsManager.instance.cardBackSprite;
                            playersScript[i].playerData.sixCards[1].sprite = P_CardsManager.instance.cardBackSprite;
                            playersScript[i].playerData.sixCards[0].gameObject.SetActive(true);
                            playersScript[i].playerData.sixCards[1].gameObject.SetActive(true);
                            playersScript[i].playerData.sixCards[0].transform.parent.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    playersScript[i].playerData.sixCards[0].transform.parent.gameObject.SetActive(false);
                }

                if (playersScript[i].playerData.isPlaying)
                {
                    if (playersScript[i].foldImage.activeSelf)
                    {
                        playersScript[i].foldImage.SetActive(false);
                    }

                    if (playersScript[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        if (playersScript[i].fold2CardsImage != null && playersScript[i].fold2CardsImage.activeSelf)
                            playersScript[i].fold2CardsImage.SetActive(false);
                        if (playersScript[i].fold4CardsImage != null && playersScript[i].fold4CardsImage.activeSelf)
                            playersScript[i].fold4CardsImage.SetActive(false);
                        if (playersScript[i].fold5CardsImage != null && playersScript[i].fold5CardsImage.activeSelf)
                            playersScript[i].fold5CardsImage.SetActive(false);
                        if (playersScript[i].fold6CardsImage != null && playersScript[i].fold6CardsImage.activeSelf)
                            playersScript[i].fold6CardsImage.SetActive(false);
                    }
                }
                else
                {
                    if (!playersScript[i].foldImage.activeSelf)
                    {
                        playersScript[i].foldImage.SetActive(true);
                    }

                    if (playersScript[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        if (playersScript[i].playerData.sixCards[0].transform.parent.gameObject.activeSelf)
                        {
                            if (P_SocketController.instance.gameTypeName == "PLO 4" && playersScript[i].fold4CardsImage != null)
                            {
                                playersScript[i].fold4CardsImage.SetActive(true);
                            }
                            else if (P_SocketController.instance.gameTypeName == "PLO 5" && playersScript[i].fold5CardsImage != null)
                            {
                                playersScript[i].fold5CardsImage.SetActive(true);
                            }
                            else if (playersScript[i].fold2CardsImage != null)
                            {
                                playersScript[i].fold2CardsImage.SetActive(true);
                            }
                        }
                    }
                }

                if (playersScript[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                {
                    if (playersScript[i].playerData.balance > 0)
                    {
                        P_SocketController.instance.isMyBalanceZero = false;
                    }
                }
            }
        }

        for (int i = 0; i < newMatchMakingPlayerData.Count; i++)
        {
            if (newMatchMakingPlayerData[i].isNull == false)
            {
                int seat = int.Parse(newMatchMakingPlayerData[i].playerData.seatNo);
                if (newMatchMakingPlayerData[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                {
                    mySeatIndex = seat;
                    mySeatIndexTemp = seat;
                    break;
                }
            }
        }

        if (!isSeatRotation)
        {
            isSeatRotation = true;
            int seatPos = P_SocketController.instance.gameTableMaxPlayers - 1;  //7
            playersScript[mySeatIndex].transform.DOMove(allPlayerPos[0].transform.position, 0.5f);
            playersScript[mySeatIndex].GetComponent<P_Players>().currentSeat = (0 + 1).ToString();
            allPlayerPos[0].transform.GetChild(0).GetComponent<P_PlayerSeat>().seatNo = mySeatIndex.ToString();

            for (int i = mySeatIndex - 1; i >= 0; i--)
            {
                playersScript[i].transform.DOMove(allPlayerPos[seatPos].transform.position, 0.5f);
                playersScript[i].GetComponent<P_Players>().currentSeat = (seatPos + 1).ToString();
                allPlayerPos[seatPos].transform.GetChild(0).GetComponent<P_PlayerSeat>().seatNo = playersScript[i].GetComponent<P_Players>().seat;
                seatPos--;
            }
            for (int i = P_SocketController.instance.gameTableMaxPlayers - 1; i >= mySeatIndex; i--)  //7
            {
                playersScript[i].transform.DOMove(allPlayerPos[seatPos].transform.position, 0.5f);
                playersScript[i].GetComponent<P_Players>().currentSeat = (seatPos + 1).ToString();
                allPlayerPos[seatPos].transform.GetChild(0).GetComponent<P_PlayerSeat>().seatNo = playersScript[i].GetComponent<P_Players>().seat;
                seatPos--;
            }
        }

        for (int i = 0; i < playersScript.Count; i++)
        {
            playersScript[i].LocalBetRotateManage();
        }
    }


    public void OnHoleCardSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        StartCoroutine(ShowHoleCardAnimation(data));

        P_SocketController.instance.isGameCounterStart = false;
    }

    private IEnumerator ShowHoleCardAnimation(JsonData data)
    {
        holeCardCount = data["holeCards"].Count;
        holeCardsTemp = new Sprite[holeCardCount];
        List<GameObject> animatedCards = new List<GameObject>();

        for (int i = 0; i < playersScript.Count; i++)
        {
            int tempI = i;

            if (playersScript[i].gameObject.activeSelf && playersScript[i].playerData.isPlaying)
            {
                // six cards
                for (int l = 0; l < data["holeCards"].Count; l++)
                {
                    int tempL = l;
                    GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;

                    gm.transform.DOMove(playersScript[tempI].playerData.sixCards[tempL].transform.parent.position, P_GameConstant.CARD_ANIMATION_DURATION);  //.SetEase(Ease.InCirc)
                    animatedCards.Add(gm);

                    //SoundManager.instance.PlaySound(SoundType.CardMove);
                    yield return new WaitForSeconds(P_GameConstant.CARD_ANIMATION_DURATION);

                    if (!playersScript[tempI].playerData.sixCards[0].transform.parent.gameObject.activeSelf)
                        playersScript[tempI].playerData.sixCards[0].transform.parent.gameObject.SetActive(true);
                    
                    Destroy(gm);
                    playersScript[tempI].playerData.sixCards[tempL].gameObject.SetActive(true);

                    if ((playersScript[tempI].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId))
                    {
                        playersScript[tempI].playerData.sixCards[tempL].transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), GameConstants.CARD_ANIMATION_DURATION);
                    }

                }

                if (data["holeCards"].Count < playersScript[tempI].playerData.sixCards.Length)
                {
                    for (int j = data["holeCards"].Count; j < playersScript[tempI].playerData.sixCards.Length; j++)
                    {
                        playersScript[tempI].playerData.sixCards[j].gameObject.SetActive(false);
                    }
                }
            }

            if (playersScript[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
            {
                for (int k = 0; k < data["holeCards"].Count; k++)
                {
                    P_CardData cardData = P_CardsManager.instance.GetCardData(
                        data["holeCards"][k]["_rank"].ToString() +
                        data["holeCards"][k]["_suit"].ToString()
                        );

                    // six cards
                    playersScript[i].playerData.sixCards[k].sprite = cardData.cardsSprite;
                    holeCardsTemp[k] = cardData.cardsSprite;
                }
            }
            else  // j player na card blind batavana che ena mate else use thase.
            {
                // six cards
                playersScript[i].playerData.sixCards[0].sprite = P_CardsManager.instance.cardBackSprite;
                playersScript[i].playerData.sixCards[1].sprite = P_CardsManager.instance.cardBackSprite;
                playersScript[i].playerData.sixCards[2].sprite = P_CardsManager.instance.cardBackSprite;
                playersScript[i].playerData.sixCards[3].sprite = P_CardsManager.instance.cardBackSprite;
                playersScript[i].playerData.sixCards[4].sprite = P_CardsManager.instance.cardBackSprite;
                playersScript[i].playerData.sixCards[5].sprite = P_CardsManager.instance.cardBackSprite;

            }
        }
        animatedCards.Clear();
    }


    public void OnCommunityCardSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        for (int i = 0; i < data.Count; i++)
        {
            int tempI = i;
            if (data[i] != null)
            {
                P_CardData cardData = P_CardsManager.instance.GetCardData(data[i]["_rank"].ToString() + data[i]["_suit"].ToString());
                communityCards[i].sprite = cardData.cardsSprite;

                if (data.Count == 3)
                {
                    GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;
                    gm.transform.localScale = communityCards[0].transform.localScale;
                    gm.GetComponent<RectTransform>().DOSizeDelta(new Vector2(56.875f, 80f), 0f);
                    gm.GetComponent<Image>().sprite = cardData.cardsSprite;
                    gm.transform.Rotate(0, -90, 0);
                    gm.transform.position = communityCards[0].transform.position;
                    gm.transform.DORotate(new Vector3(0, 90, 0), P_GameConstant.CARD_ANIMATION_DURATION, RotateMode.LocalAxisAdd);
                    gm.transform.DOMove(communityCards[i].transform.position, P_GameConstant.CARD_ANIMATION_DURATION);
                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(P_GameConstant.CARD_ANIMATION_DURATION, () => {
                        Destroy(gm, P_GameConstant.CARD_ANIMATION_DURATION * 3);
                        communityCards[tempI].gameObject.SetActive(true);
                    }));
                }
                else if (data.Count > 3 && i > 2) //(data.Count == 4 && i == 3) || (data.Count == 5 && i == 4)
                {
                    GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;
                    gm.GetComponent<RectTransform>().DOSizeDelta(new Vector2(56.875f, 80f), 0f);
                    gm.transform.localScale = communityCards[i].transform.localScale;
                    gm.GetComponent<Image>().sprite = cardData.cardsSprite;
                    gm.transform.Rotate(0, -90, 0);
                    gm.transform.position = communityCards[i].transform.position;
                    gm.transform.DORotate(new Vector3(0, 90, 0), P_GameConstant.CARD_ANIMATION_DURATION, RotateMode.LocalAxisAdd);
                    gm.transform.DOMove(communityCards[i].transform.position, P_GameConstant.CARD_ANIMATION_DURATION);
                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(P_GameConstant.CARD_ANIMATION_DURATION, () => {
                        Destroy(gm, P_GameConstant.CARD_ANIMATION_DURATION * 1);
                        communityCards[tempI].gameObject.SetActive(true);
                    }));
                }
                else
                {
                    communityCards[i].gameObject.SetActive(true);
                }
            }
            else
            {
                communityCards[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(1f, () => {
            for (int i = 0; i < playersScript.Count; i++)
            {
                if (playersScript[i].playerData.isFold != true)
                {
                    playersScript[i].UpdateLastAction("");
                }
            }
        }));
    }


    public void OnErrorSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);
        IDictionary iData = data as IDictionary;
        if (iData.Contains("message") && iData.Contains("type"))
        {
            if (data["type"].ToString() == "RAISE")
            {
                if (P_SocketController.instance.currentTurnUserId == P_SocketController.instance.gamePlayerId)
                {
                    if (!raisePopUp.activeSelf)
                    {
                        raisePopUp.SetActive(true);
                        actionBtnParent.SetActive(true);
                        actionPanelAnimator.SetBool("isOpen", true);
                    }
                    raiseErrorText.text = data["message"].ToString();
                    raiseErrorText.gameObject.SetActive(true);
                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(3f, () =>
                    {
                        raiseErrorText.text = "";
                        raiseErrorText.gameObject.SetActive(false);
                    }));
                }
            }

            if (data["type"].ToString() == "TOP_UP")
            {
                if (P_SocketController.instance.isTopUpSended) P_SocketController.instance.isTopUpSended = false;

                if (P_InGameUiManager.instance.buyInPopUp.activeSelf)
                {
                    P_InGameUiManager.instance.buyInErrorText.text = data["message"].ToString();
                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(3f, () =>
                    {
                        P_InGameUiManager.instance.buyInErrorText.text = "";
                    }));
                    if (P_BuyinPopup.instance != null)
                    {
                        P_BuyinPopup.instance.buyInButton.interactable = true;
                        P_BuyinPopup.instance.buyInCloseButton.interactable = true;
                    }
                }
            }

            if (data["type"].ToString() == "JOIN")
            {
                if (P_SocketController.instance.isJoinSended) P_SocketController.instance.isJoinSended = false;

                if (P_InGameUiManager.instance.buyInPopUp.activeSelf)
                {
                    P_InGameUiManager.instance.buyInErrorText.text = data["message"].ToString();
                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(3f, () =>
                    {
                        P_InGameUiManager.instance.buyInErrorText.text = "";
                    }));
                    if (P_BuyinPopup.instance != null)
                    {
                        P_BuyinPopup.instance.buyInButton.interactable = true;
                        P_BuyinPopup.instance.buyInCloseButton.interactable = true;
                    }
                }
            }
        }
    }


    public void OnUserStackSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i] != null)
            {
                for (int j = 0; j < playersScript.Count; j++)
                {
                    P_Players pl = playersScript[j];

                    if (!string.IsNullOrEmpty(pl.playerData.userId))
                    {
                        if (pl.playerData.userId == data[i]["userId"].ToString())
                        {
                            float stackSize = float.Parse(data[i]["stackSize"].ToString());
                            pl.playerData.balance = stackSize;
                            pl.balance.text = data[i]["stackSize"].ToString();
                            pl.playerData.minRaise = float.Parse(data[i]["minRaise"].ToString());
                            pl.playerData.maxRaise = float.Parse(data[i]["maxRaise"].ToString());

                            if (P_SocketController.instance.isTopUpSended)
                            {

                                P_SocketController.instance.isTopUpSended = false;

                                if (P_InGameUiManager.instance.buyInPopUp.activeSelf)
                                {
                                    P_InGameUiManager.instance.buyInPopUp.SetActive(false);
                                    P_InGameUiManager.instance.isCallFromMenu = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    public void OnActionByUserSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);
        IDictionary iData = data as IDictionary;

        if (iData.Contains("userId") && iData.Contains("action"))
        {

            for (int j = 0; j < playersScript.Count; j++)
            {
                if (playersScript[j].GetPlayerData().userId == data["userId"].ToString())
                {
                    playersScript[j].UpdateLastAction(data["action"].ToString());
                    if (data["action"].ToString() == "fold")
                    {
                        playersScript[j].GetPlayerData().isFold = true;
                        if ((playersScript[j].GetPlayerData().userId == P_SocketController.instance.gamePlayerId) && (P_SocketController.instance.gamePlayerId == data["userId"].ToString()))
                        {
                            P_InGameUiManager.instance.FoldLoginPlayers(P_SocketController.instance.gamePlayerId);

                            // for auto-fold login player
                            actionButtons[0].GetComponent<Button>().interactable = false;
                            actionBtnParent.SetActive(false);
                        }
                        else if ((playersScript[j].GetPlayerData().userId == data["userId"].ToString()) && (data["userId"].ToString() != P_SocketController.instance.gamePlayerId))
                        {
                            playersScript[j].playerData.sixCards[0].transform.parent.gameObject.SetActive(false);
                            P_InGameUiManager.instance.FoldLoginPlayers(data["userId"].ToString());
                        }
                    }

                    if (iData.Contains("amount"))
                    {

                        int amountInt = 0;
                        if (int.TryParse(data["amount"].ToString(), out amountInt))
                        {

                            if (amountInt > 0)
                            {
                                if (P_GameConstant.enableLog)
                                    Debug.Log("RAISE AMOUNT: " + amountInt);
                                playersScript[j].betAmount.SetActive(true);
                                playersScript[j].betAmount.transform.GetChild(0).GetComponent<Text>().text = data["amount"].ToString();
                            }
                        }
                    }
                    break;
                }
            }
        }
        else
        {
            if (P_GameConstant.enableLog)
                Debug.Log("ACTION_BY_USER error: userId or action not found");
        }
    }


    public void OnTurnChangedSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);
        IDictionary iData = data as IDictionary;

        int tempBet = 0;
        PlayerTimerReset();

        if (iData.Contains("callAmount"))
        {
            tempBet = Int32.Parse(data["callAmount"].ToString());
            if (tempBet > 0) // jab check ho to user k top me chips 0 show ho rahe the uske liye condition lagaya
            {
                P_SocketController.instance.currentBet = tempBet;
                actionButtons[3].transform.GetChild(1).GetComponent<Text>().text = P_SocketController.instance.currentBet.ToString();
            }
        }

        if (iData.Contains("userId"))
        {
            P_SocketController.instance.currentTurnUserId = data["userId"].ToString();

            if (P_SocketController.instance.currentTurnUserId == P_SocketController.instance.gamePlayerId) // current login player turn
            {
                ToggleSuggestionButton(false);
            }
            else
            {
                ToggleActionButton(false);
            }
        }
        else
        {
            suggestionBtnParent.SetActive(true);
            actionBtnParent.SetActive(false);
        }
        raisePopUp.SetActive(false);

        P_InGameUiManager.instance.StopIdleTimerFunc();
        P_InGameUiManager.instance.IdleTimerFunc(P_SocketController.instance.currentTurnUserId);
    }


    public void OnActionsSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        P_SuggestionActions selectedSuggestionAction = GetSelectedSuggestionAction();
        ResetSuggetionAction();

        bool isFoldInclude = false;
        bool isCheckInclude = false;
        bool isRaiseInclude = false;
        bool isCallInclude = false;
        bool isAllInInclude = false;
        bool isBetInclude = false;
        if (data.Count > 0)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] != null)
                {
                    if (data[i].Equals("fold"))
                    {
                        isFoldInclude = true;
                    }
                    if (data[i].Equals("check"))
                    {
                        isCheckInclude = true;
                    }
                    if (data[i].Equals("raise"))
                    {
                        isRaiseInclude = true;
                    }
                    if (data[i].Equals("call"))
                    {
                        isCallInclude = true;
                    }
                    if (data[i].Equals("allin"))
                    {
                        isAllInInclude = true;
                    }
                    if (data[i].Equals("bet"))
                    {
                        isBetInclude = true;
                    }
                }
            }
        }

        if (selectedSuggestionAction != P_SuggestionActions.Null)
        {
            switch (selectedSuggestionAction)
            {
                case P_SuggestionActions.Fold:
                    {
                        if (isFoldInclude)
                            OnPlayerActionCompleted(P_SuggestionActions.Fold, 0, "Fold");
                        ResetSuggestionButtonsActiveImage();
                    }
                    break;

                case P_SuggestionActions.Check:
                    {
                        if (isCheckInclude)
                            OnPlayerActionCompleted(P_SuggestionActions.Check, 0, "Check");
                        ResetSuggestionButtonsActiveImage();
                    }
                    break;

                case P_SuggestionActions.Call:
                    {
                        if (isCallInclude)
                            OnPlayerActionCompleted(P_SuggestionActions.Call, 0, "Call");

                        ResetSuggestionButtonsActiveImage();

                        if (!isCallInclude && isCheckInclude)
                            ActionButtonElse(data);

                    }
                    break;

                case P_SuggestionActions.Call_Any:
                    {
                        if (isCallInclude)
                            OnPlayerActionCompleted(P_SuggestionActions.Call, 0, "Call");

                        ResetSuggestionButtonsActiveImage();

                        if (!isCallInclude && isCheckInclude)
                            ActionButtonElse(data);

                    }
                    break;
            }
        }
        else
        {
            ActionButtonElse(data);
        }
    }


    void ActionButtonElse(JsonData data)
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].GetComponent<Button>().interactable = true;
        }

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i] != null)
            {
                if (data[i].Equals("fold"))
                {
                    actionButtons[0].SetActive(true);
                }
                else if (data[i].Equals("check"))
                {
                    actionButtons[1].SetActive(true);
                }
                else if (data[i].Equals("raise"))
                {
                    actionButtons[2].SetActive(true);
                }
                else if (data[i].Equals("call"))
                {
                    actionButtons[3].SetActive(true);
                }
                else if (data[i].Equals("allin"))
                {
                    actionButtons[4].SetActive(true);
                }
                else if (data[i].Equals("bet"))
                {
                    actionButtons[2].SetActive(true);
                }
            }
        }
        actionBtnParent.SetActive(true);
        suggestionBtnParent.SetActive(false);
        actionPanelAnimator.SetBool("isOpen", true);
    }


    public void OnSuggestionActionSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        for (int i = 0; i < suggestionButtons.Length; i++)
        {
            suggestionButtons[i].SetActive(false);
        }

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i] != null)
            {
                if (P_GameConstant.enableLog)
                    Debug.Log("suggestionPanel: " + data[i]);

                if (data[i].Equals("fold"))
                {
                    suggestionButtons[2].SetActive(true);
                }
                else if (data[i].Equals("check"))
                {
                    suggestionButtons[3].SetActive(true);
                }
                else if (data[i].Equals("call"))
                {
                    suggestionButtons[0].SetActive(true);
                }
                else if (data[i].Equals("call any"))
                {
                    suggestionButtons[1].SetActive(true);
                }
            }
        }
        actionBtnParent.SetActive(false);
        suggestionBtnParent.SetActive(true);
        suggestionPanelAnimator.SetBool("isOpen", true);
    }


    public void OnWinnerSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        bool showCards, folded, left, isLoginWinner = false;
        string winLoseAmount = "";

        P_InGameUiManager.instance.ResetPlayersUI();
        actionBtnParent.SetActive(false);
        suggestionBtnParent.SetActive(false);
        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.5f, () => {
            P_InGameUiManager.instance.ResetLastAction();
            holeCardsTemp = null;
        }));
        ResetSuggetionAction();
        ResetSuggestionButtonsActiveImage();
        P_InGameUiManager.instance.ResetHandMeterIcons();

        for (int i = 0; i < data["winners"].Count; i++)
        {
            int tempI = i;
            if (data["winners"][i] != null)
            {
                for (int j = 0; j < playersScript.Count; j++)
                {
                    int tempJ = j;
                    P_Players pl = playersScript[tempJ];

                    if (pl.GetPlayerData().userId == data["winners"][i]["userId"].ToString())
                    {
                        if (pl.GetPlayerData().userId == PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            // self login user winner
                            isLoginWinner = true;
                            winLoseAmount = data["winners"][tempI]["winAmount"].ToString();
                        }
                        else
                        {
                            // opponent winner
                            float pos = 0f;
                            for (int k = 0; k < data["winners"][i]["holeCards"].Count; k++)
                            {
                                P_CardData cardData = P_CardsManager.instance.GetCardData(
                                            data["winners"][i]["holeCards"][k]["_rank"].ToString() +
                                            data["winners"][i]["holeCards"][k]["_suit"].ToString()
                                            );

                                pl.playerData.sixCards[k].sprite = cardData.cardsSprite;
                                pl.playerData.sixCards[k].gameObject.SetActive(true);
                                pl.playerData.sixCards[k].transform.GetChild(0).gameObject.SetActive(false);
                                pl.playerData.sixCards[0].transform.parent.gameObject.SetActive(true);

                                if ((pl.currentSeat != "0") || (pl.gameObject.name == "0" && P_SocketController.instance.isViewer))
                                {
                                    pl.playerData.sixCards[0].transform.parent.DOScale(new Vector3(1.4f, 1.4f, 1.4f), GameConstants.CARD_ANIMATION_DURATION).SetEase(Ease.InOutBack);
                                    if (data["winners"][i]["holeCards"].Count == 2)
                                        pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(0, 5f, 0f);
                                    else if (data["winners"][i]["holeCards"].Count == 4)
                                        pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(-20, 5f, 0f);
                                    else if (data["winners"][i]["holeCards"].Count == 5)
                                        pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(-30, 5f, 0f);
                                    else if (data["winners"][i]["holeCards"].Count == 6)
                                        pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(-34, 5f, 0f);
                                    pos += 10f;
                                    pl.playerData.sixCards[k].transform.localPosition = new Vector3(pos, 0f, 0f);
                                }
                            }

                            // new condition: winner me muck text show nahi karvana hain
                            //if ((bool)data["winners"][i]["isMuckEnabled"] == true)
                            //{
                            //    pl.UpdateLastAction("");
                            //    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.5f, () =>
                            //    {
                            //        pl.muckBG.SetActive(true);
                            //        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(3f, () => {
                            //            pl.muckBG.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
                            //            {
                            //                pl.muckBG.SetActive(false);
                            //                pl.muckBG.GetComponent<Image>().color = Color.white;
                            //            });
                            //        }));
                            //    }));
                            //}
                        }

                        InstantiateWin(data["winners"][tempI]["userId"].ToString(), data["winners"][tempI]["id"].ToString(), data["winners"][tempI]["winAmount"].ToString(), true);
                    }
                }
            }
        }


        for (int i = 0; i < data["other"].Count; i++)
        {
            if (data["other"][i] != null)
            {
                for (int j = 0; j < playersScript.Count; j++)
                {
                    int tempJ = j;
                    P_Players pl = playersScript[tempJ];

                    if (pl.GetPlayerData().userId == data["other"][i]["userId"].ToString())
                    {
                        if (pl.GetPlayerData().userId == PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            // no need self login user show card

                            if (data["other"][i]["stackSize"].ToString() == "0")
                            {
                                P_SocketController.instance.isMyBalanceZero = true;
                                if (P_SocketController.instance.gameTypeName != "SIT N GO")
                                {
                                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(5f, () =>
                                    {
                                        P_InGameUiManager.instance.isTopUp = true;
                                        P_InGameUiManager.instance.p_BuyinPopup.ShowBuyInPopup(true);  //P_InGameUiManager.instance.ShowBuyInPopup(true);
                                    }));
                                }
                            }
                        }
                        else
                        {
                            // opponent show card
                            if ((bool)data["other"][i]["folded"] == false)
                            {
                                float pos = 0f;
                                for (int k = 0; k < data["other"][i]["holeCards"].Count; k++)
                                {
                                    P_CardData cardData = P_CardsManager.instance.GetCardData(
                                                data["other"][i]["holeCards"][k]["_rank"].ToString() +
                                                data["other"][i]["holeCards"][k]["_suit"].ToString()
                                                );

                                    pl.playerData.sixCards[k].sprite = cardData.cardsSprite;
                                    pl.playerData.sixCards[k].gameObject.SetActive(true);
                                    pl.playerData.sixCards[k].transform.GetChild(0).gameObject.SetActive(false);
                                    pl.playerData.sixCards[0].transform.parent.gameObject.SetActive(true);

                                    if ((pl.currentSeat != "0") || (pl.gameObject.name == "0" && P_SocketController.instance.isViewer))
                                    {
                                        pl.playerData.sixCards[0].transform.parent.DOScale(new Vector3(1.4f, 1.4f, 1.4f), GameConstants.CARD_ANIMATION_DURATION).SetEase(Ease.InOutBack);
                                        if (data["other"][i]["holeCards"].Count == 2)
                                            pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(0, 5f, 0f);
                                        else if (data["other"][i]["holeCards"].Count == 4)
                                            pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(-20, 5f, 0f);
                                        else if (data["other"][i]["holeCards"].Count == 5)
                                            pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(-30, 5f, 0f);
                                        else if (data["other"][i]["holeCards"].Count == 6)
                                            pl.playerData.sixCards[0].transform.parent.transform.localPosition = new Vector3(-34, 5f, 0f);
                                        pos += 10f;
                                        pl.playerData.sixCards[k].transform.localPosition = new Vector3(pos, 0f, 0f);
                                    }
                                }
                                if ((bool)data["other"][i]["isMuckEnabled"] == true)
                                {
                                    pl.UpdateLastAction("");
                                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.5f, () =>
                                    {
                                        pl.muckBG.SetActive(true);
                                        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(3f, () => {
                                            pl.muckBG.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
                                            {
                                                pl.muckBG.SetActive(false);
                                                pl.muckBG.GetComponent<Image>().color = Color.white;
                                            });
                                        }));
                                    }));
                                }
                            }
                        }
                    }
                }
            }
        }

        //if (P_SocketController.instance.gameTypeName == "SIT N GO")
        //{
        //    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(5f, () =>
        //    {
        //        P_InGameUiManager.instance.ShowScreen(P_InGameScreens.SitNGoWinnerLooser);
        //        if (P_SitNGoWinnerLooser.instance != null)
        //        {
        //            if (isLoginWinner)
        //            {
        //                P_SitNGoWinnerLooser.instance.SetWinner(winLoseAmount);
        //            }
        //            else
        //            {
        //                P_SitNGoWinnerLooser.instance.SetLooser("");
        //            }
        //        }
        //    }));
        //}
    }

    #endregion



    public string ScoreShow(int Score)
    {
        float Scor = Score;
        string result;
        string[] ScoreNames = new string[] { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
        int i;

        for (i = 0; i < ScoreNames.Length; i++)
            if (Scor < 10000)
                break;
            else Scor = Mathf.Floor(Scor / 100f) / 10f;

        if (Scor == Mathf.Floor(Scor))
            result = Scor.ToString() + ScoreNames[i];
        else result = Scor.ToString("F1") + ScoreNames[i];
        return result;
    }

    public void DealerIconSetTrue(string dataStr)
    {
        P_InGameUiManager.instance.DealerIconAllFalse();

        JsonData data = JsonMapper.ToObject(dataStr);

        for (int i = 0; i < playersScript.Count; i++)
        {
            if (playersScript[i].playerData.userId == data["userId"].ToString())
            {
                playersScript[i].dealer.SetActive(true);
            }
        }
    }

    public void BestHandText(string dataStr)
    {
        for (int i = 0; i < playersScript.Count; i++)
        {
            if (playersScript[i].GetPlayerData().userId == PlayerManager.instance.GetPlayerGameData().userId)
            {
                string trimResult = string.Empty;
                trimResult = dataStr.Trim('"');
                playersScript[i].realTimeResult.text = trimResult;
                if (!playersScript[i].realTimeResult.gameObject.activeSelf)
                    playersScript[i].realTimeResult.gameObject.SetActive(true);
                playersScript[i].realTimeResult.DOFade(0f, 1f).From().SetEase(Ease.OutQuad);
                P_InGameUiManager.instance.UpdateHandRankFrame(trimResult);
            }
        }
    }



    public void InstantiateWin(string userId, string name, string winAmount, bool isWin)
    {
        P_Players winnerPlayer = GetPlayerObject(userId);

        if (winnerPlayer != null)
        {
            WinnersNameText.text += "[username=" + winnerPlayer.playerData.userName +
                                    ",userId=" + winnerPlayer.playerData.userId + "] ";

            for (int i = 0; i < animationLayer.childCount; i++)
            {
                Destroy(animationLayer.GetChild(i).gameObject);
            }

            GameObject gm = Instantiate(winningPrefab, animationLayer) as GameObject;
            if (isWin)
            {
                gm.transform.Find("WinBy").GetComponent<Text>().text = name;
                gm.transform.Find("winAmount").GetComponent<Text>().text = "+" + winAmount; //ScoreShow(int.Parse(winAmount));
                if (string.IsNullOrEmpty(name))
                {
                    gm.transform.Find("WinBy").gameObject.SetActive(false);
                    gm.transform.Find("Image").gameObject.SetActive(false);
                }
                else
                {
                    gm.transform.Find("WinBy").gameObject.SetActive(true);
                    gm.transform.Find("Image").gameObject.SetActive(true);
                }
                if (winAmount.ToCharArray().Length > 5)
                {
                    SoundManager.instance.PlaySound(SoundType.bigWin);
                }
                gm.transform.position = winnerPlayer.gameObject.transform.position;
                gm.transform.SetParent(winnerPlayer.gameObject.transform.GetChild(0).transform);
                gm.transform.SetSiblingIndex(0);
                Vector3 inititalScale = gm.transform.localScale;
                gm.transform.localScale = Vector3.zero;
            }
            else
            {
                gm.SetActive(false);
            }
            StartCoroutine(WaitAndShowWinnersAnimation(winnerPlayer, winAmount, gm));
        }
    }


    private IEnumerator WaitAndShowWinnersAnimation(P_Players playerScript, string betAmount, GameObject amount)
    {
        winnerAnimationFound = true;
        yield return new WaitForSeconds(.6f);

        //SoundManager.instance.PlaySound(SoundType.IncomingPot);

        GameObject gm = Instantiate(chipscoin, WinAnimationpos.transform) as GameObject;

        gm.transform.position = WinAnimationpos.transform.position;


        gm.transform.DOMove(playerScript.transform.position, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            //SoundManager.instance.PlaySound(SoundType.Bet);
            amount.transform.DOScale(Vector3.one, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
            Destroy(gm);
        });
        yield return new WaitForSeconds(3f);
        amount.transform.DOScale(Vector3.zero, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.5f);
        winnerAnimationFound = false;
        Destroy(amount);
        P_InGameUiManager.instance.HideCardsAndMsg();
        P_InGameUiManager.instance.HideAllPots();
        P_InGameUiManager.instance.potAmountText.text = "";
        P_InGameUiManager.instance.potAmountText.gameObject.SetActive(true);
    }

    public void ShowChatOnPlayer(string serverResponse)
    {
        JsonData jsonData = JsonMapper.ToObject(serverResponse);
        for (int i = 0; i < playersScript.Count; i++)
        {
            if (playersScript[i].playerData.userId == jsonData[0]["userId"].ToString())
            {
                Debug.Log("Seat " + playersScript[i].currentSeat);
                switch (playersScript[i].currentSeat)
                {
                    case "1":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(33f, 80f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(111f, 60f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 5f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(102f, 40f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconTopBottom;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, 0f);
                        break;
                    case "2":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(112f, 12f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(126f, 51f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(7.5f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(101f, 42f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconLeftRight;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, 0f);
                        break;
                    case "3":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(112f, -7f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(126f, 51f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(7.5f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(101f, 42f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconLeftRight;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(180f, 0f, 0f);
                        break;
                    case "4":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(121f, -75f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(126f, 51f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(7.5f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(102f, 42f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconLeftRight;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(180f, 0f, 0f);
                        break;
                    case "5":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(35f, -121f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(111f, 60f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 5f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(101f, 42f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconTopBottom;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(180f, 0f, 0f);
                        break;
                    case "6":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-121f, -75f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(126f, 51f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(7.5f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(101f, 42f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconLeftRight;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, 180f);
                        break;
                    case "7":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-112f, -7f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(126f, 51f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(7.5f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(101f, 42f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconLeftRight;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, 180f);
                        break;
                    case "8":
                        playersScript[i].chatObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-112f, 12f);
                        playersScript[i].chatObj.GetComponent<RectTransform>().sizeDelta = new Vector2(126f, 51f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(7.5f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(101f, 42f);
                        playersScript[i].chatObj.GetComponent<Image>().sprite = chatIconLeftRight;
                        playersScript[i].chatObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                        playersScript[i].chatObj.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 180f, 0f);
                        break;
                }

                if (jsonData[0]["message"].ToString().Length > 22)
                    playersScript[i].chatObj.transform.GetChild(0).GetComponent<Text>().text = jsonData[0]["message"].ToString().Substring(0, 20) + "..";
                else
                    playersScript[i].chatObj.transform.GetChild(0).GetComponent<Text>().text = jsonData[0]["message"].ToString();

                playersScript[i].chatObj.SetActive(true);

                StartCoroutine(HideChatOnPlayer(playersScript[i].chatObj));
            }
        }
    }

    IEnumerator HideChatOnPlayer(GameObject chatObj)
    {
        yield return new WaitForSeconds(2f);
        chatObj.SetActive(false);
    }
}

public class MatchMakingPlayerData
{
    public P_PlayerData playerData;
    public bool isTurn;
    public bool isCheckAvailable;
    public bool isNull;
    public string playerType;
}