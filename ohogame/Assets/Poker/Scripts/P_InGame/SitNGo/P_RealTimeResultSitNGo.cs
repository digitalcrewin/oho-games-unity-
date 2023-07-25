using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;
using TMPro;

public class P_RealTimeResultSitNGo : MonoBehaviour
{
    public static P_RealTimeResultSitNGo instance;

    [Space(10)]

    [SerializeField] Text levelTxt;
    [SerializeField] Text currentSbBbTxt;
    [SerializeField] Text blindUpTxt;
    [SerializeField] Text nextLevelTxt;
    [SerializeField] Text rankTxt;
    [SerializeField] Text trophyCountTxt;
    [SerializeField] Text entrantsTxt;
    [SerializeField] Text remainingEntrantsTxt;
    [SerializeField] Text paidTxt;
    [SerializeField] Text reEntriesTxt;
    [SerializeField] Text largestTxt;
    [SerializeField] Text averageTxt;
    [SerializeField] Text smallestTxt;
    [SerializeField] Text fistPrizeTxt;
    [SerializeField] Text nextPrizeTxt;

    [Space(10)]

    [SerializeField] Button rankBtn;
    [SerializeField] Button prizeBtn;
    [SerializeField] Button tablesBtn;
    [SerializeField] Button blindsBtn;
    [SerializeField] Button closeBtn;

    [Space(10)]

    [SerializeField] Sprite rankPrizeLinkBtnBG;
    [SerializeField] Sprite rankPrizeLinkBtnSelectedBG;

    [Space(10)]

    [SerializeField] Color rankPrizeLinkTextSelectedColor;

    [Space(10)]

    [SerializeField] GameObject rankParent;
    [SerializeField] GameObject prizeParent;
    [SerializeField] GameObject tablesParent;
    [SerializeField] GameObject blindsParent;

    [Space(10)]

    [SerializeField] Transform rankContent;
    [SerializeField] GameObject rankScrollItemPrefab;
    [SerializeField] Transform prizeContent;
    [SerializeField] GameObject prizeScrollItemPrefab;
    [SerializeField] Transform blindContent;
    [SerializeField] GameObject blindScrollItemPrefab;


    Coroutine sitNGoGameResultTimerCo;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ClearAllText();

        OnClickOnButton("rankBtn"); //default

        P_SocketController.instance.SendGameResult();
    }

    void OnDestroy()
    {
        if (sitNGoGameResultTimerCo != null)
        {
            StopCoroutine(sitNGoGameResultTimerCo);
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

    void ClearAllText()
    {
        levelTxt.text = "Level";
        currentSbBbTxt.text = "-";
        blindUpTxt.text = "-";
        nextLevelTxt.text = "-";
        rankTxt.text = "-";
        trophyCountTxt.text = "-";
        entrantsTxt.text = "-";
        remainingEntrantsTxt.text = "- Remaining";
        paidTxt.text = "-";
        reEntriesTxt.text = "-";
        largestTxt.text = "-";
        averageTxt.text = "-";
        smallestTxt.text = "-";
        fistPrizeTxt.text = "-";
        nextPrizeTxt.text = "-";
    }

    void ClearRankScrollView()
    {
        for (int i = 0; i < rankContent.childCount; i++)
        {
            Destroy(rankContent.transform.GetChild(i).gameObject);
        }
    }

    void ClearPrizeScrollView()
    {
        for (int i = 0; i < prizeContent.childCount; i++)
        {
            Destroy(prizeContent.transform.GetChild(i).gameObject);
        }
    }

    void ClearBlindScrollView()
    {
        for (int i = 0; i < blindContent.childCount; i++)
        {
            Destroy(blindContent.transform.GetChild(i).gameObject);
        }
    }





    public void OnClickOnButton(string btnNameStr)
    {
        switch (btnNameStr)
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



    public void OnGameResultRes(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        levelTxt.text = "Level " + data["currentLevel"].ToString();
        currentSbBbTxt.text = data["currentSB"].ToString() + "/" + data["currentBB"].ToString();

        double blindUpDouble = 0D;
        if (Double.TryParse(data["upTimeForNextLevel"].ToString(), out blindUpDouble)) { }
        if (blindUpDouble > 0)
        {
            TimeSpan t = TimeSpan.FromSeconds(blindUpDouble);
            sitNGoGameResultTimerCo = StartCoroutine(StartSitNGoClock(t));
        }

        nextLevelTxt.text = data["nextSB"].ToString() + "/" + data["nextBB"].ToString();

        rankTxt.text = data["currentPlayerRank"].ToString() + "/" + data["totalPlayers"].ToString();
        entrantsTxt.text = data["totalPlayers"].ToString();

        largestTxt.text = data["largestStack"].ToString();
        averageTxt.text = data["average"].ToString();
        smallestTxt.text = data["smallestStack"].ToString();

        fistPrizeTxt.text = "₹" + data["firstRankPrize"].ToString();
        nextPrizeTxt.text = "₹" + data["secondRankPrize"].ToString();

        ClearRankScrollView();
        for (int i = 0; i < data["playersRanked"].Count; i++)
        {
            int tempI = i;
            GameObject go = Instantiate(rankScrollItemPrefab, rankContent);
            go.transform.GetChild(0).GetComponent<Text>().text = (tempI + 1) + "";
            go.transform.GetChild(1).GetComponent<Text>().text = data["playersRanked"][i]["id"].ToString();

            Text stackTxt = go.transform.GetChild(2).GetComponent<Text>();
            for (int j = 0; j < data["playersEliminated"].Count; j++)
            {
                if (data["playersEliminated"][j]["userId"].ToString() == data["playersRanked"][i]["userId"].ToString())
                {
                    stackTxt.text = "ELIMINATED";
                }
            }
            if (stackTxt.text != "ELIMINATED")
            {
                stackTxt.text = data["playersRanked"][i]["stackSize"].ToString();
            }
        }

        ClearPrizeScrollView();
        for (int i = 0; i < data["prize_structure"].Count; i++)
        {
            GameObject go = Instantiate(prizeScrollItemPrefab, prizeContent);
            go.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = data["prize_structure"][i]["rank"].ToString();
            go.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "<sup><size=30><sprite=0></size></sup>" + data["prize_structure"][i]["price"].ToString();
        }

        ClearBlindScrollView();
        for (int i = 0; i < data["blind_structure"].Count; i++)
        {
            GameObject go = Instantiate(blindScrollItemPrefab, blindContent);
            go.transform.GetChild(0).GetComponent<Text>().text = data["blind_structure"][i]["level"].ToString();
            go.transform.GetChild(1).GetComponent<Text>().text = data["blind_structure"][i]["small_blind"].ToString() + " / " + data["blind_structure"][i]["big_blind"].ToString();
            go.transform.GetChild(2).GetComponent<Text>().text = data["blind_structure"][i]["ante"].ToString();
            go.transform.GetChild(3).GetComponent<Text>().text = data["blind_structure"][i]["blinds_up"].ToString() + "m";
        }
    }

    IEnumerator StartSitNGoClock(TimeSpan t)
    {
        while (t.TotalSeconds > 0)
        {
            //t = t.Add(TimeSpan.FromSeconds(1));
            t = t.Subtract(TimeSpan.FromSeconds(1));

            if (t.Hours > 0)
            {
                blindUpTxt.text = t.Hours + ":" + t.Minutes + ":" + t.Seconds;  //h:m:s
            }
            else
            {
                if (t.Minutes > 0)
                    blindUpTxt.text = t.Minutes + ":" + t.Seconds;
                else
                    blindUpTxt.text = t.Seconds + "";
            }
            yield return new WaitForSeconds(1);
        }
    }
}
