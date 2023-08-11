using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    void OnEnable()
    {
        instance = this;
        ShowAddOnPopUp(true);
    }

    void OnDisable()
    {
        instance = null;
        ShowAddOnPopUp(false);
    }

    public void ShowAddOnPopUp(bool isShow)
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
}
