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

    void Awake()
    {
        instance = this;
    }

    public void SetWinner()
    {
        looserParent.SetActive(false);
        winnerParent.SetActive(true);
    }

    public void SetLooser()
    {
        winnerParent.SetActive(false);
        looserParent.SetActive(true);
    }
}
