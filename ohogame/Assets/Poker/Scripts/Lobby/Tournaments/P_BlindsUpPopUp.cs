using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class P_BlindsUpPopUp : MonoBehaviour
{
    public static P_BlindsUpPopUp instance;

    [Space(10)]
    [SerializeField] GameObject blindsUpPopUp;
    [SerializeField] Text blindsUpValueTxt;

    void OnEnable()
    {
        instance = this;
        ShowBlindsUpPopUp(true);
    }

    void OnDisable()
    {
        instance = null;
        ShowBlindsUpPopUp(false);
    }

    public void ShowBlindsUpPopUp(bool isShow)
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
