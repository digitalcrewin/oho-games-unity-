using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;

public class RummyMainMenuController : MonoBehaviour
{
    public static RummyMainMenuController instance;

    //public ExitDialog exitDialog;
    public GameObject bottomPanel, background;

    public GameObject[] screens; // All screens prefab
    public Transform[] screenLayers; // screen spawn parent

    //DEV_CODE
    public GameObject[] bottomMenus;

    private List<RummyMainMenuActiveScreen> mainMenuActiveScreens = new List<RummyMainMenuActiveScreen>();

    public Text userNameText, balanceText;
    public Image userPic;

    private void Awake()
    {
        instance = this;

        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }

    void Start()
    {
        //yield return new WaitForSeconds(2f);

        Debug.Log(R_PlayerManager.instance.IsLogedIn());
        /*if (R_PlayerManager.instance != null && R_PlayerManager.instance.IsLogedIn())
        {
            //GameObject.Find("Canvas/Horizontal Scroll Snap/Content/Page1/R_Dashboard").SetActive(true);
            GameObject.Find("Canvas/Horizontal Scroll Snap/Content/Page1/Rummy_Home").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("Canvas/Horizontal Scroll Snap/Content/Page1/Rummy_Home").SetActive(false);
            ShowScreen(RummyMainMenuScreens.Login);
        }

        if (PlayerPrefs.HasKey("Exit"))
        {
            PlayerPrefs.DeleteKey("Exit");
            ShowScreen(RummyMainMenuScreens.Loading);
            Invoke("HideMenuLoader", 12f);
        }*/
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(R_WebServices.instance.GETRequestData(R_RequestType.Profile, UserDetailsResponse));
    }

    void UserDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => UserDetails: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (data["statusCode"].ToString() == "200")
        {
            int realAmount = int.Parse(data["data"]["user_wallet"]["real_amount"] != null ? data["data"]["user_wallet"]["real_amount"].ToString().Split('.')[0] : "0");
            int bonusAmount = int.Parse(data["data"]["user_wallet"]["bonus_amount"] != null ? data["data"]["user_wallet"]["bonus_amount"].ToString().Split('.')[0] : "0");
            int winAmount = int.Parse(data["data"]["user_wallet"]["win_amount"] != null ? data["data"]["user_wallet"]["win_amount"].ToString().Split('.')[0] : "0");
            int totalBalance = realAmount + bonusAmount + winAmount;
            balanceText.text = totalBalance.ToString();
        }
        else
        {
            Debug.LogError(data["error"].ToString());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainDashboard();
        }
    }

    public void ShowScreen(RummyMainMenuScreens screenName, object[] parameter = null)
    {
        //if(screenName == RummyMainMenuScreens.MainMenu)
        //{
        //    SwitchToMainMenu(true);
        //    return;
        //}

        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((RummyScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            RummyMainMenuActiveScreen mainMenuScreen = new RummyMainMenuActiveScreen();
            mainMenuScreen.screenName = screenName;
            mainMenuScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;
            mainMenuActiveScreens.Add(mainMenuScreen);
            gm.SetActive(true);

            // switch (screenName)
            // {
            //     default:
            //         break;
            // }
        }
    }

    public void DestroyScreen(RummyMainMenuScreens screenName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenName == screenName)
            {
                Destroy(mainMenuActiveScreens[i].screenObject);
                mainMenuActiveScreens.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(RummyScreenLayer layerName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenLayer == layerName)
            {
                Destroy(mainMenuActiveScreens[i].screenObject);
                mainMenuActiveScreens.RemoveAt(i);
            }
        }
    }

    public bool IsScreenActive(RummyMainMenuScreens screenName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }

    private RummyScreenLayer GetScreenLayer(RummyMainMenuScreens screenName)
    {
        switch (screenName)
        {
            case RummyMainMenuScreens.ActiveTable:
                return RummyScreenLayer.LAYER1;

            //case RummyMainMenuScreens.EditProfile:
                //return RummyScreenLayer.LAYER2;

            case RummyMainMenuScreens.Loading:
                return RummyScreenLayer.LAYER5;

            default:
                return RummyScreenLayer.LAYER2;
        }
    }

    private void MainMenuController_PageToggleClickEvent(int pageNo)
    {
        switch (pageNo)
        {
            case 0:
                break;
            case 1:
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    private void MenuSelection(int index)
    {
        for (int i = 0; i < bottomMenus.Length; i++)
        {
            if (i == index)
            {
                bottomMenus[i].GetComponent<Image>().enabled = true;
                bottomMenus[i].transform.Find("Buttons").gameObject.SetActive(true);
            }
            else
            {
                bottomMenus[i].GetComponent<Image>().enabled = false;
                bottomMenus[i].transform.Find("Buttons").gameObject.SetActive(false);
            }
        }
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                DestroyScreen(RummyMainMenuScreens.ActiveTable);
                break;
            case "wallet":
                PlayerPrefs.SetString("ShowWalletScreen", "Yes");
                SceneManager.LoadScene("GameScene");
                break;
            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in MainMenuController = " + eventName);
#endif
                break;
        }

    }

    public void ShowActiveTableScreen()
    {
        ShowScreen(RummyMainMenuScreens.ActiveTable);
    }

    public void GoToMainDashboard()
    {
        if (R_GameConstants.isSeparateGame)
        {
            // exitDialog.ShowDialog("Are you sure want to quit?", () => Application.Quit(), null);
            Application.Quit();
        }
        else
        {
            // exitDialog.ShowDialog("Are you sure want to quit?", () => GlobalGameManager.instance.LoadScene(Scenes.MainDashboard), null); //SceneManager.LoadScene("GameScene")
            GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);
        }
    }

    void HideMenuLoader()
    {
        DestroyScreen(RummyMainMenuScreens.Loading);
    }

    public void OnServerResponseFound(R_RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log(requestType + " - " + serverResponse);
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                // if (requestType == RummyRequestType.Login)
                // {

                // }
                // else
                // {
                // //    ShowMessage(errorMessage);
                // }
            }

            return;
        }
        // add code here
        DestroyScreen(RummyMainMenuScreens.Loading);
    }

    public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "Ok")
    {
        //if (!IsScreenActive(RummyMainMenuScreens.Message))
        //{
            //MainMenuActiveScreen mainMenuScreen = new MainMenuActiveScreen();
            //mainMenuScreen.screenName = RummyMainMenuScreens.Message;
            //mainMenuScreen.screenLayer = GetScreenLayer(RummyMainMenuScreens.Message);

            //GameObject gm = Instantiate(screens[(int)RummyMainMenuScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            //mainMenuScreen.screenObject = gm;

            //mainMenuActiveScreens.Add(mainMenuScreen);

            //gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
        //}
    }

    public void ShowMessage(string messageToShow, Action yesButtonCallBack, Action noButtonCallBack, string yesButtonText = "Yes", string noButtonText = "No")
    {
        //if (!IsScreenActive(RummyMainMenuScreens.Message))
        //{
        //    MainMenuActiveScreen mainMenuScreen = new MainMenuActiveScreen();
        //    mainMenuScreen.screenName = RummyMainMenuScreens.Message;
        //    mainMenuScreen.screenLayer = GetScreenLayer(RummyMainMenuScreens.Message);

        //    GameObject gm = Instantiate(screens[(int)RummyMainMenuScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
        //    mainMenuScreen.screenObject = gm;

        //    mainMenuActiveScreens.Add(mainMenuScreen);
        //    //gm.GetComponent<MessageScript>().ShowDoubleButtonPopUp(messageToShow, yesButtonCallBack, noButtonCallBack, yesButtonText, noButtonText);
        //}
    }
    
    public List<GameObject> AnimatedElements = new List<GameObject>();

    public void PlayMainMenuAnimations()
    {
        if (AnimatedElements.Count > 0)
        {
            foreach(GameObject g in AnimatedElements)
            {
                Animator anim = g.GetComponent<Animator>();
                anim.SetTrigger("op");
            }
        }
    }

}


public class RummyMainMenuActiveScreen
{
    public GameObject screenObject;
    public RummyMainMenuScreens screenName;
    public RummyScreenLayer screenLayer;
}

public enum RummyMainMenuScreens
{
    Loading,
    Login,
    ActiveTable,
    RummyRules
}


public enum RummyScreenLayer
{
    LAYER1,
    LAYER2,
    LAYER3,
    LAYER4,
    LAYER5
}
