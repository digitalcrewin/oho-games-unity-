using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_TourneyWaitingForTable : MonoBehaviour
{
    public static P_TourneyWaitingForTable instance;
    [SerializeField] Image loadingImg;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //StartCoroutine(GlobalGameManager.instance.RunAfterDelay(10f, () =>
        //{
        //    OnClickClose();
        //}));
    }

    float degreesPerSecond = 100;
    void Update()
    {
        loadingImg.transform.Rotate(new Vector3(0, 0, degreesPerSecond) * Time.deltaTime);
    }

    public void OnClickClose()
    {
        if (P_InGameUiManager.instance != null)
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.TourneyWaitingForTable);
    }
}
