using UnityEngine;
using System.Collections;

public class ScoringObject : MonoBehaviour
{
    public int points = 10;
    public Color flashColor = Color.white;
    public float popScale = 1.2f; 
    public float effectDuration = 0.2f;
    
    [Header("Audio Settings")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private Color originalColor;
    private Coroutine effectCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalScale = transform.localScale;
            originalColor = spriteRenderer.color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pinball"))
        {
            // Pusti 2D zvuk
            AudioHelper.Play2DSound(hitSound, soundVolume);
            
            ScoreManager.Instance.AddScore(points);

            if (spriteRenderer != null)
            {
                if (effectCoroutine != null)
                {
                    StopCoroutine(effectCoroutine);
                    transform.localScale = originalScale;
                    spriteRenderer.color = originalColor;
                }
                effectCoroutine = StartCoroutine(HitEffect());
            }
        }
    }

    private IEnumerator HitEffect()
    {
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * popScale;

        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / effectDuration;

            float scaleProgress = Mathf.Sin(progress * Mathf.PI);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleProgress);

            spriteRenderer.color = Color.Lerp(flashColor, originalColor, progress);

            yield return null;
        }

        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;
    }
}