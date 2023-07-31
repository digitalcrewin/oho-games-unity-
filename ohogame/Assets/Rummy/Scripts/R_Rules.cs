using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class R_Rules : MonoBehaviour
{
    public void CloseThisScreen()
    {
        if (RummyMainMenuController.instance != null)
        {
            RummyMainMenuController.instance.DestroyScreen(RummyMainMenuScreens.RummyRules);
        }
        else if (MainDashboardScreen.instance != null)
        {
            Debug.Log("sdfsdfosdf");
            MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.R_Rules);
        }
    }
}
