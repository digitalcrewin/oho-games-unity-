using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferralInput : MonoBehaviour
{
    public InputField referralCodeInput;

    public void OnClickSubmit()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferAndEarn);
    }
    public void OnClickSkip()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferAndEarn);
    }
}
