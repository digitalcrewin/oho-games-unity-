using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_GameSettings : MonoBehaviour
{
    public void OnClickOnButton(string eventName)
    {
        switch (eventName)
        {
            case "closeSetting":
                {
                    Rummy_InGameUiManager.instance.DestroyScreen(Rummy_InGameScreens.GameSettings);
                }
                break;
        }
    }
}
