using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class R_SortCardInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        //if (eventData.pointerCurrentRaycast.gameObject != null)
        //{
        //    Debug.Log("OnDrag " + eventData.pointerCurrentRaycast.gameObject.name);
        //}

        if (R_SortCardManager.instance.SelectedCard != null)
        {
            R_SortCardManager.instance.MoveSelectedCard(eventData.position);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //Debug.Log("OnPointerDown " + eventData.pointerCurrentRaycast.gameObject.name);
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<R_SortCardView>() != null)
            {
                R_SortCardManager.instance.SelectCard(eventData.pointerCurrentRaycast.gameObject.GetComponent<R_SortCardView>());
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //Debug.Log("OnPointerUp " + eventData.pointerCurrentRaycast.gameObject.name);
        }

        R_SortCardManager.instance.OnCardRelease();
    }
}
