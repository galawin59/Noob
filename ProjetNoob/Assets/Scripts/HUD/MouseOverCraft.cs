using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverCraft : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void delegateOnMouseOver(int id);
    public event delegateOnMouseOver onMouseOverEnter = (int id) => { };
    public event delegateOnMouseOver onMouseOverExit = (int id) => { };

    public int id;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        onMouseOverEnter(id);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        onMouseOverExit(id);
    }
}
