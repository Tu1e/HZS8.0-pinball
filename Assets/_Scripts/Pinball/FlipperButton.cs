using UnityEngine;
using UnityEngine.EventSystems;

public class FlipperButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Flipper Reference")]
    public FlipperController flipperController;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (flipperController != null)
        {
            flipperController.OnButtonDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (flipperController != null)
        {
            flipperController.OnButtonUp();
        }
    }
}
