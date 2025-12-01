using UnityEngine;

public class StopperController : MonoBehaviour
{
    private Collider2D stopperCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        stopperCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        stopperCollider.isTrigger = false;
    }

    public void ResetGate()
    {
        stopperCollider.isTrigger = true;
    }

    public void ActivateGate()
    {
        stopperCollider.isTrigger = false;
    }
}