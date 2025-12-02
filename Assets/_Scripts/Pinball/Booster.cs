using UnityEngine;

public class Booster : MonoBehaviour
{
    [SerializeField] float boostForce = 1.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            rb.linearVelocityY = 0;
            rb.AddForceY(boostForce);
            DestroyBooser();
        }
    }

    private void DestroyBooser()
    {
        gameObject.SetActive(false);
    }
}
