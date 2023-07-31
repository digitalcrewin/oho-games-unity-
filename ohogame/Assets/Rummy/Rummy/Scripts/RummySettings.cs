using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RummySettings : MonoBehaviour
{
    [SerializeField] Sprite[] tableSprites;
    [SerializeField] Image table;

    int tableNumber = 0;

    public void OnTableForwardButtonPressed()
    {
        if (tableNumber + 1 <= tableSprites.Length - 1)
        {
            table.sprite = tableSprites[tableNumber += 1];
        }
    }

    public void OnTableBackButtonPressed() 
    {
        if (tableNumber - 1 >= 0)
        {
			table.sprite = tableSprites[tableNumber -= 1];
        }
    }

    public void OnPressedLeaveTable()
    {
        if (MainDashboardScreen.instance != null)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
            MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.RummyGameplay);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }
}
