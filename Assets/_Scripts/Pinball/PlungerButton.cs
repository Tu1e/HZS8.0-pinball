using UnityEngine;
using UnityEngine.EventSystems;

public class PlungerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Plunger Reference")]
    public PlungerScript plungerScript;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (plungerScript != null)
        {
            plungerScript.OnButtonDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (plungerScript != null)
        {
            plungerScript.OnButtonUp();
        }
    }
}
