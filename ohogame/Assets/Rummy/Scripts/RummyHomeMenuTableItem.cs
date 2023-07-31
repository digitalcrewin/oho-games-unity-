using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RummyHomeMenuTableItem : MonoBehaviour
{
    public static RummyHomeMenuTableItem instance;

    [Header("------Icon------")]
    public GameObject lockIcon;
    public GameObject infoIcon;
    public Image cityIcon;
    public Text cityText;

    [Header("------Player button------")]
    public Button playerLeftBtn;
    public Button playerRightBtn;
    public Text playerCountText;

    [Header("------Entry fee button------")]
    public Button entryFeeLeftBtn;
    public Button entryFeeRightBtn;
    public Text entryFeeCountText;

    [Header("------Play now button------")]
    public Image playNowImage;
    public Button playNowBtn;

    [Header("------Play now button sprites------")]
    public Sprite enableBtnSprite;
    public Sprite disableBtnSprite;

    float scrollSpeed = 1f;

    void Awake() {
        instance = this;
    }

    public void ScrollLeft(ScrollRect scrollRect) {
        // Debug.Log("ScrollLeft");
        // if (scrollRect != null) {
        //     Debug.Log("ScrollLeft not null normalize="+scrollRect.horizontalNormalizedPosition);
        //     if (scrollRect.horizontalNormalizedPosition >= 0f) {
        //         Debug.Log("ScrollLeft Normalized >= 0f");
        //         scrollRect.horizontalNormalizedPosition -= scrollSpeed;
        //     }
        // }
    }

    public void ScrollRight(ScrollRect scrollRect) {
        // Debug.Log("ScrollRight");
        // Debug.Log("ScrollRight not null normalize="+scrollRect.horizontalNormalizedPosition);
        // if (scrollRect != null) {
        //     if (scrollRect.horizontalNormalizedPosition <= 1f) {
        //         Debug.Log("ScrollRight Normalized >= 1f");
        //         scrollRect.horizontalNormalizedPosition += scrollSpeed;
        //     }
        // }
    }
}
