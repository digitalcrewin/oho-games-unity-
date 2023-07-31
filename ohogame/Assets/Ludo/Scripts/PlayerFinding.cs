using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerFinding : MonoBehaviour
{
    public static PlayerFinding instance;

    public TMP_Text priceMoney;  //now priceMoney set as enter amount (entry fees)
    public TMP_Text enterAmount;
    public TMP_Text roomCode;
    public TMP_Text selfName;
    public TMP_Text opponentName;
    public TMP_Text timer1;
    public TMP_Text timer2;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        selfName.text = PlayerManager.instance.GetPlayerGameData().fullName;
    }

    void Update()
    {
        //OnEscape();
    }

    void OnEscape()
    {
        if (Input.GetKeyDown("escape"))
        {
            L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
        }
    }

    public void OnClickPanel()
    {
        string lastPath = L_GlobalGameManager.instance.currentScreenPathList.Last();

        if (lastPath == "ClassicLudoSelection")
        {
            //MainMenuController.instance.ShowScreen(MainMenuScreens.ClassicLudoGamePlay);
            //GlobalGameManager.instance.currentScreenPathList.Clear();
        }
        else if (lastPath == "QuickLudoSelection")
        {
            //MainMenuController.instance.ShowScreen(MainMenuScreens.Winner);
            //GlobalGameManager.instance.currentScreenPathList.Clear();
        }
    }

    public void OnClickBackButton()
    {
        L_MainMenuController.instance.PlayButtonSound();

        L_MainMenuController.instance.ShowScreen(MainMenuScreens.QuitConfirm);
        if (QuitConfirm.instance != null)
            QuitConfirm.instance.isQuitApp = false;
    }

    public void OnGameObjectFirstTime(string opponentNameData)
    {
        opponentName.text = opponentNameData;
    }

    public void OnGameStart(string unixTimestampStr)
    {
        Debug.Log("on game start data="+ unixTimestampStr);
        roomCode.text = "ROOM CODE : " + unixTimestampStr;
        int parseUnixTimeStamp = 0;
        if (Int32.TryParse(unixTimestampStr, out parseUnixTimeStamp))
        {
            DateTimeOffset dtOffset = DateTimeOffset.FromUnixTimeSeconds(parseUnixTimeStamp);
            Debug.Log("datetime="+dtOffset.DateTime);
            Debug.Log("seconds="+dtOffset.Second);
        }
    }
}
