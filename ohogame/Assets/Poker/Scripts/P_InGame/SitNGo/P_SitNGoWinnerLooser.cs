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
    }

    public void OnclickPlayAgainBtn()
    {
        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.SitNGoWinnerLooser);
    }
}
