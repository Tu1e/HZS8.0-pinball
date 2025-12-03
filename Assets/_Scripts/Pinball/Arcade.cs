using UnityEngine;

public class Arcade : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] int points;

    [Header("Audio Settings")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            ScoreManager.Instance.AddScore(points);
            AudioHelper.Play2DSound(hitSound, soundVolume);
            animator.SetTrigger("Hit");
        }
    }
}
