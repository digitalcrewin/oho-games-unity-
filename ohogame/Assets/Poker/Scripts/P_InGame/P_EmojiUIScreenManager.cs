using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_EmojiUIScreenManager : MonoBehaviour
{

    public static P_EmojiUIScreenManager instance;

    [Space(10)]

    [SerializeField] Image profileimage;
    [SerializeField] Image frame;
    [SerializeField] Image ruppeIconOfBalance;

    [Space(5)]

    [SerializeField] Text userName;
    [SerializeField] Text balanceTxt;
    [SerializeField] Text locationTxt;
    [SerializeField] Text deviceTxt;

    [Space(5)]

    [SerializeField] Text vpipTxt;
    [SerializeField] Text pfrTxt;
    [SerializeField] Text bet3Txt;
    [SerializeField] Text foldto3Txt;

    [Space(5)]

    [SerializeField] Text cbetTxt;
    [SerializeField] Text foldToCbetTxt;
    [SerializeField] Text stealTxt;
    [SerializeField] Text checkRaiseTxt;

    [Space(5)]

    [SerializeField] Text wtsTxt;
    [SerializeField] Text wsdTxt;
    [SerializeField] Slider wtsSlider;
    [SerializeField] Slider wsdSlider;


    [Space(10)]

    public string userId = string.Empty;

    public GameObject[] containerAry;

    public int containerVal;

    public GameObject hideOnDealerClick;
    public GameObject showOnDealerClick;

    public Text UserId, levletxt;

    public GameObject addBtn;
    public GameObject dealerBtn;


    P_PlayerData playerDataLocal;


    // Start is called before the first frame update
    void Awake()
    {
        //GlobalGameManager.currentScreenName = "TableUserProfile";
        instance = this;
    }



    private void Start()
    {
        //ClearAll();
        //if (P_InGameUiManager.instance != null && P_InGameUiManager.instance.TempUserID != null && P_InGameUiManager.instance.TempUserID != "0")
        //{
        //    GetUserDetails(P_InGameUiManager.instance.TempUserID);
        //}
    }

    private void ClearAll()
    {
        //levletxt.text = "Lvl. " + ">>";
        userName.text = "";
        //UserId.text = "";
    }

    public void OnDealerBtnClick()
    {
        //if (P_InGameUiManager.instance != null)
        //    P_InGameUiManager.instance.ShowScreen(P_InGameScreens.DealerImageScreen);
    }

    public void GetUserDetails(P_PlayerData playerData) // string playerid, string username, float balance
    {
        Debug.Log("Emoji playerid: " + playerData.userId + ", " + playerData.userName + ", " + playerData.balance);
        playerDataLocal = playerData;
        StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/poker-profile/?user_id=" + playerData.userId, UserDetailsResponse));
        //StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_BASE_URL + "user/" + playerid, UserDetailsResponse));
    }

    void UserDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => poker profile: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (data["statusCode"].ToString() == "200")  //(bool.Parse(data["status"].ToString()))
        {
            userName.text = (data["data"]["name"]==null) ? ( (!string.IsNullOrEmpty(playerDataLocal.userName)) ? playerDataLocal.userName : "" ) : data["data"]["name"].ToString();
            
            balanceTxt.text = "";
            if (!string.IsNullOrEmpty(playerDataLocal.balance.ToString()))
            {
                balanceTxt.text = playerDataLocal.balance.ToString();
                ruppeIconOfBalance.gameObject.SetActive(true);
            }
            
            deviceTxt.text = "Device: <color=yellow>" + data["data"]["device_type"].ToString() + "</color>";


            float betPreflopCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["bet_preflop_count"].ToString(), vpipTxt);
            float preflopRaiseCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["preflop_raise_count"].ToString(), pfrTxt);
            float thirdBetPreflopCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["third_bet_preflop_count"].ToString(), bet3Txt);
            float foldOn3betPreflopCountInt = UserDetailsFloatDataHelper(data["data"]["stats"]["fold_on_3bet_preflop_count"].ToString(), foldto3Txt);

            float continuationBetCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["continuation_bet_count"].ToString(), cbetTxt);
            float foldOnContinuationBetCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["fold_on_continuation_bet_count"].ToString(), foldToCbetTxt);
            float raiseAtLastPositionOnTableCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["raise_at_last_position_on_table_count"].ToString(), stealTxt);
            float checkRaiseFlopCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["check_raise_flop_count"].ToString(), checkRaiseTxt);

            float showdownCountAfterFlopFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["showdown_count_after_flop"].ToString(), wtsTxt);
            float wonAtShowdownCountFloat = UserDetailsFloatDataHelper(data["data"]["stats"]["won_at_showdown_count"].ToString(), wsdTxt);

            wtsSlider.value = (showdownCountAfterFlopFloat / 100);
            wsdSlider.value = (wonAtShowdownCountFloat / 100);



            //UserId.text = "UserID:" + data["data"]["userId"].ToString();
        }
        else
        {
            //Debug.LogError(data["error"].ToString());
            Debug.LogError(data["message"].ToString());
        }
    }

    float UserDetailsFloatDataHelper(string data, Text txt)
    {
        float varRef = 0F;
        if (float.TryParse(data, out varRef)) { }
        txt.text = Math.Round(varRef, 2) + "%";
        return varRef;
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log(serverResponse);
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                P_InGameUiManager.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetUserDetails)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                for (int i = 0; i < data["getData"].Count; i++)
                {

                    loadImages(data["getData"][i]["profileImage"].ToString(), data["getData"][i]["frameURL"].ToString());
                    //levletxt.text = "Lvl. " + data["getData"][i]["userLevel"].ToString() + ">>";
                    userName.text = data["getData"][i]["userName"].ToString();
                    //UserId.text = "UserID:" + data["getData"][i]["userId"].ToString();
                }

            }
            else
            {
                if (P_InGameUiManager.instance != null)
                    P_InGameUiManager.instance.ShowMessage(data["message"].ToString());
            }
        }
    }
    public void loadImages(string urlAvtar, string urlframe)
    {
        StartCoroutine(loadSpriteImageFromUrl(urlAvtar, profileimage));
        StartCoroutine(loadSpriteImageFromUrl(urlframe, frame));
    }
    IEnumerator loadSpriteImageFromUrl(string URL, Image image)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed" + image.gameObject.name);
        }
        else
        {
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }

    void ShowContainer(int val)
    {
        for (int i = 0; i < containerAry.Length; i++)
        {
            if (i == P_InGameUiManager.instance.emojiContainerVal)
            {
                containerAry[i].SetActive(true);

                if (i == 2)
                {
                    addBtn.SetActive(false);
                    dealerBtn.SetActive(true);
                    hideOnDealerClick.SetActive(false);
                    showOnDealerClick.SetActive(true);
                }
                else
                {
                    if (i == 0)
                        addBtn.SetActive(false);
                    else
                        addBtn.SetActive(true);
                    dealerBtn.SetActive(false);
                    hideOnDealerClick.SetActive(true);
                    showOnDealerClick.SetActive(false);
                }
            }
            else
            {
                containerAry[i].SetActive(false);
            }
        }
    }

    public void SelectEmojiButton(string str)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        int emojiIndex = 0;
        Debug.Log("Here Get The emoji name which show ---  " + str);

        switch (str)
        {
            case "bluffing":
                emojiIndex = 0;
                break;
            case "youRaPro":
                emojiIndex = 1;
                break;
            case "beerCheers":
                emojiIndex = 2;
                break;
            case "murgi":
                emojiIndex = 3;
                break;
            case "rocket":
                emojiIndex = 4;
                break;
            case "dung":
                emojiIndex = 5;
                break;
            case "oscar":
                emojiIndex = 6;
                break;
            case "donkey":
                emojiIndex = 7;
                break;
            case "thumbUp":
                emojiIndex = 8;
                break;
            case "cherees":
                emojiIndex = 9;
                break;
            case "kiss":
                emojiIndex = 10;
                break;
            case "fish":
                emojiIndex = 11;
                break;
            case "gun":
                emojiIndex = 12;
                break;
            case "rose":
                emojiIndex = 13;
                break;
            case "perfume":
                emojiIndex = 14;
                break;
            case "ring":
                emojiIndex = 15;
                break;
            case "car":
                emojiIndex = 16;
                break;
        }

        if (P_InGameUiManager.instance != null)
            P_InGameUiManager.instance.CallEmojiSocket(emojiIndex);

        OnClickOnButton("back");
    }


    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {

                    StopCoroutine("loadSpriteImageFromUrl");
                    //GlobalGameManager.currentScreenName = "Game";
                    //transform.parent.parent.GetComponent<P_InGameUiManager>().DestroyScreen(P_InGameScreens.EmojiScreen);
                    P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.EmojiScreen);
                    //P_SocketController.instance.JoinSimilarButtonsEnable();
                    //P_SocketController.instance.joinSimilarTblBtnContainer.gameObject.SetActive(true);
                    //P_SocketController.instance.JoinSimilarButtonsInteractable(true);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }
}

