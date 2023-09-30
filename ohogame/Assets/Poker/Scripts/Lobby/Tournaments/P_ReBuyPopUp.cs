using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using LitJson;

public class P_ReBuyPopUp : MonoBehaviour
{
    public static P_ReBuyPopUp instance;

    [SerializeField] GameObject reBuyPopUp;
    [SerializeField] Text reBuyErrorTxt;
    [SerializeField] Text reBuyBuyInTxt;
    [SerializeField] Text reBuyMyBalanceTxt;
    [SerializeField] Button reBuyBtn;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        ResetReBuyBtn();
    }

    void ResetReBuyBtn()
    {
        if (reBuyBtn.interactable == false)
            reBuyBtn.interactable = true;
    }

    public void ShowReBuyPopup(bool isShow)
    {
        if (isShow)
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 0, 0), 0.3f);

            try
            {
                reBuyBuyInTxt.text = P_SocketController.instance.tableData["game_json_data"]["minimum_buyin"].ToString();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            try
            {
                StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/get-wallet", (string serverResponse, bool isErrorMessage, string errorMessage) =>
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["statusCode"].ToString() == "200")
                    {
                        int totalBalance = int.Parse(data["data"]["real_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["bonus_amount"].ToString().Split('.')[0]) + int.Parse(data["data"]["win_amount"].ToString().Split('.')[0]);
                        reBuyMyBalanceTxt.text = "" + totalBalance;
                    }
                }));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        else
        {
            gameObject.SetActive(false);
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.offsetMin = new Vector2(rt.offsetMin.x, -1170f); //bottom
            rt.offsetMax = new Vector2(rt.offsetMax.y, -1170f); //top
        }
    }

    public void OnClickReBuyBtn()
    {
        reBuyBtn.interactable = false;
        P_SocketController.instance.SendReBuyIn();
    }

    public void OnSuccess()
    {
        reBuyErrorTxt.color = Color.green;
        reBuyErrorTxt.text = "Successfully RE-BUY";
        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(2f, () =>
        {
            reBuyErrorTxt.text = "";
            reBuyErrorTxt.color = Color.red;
            ShowReBuyPopup(false);
        }));
    }

    public void OnErrorAfterAddOnClicked(string errMsg)
    {
        reBuyErrorTxt.text = errMsg;
        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(2f, () =>
        {
            reBuyErrorTxt.text = "";
            ShowReBuyPopup(false);
        }));
    }
}
