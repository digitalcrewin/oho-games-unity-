using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_TourneyThanksForPlaying : MonoBehaviour
{
    public static P_TourneyThanksForPlaying instance;

    void Awake()
    {
        instance = this;
    }

    public void OnClickClose()
    {
        if (P_InGameUiManager.instance != null)
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.TourneyThanksForPlaying);
    }
}
