using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions.Example02;

public class RummyHomeMenu : MonoBehaviour
{
    public static RummyHomeMenu instance;
    private int playerCount = 2;
    //private Token.TokenType selectedToken = Token.TokenType.Red;
    public Transform scrollContent;
    public GameObject tableItem;
    public Text msgText;

    // for random id
    int player1RandomId = 0;
    int player2RandomId = 0;
    public InputField p1Input, p2Input;
    public Text randomIdInfoText, randomValidationText;
    private int maxNum = 2;
    public Button tempSendBtn;
    public List <int> randomList;
  public int lengthRummyTable;
    public Image userPic, frameImage;
    public Text userNameText, freeChipsText, realMoneyText;
private void Awake() {
    instance=this;
}
    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        msgText.text = "Loading";
        StartCoroutine(R_WebServices.instance.GETRequestData(R_RequestType.Games, OnServerResponseFound));

        //userNameText.text = PlayerManager.instance.GetPlayerGameData().userName;
        //freeChipsText.text = PlayerManager.instance.GetPlayerGameData().coins.ToString();
        //userPic.sprite = PlayerManager.instance.GetPlayerGameData().userPic;
        //realMoneyText.text = PlayerManager.instance.GetPlayerGameData().real.ToString(););
        //StartCoroutine(R_GlobalGameManager.instance.LoadSpriteImageFromUrl(PlayerManager.instance.GetPlayerGameData().FrameUrl, frameImage));

        userNameText.text = R_PlayerManager.instance.GetPlayerGameData().userName;
        StartCoroutine(R_GlobalGameManager.instance.LoadSpriteImageFromUrl(R_PlayerManager.instance.GetPlayerGameData().avatarURL, userPic));

        // player1RandomId = GenerateRandomList (10,100);
        // player2RandomId = GenerateRandomList (10,100);
        // Debug.Log("this user randomId:"+player1RandomId+",  Other user randomId:"+player2RandomId);
        // randomIdInfoText.text = "p1:"+player1RandomId+" vs p2:"+player2RandomId;
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            //case "setting":
            //    {
            //        L_MainMenuController.instance.ShowScreen(L_MainMenuScreens.Setting);
            //    }
            //    break;

            //case "shop":
            //    {
            //        L_MainMenuController.instance.ShowScreen(L_MainMenuScreens.Shop);
            //    }
            //    break;

            case "profile":
                {
                    if (GlobalGameManager.instance != null)
                    {
                        GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);
                        if (MainDashboardScreen.instance != null)
                        {
                            MainDashboardScreen.instance.OnClickOnButton("profilemodification");
                        }
                    }
                }
                break;

            case "wallet":
                {
                    if (GlobalGameManager.instance != null)
                    {
                        GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);
                        if (MainDashboardScreen.instance != null)
                        {
                            MainDashboardScreen.instance.OnClickOnButton("wallet");
                        }
                    }
                }
                break;

            //case "jackpot":
            //    {
            //        L_MainMenuController.instance.ShowScreen(L_MainMenuScreens.Jackpot);
            //    }
            //    break;

            case "pointRummy":
                {
                    //GlobalGameManager.instance.LoadScene(Scenes.RummyInGame);
                    R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);
Debug.Log("Point rummy clicked");
                }
                break;

            case "practiceRummy":
                {
                    //GlobalGameManager.instance.LoadScene(Scenes.RummyInGame);
                    R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);
Debug.Log("Practice rummy clicked");
                }
                break;

            case "back":
                {
                    RummyMainMenuController.instance.GoToMainDashboard();
                }
                break;

            case "tempSendBtn":
                {
                    if (!string.IsNullOrEmpty(p1Input.text))
                    {
                        R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);
                        R_SocketController.instance.rummyUserId = p1Input.text;
                    }
                }
                break;

            case "rummyRulesBtn":
                {
                    RummyMainMenuController.instance.ShowScreen(RummyMainMenuScreens.RummyRules);
                }
                break;

            case "closeActiveTablesScreen":
                {
                    RummyMainMenuController.instance.DestroyScreen(RummyMainMenuScreens.ActiveTable);
                }
                break;

            default:
#if ERROR_LOG
			Debug.LogError("unhdnled eventName found in HomeMenu = " + eventName);
#endif
                break;
        }
    }


   public void OnServerResponseFound(string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (isShowErrorMessage)
        {
            if (errorMessage.Length > 0)
            {
                if (R_GameConstants.enableLog)
                    Debug.Log("errorMessage: " + errorMessage);
                msgText.text = "Error in Server Response!";
            }
            return;
        }

        if (R_GameConstants.enableLog)
            Debug.Log("serverResponse: " + serverResponse);

        if (string.IsNullOrEmpty(serverResponse))
        {
            msgText.text = "No Server Response Found!";
            return;
        }

         JsonData data = JsonMapper.ToObject(serverResponse);
        //if (data["rows"] != null)
        if (data["data"] != null)
        {
            //int tableCount = data["rows"].Count;
            int tableCount = data["data"].Count;
            lengthRummyTable = tableCount;
             StartCoroutine(tableListUpdate(tableCount,data));
           
        }
    }
    IEnumerator tableListUpdate(int  tableCount, JsonData data)
    {
    //UnityEngine.UI.Extensions.Example02.ScrollView.instance.callInitialize();

        yield return new WaitForSeconds(0.2f);
        if (tableCount > 0)
        {

            for (int i = 0; i < tableCount; i++)
            {
//                Debug.Log("++++++++++++++++++++" + scrollContent.GetChild(i).gameObject.transform.name);
                GameObject gm = Instantiate(tableItem, scrollContent.GetChild(i).gameObject.transform.GetChild(0));
                // gm.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = data["rows"][i]["name"].ToString();
                // gm.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = data["rows"][i]["minPlayers"].ToString() + " / " + data["rows"][i]["maxPlayers"].ToString();
                // gm.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = data["rows"][i]["minChips"].ToString() + " Chips";

                // int index = i;
                // gm.name = gm.name + "_" + index;
                // gm.transform.GetComponent<Button>().onClick.AddListener(() => OnClickOnTableItem(index, data["rows"][index]["id"].ToString(), data["rows"][index]));

                RummyHomeMenuTableItem rummyHomeMenuTableItem = gm.GetComponent<RummyHomeMenuTableItem>();
                rummyHomeMenuTableItem.cityText.text = data["rows"][i]["name"].ToString();
                rummyHomeMenuTableItem.playerCountText.text = data["rows"][i]["maxPlayers"].ToString();
                rummyHomeMenuTableItem.entryFeeCountText.text = data["rows"][i]["minChips"].ToString() + " Chips";

                int index = i;
                gm.name = gm.name + "_" + index;
                rummyHomeMenuTableItem.playNowBtn.onClick.AddListener(() => OnClickOnTableItem(index, data["rows"][index]["id"].ToString(), data["rows"][index]));
            }
            msgText.text = string.Empty;

            //if (GlobalGameManager.instance.goToThisScreen.Equals("Rummy_ActiveTable"))
            //{
            //    Debug.Log("gotothisscreen=" + GlobalGameManager.instance.goToThisScreen);
            //    RummyMainMenuController.instance.ShowActiveTableScreen();
            //    GlobalGameManager.instance.goToThisScreen = string.Empty;
            //}
        }
    }

    void OnClickOnTableItem(int index, string gameType, JsonData selectedRow)
    {
        //if (index==0)
        //{
            //Debug.Log(gameType+" selected currentUserId="+PlayerManager.instance.GetPlayerGameData().userId);
            R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);
        Debug.Log("heyyy="+R_GlobalGameManager.instance.mainSocketController.name);
            R_GlobalGameManager.instance.mainSocketController.GetComponent<R_SocketController>().enabled = true;
            R_SocketController.instance.rummyUserId = R_PlayerManager.instance.GetPlayerGameData().userId; //PlayerManager.instance.
        R_SocketController.instance.rummyGameType = gameType;
            R_SocketController.instance.selectedRow = selectedRow;
            if(R_GlobalGameManager.instance.isReJoinGame)
                R_SocketController.instance.ReStart();
        //}
    }


    public int GenerateRandomList (int min, int max)
    {
        //for(int i = 0; i < maxNum; i++){
            int numToAdd = Random.Range(min,max);
            while(!randomList.Contains(numToAdd))
            {
                numToAdd = Random.Range(min,max);
                randomList.Add(numToAdd);
            }
            return numToAdd;
        // }
    }

    IEnumerator SetRandomValidation()
    {
        randomValidationText.text = "Enter correct input";
        yield return new WaitForSeconds(0.5f);
        randomValidationText.text = string.Empty;
    }
}
