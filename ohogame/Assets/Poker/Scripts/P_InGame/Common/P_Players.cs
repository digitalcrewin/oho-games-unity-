using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class P_Players : MonoBehaviour
{
    public static P_Players instance;

    [SerializeField]
    public P_PlayerData playerData;
    public Text userName;
    public Text balance;
    public Text realTimeResult;
    public Image timerImage;
    public RectTransform fx_holder;
    public GameObject dealer;
    public GameObject foldImage, fold2CardsImage, fold4CardsImage, fold5CardsImage, fold6CardsImage;
    public GameObject betAmount;
    public string seat, currentSeat, seatNo;

    public GameObject lastActionImage;
    public GameObject muckBG;
    public Text lastActionText;
    public Sprite[] EventSprite;
    public bool isItMe;
    public GameObject chatObj;

    private void Awake()
    {
        instance = this;
    }

    public P_PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void ResetTurn()
    {
        timerImage.fillAmount = 0;
    }

    public void Init(MatchMakingPlayerData matchMakingPlayerData)
    {
        playerData = matchMakingPlayerData.playerData;
        if (playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
        {
            isItMe = true;
        }
        else
        {
            isItMe = false;
        }
    }

    public void ResetData()
    {
        if (PlayerManager.instance.GetPlayerGameData().userId == playerData.userId)
        {
            lastActionImage.SetActive(false);
            P_InGameManager.instance.suggestionBtnParent.SetActive(false);
        }

        userName.text = "";
        balance.text = "";

        playerData.userId = "";
        playerData.userName = "";
        playerData.tableId = "";
        playerData.isPlaying = false;
        playerData.isDealer = false;
        playerData.isFolded = false;
        playerData.isShowCards = false;
        playerData.isLeft = false;
        playerData.isSmallBlind = false;
        playerData.isBigBlind = false;
        playerData.isFold = false;
        playerData.isTurn = false;
        playerData.isCheckAvailable = false;
        playerData.isBlock = false;
        playerData.isStart = false;
        playerData.balance = 0f;
        playerData.totalBet = 0f;
        playerData.cards = new P_CardData[0];
        playerData.seatNo = "";
        realTimeResult.text = "";

        P_InGameManager.instance.mySeatIndex = 0;
        P_InGameManager.instance.mySeatIndexTemp = -1;
        P_InGameManager.instance.isSeatRotation = false;

        // for bug: When player click on leave seat button in between game, & click on + button again UI issue showing. (fold, leave & join before winner)
        fold2CardsImage.SetActive(false);
        fold4CardsImage.SetActive(false);
        fold5CardsImage.SetActive(false);
        fold6CardsImage.SetActive(false);
    }

    public void UpdateLastAction(string textToShow)
    {
        if (textToShow == "" || string.IsNullOrEmpty(textToShow))
        {
            lastActionImage.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
            {
                lastActionImage.SetActive(false);
                lastActionImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            });
        }
        else
        {
            switch (textToShow)
            {
                case "Call":
                case "call":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[0];
                    break;
                case "Check":
                case "check":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[1];
                    break;
                case "Bet":
                case "bet":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[2];
                    break;
                case "Raise":
                case "raise":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[3];
                    break;
                case "AllIn":
                case "allIn":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[4];
                    break;
                case "Fold":
                case "fold":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[5];
                    break;
                case "sb":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[6];
                    break;
                case "bb":
                    lastActionImage.GetComponent<Image>().sprite = EventSprite[7];
                    break;

                default:
                    break;
            }

            lastActionText.text = textToShow;
            lastActionImage.SetActive(true);
            lastActionImage.GetComponent<Image>().DOFade(0f, 0.5f).From().SetEase(Ease.OutQuad);
        }
        
    }

    public void LocalBetRotateManage()
    {
        if (P_SocketController.instance.isViewer && seat=="0")
        {
            SixCardsZeroPosition();
        }

        switch (currentSeat)
        {
            case "1":
                betAmount.transform.localPosition = new Vector2(0f, 62f);
                dealer.transform.localPosition = new Vector2(-56.6f, -0.4f);
                realTimeResult.transform.localPosition = new Vector2(109.9f, -62.2f);
                realTimeResult.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                SixCardsZeroPosition();
                break;

            case "2":
                betAmount.transform.localPosition = new Vector2(114.2f, 13f);
                dealer.transform.localPosition = new Vector2(70.702f, -29.5f);
                SixCardsManage();
                break;

            case "3":
                betAmount.transform.localPosition = new Vector2(113.9f, -64.5f);
                dealer.transform.localPosition = new Vector2(70.702f, -98.1f);
                SixCardsManage();
                break;

            case "4":
                betAmount.transform.localPosition = new Vector2(113.9f, -64.5f);
                dealer.transform.localPosition = new Vector2(70.702f, -98.1f);
                SixCardsManage();
                break;

            case "5":
                betAmount.transform.localPosition = new Vector2(0f, -108f);
                dealer.transform.localPosition = new Vector2(-71f, -67f);
                SixCardsManage();
                break;

            case "6":
                betAmount.transform.localPosition = new Vector2(-113.9f, -64.5f);
                dealer.transform.localPosition = new Vector2(-69.1f, -98.1f);
                SixCardsManage();
                break;

            case "7":
                betAmount.transform.localPosition = new Vector2(-113.9f, -64.5f);
                dealer.transform.localPosition = new Vector2(-70.702f, -98.1f);
                SixCardsManage();
                break;

            case "8":
                betAmount.transform.localPosition = new Vector2(-104.88f, 13f);
                dealer.transform.localPosition = new Vector2(-70.702f, -29.5f);
                SixCardsManage();
                break;
        }
    }

    //when rotation occurs twocards rotation, pos, sizedelta manage.
    void TwoCardsManage()
    {
        playerData.twoCards[0].transform.parent.localPosition = new Vector2(30f, 5f);  //26, 38
        playerData.twoCards[0].rectTransform.sizeDelta = new Vector3(26f, 38f);

        playerData.twoCards[0].transform.localPosition = new Vector2(0f, 0f);
        playerData.twoCards[0].rectTransform.sizeDelta = new Vector3(26f, 38f);
        playerData.twoCards[0].transform.localScale = new Vector3(1f, 1f, 1f);
        playerData.twoCards[1].transform.localPosition = new Vector2(10f, 0f);
        playerData.twoCards[1].rectTransform.sizeDelta = new Vector3(26f, 38f);
        playerData.twoCards[1].transform.localEulerAngles = new Vector3(0f, 0f, -16f);
        playerData.twoCards[1].transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void SixCardsManage()
    {
        playerData.sixCards[0].transform.parent.localPosition = new Vector3(15f, 5f, 0f);
        playerData.sixCards[0].rectTransform.sizeDelta = new Vector2(26f, 38f);

        playerData.sixCards[0].transform.localPosition = new Vector3(0f, 0f, 0f);
        playerData.sixCards[1].transform.localPosition = new Vector3(3f, 0f, 0f);
        playerData.sixCards[2].transform.localPosition = new Vector3(6f, 0f, 0f);
        playerData.sixCards[3].transform.localPosition = new Vector3(9f, 0f, 0f);
        playerData.sixCards[4].transform.localPosition = new Vector3(12f, 0f, 0f);
        playerData.sixCards[5].transform.localPosition = new Vector3(15f, 0f, 0f);

        for (int i = 0; i < playerData.sixCards.Length; i++)
        {
            playerData.sixCards[i].rectTransform.sizeDelta = new Vector2(26f, 38f);
            playerData.sixCards[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            playerData.sixCards[i].transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void SixCardsZeroPosition()
    {
        // six cards
        playerData.sixCards[0].transform.parent.localPosition = new Vector3(78.5f, 1f, 0f);
        playerData.sixCards[0].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(141.2958f, 72f);

        playerData.sixCards[0].transform.localPosition = new Vector3(-50f, 0f, 0f);
        playerData.sixCards[1].transform.localPosition = new Vector3(-30f, 0f, 0f);
        playerData.sixCards[2].transform.localPosition = new Vector3(-10f, 0f, 0f);
        playerData.sixCards[3].transform.localPosition = new Vector3(10f, 0f, 0f);
        playerData.sixCards[4].transform.localPosition = new Vector3(30f, 0f, 0f);
        playerData.sixCards[5].transform.localPosition = new Vector3(50f, 0f, 0f);

        for (int i = 0; i < playerData.sixCards.Length; i++)
        {
            playerData.sixCards[i].rectTransform.sizeDelta = new Vector2(58f, 83f);
            playerData.sixCards[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            if (playerData.sixCards[i].transform.localScale.x != 0.7f)
                playerData.sixCards[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void OnClickEmoji()
    {
        P_InGameUiManager.instance.ShowScreen(P_InGameScreens.EmojiScreen);
        if (P_EmojiUIScreenManager.instance != null)
            P_EmojiUIScreenManager.instance.GetUserDetails(GetPlayerData().userId);
    }
}

[System.Serializable]
public class P_PlayerData
{
    public string userId;
    public string userName;
    public string tableId;
    public bool isPlaying, isDealer, isFolded, isShowCards, isLeft, isSmallBlind, isBigBlind, isFold, isTurn, isCheckAvailable, isBlock, isStart, doesHaveCards;
    public float balance, totalBet, minRaise, maxRaise;
    public P_CardData[] cards;
    public Image[] twoCards;
    public Image[] sixCards;
    //public string avatarurl;
    //public string userVIPCard, cardValidity, bufferTime;
    public string seatNo;
    //public string winPercent;
    //public string flagurl;
}