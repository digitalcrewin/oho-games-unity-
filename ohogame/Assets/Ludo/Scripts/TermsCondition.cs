using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TermsCondition : MonoBehaviour
{
    public void OnClickClose()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.Login_SignUp_Otp);
    }
}
