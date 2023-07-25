using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_SitNGoWinnerLooser : MonoBehaviour
{
    public static P_SitNGoWinnerLooser instance;

    [Space(10)]

    [SerializeField] GameObject winnerParent;
    [SerializeField] GameObject looserParent;
    [SerializeField] Button shareBtn;
    [SerializeField] Button playAgainBtn;
    [SerializeField] Button closeBtn;
    [SerializeField] Text winAmountTxt;
    [SerializeField] Text loseAmountTxt;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        shareBtn.onClick.AddListener(() => { OnclickShareBtn(); });
        playAgainBtn.onClick.AddListener(() => { OnclickPlayAgainBtn(); });
    }

    public void SetWinner(string amountStr)
    {
        looserParent.SetActive(false);
        winnerParent.SetActive(true);
        winAmountTxt.text = "<size=50>₹</size> " + amountStr;
    }

    public void SetLooser(string amountStr)
    {
        winnerParent.SetActive(false);
        looserParent.SetActive(true);
        if (string.IsNullOrEmpty(amountStr))
            loseAmountTxt.text = "";
        else
            loseAmountTxt.text = "<size=50>₹</size> " + amountStr;
    }

    void OnclickShareBtn()
    {
        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.SitNGoWinnerLooser);
        Exit();
    }

    public void OnclickPlayAgainBtn()
    {
        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.SitNGoWinnerLooser);
        Exit();
    }

    //same code P_TableMenu.cs -> case "exit"
    void Exit()
    {
        if (P_SocketController.instance != null)
        {
            if ((!P_SocketController.instance.isViewer) && (!P_SocketController.instance.isMyBalanceZero))
            {
                if (P_SocketController.instance.isJoinSended)
                {
                    P_SocketController.instance.SendLeave();
                }
            }
            else
            {
                P_SocketController.instance.SendLeaveViewer();
            }
            P_SocketController.instance.isJoinSended = false;
        }
        P_InGameUiManager.instance.ShowScreen(P_InGameScreens.Loading);

        P_MainSceneManager.instance.LoadScene(P_MainScenes.LobbyScene);

        // back to selected lobby
        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.5f, () =>
        {
            if (P_Lobby.instance != null)
            {
                for (int i = 0; i < P_Lobby.instance.gameTypeStr.Length; i++)
                {
                    int tempI = i;
                    //Debug.Log("P_Lobby gameTypeStr[i]: " + P_Lobby.instance.gameTypeStr[i] + ", lobbySelectedGameType: P_SocketController.instance.lobbySelectedGameType");
                    if (P_Lobby.instance.gameTypeStr[i] == P_SocketController.instance.lobbySelectedGameType)
                    {
                        P_Lobby.instance.OnGameTypeButtonClick(tempI, P_SocketController.instance.lobbySelectedGameType);
                    }
                }
            }
            P_SocketController.instance.lobbySelectedGameType = "";
        }));
    }
}
