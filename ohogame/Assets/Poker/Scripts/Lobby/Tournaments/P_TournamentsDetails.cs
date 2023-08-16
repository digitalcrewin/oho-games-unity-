using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class P_TournamentsDetails : MonoBehaviour
{
    public static P_TournamentsDetails instance;

    [SerializeField] Transform gameTypeContent;

    [SerializeField] Color nonSelectedGameTypeColor;
    [SerializeField] Color selectedGameTypeColor;

    [SerializeField] GameObject detailsItem, entriesItem, prizeItem, tablesItem;

    [Space(10)]

    [SerializeField] Text titleText;

    [Space(10)]

    // details
    [SerializeField] Text detailsStartsWhenTxt;
    [SerializeField] Text detailsPlayerCountTxt;
    [SerializeField] Image detailsPlayersLineImg;
    [SerializeField] Text detailsBuyInTxt;
    [SerializeField] Text detailsPrizeTxt;
    [SerializeField] Text blindsUpTxt;
    [SerializeField] Text avgStackTxt;


    [Space(10)]

    [SerializeField] Text prizePrizePoolTxt;
    [SerializeField] Text prizePlacesPaidTxt;
    [SerializeField] Transform prizeScrollContent;
    [SerializeField] GameObject rankPrizeItemPrefab;
    [SerializeField] GameObject rankPrizeNoData;


    [Space(10)]

    // entries (players)
    [SerializeField] Transform entriesScrollContent;
    [SerializeField] GameObject entriesItemPrefab;
    [SerializeField] GameObject entriesNoData;
    [SerializeField] GameObject entriesSelfEntry;
    [SerializeField] Text entriesSelfEntryName;
    [SerializeField] TMP_Text entriesSelfEntryStack;
    //[SerializeField] Button entriesRegisterBtn;
    //[SerializeField] Text entriesRegisterBtnText;
    //[SerializeField] Image entriesRegisterBtnImage;
    //[SerializeField] Text entriesRegisterErrorText;

    [Space(10)]

    [SerializeField] Transform tablesScrollContent;
    [SerializeField] GameObject tablesItemPrefab;
    [SerializeField] GameObject tablesNoData;

    [Space(10)]

    [SerializeField] Button registerBtn;
    [SerializeField] Text registerBtnText;
    [SerializeField] Image registerBtnImage;
    [SerializeField] Sprite registerBtnBG;
    [SerializeField] Sprite unRegisterBtnBG;
    [SerializeField] Sprite buyChipsBtnBG;

    [Space(10)]

    [SerializeField] GameObject registerConfirmPopUp;
    [SerializeField] Text regCnfErrorTxt;
    [SerializeField] Text regCnfBuyInTxt;
    [SerializeField] Text regCnfMyBalanceTxt;
    [SerializeField] Button regCnfRegBtn;
    [SerializeField] Button regCnfCloseBtn;

    [Space(10)]

    [SerializeField] GameObject registerSuccessPopUp;
    [SerializeField] Text regSuccessTxt;
    [SerializeField] Image regSuccessImg;

    JsonData roomData;
    public JsonData RoomData
    {
        //get { return roomData; }   // get method
        set { roomData = value; }  // set method
    }

    public bool isMyIdRegistered = false;

    Coroutine tournamentsTimerCo;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //SetIntialData();
        Debug.Log("isMyIdRegistered:" + isMyIdRegistered);
        if (isMyIdRegistered)
        {
            registerBtnImage.sprite = unRegisterBtnBG;
            registerBtnText.text = "Unregister";
        }
        else
        {
            registerBtnImage.sprite = registerBtnBG;
            registerBtnText.text = "Register";
        }
    }

    void SetIntialData()
    {
        for (int i = 0; i < gameTypeContent.childCount; i++)
        {
            Button gameTypeBtn = gameTypeContent.GetChild(i).GetComponent<Button>();
            gameTypeBtn.onClick.AddListener(() =>
            {
                GameTypeButtonClickSetImageNColor(gameTypeBtn, gameTypeBtn.gameObject.name);
            });
        }

        //float maxPlayers = 0f, totalPlayers = 0f;
        //if (float.TryParse(roomData["game_json_data"]["maximum_player"].ToString(), out maxPlayers)) { }
        //if (float.TryParse(roomData["totalPlayers"].ToString(), out totalPlayers)) { }
        //detailsStartsWhenTxt.text = "Starts when " + maxPlayers + " player joins";
        //detailsPlayerCountTxt.text = totalPlayers + "/" + maxPlayers;
        //titleText.text = roomData["game_json_data"]["room_name"].ToString();
        titleText.text = "Delhi Tournament";

        //detailsBuyInTxt.text = roomData["game_json_data"]["minimum_buyin"].ToString();
        //detailsPrizeTxt.text = roomData["game_json_data"]["prize_money"].ToString();
        //avgStackTxt.text = roomData["game_json_data"]["default_stack"].ToString();

        //try
        //{
        //    detailsPlayersLineImg.fillAmount = (totalPlayers / maxPlayers);
        //}
        //catch (System.Exception e)
        //{
        //    // for division error
        //    if (P_GameConstant.enableErrorLog)
        //        Debug.Log("Division error in players line image");
        //    detailsPlayersLineImg.fillAmount = 0f;
        //}

        //P_SocketController.instance.gameTableMaxPlayers = (int)maxPlayers;
    }

    void GameTypeButtonClickSetImageNColor(Button buttonSelected, string gameTypeSelected)
    {
        buttonSelected.transform.GetChild(0).gameObject.SetActive(false);
        buttonSelected.transform.GetChild(1).gameObject.SetActive(true);
        buttonSelected.transform.GetChild(2).GetComponent<Text>().color = selectedGameTypeColor;
        buttonSelected.transform.GetChild(3).gameObject.SetActive(true);

        for (int i = 0; i < gameTypeContent.childCount; i++)
        {
            if (gameTypeContent.GetChild(i).gameObject.name != buttonSelected.gameObject.name)
            {
                Button gameTypeBtnOff = gameTypeContent.GetChild(i).GetComponent<Button>();
                gameTypeBtnOff.transform.GetChild(1).gameObject.SetActive(false);
                gameTypeBtnOff.transform.GetChild(0).gameObject.SetActive(true);
                gameTypeBtnOff.transform.GetChild(2).GetComponent<Text>().color = nonSelectedGameTypeColor;
                gameTypeBtnOff.transform.GetChild(3).gameObject.SetActive(false);
            }
        }

        detailsItem.SetActive(false);
        entriesItem.SetActive(false);
        prizeItem.SetActive(false);
        tablesItem.SetActive(false);

        if (buttonSelected.gameObject.name == "Details")
        {
            detailsItem.SetActive(true);
        }
        else if (buttonSelected.gameObject.name == "Entries")
        {
            entriesItem.SetActive(true);
            //if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
            //    StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/players", PlayersResponse));
        }
        else if (buttonSelected.gameObject.name == "Prize")
        {
            prizeItem.SetActive(true);
            //if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
            //    StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/prize-data", PrizeDataResponse));
        }
        else if (buttonSelected.gameObject.name == "Tables")
        {
            tablesItem.SetActive(true);
        }    
    }


    public void OnClickOnButton(string buttonName)
    {
        switch (buttonName)
        {
            case "back":
                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.LobbyTournaments);
                break;

            case "blind_structure":
                P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbyTournamentsBlindStructure);
                //if (P_TournamentsBlindStructure.instance != null)
                //    P_TournamentsBlindStructure.instance.GameId = roomData["game_id"].ToString();
                break;

            case "registerBtn":
                if (registerBtnText.text == "Register")
                {
                    //entriesRegisterBtn.interactable = false;

                    //P_SocketController.instance.SendJoin(P_SocketController.instance.TABLE_ID, roomData["game_json_data"]["minimum_buyin"].ToString());

                    //StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.3f, () =>
                    //{
                    //    if (!string.IsNullOrEmpty(roomData["game_id"].ToString()))
                    //        StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/poker/games/" + roomData["game_id"].ToString() + "/players", PlayersResponse));

                    //    //GameStarted();
                    //}));
                    registerConfirmPopUp.SetActive(true);

                }
                else if (registerBtnText.text == "Unregister")
                {
                    registerBtn.interactable = false;
                    string jsonRequestDeReg = "{\"game_id\":" + roomData["game_id"].ToString() + "}";
                    Debug.Log("register request json:" + jsonRequestDeReg);
                    StartCoroutine(P_WebServices.instance.POSTRequestData(P_RequestType.DeRegisterTournament, jsonRequestDeReg, (string downloadText, bool isError, string errorString) =>
                    {
                        bool isResponseError = false;
                        string unRegErrMsg = "Error in Unregister";
                        if (isError)
                        {
                            if (P_GameConstant.enableErrorLog)
                                Debug.Log("Tournament DeRegister Error: " + errorString);

                            isResponseError = true;
                        }
                        else
                        {
                            if (P_GameConstant.enableLog)
                                Debug.Log("Tournament DeRegister: " + downloadText);

                            JsonData data = JsonMapper.ToObject(downloadText);
                            IDictionary iData = data as IDictionary;
                            if (iData.Contains("statusCode"))
                            {
                                if (data["statusCode"].ToString() == "200")
                                {
                                    registerBtnImage.sprite = registerBtnBG;
                                    registerBtnText.text = "Register";
                                }
                                else
                                {
                                    isResponseError = true;
                                    if (iData.Contains("message"))
                                    {
                                        unRegErrMsg = data["message"].ToString();
                                    }
                                }
                            }
                            else
                            {
                                isResponseError = true;
                            }
                        }
                        if (isResponseError)
                        {
                            P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.Message);
                            if (P_Message.instance != null)
                            {
                                P_Message.instance.HideHeadingCloseButton();
                                P_Message.instance.headingText.text = "Error";
                                P_Message.instance.ShowSingleButtonPopUp(unRegErrMsg, () => {
                                    P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.Message);
                                });
                            }
                            registerBtnImage.sprite = unRegisterBtnBG;
                            registerBtnText.text = "Unregister";
                        }
                        registerBtn.interactable = true;
                    }));
                }
                break;

            case "regCnfRegisterBtn":

                regCnfRegBtn.interactable = false;
                regCnfCloseBtn.interactable = false;
                string jsonRequest = "{\"game_id\":" + roomData["game_id"].ToString() + "}";
                Debug.Log("register request json:" + jsonRequest);
                StartCoroutine(P_WebServices.instance.POSTRequestData(P_RequestType.RegisterTournament, jsonRequest, (string downloadText, bool isError, string errorString) =>
                {
                    bool isResponseError = false;
                    string regErrMsg = "Error in Register";
                    if (isError)
                    {
                        if (P_GameConstant.enableErrorLog)
                            Debug.Log("Tournament Register Error: " + errorString);

                        isResponseError = true;
                    }
                    else
                    {
                        if (P_GameConstant.enableLog)
                            Debug.Log("Tournament Register: " + downloadText);

                        JsonData data = JsonMapper.ToObject(downloadText);
                        IDictionary iData = data as IDictionary;
                        if (iData.Contains("statusCode"))
                        {
                            if (data["statusCode"].ToString() == "200")
                            {
                                registerBtnImage.sprite = unRegisterBtnBG;
                                registerBtnText.text = "Unregister";
                                registerConfirmPopUp.SetActive(false);
                                regSuccessTxt.text = "Registered Successfully";
                                registerSuccessPopUp.SetActive(true);
                            }
                            else
                            {
                                isResponseError = true;
                                if (iData.Contains("message"))
                                {
                                    regErrMsg = data["message"].ToString();
                                }
                            }
                        }
                        else
                        {
                            isResponseError = true;
                        }
                    }
                    if (isResponseError)
                    {
                        
                        P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.Message);
                        if (P_Message.instance != null)
                        {
                            P_Message.instance.HideHeadingCloseButton();
                            P_Message.instance.headingText.text = "Error";
                            P_Message.instance.ShowSingleButtonPopUp(regErrMsg, () => {
                                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.Message);
                                registerConfirmPopUp.SetActive(false);
                            });
                        }
                    }
                    regCnfRegBtn.interactable = true;
                    regCnfCloseBtn.interactable = true;
                }));
                
                break;

            case "regCnfClosePopUpBtn":
                registerConfirmPopUp.SetActive(false);
                break;

            case "regSuccessClosePopUpBtn":
                registerSuccessPopUp.SetActive(false);
                //StartCoroutine(GlobalGameManager.instance.RunAfterDelay(3f, () => {
                //    if (registerBtnText.text == "Unregister")
                //    {
                //        P_MainSceneManager.instance.ScreenDestroy();
                //        P_MainSceneManager.instance.LoadScene(P_MainScenes.InGame);
                //        P_InGameUiManager.instance.ShowTournamentReBuyPopUp();
                //    }
                //}));
                break;
        }
    }
}
