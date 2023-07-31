using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class R_Dashboard : MonoBehaviour
{

    public static R_Dashboard instance;

    public GameObject menuObj, settingsObj, dashbBackDesignObj;
    public Text userNameText;

    void Awake()
    {
        instance = this;

        dashbBackDesignObj.transform.SetParent(RummyMainMenuController.instance.background.transform);
        dashbBackDesignObj.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        dashbBackDesignObj.GetComponent<RectTransform>().offsetMax = new Vector2(1, 1);
        dashbBackDesignObj.SetActive(true);
    }

    void OnEnable()
    {        
        // Screen.orientation = ScreenOrientation.Landscape;
        // R_GlobalGameManager.instance.loadingScreen.SetActive(false);
        // RummyMainMenuController.instance.screens[0].SetActive(false);

        // if (GlobalGameManager.instance != null)
        //     userNameText.text = PlayerManager.instance.GetPlayerGameData().userName;
        // else
        //     userNameText.text = R_PlayerManager.instance.GetPlayerGameData().name;
    }

    void Start()
    {
        StartCoroutine(ForOnEnableLandscape());
    }

    public void OnClickOnButton(string eventName)
    {
        switch (eventName)
        {
            case "practice":
                R_GlobalGameManager.instance.LoadScene(R_Scenes.InGame);
                break;

            case "logout":
                if (GlobalGameManager.instance != null)
                {
                    // R_GlobalGameManager.instance.DestroyScene(R_Scenes.MainMenuScene);
                    GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);
                    if (MainDashboardScreen.instance != null)
                    {
                        StartCoroutine(ForLogoutWaiting());
                        // Screen.orientation = ScreenOrientation.Portrait;
                        // MainDashboardScreen.instance.OnClickOnButton("menu");
                    }
                }
                else
                {
                    // R_PlayerManager.instance.DeletePlayerGameData();
                    // R_PrefsManager.DeletePlayerData();
                    // RummyMainMenuController.instance.ShowScreen(RummyMainMenuScreens.Login);
                    // settingsObj.SetActive(false);
                    // transform.parent.gameObject.SetActive(false);
                }
                break;

            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in R_Dashboard = " + eventName);
#endif
                break;
        }
    }

    IEnumerator ForOnEnableLandscape()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(true);
        // R_GlobalGameManager.instance.loadingScreen.SetActive(false);
        RummyMainMenuController.instance.screens[0].SetActive(false);

        if (GlobalGameManager.instance != null)
            userNameText.text = PlayerManager.instance.GetPlayerGameData().userName;
        else
            userNameText.text = R_PlayerManager.instance.GetPlayerGameData().name;
    }

    IEnumerator ForLogoutWaiting()
    {
        MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
        MainDashboardScreen.instance.transform.Find("Canvas/LAYER_5/Loading(Clone)/Image").GetComponent<Image>().enabled = false;
        Screen.orientation = ScreenOrientation.Portrait;
        yield return new WaitForSeconds(1f);
        // MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.Loading);
        //MainDashboardScreen.instance.DestroyScreen(ScreenLayer.LAYER5);
        MainDashboardScreen.instance.OnClickOnButton("menu");
        R_GlobalGameManager.instance.DestroyScene(R_Scenes.MainMenuScene);
    }

    void OnDestroy()
    {
        Destroy(RummyMainMenuController.instance.background.transform.Find(dashbBackDesignObj.name).gameObject);
    }
}
