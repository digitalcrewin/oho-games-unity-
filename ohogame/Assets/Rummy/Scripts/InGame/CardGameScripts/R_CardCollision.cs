using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_CardCollision : MonoBehaviour
{
    public R_CardView parentCardView;
    bool selected = false;

    public void SetSelected(bool selected)
    {
        this.selected = selected;
    }
    private void Start()
    {
        parentCardView = GetComponentInParent<R_CardView>();
    }
    //Child Collision of cards
    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.tag != "DropCard")
        {
            if (selected)
            {
                R_CardCollision cardCollided = collider.gameObject.GetComponent<R_CardCollision>();
                if (cardCollided != null)
                {
                    parentCardView.SetDummyParent(cardCollided.parentCardView.gameObject.transform.parent);
                    parentCardView.SetCardIndexToFill(cardCollided.parentCardView.transform.GetSiblingIndex());
                }
                else if (collider.gameObject.GetComponent<R_GroupView>() != null)
                {
                    parentCardView.SetDummyParent(collider.gameObject.transform);
                    parentCardView.SetCardIndexToFill(collider.transform.childCount);
                }
            }
        }
    }
}
