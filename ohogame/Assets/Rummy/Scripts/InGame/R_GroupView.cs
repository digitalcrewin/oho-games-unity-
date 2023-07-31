using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_GroupView : MonoBehaviour
{
    public static R_GroupView instance;
    public bool isThisGroupSelected;

    //int groupId;
    [SerializeField] BoxCollider2D groupColliderRight;

    private void Awake()
    {
        instance = this;
    }

    private void OnRectTransformDimensionsChange()
    {
        SetColliderPosition();
    }
    public void SetColliderPosition()
    {
        float width = gameObject.GetComponent<RectTransform>().rect.width;
        groupColliderRight.offset = new Vector2((width / 2) - 4, 0);
    }
}
