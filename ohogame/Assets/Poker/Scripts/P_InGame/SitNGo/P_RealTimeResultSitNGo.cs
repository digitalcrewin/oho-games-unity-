using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_RealTimeResultSitNGo : MonoBehaviour
{
    public static P_RealTimeResultSitNGo instance;

    [Space(10)]

    public Button rankBtn;
    public Button prizeBtn;
    public Button tablesBtn;
    public Button blindsBtn;
    public Button closeBtn;

    [Space(10)]

    public Sprite rankPrizeLinkBtnBG;
    public Sprite rankPrizeLinkBtnSelectedBG;

    [Space(10)]

    public Color rankPrizeLinkTextSelectedColor;

    [Space(10)]

    public GameObject rankParent;
    public GameObject prizeParent;
    public GameObject tablesParent;
    public GameObject blindsParent;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        OnClickOnButton("rankBtn"); //default
    }

    public void OnClickOnButton(string btnNameStr)
    {
        switch(btnNameStr)
        {
            case "rankBtn":
                AllBtnUnSelected();
                rankBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnSelectedBG;
                rankBtn.transform.GetChild(0).GetComponent<Text>().color = rankPrizeLinkTextSelectedColor;
                rankParent.SetActive(true);
                break;

            case "prizeBtn":
                AllBtnUnSelected();
                prizeBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnSelectedBG;
                prizeBtn.transform.GetChild(0).GetComponent<Text>().color = rankPrizeLinkTextSelectedColor;
                prizeParent.SetActive(true);
                break;

            case "tablesBtn":
                AllBtnUnSelected();
                tablesBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnSelectedBG;
                tablesBtn.transform.GetChild(0).GetComponent<Text>().color = rankPrizeLinkTextSelectedColor;
                tablesParent.SetActive(true);
                break;

            case "blindsBtn":
                AllBtnUnSelected();
                blindsBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnSelectedBG;
                blindsBtn.transform.GetChild(0).GetComponent<Text>().color = rankPrizeLinkTextSelectedColor;
                blindsParent.SetActive(true);
                break;

            case "closeBtn":
                if (P_InGameUiManager.instance != null)
                    P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.RealTimeResultSitNGo);
                break;
        }
    }

    void AllBtnUnSelected()
    {
        rankBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnBG;
        prizeBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnBG;
        tablesBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnBG;
        blindsBtn.GetComponent<Image>().sprite = rankPrizeLinkBtnBG;

        rankBtn.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        prizeBtn.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        tablesBtn.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        blindsBtn.transform.GetChild(0).GetComponent<Text>().color = Color.white;

        rankParent.SetActive(false);
        prizeParent.SetActive(false);
        tablesParent.SetActive(false);
        blindsParent.SetActive(false);
    }
}
