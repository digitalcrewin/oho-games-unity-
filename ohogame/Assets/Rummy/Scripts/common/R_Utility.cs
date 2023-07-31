using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class R_Utility : MonoBehaviour
{
   public static string GetTrimmedAmount(string currentValueInString)
    {
        double currentValue = double.Parse(currentValueInString);
        string updatedAmount = "";


        if (currentValue > 1000000000) // One Billion
        {
            updatedAmount = "" + (currentValue / 1000000000).ToString("0.00") + "B";
        }
        else if (currentValue > 1000000 && currentValue < 1000000000) // One Million
        {
            updatedAmount = "" + (currentValue / 1000000).ToString("0.00") + "M";
        }        
        else if (currentValue > 1000 && currentValue < 1000000) // One Thousand
        {
            updatedAmount = "" + (currentValue / 1000).ToString("0.00") + "K";
        }
        else if (currentValue <= 1000)
        {
            updatedAmount = "" + (currentValue / 1000).ToString("0.00") + "K";
        }
        else if (currentValue <= 1000000) // One Million
        {
            updatedAmount = "" + (currentValue / 1000000).ToString("0.00") + "M";
        }
        else if (currentValue <= 1000000000) // One Billion
        {
            updatedAmount = "" + (currentValue / 1000000000).ToString("0.00") + "B";
        }
        else
        {
            updatedAmount = currentValueInString;
        }

        if (updatedAmount.Contains(".00"))
        {
            updatedAmount = updatedAmount.Replace(".00", "");
        }

        return updatedAmount;
    }
}
