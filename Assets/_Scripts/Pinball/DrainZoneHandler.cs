using UnityEngine;

public class DrainZoneHandler : MonoBehaviour
{
    private GameManager gameManager;
    
    [Header("Audio Settings")]
    public AudioClip dieSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager nije pronađen u sceni!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameManager != null && other.CompareTag("Pinball"))
        {
            gameManager.LoseLife();
            
            // Pusti 2D zvuk
            AudioHelper.Play2DSound(dieSound, soundVolume);
        }
    }
}