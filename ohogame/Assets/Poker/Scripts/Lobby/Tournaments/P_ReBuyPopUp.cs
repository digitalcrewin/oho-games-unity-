using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class P_ReBuyPopUp : MonoBehaviour
{
    public static P_ReBuyPopUp instance;

    [SerializeField] GameObject reBuyPopUp;
    [SerializeField] Text reBuyErrorTxt;
    [SerializeField] Text reBuyBuyInTxt;
    [SerializeField] Text reBuyMyBalanceTxt;

    void OnEnable()
    {
        instance = this;
        ShowReBuyPopup(true);
    }

    void OnDisable()
    {
        instance = null;
        ShowReBuyPopup(false);
    }

    void ShowReBuyPopup(bool isShow)
    {
        if (isShow)
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 0, 0), 0.3f);
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
        Debug.Log("Re Buy clicked...");
    }
}
