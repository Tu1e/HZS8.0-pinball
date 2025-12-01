using UnityEngine;

public class ExplosionISS : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab;
    public int points = 100;
    
    [Header("Boost Settings")]
    public float boostForce = 300f; // Sila boosta
    public bool boostAwayFromCenter = true; // Boost od centra ISS-a ili u smeru kretanja loptice
    
    [Header("Audio Settings")]
    public AudioClip explosionSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            // Dodaj poene
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(points);
            }

            // Boostuj lopticu
            Rigidbody2D ballRb = other.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                Vector2 boostDirection;
                
                if (boostAwayFromCenter)
                {
                    // Boost lopticu od centra ISS objekta
                    boostDirection = (other.transform.position - transform.position).normalized;
                }
                else
                {
                    // Boost u smeru trenutnog kretanja
                    boostDirection = ballRb.linearVelocity.normalized;
                }
                
                ballRb.AddForce(boostDirection * boostForce, ForceMode2D.Impulse);
            }

            // Spawn explosion animaciju na centru ovog objekta
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Pusti 2D zvuk eksplozije
            AudioHelper.Play2DSound(explosionSound, soundVolume);

            // Uništi ISS objekat
            Destroy(gameObject);
        }
    }
}
