using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEditor;
using LitJson;
using System.Text;

public class R_CardManager : MonoBehaviour
{
    public static R_CardManager Instance;

    [Header("_______________GameObjects____________________")]
    public GameObject groupCreator;          //for now maximum 6 groups can be created
    public GameObject dummyCard;             //Dummy card will show on card move
    public GameObject group_Btn;
    public GameObject discard_Btn;
    public GameObject groupPrefab, cardPrefab;
    public GameObject showArea, dropArea, takeCardParent, takecardbtn; // sortingbtn;
    public GameObject Playerselect1, Playerselect2;

    [Header("_____________________Lists______________________")]
    [Tooltip("List of groups in which cards has been divided")]
    public List<R_GroupView> groupList = new List<R_GroupView>();
    public List<GameObject> Player_1_Card, drop_Card;
    public List<string> shufflesCard;
    [Tooltip("Cards that are selected for grouping")]
    public List<GameObject> selectedCards;
    [Tooltip("Add Cards That you want to draw when game starts")]
    public List<string> cardsToDraw;

    [Header("_________________Other Variables___________________")]
    public bool isShowingCards;
    public bool wholeGroupSelected;
    public int currentTurn, groupCount = 1, currnetShuffle_c = 13;
    public int selectedCardCount;
    public string Newcard;
    public RectTransform handCards;
    public Text debug_Txt;
    public Image jokerImg;
    private int lastNumber;

    [Header("_________________Sprite Array___________________")]
    public Sprite[] cardSpritesArray;

    public R_Task coTossedCards, coFirstPlayer;
    public string tempArrangedCard = string.Empty;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // GetFirstTurnOfPlayer();

        // to test manually
        //["TOSSED_CARDS",[{"playerId":"31","card":"JJ","rank":0},{"playerId":"49","card":"AH","rank":1}]]
        // SetTossedCards("[\"TOSSED_CARDS\",[{\"playerId\":\"1\",\"card\":\"JJ\",\"rank\":0},{\"playerId\":\"2\",\"card\":\"AH\",\"rank\":1}]]");

        //["HAND_CARDS",["QD","4S","8C","8C","5C","5D","AH","JD","5H","9H","JS","KS","7H"]]
        //[\"5H\",\"JS\",\"10D\",\"KH\",\"8H\",\"7H\",\"7D\",\"7H\",\"9S\",\"8C\",\"4S\",\"2D\",\"KH\"]]
        //[\"6D\",\"10C\",\"6C\",\"JJ\",\"JH\",\"KS\",\"5D\",\"9C\",\"5C\",\"9D\",\"QH\",\"6C\",\"7H\"]
        //[\"4H\",\"JH\",\"6S\",\"QS\",\"9D\",\"AS\",\"JC\",\"5S\",\"4H\",\"9C\",\"5D\",\"3D\",\"JS\"]
        // SetHandCards("[\"HAND_CARDS\",[\"4H\",\"JH\",\"6S\",\"QS\",\"9D\",\"AS\",\"JC\",\"5S\",\"4H\",\"9C\",\"5D\",\"3D\",\"JS\"] ]");
        // SetHandCards("[\"HAND_CARDS\",[[\"7S\",\"10S\",\"JS\",\"QS\",\"KS\"],[\"2H\",\"7H\",\"KH\"],[\"6D\",\"JD\"],[\"AC\",\"4D\",\"3H\"]]]");
    }


    // copied from GetFirstTurnOfPlayer()
//     public void SetTossedCards(string socketPacket)
//     {
//         // Debug.Log("SetTossedCards");

//         JsonData data = JsonMapper.ToObject(socketPacket);
//         int cardCount = data[1].Count;
//         if (cardCount > 0)
//         {
//             for (int i = 0; i < cardCount; i++)
//             {
//                 // Debug.Log("i="+i);
//                 // Debug.Log("playerId="+data[i]["playerId"].ToString());
//                 // Debug.Log("card="+data[1][i]["card"].ToString());
//                 // Debug.Log("rank="+data[i]["rank"].ToString());

//                 Playerselect1.SetActive(false);
//                 Playerselect2.SetActive(false);
//                 string currentCard = data[1][i]["card"].ToString();
// // Debug.Log("-- NUM="+num);
// // Debug.Log("-- suit="+suit);
//                 R_PlayerData playerData = new R_PlayerData();
//                 playerData.userId = data[1][i]["playerId"].ToString();
//                 // Rummy_MatchMakingPlayerData rmData = new Rummy_MatchMakingPlayerData();
//                 // rmData.playerData = playerData;

//                 if (R_SocketController.instance.rummyUserId==data[1][i]["playerId"].ToString())
//                 {
//                     Playerselect1.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(currentCard);
//                     Rummy_InGameManager.instance.allPlayersScript[0].InitPlayerData(playerData);
//                 }
//                 else
//                 {
//                     Rummy_InGameManager.instance.allPlayersObject[3].SetActive(true);
//                     Playerselect2.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(currentCard);
//                     Rummy_InGameManager.instance.allPlayersScript[3].InitPlayerData(playerData);
//                 }
//             }
//             coTossedCards = new R_Task(SetCardsFromTossedCards());
//             coTossedCards.Finished += delegate(bool manual) {
//                 if (!manual)
//                 {
//                     if (!string.IsNullOrEmpty(R_SocketController.instance.firstPlayerData))
//                     {
//                         SetFirstPlayer(R_SocketController.instance.firstPlayerData);
//                     }
//                     coTossedCards.Finished -= delegate(bool manual){};
//                 }
//             };
//         }
//     }


//     public void SetFirstPlayer(string socketPacket)
//     {
//         //["FIRST_PLAYER",{"playerId":"31","card":"JJ","rank":0}]
//         JsonData data = JsonMapper.ToObject(socketPacket);
//         int cardCount = data[1].Count;
//         if (cardCount > 0)
//         {
//             // Debug.Log("[playerId]="+data[1]["playerId"].ToString());
//             // Debug.Log("[card]="+data[1]["card"].ToString());
//             // Debug.Log("[rank]="+data[1]["rank"].ToString());
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
//             coFirstPlayer.Finished += delegate(bool manual) {
//                 if (!manual)
//                 {
//                     if (!string.IsNullOrEmpty(R_SocketController.instance.handCardsData))
//                     {
//                         SetHandCards(R_SocketController.instance.handCardsData);
//                     }
//                     if (!string.IsNullOrEmpty(R_SocketController.instance.deckInfoData))
//                     {
//                         SetDeckInfo(R_SocketController.instance.deckInfoData);
//                     }
//                     coFirstPlayer.Finished -= delegate(bool manual){};
//                 }
//             };
//         }
//     }


    // copied from setCards()
    public void SetHandCards(string socketPacket)
    {
        //["HAND_CARDS",["5H","JS","10D","KH","8H","7H","7D","7H","9S","8C","4S","2D","KH"]]
        //["HAND_CARDS",[["7S","10S","JS","QS","KS"],["2H","7H","KH"],["6D","JD"],["AC","4D","3H"]]]
        JsonData data = JsonMapper.ToObject(socketPacket);
        int cardCount = data[1].Count;
// Debug.Log("cardCount:"+cardCount);
        if (cardCount > 0)
        {
            for (int g = 0; g < handCards.childCount; g++)
            {
                Destroy(handCards.GetChild(g).gameObject);
            }
            groupList.Clear();
            Player_1_Card.Clear();

            if(cardCount == 13)
            {
                GameObject group = GameObject.Instantiate(groupPrefab, handCards);
                groupList.Add(group.GetComponent<R_GroupView>());
                group.name = "group0";

                for (int i = 0; i < cardCount; i++)
                {
    // Debug.Log("data[1][i]:"+data[1][i]);
                    Sprite cardSprite = null;
                    string currentCard = data[1][i].ToString();
                    int num = 0;
                    R_SUITS suit;
                    GetCardNumber(currentCard, out num, out suit);
                    cardSprite = GetSpriteFromSpriteArray(currentCard);
    // Debug.Log("cardSprite="+cardSprite);
                    Image cardImage = GameObject.Instantiate(cardPrefab, group.transform).GetComponent<Image>();
                    cardImage.sprite = cardSprite;
                    cardImage.gameObject.name = currentCard;
    // Debug.Log("cardImage obj name="+i.ToString());
                    Player_1_Card.Add(cardImage.gameObject);
                    cardImage.gameObject.GetComponent<R_CardView>().SetCardData(num, suit, dummyCard);
                }
                CheckForEmptyGroups();

                string cardJsonData = socketPacket;
                try { tempArrangedCard = cardJsonData.Replace("[\"HAND_CARDS\",",string.Empty).Replace("]]","]").Trim(); }
                catch(Exception e) {Debug.Log(e.Message);}
            }
            else
            {
                // Debug.Log("cardCount:"+cardCount);
                for (int i = 0; i < cardCount; i++)
                {
                    GameObject group = GameObject.Instantiate(groupPrefab, handCards);
                    groupList.Add(group.GetComponent<R_GroupView>());
                    group.name = "group"+i;

                    // Debug.Log("cardCount2."+i+": "+data[1][i].Count);
                    int cardCount3 = data[1][i].Count;
                    for (int j = 0; j < cardCount3; j++)
                    {
                    // Debug.Log("data[1][i][j]:"+data[1][i][j]);
                        Sprite cardSprite = null;
                        string currentCard = data[1][i][j].ToString();
                        int num = 0;
                        R_SUITS suit;
                        GetCardNumber(currentCard, out num, out suit);
                        cardSprite = GetSpriteFromSpriteArray(currentCard);
                    // Debug.Log("cardSprite="+cardSprite);
                        Image cardImage = GameObject.Instantiate(cardPrefab, group.transform).GetComponent<Image>();
                        cardImage.sprite = cardSprite;
                        cardImage.gameObject.name = currentCard;
                    // Debug.Log("cardImage obj name="+i.ToString());
                        Player_1_Card.Add(cardImage.gameObject);
                        cardImage.gameObject.GetComponent<R_CardView>().SetCardData(num, suit, dummyCard);
                    }
                    CheckForEmptyGroups();

                    string cardJsonData = socketPacket;
                    try { tempArrangedCard = cardJsonData.Replace("[\"HAND_CARDS\",",string.Empty).Replace("]]","]").Trim(); }
                    catch(Exception e) {Debug.Log(e.Message);}
                }
            }
            Rummy_InGameManager.instance.sortingBtn.SetActive(true);
            Rummy_InGameManager.instance.lastGameToggle.SetActive(true);
            Rummy_InGameManager.instance.chatBtn.interactable = true;
            Rummy_InGameUiManager.instance.ShowTableMessage(String.Empty);

            // Debug.Log("$$$$ tempArrangedCard="+tempArrangedCard);
            R_SocketController.instance.SendArrangedCards("{\"arrangedCards\":" + tempArrangedCard +"}");
        }
    }


    public void SetDeckInfo(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        int cardCount = data[1].Count;
// Debug.Log("SetDeckInfo cardCount:"+cardCount);
        if (cardCount > 0)
        {
// Debug.Log("SetDeckInfo card: "+data[1][0].ToString());
            jokerImg.sprite = GetSpriteFromSpriteArray(data[1][0].ToString());
            takeCardParent.SetActive(true);
            dropArea.SetActive(true);
            // sortingbtn.SetActive(true);
        }
    }

public bool isDiscardedCardFirstTime = false;
    public void SetDiscardedCardInfo(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        if (!isDiscardedCardFirstTime)
        {
            dropArea.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(data[1].ToString());
            isDiscardedCardFirstTime = true;
        }
        else if(isDiscardedCardFirstTime)
        {
            // dropArea.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(data[1]["cardValue"].ToString());
            Rummy_InGameManager.instance.AnimDiscardCard(data, GetSpriteFromSpriteArray(data[1]["cardValue"].ToString()));
        }

// Debug.Log("SetDiscardedCardInfo:"+data[1].ToString());
// Debug.Log("SetDiscardedCardInfo AFTER iDictionary = "+data[1].ToString().GetType());
//         try
//         {
//             Debug.Log("cardValue="+data[1]["cardValue"].ToString());
//         }
//         IDictionary iData = data[1] as IDictionary;
// Debug.Log("SetDiscardedCardInfo AFTER iDictionary Values = "+iData.Values.GetType());
//         // if (iData.Contains("cardValue"))
//         if (data[1].ToString().Contains("cardValue"))
//         {
//             Debug.Log("cardValue="+data[1]["cardValue"].ToString());
//             //dropArea.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(data[1]["cardValue"].ToString());
//         }
//         else
//         {
//             Debug.Log("cardValue="+data[1].ToString());
//             //dropArea.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(data[1].ToString());
//         }
    }


    // copied from GetNewCard()
    public void SetCardPicked(string socketPacket)
    {
        JsonData data = JsonMapper.ToObject(socketPacket);
        int cardCount = data[1].Count;
// Debug.Log("SetCardPicked cardCount:"+cardCount);
        if (cardCount > 0)  // ["CARD_PICKED",[]]
        {
            string currentCard = data[1][0].ToString();
// Debug.Log("SetCardPicked card: "+currentCard);
            Sprite cardSprite = GetSpriteFromSpriteArray(currentCard);
            Image cardImage = GameObject.Instantiate(cardPrefab, handCards.GetChild(handCards.childCount - 1).transform).GetComponent<Image>();
            cardImage.sprite = cardSprite;
            cardImage.gameObject.name = currentCard;
            Rummy_InGameManager.instance.cardPulledByMe = cardImage.gameObject;
// Debug.Log("$$$cardPulledByMe",Rummy_InGameManager.instance.cardPulledByMe);
            Player_1_Card.Add(cardImage.gameObject);
            int num = 0;
            R_SUITS suit;
            GetCardNumber(currentCard, out num, out suit);
            cardImage.gameObject.GetComponent<R_CardView>().SetCardData(num, suit, dummyCard);
            SendArrangedCard();
        }
    }

    public string GetArrangedCardFromHierarchy()
    {
        int childCount = handCards.childCount;
        string finalStr = string.Empty;

        for (int i = 0; i < childCount; i++)
        {
            // Debug.Log("subchild of i ("+i+") = "+ handCards.GetChild(i).childCount);
            int subChildCount = handCards.GetChild(i).childCount;
            if (subChildCount > 1)
            {
                StringBuilder sb = new StringBuilder();
                JsonWriter writer = new JsonWriter(sb);
                writer.WriteArrayStart();

                for (int j = 0; j < subChildCount; j++)
                {
                    if (j>0)
                    {
                        // Debug.Log("card name: "+handCards.GetChild(i).GetChild(j).name);
                        writer.Write(handCards.GetChild(i).GetChild(j).name);
                    }
                }
                writer.WriteArrayEnd();

                // Debug.Log("sb.ToString()="+sb.ToString());
                if (i==(childCount-1))
                { finalStr += sb.ToString(); }
                else { finalStr += sb.ToString() + ","; }
            }
        }
        return finalStr;
    }

    public void SendArrangedCard()
    {
        string finalStr = GetArrangedCardFromHierarchy();
        // Debug.Log("finalStr="+finalStr);
        // Debug.Log("tempArrangedCard="+tempArrangedCard);

        if (!string.IsNullOrEmpty(finalStr))
        {
            // Debug.Log("!string.IsNullOrEmpty(finalStr)");

            if (!finalStr.Equals(tempArrangedCard))
            {
                // Debug.Log("!finalStr.Equals(tempArrangedCard)");
                R_SocketController.instance.SendArrangedCards("{\"arrangedCards\":[" + finalStr +"]}");
                tempArrangedCard = finalStr;
            }
        }
    }

    public void DragOnShowCard()
    {
        string finalStr = GetArrangedCardFromHierarchy();

        if (!string.IsNullOrEmpty(finalStr))
        {
            // Debug.Log("final str = "+finalStr);
            R_SocketController.instance.SendFinishCards("{\"arrangedCards\":[" + finalStr +"]}");
            Rummy_InGameManager.instance.isFinishSend = true;
        }
    }





    void GetCardNumber(string currentCardFunc, out int num, out R_SUITS suit)
    {
        char[] cardFunc = currentCardFunc.ToCharArray(); // shufflesCard[i].ToCharArray();
        string numStr = currentCardFunc.Substring(0, cardFunc.Length - 1); //int num = int.Parse(shufflesCard[i].Substring(0, card.Length - 1));
        num = 0;
        if (numStr=="A")
        {
            num = 1;
        }
        else if (numStr=="J")
        {
            num = 11;
        }
        else if (numStr=="Q")
        {
            num = 12;
        }
        else if (numStr=="K")
        {
            num = 13;
        }
        else
        {
            num = int.Parse(numStr);
        }

        GetCardSuit(cardFunc, out suit);
    }

    void GetCardSuit(char[] cardFunc, out R_SUITS suit)
    {
        suit = R_SUITS.SPAIDS;
        switch (cardFunc[cardFunc.Length - 1])
        {
            case 'S':
                suit = R_SUITS.SPAIDS;
                break;
            case 'C':
                suit = R_SUITS.CLUBS;
                break;
            case 'H':
                suit = R_SUITS.HEARTS;
                break;
            case 'D':
                suit = R_SUITS.DIAMONDS;
                break;
            case 'J':
                suit = R_SUITS.JOKER;
                break;
        }
    }

    public void SetCardNotInteractable()
    {
        int childCount = handCards.childCount;

        for (int i = 0; i < childCount; i++)
        {
            // Debug.Log("subchild of i ("+i+") = "+ handCards.GetChild(i).childCount);
            int subChildCount = handCards.GetChild(i).childCount;
            if (subChildCount > 1)
            {
                for (int j = 0; j < subChildCount; j++)
                {
                    if (j>0)
                    {
                        // Debug.Log("card name: "+handCards.GetChild(i).GetChild(j).name);
                        handCards.GetChild(i).GetChild(j).GetComponent<Image>().raycastTarget = false;
                    }
                }
            }
        }
    }

    // copied from setCards()
    // public IEnumerator SetCardsFromTossedCards()
    // {
    //     yield return new WaitForSeconds(1f);
    //     Playerselect1.SetActive(true);
    //     Playerselect2.SetActive(true);
    //     yield return new WaitForSeconds(2f);
    //     Playerselect1.SetActive(false);
    //     Playerselect2.SetActive(false);

    //     // to test manually
    //     //["FIRST_PLAYER",{"playerId":"31","card":"JJ","rank":0}]
    //     // SetFirstPlayer("[\"FIRST_PLAYER\",{\"playerId\":\"31\",\"card\":\"JJ\",\"rank\":0}]");
    // }

    // void CallbackAfterFirstPlayerMsg()
    // {
    //     // SetHandCards("[\"HAND_CARDS\",[\"4H\",\"JH\",\"6S\",\"QS\",\"9D\",\"AS\",\"JC\",\"5S\",\"4H\",\"9C\",\"5D\",\"3D\",\"JS\"] ]");
    // }





    //Create new group prefab
    public Transform AddGroup()
    {
        //GameObject newGroup = Instantiate(groupPrefab, handCards);
        //newGroup.transform.SetAsFirstSibling();
        //groupList.Add(newGroup.GetComponent<GroupView>());
        // groupList.Add(GameObject.Instantiate(groupPrefab, handCards).GetComponent<R_GroupView>());  //MZ
        // GP
        GameObject newGroup = Instantiate(groupPrefab, handCards);
        groupList.Add(newGroup.GetComponent<R_GroupView>());
        newGroup.transform.SetAsFirstSibling();


        // sortingbtn.SetActive(false);
        // Rummy_InGameManager.instance.sortingBtn.SetActive(false);
        return groupList[groupList.Count - 1].gameObject.transform;
    }
    public void CheckForEmptyGroups()
    {
        for (int i = 0; i < groupList.Count; i++)
        {
            if (groupList[i].transform.childCount == 1)
            {
                Destroy(groupList[i].gameObject);
                groupList.RemoveAt(i);
            }
        }

        //   if(groupList.Count==1)
        //   {
        //       groupList[0].GetComponent<GridLayoutGroup>().constraintCount = 4;
        //   }
        //   else
        //   {
        //       groupList[0].GetComponent<GridLayoutGroup>().constraintCount = 1;
        //   }
    }
    int GetRandom(int min, int max)
    {
        int rand = Random.Range(min, max);
        while (rand == lastNumber)
            rand = Random.Range(min, max);
        lastNumber = rand;
        return rand;
    }
    // void GetFirstTurnOfPlayer()
    // {
    //     Getshuffle();
    //     Playerselect1.SetActive(false);
    //     Playerselect2.SetActive(false);
    //     int randomCard1 = GetRandom(0, 51);
    //     int randomCard2 = GetRandom(0, 51);
    //     if (randomCard1 == randomCard2)
    //     {
    //         randomCard2 = GetRandom(0, 51);
    //     }
    //     // Playerselect1.GetComponent<Image>().sprite = Resources.Load<Sprite>("Card/" + shufflesCard[randomCard1]);
    //     // Playerselect2.GetComponent<Image>().sprite = Resources.Load<Sprite>("Card/" + shufflesCard[randomCard2]);
    //     Playerselect1.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(shufflesCard[randomCard1]);
    //     Playerselect2.GetComponent<Image>().sprite = GetSpriteFromSpriteArray(shufflesCard[randomCard2]);
    //     if (randomCard1 >= randomCard2)
    //     {
    //         currentTurn = 0;
    //     }
    //     else
    //     {
    //         currentTurn = 1;
    //     }
    //     StartCoroutine(setCards());
    // }
    // Function which shuffle and
    // print the array
    // void Getshuffle()
    // {
    //     string[] a = {  "1s" ,"2s" , "3s" , "4s" , "5s" , "6s" , "7s" , "8s" , "9s" , "10s", "11s", "12s", "13s",
    //                     "14h","15h", "16h", "17h", "18h", "19h", "20h", "21h", "22h", "23h", "24h", "25h", "26h",
    //                     "27d","28d", "29d", "30d", "31d", "32d", "33d", "34d", "35d", "36d", "37d", "38d", "39d",
    //                     "40c","41c", "42c", "43c", "44c", "45c", "46c", "47c", "48c", "49c", "50c", "51c", "52c",
    //                     "53j", "54j"};
    //     for (int i = 0; i < a.Length; i++)
    //     {
    //         // Random for remaining positions.
    //         int r = i + Random.Range(0, 54 - i);
    //         //swapping the elements
    //         string temp = a[r];
    //         a[r] = a[i];
    //         a[i] = temp;
    //         shufflesCard.Add(a[i]);
    //     }
    // }
    public void onClickDropBtn()
    {
        if (!isShowingCards)
        {
            if (drop_Card.Count != 0 && Player_1_Card.Count == 13)
            {
                drop_Card[drop_Card.Count - 1].GetComponent<R_CardView>().GetDropCard();
                ActivateCreateGroupUI(false);
            }
        }
    }
    //Make All Cards With there no and other parameters
    // private IEnumerator setCards()
    // {
    //     yield return new WaitForSeconds(1f);
    //     Playerselect1.SetActive(true);
    //     Playerselect2.SetActive(true);
    //     yield return new WaitForSeconds(2f);
    //     Playerselect1.SetActive(false);
    //     Playerselect2.SetActive(false);
    //     showArea.SetActive(true);
    //     dropArea.SetActive(true);
    //     takecardbtn.SetActive(true);
    //     takeCardParent.SetActive(true);
    //     sortingbtn.SetActive(true);
    //     GameObject group;
    //     // this.cardsData = cardsData;
    //     group = GameObject.Instantiate(groupPrefab, handCards);
    //     //group.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector3.zero;        //Group text
    //     Debug.Log(group.transform.GetChild(0).name, group.transform.GetChild(0));
    //     groupList.Add(group.GetComponent<R_GroupView>());
    //     for (int i = 0; i < 13; i++) //i < 13currnetShuffle_c
    //     {
    //         Sprite cardSprite = null;
    //         R_SUITS suit = R_SUITS.SPAIDS;
    //         char[] card = shufflesCard[i].ToCharArray();
    //         //char[] card = cardsToDraw[i].ToCharArray();
    //         int num = int.Parse(shufflesCard[i].Substring(0, card.Length - 1));
    //         //int num = int.Parse(cardsToDraw[i].Substring(0, card.Length - 1));
    //         switch (card[card.Length - 1])
    //         {
    //             case 's':
    //                 suit = R_SUITS.SPAIDS;
    //                 break;
    //             case 'c':
    //                 suit = R_SUITS.CLUBS;
    //                 break;
    //             case 'h':
    //                 suit = R_SUITS.HEARTS;
    //                 break;
    //             case 'd':
    //                 suit = R_SUITS.DIAMONDS;
    //                 break;
    //             case 'j':
    //                 suit = R_SUITS.JOKER;
    //                 break;
    //         }
    //         cardSprite = GetSpriteFromSpriteArray(cardsToDraw[i]);
    //         //cardSprite = Resources.Load<Sprite>("Card/" + cardsToDraw[i]);//assets.GetCardSprite(SUITS.DIAMONDS, num);
    //         Image cardImage = GameObject.Instantiate(cardPrefab, group.transform).GetComponent<Image>();
    //         cardImage.sprite = cardSprite;
    //         cardImage.gameObject.name = i.ToString();
    //         Player_1_Card.Add(cardImage.gameObject);
    //         cardImage.gameObject.GetComponent<R_CardView>().SetCardData(num, suit, dummyCard);
    //         //cardImage.gameObject.SetActive(false);
    //         //StartCoroutine(WaitAndShowCardAnimation(cardImage, i,cardSprite, suit));
    //     }
    //     //GetCardShorting();
    //     CheckForEmptyGroups();
    // }

    //private IEnumerator WaitAndShowCardAnimation(Image img, int num,Sprite sp, SUITS suit)
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    //img.transform.DOMove(groupPrefab.transform.GetChild(num + 1).transform.position, 0.27f);
    //    //img.transform.DOScale(Vector3.one, 0.27f);
    //playerCards[j].transform.localScale

    //    groupPrefab.transform.GetChild(num);

    //    img.sprite = sp;
    //    img.gameObject.name = num.ToString();
    //    Player_1_Card.Add(img.gameObject);
    //    img.gameObject.GetComponent<CardView>().SetCardData(num, suit, dummyCard);
    //}
    // public void GetNewCard()
    // {
    //     if (!isShowingCards)
    //     {
    //         if (currnetShuffle_c + 1 <= shufflesCard.Count && Player_1_Card.Count == 13)
    //         {
    //             Sprite cardSprite = null;
    //             R_SUITS suit = R_SUITS.SPAIDS;
    //             char[] card = shufflesCard[currnetShuffle_c].ToCharArray();
    //             //char[] card = Newcard.ToCharArray();
    //             int num = int.Parse(shufflesCard[currnetShuffle_c].Substring(0, card.Length - 1));
    //             //int num = int.Parse(Newcard.Substring(0, card.Length - 1));
    //             switch (card[card.Length - 1])
    //             {
    //                 case 's':
    //                     suit = R_SUITS.SPAIDS;
    //                     break;
    //                 case 'c':
    //                     Debug.Log("____");
    //                     suit = R_SUITS.CLUBS;
    //                     break;
    //                 case 'h':
    //                     suit = R_SUITS.HEARTS;
    //                     break;
    //                 case 'd':
    //                     suit = R_SUITS.DIAMONDS;
    //                     break;
    //                 case 'j':
    //                     suit = R_SUITS.JOKER;
    //                     break;
    //             }
    //             cardSprite = cardSpritesArray[currnetShuffle_c];
    //             //cardSprite = Resources.Load<Sprite>("Card/" + Newcard);//assets.GetCardSprite(SUITS.DIAMONDS, num);
    //             Debug.Log(cardSprite.name, cardSprite);
    //             Image cardImage = GameObject.Instantiate(cardPrefab, handCards.GetChild(handCards.childCount - 1).transform).GetComponent<Image>();
    //             Debug.Log(cardImage.name, cardImage);
    //             cardImage.sprite = cardSprite;
    //             cardImage.gameObject.name = shufflesCard[currnetShuffle_c];
    //             //cardImage.gameObject.name = Newcard;
    //             Debug.Log(cardImage.gameObject.name, cardImage.gameObject);
    //             Player_1_Card.Add(cardImage.gameObject);
    //             cardImage.gameObject.GetComponent<R_CardView>().SetCardData(num, suit, dummyCard);
    //             currnetShuffle_c++;
    //         }
    //     }
    // }
    int forGroupName = 0;
    bool isFirstSort = false;
    //Sorting Card with Groups
    public void GetCardShorting()
    {
        if (!isFirstSort || groupList.Count==1)
        {
            Player_1_Card = Player_1_Card.OrderBy(yt => yt.GetComponent<R_CardView>().cardNumber).ToList();

            for (int i = 0; i < Player_1_Card.Count; i++)
            {
                Player_1_Card[i].transform.SetSiblingIndex(i);
            }
            for (int i = 0; i < Player_1_Card.Count; i++)
            {
                // Debug.Log(Player_1_Card[i].transform.parent.name, Player_1_Card[i].transform.parent);
                Player_1_Card[i].transform.parent = null;
            }
            Destroy(handCards.GetChild(0).gameObject);
            // Debug.Log(handCards.GetChild(0).name, handCards.GetChild(0));
            //groupList.RemoveAt(0);
            groupList.RemoveRange(0, groupList.Count);
            //Sorting Crad Types
            CreteSortingGroupUsingName(R_SUITS.SPAIDS);
            CreteSortingGroupUsingName(R_SUITS.HEARTS);
            CreteSortingGroupUsingName(R_SUITS.DIAMONDS);
            CreteSortingGroupUsingName(R_SUITS.CLUBS);
            for (int i = 0; i < Player_1_Card.Count; i++)
            {
                if (Player_1_Card[i].GetComponent<R_CardView>().cardSuit == R_SUITS.JOKER)
                {
                    Player_1_Card[i].transform.parent = groupList[groupList.Count - 1].transform;
                }
            }
            // sortingbtn.SetActive(false);
            // Rummy_InGameManager.instance.sortingBtn.SetActive(false);
            // CheckForEmptyGroups();
            ClearEmptyGroupsAfterSorting();
            forGroupName = 0;

            isFirstSort = true;
        }
        else
        {
            // GP, second time sort using R_CardSort, R_CardSorter, R_CardDeck
            // Debug.Log("SECOND time Sort");

            List<GameObject> gList = new List<GameObject>();

            for(int i=0;i<groupList.Count;i++)
            {
                int count = groupList[i].transform.childCount;

                for (int j = 1; j < count; j++)
                    gList.Add(groupList[i].transform.GetChild(j).gameObject);

                List<GameObject> input = gList;
                R_CardDeck cardDeck = new R_CardDeck(input);
                R_CardSorter sorter = new R_CardSorter
                {
                    SortBy = R_CardSortOrderMethod.SuitThenKind
                };
                cardDeck.Sort(sorter);
                cardDeck.SortCardList(gList, groupList[i].gameObject);
                gList.Clear();
            }
        }
        SendArrangedCard();
    }

    void CreteSortingGroupUsingName(R_SUITS sUITS)
    {
        bool spaidsCreate = false;

        GameObject group = new GameObject();
        for (int j = 0; j < Player_1_Card.Count; j++)
        {
            if (Player_1_Card[j].GetComponent<R_CardView>().cardSuit == sUITS && spaidsCreate == false)
            {
                group = GameObject.Instantiate(groupPrefab, handCards);
                // group.name = "group" + j.ToString();
                group.name = "group" + forGroupName;
                groupList.Add(group.GetComponent<R_GroupView>());
                spaidsCreate = true;
                forGroupName++;
            }
        }
        if (spaidsCreate)
        {
            for (int j = 0; j < Player_1_Card.Count; j++)
            {
                if (Player_1_Card[j].GetComponent<R_CardView>().cardSuit == sUITS)
                {
                    Player_1_Card[j].transform.parent = group.transform;
                }
            }
        }
    }
    void ClearEmptyGroupsAfterSorting()
    {
        for (int i = 0; i < groupList.Count; i++)
        {
            if (groupList[i].transform.childCount == 1)
            {
                Destroy(groupList[i].gameObject);
                groupList.RemoveAt(i);
            }
        }

        for (int i = 0; i < handCards.childCount; i++)
        {
            // Debug.Log("INSIDE EMPTY GROUP FOR LOOP childCount"+handCards.GetChild(i).gameObject.transform.childCount);
            if (handCards.GetChild(i).gameObject.transform.childCount == 1)
            {
                // Debug.Log("INSIDE EMPTY GROUP FOR LOOP IF STATEMENT");
                Destroy(handCards.GetChild(i).gameObject);
            }
        }
    }
    public bool CanCreateGroup()
    {
        if (groupCount <= 6)
        {
            return true;
        }
        return false;
    }
    public void ActivateCreateGroupUI(bool val)
    {
        // if (val && groupList.Count < 6)     // no of groups that you want to allow add that number
        // {
        //     groupCreator.gameObject.SetActive(val);
        //     groupCreator.transform.SetParent(handCards.transform);
        //     groupCreator.transform.SetAsLastSibling();
        // }
        // else
        // {
        //     // groupCreator.transform.parent = null;
        //     groupCreator.transform.SetParent(GameObject.Find("InGame").transform);
        //     groupCreator.gameObject.SetActive(false);
        // }
    }

    public void SetSelectedCardCount(int num, GameObject card)
    {
        selectedCardCount += num;
        if (num > 0)
        {
            selectedCards.Add(card);
        }
        else
        {
            selectedCards.Remove(card);
        }
    }
    public int GetSelectedCardCount()
    {
        return selectedCardCount;
    }

    public void CreateNewGroup()
    {
        CheckForGroupSelection();
        if (groupList.Count < 6)
        {
            Transform group = AddGroup();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                selectedCards[i].transform.SetParent(group);
                selectedCards[i].transform.SetSiblingIndex(selectedCards[i].transform.parent.childCount -1);

            }
            group.name = "group" + (groupList.Count-1);

            CheckForEmptyGroups();
            ResetSelectedCard();
            CheckForEmptyGroups();

            SendArrangedCard();
        }
        else
        {
            // Debug.Log("ONLY SIX GROUPS ARE ALLOWED");
            // StartCoroutine(ShowNotification("Only six groups are allowed"));
            StartCoroutine(Rummy_InGameUiManager.instance.SetTableText("Only six groups are allowed", 2f));
            ResetSelectedCard();
            CheckForEmptyGroups();
        }
    }

    // discard_Btn
    public void OnDiscardBtn()
    {
        if (selectedCards.Count==1 && !string.IsNullOrEmpty(selectedCards[0].name))
        {
            string cardName = selectedCards[0].name;
            string cardGroupName = selectedCards[0].transform.parent.name;
            // Debug.Log("OnDiscardBtn selectedCardName: "+cardName);
            // Debug.Log("OnDiscardBtn selectedCard GroupName: "+cardGroupName);
            R_SoundManager.instance.PlaySound(R_SoundType.CARD_DISCARD);
            R_SocketController.instance.SendDiscard(cardName);
            Rummy_InGameManager.instance.isCardDicardByMe=true;
            Player_1_Card.Remove(selectedCards[0]);
            Destroy(selectedCards[0]);
            Transform cardT = selectedCards[0].transform.parent;
            selectedCards.RemoveAt(0);
            if (selectedCardCount>0) {selectedCardCount--;}

            // Destroy(handCards.Find(cardGroupName+"/"+cardName).gameObject);
            // Transform cardT = handCards.Find(cardGroupName+"/"+cardName).parent;

            // Debug.Log("parent: "+cardT.name, cardT.gameObject);
            // Debug.Log("parent child count: "+cardT.childCount);
            if (cardT.childCount==2)
            {
                groupList.Remove(cardT.GetComponent<R_GroupView>());
                // Debug.Log("child count: 2");
                Destroy(cardT.gameObject);
            }

            ResetSelectedCard(); //discard_Btn.SetActive(false);
            Rummy_InGameManager.instance.isCardPulledByMe=false;
            showArea.SetActive(false);
            // Debug.Log("OnDiscardBtn after destroy");

            R_Task coSendArrangedCards = new R_Task(SendArrangedCardAfterDelay());
            coSendArrangedCards.Finished += delegate(bool manual) {
                if (!manual)
                {
                    coSendArrangedCards.Finished -= delegate(bool manual){};
                    // Debug.Log("SendArrangedCard()");
                }
            };
            // SendArrangedCard();
            SetCardsColor();
        }
    }

    IEnumerator SendArrangedCardAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SendArrangedCard();
    }

    void SetCardsColor()
    {
        int childCount = handCards.childCount;

        for (int i = 0; i < childCount; i++)
        {
            // Debug.Log("subchild of i ("+i+") = "+ handCards.GetChild(i).childCount);
            int subChildCount = handCards.GetChild(i).childCount;
            if (subChildCount > 1)
            {
                for (int j = 0; j < subChildCount; j++)
                {
                    if (j>0)
                    {
                        // Debug.Log("card name: "+handCards.GetChild(i).GetChild(j).name);
                        handCards.GetChild(i).GetChild(j).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    }
                }
                // Debug.Log("sb.ToString()="+sb.ToString());
            }
        }
    }

    public void ResetSelectedCard()
    {
        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        selectedCards.Clear();
        selectedCardCount = 0;
        group_Btn.SetActive(false);
        discard_Btn.SetActive(false);
    }

    public void CheckForGroupSelection()
    {
        int count = selectedCards.Count;
        for (int i = 0; i < selectedCards.Count; i++)
        {
            for (int j = 1; j < selectedCards[i].transform.parent.childCount; j++)
            {
                var inList = selectedCards.Find(x => x.name == selectedCards[i].transform.parent.GetChild(j).name);
                // Debug.Log("this is in the list ____" + inList, selectedCards[i].transform.parent.GetChild(j));

                if (inList == null)
                {
                    selectedCards[i].transform.parent.GetChild(j).GetComponentInParent<R_GroupView>().isThisGroupSelected = false;
                    break;
                }
                else
                {
                    selectedCards[i].transform.parent.GetChild(j).GetComponentInParent<R_GroupView>().isThisGroupSelected = true;
                }
            }
        }
        MergeGroups();
        CheckForEmptyGroups();
    }

    public void MergeGroups()
    {
        for (int i = 0; i < groupList.Count; i++)
        {
            if(groupList[i].GetComponent<R_GroupView>().isThisGroupSelected)
            {
                for (int j = 0; j < selectedCards.Count; j++)
                {
                    selectedCards[j].transform.SetParent(groupList[i].transform);
                }
            }
        }
    }

    public IEnumerator ShowNotification(string info)
    {
        debug_Txt.text = info;
        yield return new WaitForSeconds(2f);
        debug_Txt.text = null;
    }


    public Sprite GetSpriteFromSpriteArray(string name)
    {
        Sprite retSprite = null;
        // Debug.Log("GetSprite name="+name);
        for (int i = 0; i < cardSpritesArray.Length; i++)
        {
            // Debug.Log("cardSpritesArray[i].name="+cardSpritesArray[i].name);
            if (cardSpritesArray[i].name == name)
            {
                // Debug.Log("cardSpritesArray[i].name == name"+name);
                retSprite = cardSpritesArray[i];
            }
        }
        return retSprite;
    }
}
