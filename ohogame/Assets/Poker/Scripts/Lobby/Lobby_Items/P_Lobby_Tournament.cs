using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Lobby_Tournament : MonoBehaviour
{
    public Sprite registeringBG, startedNLateBG, finishedBG;
    public Button registeringBtn;
    public Text registeringTxt;
    public Image registeringImg;

    [Space(10)]
    public Text titleTxt;
    public Text dateMonthTxt;
    public Text winAmountTxt;
    public Text activePlayersTxt;
    public Text timerTxt;
    public Text buyInTxt;
}
