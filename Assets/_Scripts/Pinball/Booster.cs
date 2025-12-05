using UnityEngine;

public class Booster : MonoBehaviour
{
    [SerializeField] float boostForce = 15f; // Boost speed in units/second

    [Header("Audio Settings")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            Debug.Log("Booster triggered!");
            
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Debug.Log($"Before boost - Velocity: {rb.linearVelocity}");
                
                if(hitSound != null)
                {
                    AudioHelper.Play2DSound(hitSound, soundVolume);
                }
                
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(100);
                }
                
                // Directly set velocity for instant boost (bypasses physics)
                // Keep horizontal velocity, but set vertical velocity to boost value
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, boostForce);
                
                Debug.Log($"After boost - New Velocity: {rb.linearVelocity}");
                
                DestroyBooser();
            }
            else
            {
                Debug.LogWarning("Pinball has no Rigidbody2D!");
            }
        }
    }

    private void DestroyBooser()
    {
        gameObject.SetActive(false);
    }
}
