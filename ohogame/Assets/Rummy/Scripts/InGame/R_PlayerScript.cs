using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class R_PlayerScript : MonoBehaviour
{
    public static R_PlayerScript instance;
    
    [SerializeField]
    public R_PlayerData playerData;
    public RectTransform fx_holder;
    private Image[] cardsImage;
    public Sprite[] EventSprite;
    public Sprite defultavtar;
    private Image timerBar;
    public Image avtar, frame, flag, timerBarImg;
    public GameObject lastActionImage;
    public Text nameText, balanceText;
    private Text lastActionText, userName, localBetPot, RealTimeResulttxt;
    private GameObject foldScreen, parentObject, emptyObject, RealTimeResult, localbetBG;
    private bool isItMe;
    public string otheruserId;
    public string seat, currentSeat;
    public GameObject winPercentage;
    public Image firstCardImage;
    public Text chatText;
   
    private int localBetAmount = 0;
    private int localBetRoundNo = 0;

    Image tableBtnTimer;

    void Awake()
    {
        instance = this;    
    }

    public void Init(Rummy_MatchMakingPlayerData matchMakingPlayerData)
    {
        playerData = matchMakingPlayerData.playerData;
    }

    public void InitPlayerData(R_PlayerData pData)
    {
        playerData = pData;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Image[] GetCardsImage()
    {
        return cardsImage;
    }

    public bool IsMe()
    {
        return isItMe;
    }

    public void ToggleCards(bool isShow, bool isShowOriginalCards = false)
    {

    }
}

[System.Serializable]
public class R_PlayerData
{
    public string userId;
    public string userName;
    public string tableId;
    public bool isDealer, isTurn, isCheckAvailable, isBlock, isStart;
    public float balance;
    public CardData[] cards;
    public string profileImage;
    public string cardValidity, bufferTime;
    public string seatNo;
    public string winPercent;
    public string flagurl;
}
