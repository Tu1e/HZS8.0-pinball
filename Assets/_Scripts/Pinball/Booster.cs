using UnityEngine;

public class Booster : MonoBehaviour
{
    [SerializeField] float boostForce = 1.0f;

    [Header("Audio Settings")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            if(hitSound != null)
            {
                AudioHelper.Play2DSound(hitSound, soundVolume);
            }
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
