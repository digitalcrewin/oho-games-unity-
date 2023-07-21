using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Lobby_SitnGo : MonoBehaviour
{
    public static P_Lobby_SitnGo instance;

    public Text titleText, entryText, trophyAmountText, bagAmountText, startsText, playersText, firstAmountText, statusText;
    public Image playerLineImage, statusImage;
    public Button bgButton, registerStatusBtn;
    public Sprite registeringSprite, startedSprite, finishedSprite;

    private void Awake()
    {
        instance = this;
    }
}
