using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickableItem : MonoBehaviour, IPointerClickHandler
{
    public delegate void delegateOnClick(int id);
    public event delegateOnClick onLeftClick = (int id) => { };
    public event delegateOnClick onRightClick = (int id) => { };
    public int id;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onLeftClick(id);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRightClick(id);
        }
    }
}
