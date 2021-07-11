using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseEvent : EventComponent, IPointerClickHandler, 
    IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SendListener("OnPointerClick");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SendListener("OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SendListener("OnPointerExit");
    }
}
