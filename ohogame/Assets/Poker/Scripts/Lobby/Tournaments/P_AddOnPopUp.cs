using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class P_AddOnPopUp : MonoBehaviour
{
    public static P_AddOnPopUp instance;

    [Space(10)]
    [SerializeField] GameObject addExtraStackPopUp;
    [SerializeField] Text aESErrorTxt;
    [SerializeField] Text aESBuyInTxt;
    [SerializeField] Text aESStackTxt;
    [SerializeField] Text aESTimerTxt;
    [SerializeField] Image aESTimerImg;
    [SerializeField] Button aESAddOnButton;

    float timer, displayValue, increaseTimer = 0;

    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        ResetTimerAddOnBtn();
    }

    void OnDisable()
    {

    }

    void ResetTimerAddOnBtn()
    {
        if (aESAddOnButton.interactable == false)
            aESAddOnButton.interactable = true;

        //timer = 0;
        //displayValue = 0;
        displayValue = timer = 20;
        increaseTimer = 0;
    }

    void FixedUpdate()
    {
        if (timer > 1)
        {
            timer -= Time.deltaTime;
            increaseTimer += Time.deltaTime;
            aESTimerTxt.text = string.Format("{0:00}:{1:00}", 0, timer);
            aESTimerImg.fillAmount = increaseTimer / displayValue;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ShowAddOnPopUp(bool isShow)
    {
        if (isShow)
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 0, 0), 0.3f);

            try
            {
                aESBuyInTxt.text = P_SocketController.instance.tableData["game_json_data"]["minimum_buyin"].ToString();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            try
            {
                aESStackTxt.text = P_SocketController.instance.tableData["game_json_data"]["default_stack"].ToString();
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

    public void OnClickAddOn()
    {
        aESAddOnButton.interactable = false;
        P_SocketController.instance.SendAddOn();
    }

    public void OnSuccess()
    {
        aESErrorTxt.color = Color.green;
        aESErrorTxt.text = "Successfully ADD-ON";
        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(2f, () =>
        {
            aESErrorTxt.text = "";
            aESErrorTxt.color = Color.red;
            ShowAddOnPopUp(false);
        }));
    }

    public void OnErrorAfterAddOnClicked(string errMsg)
    {
        aESErrorTxt.text = errMsg;
        StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(2f, () =>
        {
            aESErrorTxt.text = "";
            ShowAddOnPopUp(false);
        }));
    }
}
