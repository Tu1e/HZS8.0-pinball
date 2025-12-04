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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            float rand = Random.Range(-500f, 500f);
            other.GetComponent<Rigidbody2D>().AddForceX(rand);
        }
    }
}