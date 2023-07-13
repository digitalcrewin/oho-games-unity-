using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class P_HandHistory : MonoBehaviour
{
    public static P_HandHistory instance;

    public GameObject LoadingText, logText;
    public GameObject DetailsPanel, SummaryPanel;
    public GameObject DetailsButtonImage, SummaryButtonImage;
    public GameObject HandDetailRoundPrefab, PlayerDetailsPrefab, PotDetailsPrefab, evChopPrefab, HandSummaryItemPrefab, roundNamePrefab;
    public Transform HandDetailsContent, HandSummaryContent;

    private int pageNo = 0;
    private int totalPages;
    private AllHandHistroy histories;

    public Text PageNoText;
    public Text showText;
    public Button ButtonNext;
    public Button ButtonBack;
    public Button OpenSlider;

    public Button OpenHandDetailsButton;
    public Button OpenHandSummaryButton;

    public GameObject Slider;

    public Text TimeText;
    public Text SB_BB;
    public Text HandID;

    public Text DebugText;

    public Image[] communityCardsSummary;
    public Image[] communityCardsDetails;
    public Text totalPotTextSummary;
    public Text totalPotTextDetails;


    public int handHistoryTotalCount;
    public int handHistoryCurrentPageCount;
    public int handHistoryPageCount;

    [Space(10)]
    public Text errorText;

    private void Awake()
    {
        instance = this;
        Init();
    }

    private void Start()
    {
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "close":
                {
                    if (P_InGameUiManager.instance != null)
                        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.HandHistory);
                }
                break;

            case "summary":
                OpenScreen("Summary");
                break;

            case "details":
                OpenScreen("Details");
                break;

            case "previousButton":
                if((handHistoryCurrentPageCount + 1) == handHistoryTotalCount)
                {
                    ButtonBack.interactable = false;
                }
                else
                {
                    ButtonBack.interactable = true;
                }

                OnGetTableHandHistoryDetails(handHistoryCurrentPageCount + 1); //handHistoryTotalCount
                break;

            case "nextButton":
                if (handHistoryCurrentPageCount > 1)
                {
                    ButtonBack.interactable = true;
                }

                OnGetTableHandHistoryDetails(handHistoryCurrentPageCount - 1); //handHistoryTotalCount
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
                }
                break;
        }
    }

    private void IncreasePage()
    {
        pageNo++;
        if (pageNo > totalPages - 1)
        {
            pageNo = totalPages - 1;
        }
        int val = totalPages - 1;
        PageNoText.text = pageNo + "/" + val;
        FillDetailsFromPageNo();
    }

    private void DecreasePage()
    {
        pageNo--;
        if (pageNo < 0)
        {
            pageNo = 0;
        }
        int val = totalPages - 1;
        PageNoText.text = pageNo + "/" + val;
        FillDetailsFromPageNo();
    }

    public void Init()
    {
        OpenHandDetailsButton.onClick.RemoveAllListeners();
        OpenHandSummaryButton.onClick.RemoveAllListeners();
        OpenHandDetailsButton.onClick.AddListener(() => OpenScreen("Details"));
        OpenHandSummaryButton.onClick.AddListener(() => OpenScreen("Summary"));

        OpenScreen("Summary");
    }


    private P_HandSummaryItemControl Phandsummary;
    private P_HandSummaryItemControl PhandDetailssummary;
    private P_PlayerDeatilsControl pPlayerDetailsControl;

    int counter = 0;


    public void OnGetTableHandHistoryDetails(int handHistoryTotalCountLocal)
    {
        if (!P_WebServices.instance.IsInternetAvailable())
        {
            errorText.text = "No Internet";
            errorText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
                errorText.text = "LOADING";
                errorText.transform.parent.gameObject.SetActive(true);
                Debug.Log("handHistoryTotalCount: " + handHistoryTotalCountLocal);

                StartCoroutine(P_WebServices.instance.GETRequestDataURL(P_GameConstant.GAME_URLS[(int)P_RequestType.PokerHandHistory] + P_SocketController.instance.TABLE_ID + "?" + "page=" + handHistoryTotalCountLocal, (string serverResponse, bool isErrorMessage, string errorString) =>  //currentpage moklvanu thai
                {
                    errorText.text = "";
                    errorText.transform.parent.gameObject.SetActive(false);
                    if (isErrorMessage)
                    {
                        Debug.Log("error=" + errorString);
                        errorText.text = "Error from server";
                        errorText.transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        handHistoryTotalCount = handHistoryTotalCountLocal;

                        Debug.Log("Response ========> GetTableHandHistory :" + serverResponse);
                        JsonData data = JsonMapper.ToObject(serverResponse);

                        HandID.text = data["data"]["table_round_id"].ToString();
                        totalPotTextSummary.text = data["data"]["potTotal"].ToString();
                        totalPotTextDetails.text = data["data"]["potTotal"].ToString();
                        SB_BB.text = "(" + data["data"]["table_attributes"]["smallBlind"].ToString() + "/" + data["data"]["table_attributes"]["bigBlind"].ToString() + " " + "HOLD’EM)";
                        PageNoText.text = data["data"]["currentPage"].ToString() + " / " + data["data"]["count"].ToString();

                        handHistoryPageCount = (int)data["data"]["page"];
                        handHistoryCurrentPageCount = int.Parse(data["data"]["currentPage"].ToString());
                        handHistoryTotalCount = (int)data["data"]["count"];
                        PageNoText.text = (handHistoryTotalCount - (handHistoryTotalCountLocal - 1)) + " / " + data["data"]["count"].ToString();

                        showText.text = "Showing last " + data["data"]["count"].ToString() + " hands";  //Showing last 20 hands.


                        if (handHistoryTotalCount == handHistoryPageCount)
                        {
                            Debug.Log("Button next false.");
                            ButtonNext.interactable = false;
                        }
                        else
                        {
                            Debug.Log("Button next true.");
                            ButtonNext.interactable = true;
                        }

                        for (int i = 0; i < HandSummaryContent.transform.childCount; i++)  //hand summary mate
                        {
                            if (HandSummaryContent.transform.GetChild(i).name.Contains("HandSummaryItemPrefab"))
                                Destroy(HandSummaryContent.transform.GetChild(i).gameObject);
                        }

                        for (int i = 0; i < HandDetailsContent.transform.childCount; i++)  //hand summary mate
                        {
                            if (HandDetailsContent.transform.GetChild(i).name.Contains("HandSummaryItemPrefab"))
                                Destroy(HandDetailsContent.transform.GetChild(i).gameObject);
                        }

                        for (int i = 0; i < data["data"]["table_attributes"]["players"].Count; i++) //hand summary mate
                        {
                            if (data["data"]["table_attributes"]["players"] != null)
                            {
                                int tempi = i;
                                GameObject go = Instantiate(HandSummaryItemPrefab, HandSummaryContent.transform);  //jetla players hoi etla instantiate thai.
                                Phandsummary = go.GetComponent<P_HandSummaryItemControl>();
                                Phandsummary.PlayerNameText.text = data["data"]["table_attributes"]["players"][i]["userName"].ToString();

                                GameObject goDetails = Instantiate(HandSummaryItemPrefab, HandDetailsContent.transform);  //je player no data summary ma aave ee details ma pan aavse.
                                PhandDetailssummary = goDetails.GetComponent<P_HandSummaryItemControl>();
                                PhandDetailssummary.PlayerNameText.text = data["data"]["table_attributes"]["players"][i]["userName"].ToString();

                                //code sacho che pan check karvanu che.backend side thi ek player na card nathi aavta.
                                for (int j = 0; j < data["data"]["result_json"]["players"].Count; j++)  //i
                                {
                                    int tempj = j;
                                    IDictionary iPlayer = data["data"]["result_json"]["players"][tempj] as IDictionary;
                                    for (int k = 0; k < data["data"]["result_json"]["players"][tempj]["cards"].Count; k++)
                                    {
                                        int tempk = k;

                                        if (data["data"]["result_json"]["players"][tempj]["cards"] != null)  //players na cards show karva mate.
                                        {
                                            if (data["data"]["table_attributes"]["players"][tempi]["userId"].ToString() == data["data"]["result_json"]["players"][tempj]["userId"].ToString())
                                            {
                                                
                                                // new condition:
                                                //    data me X ho to card back sprite
                                                //    baki jo card value ho, wo cards show hoge
                                                if (
                                                    data["data"]["result_json"]["players"][tempj]["cards"][tempk]["cardRank"].ToString() == "X" ||
                                                    data["data"]["result_json"]["players"][tempj]["cards"][tempk]["cardSuit"].ToString() == "X"
                                                )
                                                {
                                                    Phandsummary.userCards[tempk].sprite = P_CardsManager.instance.cardBackSprite;
                                                    PhandDetailssummary.userCards[tempk].sprite = P_CardsManager.instance.cardBackSprite;
                                                }
                                                else
                                                {
                                                    P_CardData cardData = P_CardsManager.instance.GetCardData(
                                                    data["data"]["result_json"]["players"][tempj]["cards"][tempk]["cardRank"].ToString() +
                                                    data["data"]["result_json"]["players"][tempj]["cards"][tempk]["cardSuit"].ToString()
                                                    );
                                                    Phandsummary.userCards[tempk].sprite = cardData.cardsSprite;
                                                    PhandDetailssummary.userCards[tempk].sprite = cardData.cardsSprite;
                                                }

                                                Phandsummary.userCards[tempk].gameObject.SetActive(true);
                                                PhandDetailssummary.userCards[tempk].gameObject.SetActive(true);

                                                string betText = data["data"]["result_json"]["players"][tempj]["chips"].ToString();

                                                if (betText.Contains("-"))
                                                {
                                                    Phandsummary.BetText.color = new Color32(212, 80, 85, 255);
                                                    PhandDetailssummary.BetText.color = new Color32(212, 80, 85, 255);
                                                }
                                                else
                                                {
                                                    Phandsummary.BetText.color = new Color32(51, 218, 175, 255);
                                                    PhandDetailssummary.BetText.color = new Color32(51, 218, 175, 255);
                                                }

                                                Phandsummary.BetText.text = data["data"]["result_json"]["players"][tempj]["chips"].ToString(); //[tempk]
                                                PhandDetailssummary.BetText.text = data["data"]["result_json"]["players"][tempj]["chips"].ToString(); //[tempk]


                                                if ((bool)data["data"]["result_json"]["players"][tempj]["folded"] == true)  //for to show hand pair in handhistory.
                                                {
                                                    Phandsummary.FoldBG.SetActive(true);
                                                    PhandDetailssummary.FoldBG.SetActive(true);
                                                    Phandsummary.ActionTwoText.gameObject.SetActive(false);
                                                    PhandDetailssummary.ActionTwoText.gameObject.SetActive(false);
                                                }
                                                else if (iPlayer.Contains("isMuckEnabled"))
                                                {
                                                    if ((bool)data["data"]["result_json"]["players"][tempj]["isMuckEnabled"] == true)
                                                    {
                                                        Phandsummary.MuckBG.SetActive(true);
                                                        PhandDetailssummary.MuckBG.SetActive(true);
                                                        Phandsummary.ActionTwoText.gameObject.SetActive(false);
                                                        PhandDetailssummary.ActionTwoText.gameObject.SetActive(false);
                                                    }
                                                    else
                                                    {
                                                        Phandsummary.ActionTwoText.text = data["data"]["result_json"]["players"][tempj]["hand"].ToString();
                                                        PhandDetailssummary.ActionTwoText.text = data["data"]["result_json"]["players"][tempj]["hand"].ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    Phandsummary.ActionTwoText.text = data["data"]["result_json"]["players"][tempj]["hand"].ToString();
                                                    PhandDetailssummary.ActionTwoText.text = data["data"]["result_json"]["players"][tempj]["hand"].ToString();
                                                }

                                                Phandsummary.ActionOneText.text = data["data"]["result_json"]["players"][tempj]["playerPosition"].ToString();
                                                PhandDetailssummary.ActionOneText.text = data["data"]["result_json"]["players"][tempj]["playerPosition"].ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < HandDetailsContent.transform.childCount; i++)
                        {
                            if (HandDetailsContent.transform.GetChild(i).name.Contains("RoundNamePrefab"))
                            {
                                Destroy(HandDetailsContent.transform.GetChild(i).gameObject);
                                counter = 0;
                            }
                        }

                        //to destroy all the player details.
                        for (int i = 0; i < HandDetailsContent.transform.childCount; i++)
                        {
                            if (HandDetailsContent.transform.GetChild(i).name.Contains("PlayerlistDetailPrefab"))
                                Destroy(HandDetailsContent.transform.GetChild(i).gameObject);
                        }

                        //for display user details in details panel.
                        for (int i = 0; i < data["data"]["hand_histories"].Count; i++)
                        {
                            if (data["data"]["hand_histories"][i] != null)
                            {
                                GameObject goHandDetails = Instantiate(roundNamePrefab, HandDetailsContent.transform);
                                goHandDetails.transform.SetSiblingIndex(counter);
                                goHandDetails.transform.GetChild(0).GetComponent<Text>().text = data["data"]["hand_histories"][i]["bettingRound"].ToString();
                                counter++;

                                for (int j = 0; j < data["data"]["hand_histories"][i]["userBetRecords"].Count; j++)
                                {
                                    if (data["data"]["hand_histories"][i]["userBetRecords"] != null)
                                    {
                                        int tempj = j;
                                        GameObject goDetails = Instantiate(PlayerDetailsPrefab, HandDetailsContent.transform);

                                        goDetails.transform.SetSiblingIndex(counter);
                                        counter++;

                                        pPlayerDetailsControl = goDetails.GetComponent<P_PlayerDeatilsControl>();
                                        pPlayerDetailsControl.ActionOneText.text = data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString();

                                        for (int k = 0; k < data["data"]["table_attributes"]["players"].Count; k++) //hand summary mate che.je userid hoi enu j name aave.details prefab ma.
                                        {
                                            if (data["data"]["table_attributes"]["players"][k]["userId"].ToString() == data["data"]["hand_histories"][i]["userBetRecords"][tempj]["userId"].ToString())
                                            {
                                                pPlayerDetailsControl.PlayerNameText.text = data["data"]["table_attributes"]["players"][k]["userName"].ToString();
                                            }
                                        }

                                        pPlayerDetailsControl.ActionTwoText.text = data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString();
                                        pPlayerDetailsControl.BetText.text = data["data"]["hand_histories"][i]["userBetRecords"][tempj]["betAmount"].ToString();

                                        if (data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString() == "sb" || data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString() == "bb")
                                            pPlayerDetailsControl.ActionTwoImage.color = new Color32(45, 103, 253, 255);
                                        else if (data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString() == "call")
                                            pPlayerDetailsControl.ActionTwoImage.color = new Color32(90, 170, 77, 255);
                                        else if (data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString() == "raise")
                                            pPlayerDetailsControl.ActionTwoImage.color = new Color32(231, 130, 36, 255);
                                        else if (data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString() == "allin")
                                            pPlayerDetailsControl.ActionTwoImage.color = new Color32(114, 116, 181, 255);
                                        else if (data["data"]["hand_histories"][i]["userBetRecords"][tempj]["action"].ToString() == "fold")
                                            pPlayerDetailsControl.ActionTwoImage.color = new Color32(114, 116, 181, 255);
                                    }
                                }
                            }
                        }


                        for (int i = 0; i < data["data"]["result_json"]["communityCards"].Count; i++)
                        {
                            // API ma je data aave (Community Crads) ee display karvana che.
                            if (data["data"]["result_json"]["communityCards"][i] != null)
                            {
                                P_CardData cardData = P_CardsManager.instance.GetCardData(
                                        data["data"]["result_json"]["communityCards"][i]["cardRank"].ToString() +
                                        data["data"]["result_json"]["communityCards"][i]["cardSuit"].ToString()
                                        );

                                communityCardsDetails[i].sprite = cardData.cardsSprite;
                                communityCardsDetails[i].gameObject.SetActive(true);
                                communityCardsSummary[i].sprite = cardData.cardsSprite;
                                communityCardsSummary[i].gameObject.SetActive(true);
                                communityCardsDetails[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                                communityCardsSummary[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                            }
                        }
                    }
                }));
        }
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log("REspoNSE: " + serverResponse);
        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                Debug.LogError("ERror hand histry.........................................");
                P_InGameUiManager.instance.ShowMessage(errorMessage);
            }

            return;
        }

        if (requestType == RequestType.GetTableHandHistory)
        {
            Debug.Log("Response ========> GetTableHandHistory :" + serverResponse);
            logText.GetComponent<Text>().text = serverResponse;
            if (!string.IsNullOrEmpty(serverResponse))
            {
                histories = JsonUtility.FromJson<AllHandHistroy>("{\"histories\":" + serverResponse + "}");
                Debug.Log("histories list count:" + histories.histories.Length);
                FillDetailsFromPageNo();
            }
        }
    }

    

    private void OpenScreen(string screenName)
    {
        if (screenName == "Details")
        {
            DetailsPanel.SetActive(true);
            SummaryPanel.SetActive(false);
            SummaryButtonImage.SetActive(false);
            DetailsButtonImage.SetActive(true);
            OpenHandDetailsButton.transform.GetChild(0).GetComponent<Text>().color = new Color(0.90f, 0.76f, 0.14f);
            OpenHandSummaryButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f);
        }
        else
        {
            DetailsPanel.SetActive(false);
            SummaryPanel.SetActive(true);
            SummaryButtonImage.SetActive(true);
            DetailsButtonImage.SetActive(false);
            OpenHandSummaryButton.transform.GetChild(0).GetComponent<Text>().color = new Color(0.90f, 0.76f, 0.14f);
            OpenHandDetailsButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f);
        }
    }

    private IEnumerator LoadDemoCustomJSON(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("secret-key", "$2b$10$xXEJFkicTzXmf7wmrMCquuI/hP31tR8Wj/B1o0laX2Pf8tk20E9zq");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                histories = JsonUtility.FromJson<AllHandHistroy>("{\"histories\":" + webRequest.downloadHandler.text + "}");
                Debug.Log("histories list count:" + histories.histories.Length);
                FillDetailsFromPageNo();
            }
        }
    }

    private void ClearPage()
    {
        if (HandDetailsContent.childCount > 0)
        {
            for (int i = 0; i < HandDetailsContent.childCount; i++)
            {
                Destroy(HandDetailsContent.GetChild(i).gameObject);
            }
        }
        if (HandSummaryContent.childCount > 0)
        {
            for (int i = 0; i < HandSummaryContent.childCount; i++)
            {
                Destroy(HandSummaryContent.GetChild(i).gameObject);
            }
        }
    }

    private void FillDetailsFromPageNo()
    {
        ClearPage();

        totalPages = histories.histories.Length;
        int val = totalPages - 1;
        if (totalPages <= 0)
        {
            val = 0;
            totalPages = 0;
        }
        PageNoText.text = (pageNo + 1) + "/" + totalPages;
    }
}



[Serializable]
public class AllHandHistroy
{
    public HandHistoryData[] histories;
}

[Serializable]
public class HandHistoryData
{
    public List<HandSummary> handSummary = new List<HandSummary>();
    public HandDetails handDetails;
    public MatchDetails matchDetails;
    public string handId;
}

[Serializable]
public class PreFlop
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public double totalBet;
    public double totalCoins;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
    public List<evChop> evChop = new List<evChop>();
    public bool runItMultipleTimes;
    public List<comCard> comCards = new List<comCard>();
    public int runIt;
}

[Serializable]
public class PostFlop
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public double totalBet;
    public double totalCoins;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    //public List<Pot> sidePot = new List<Pot>();
    public double currentPot;
    public string seatName;
    public List<evChop> evChop = new List<evChop>();
    public bool runItMultipleTimes;
    public List<comCard> comCards = new List<comCard>();
    public int runIt;
}

[Serializable]
public class PostTurn
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public double totalBet;
    public double totalCoins;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
    public List<evChop> evChop = new List<evChop>();
    public bool runItMultipleTimes;
    public List<comCard> comCards = new List<comCard>();
    public int runIt;
}

[Serializable]
public class PostRiver
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public double totalBet;
    public double totalCoins;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
    public List<evChop> evChop = new List<evChop>();
    public bool runItMultipleTimes;
    public List<comCard> comCards = new List<comCard>();
    public int runIt;
}

[Serializable]
public class ShowDown
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public double totalBet;
    public double totalCoins;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
    public List<evChop> evChop = new List<evChop>();
    public bool runItMultipleTimes;
    public List<comCard> comCards = new List<comCard>();
    public int runIt;
}

[Serializable]
public class Details
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
}

[Serializable]
public class evChop
{
    public string action;
    public string amount;
    public string winPercent;
    public string potAmount;
}

[Serializable]
public class comCard
{
    public string c0;
    public string c1;
    public string c2;
    public string c3;
    public string c4;
}

[Serializable]
public class HandSummary
{
    public List<string> communityCard = new List<string>();
    public double betAmount;
    public double winAmount;
    public int userId;
    public bool isWin;
    public string userName;
    public int totalBet;
    public int totalCoins;
    public string name;
    public string seatName;
    public string discription;
    public List<string> possibleCards = new List<string>();
    public List<string> cards = new List<string>();
    public List<string> winningCards = new List<string>();
    public List<string> mergeCards = new List<string>();
    public string winBy;
    public string handStrength;
}

[Serializable]
public class HandDetails
{
    public List<PreFlop> PREFLOP = new List<PreFlop>();
    public List<PostFlop> POSTFLOP = new List<PostFlop>();
    public List<PostTurn> POSTTURN = new List<PostTurn>();
    public List<PostRiver> POSTRIVER = new List<PostRiver>();
    public List<ShowDown> SHOWDOWN = new List<ShowDown>();

}

[Serializable]
public class MatchDetails
{
    public string blind;
    public int players;
    public string bet;
    public int activePlayers;
    public string gameTime;
    public int totalBet;
}

