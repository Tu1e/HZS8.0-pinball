using UnityEngine;
using System.Collections;

public class RocketLaunch : MonoBehaviour
{
    public float interval = 2.5f;
    public float launchForce = 10f;
    public float activationDistance = 1.0f;

    [Header("Audio Settings")]
    public AudioClip launchSound;
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("Animation Settings")]
    public float animationDuration = 0.917f; // Tačna dužina animacije u sekundama
    public float activeLaunchTime = 1f;

    public GameObject launcherVisuals;

    private Animator animator;
    private BoxCollider2D box;

    void Start()
    {
        animator = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();

        if (launcherVisuals != null)
        {
            launcherVisuals.SetActive(false);
        }

        StartCoroutine(ShootingLoop());
    }

    IEnumerator ShootingLoop()
    {
        while (true)
        {
            // A. ZAMRZNI NA FRAME 0
            if (animator != null)
            {
                animator.Play("ShootAnim", -1, 0f);
                animator.speed = 0f;
            }
            
            // B. PAUZA - čekaj interval
            yield return new WaitForSeconds(interval);

            // C. AKTIVIRAJ VIZUELNI OBJEKAT
            if (launcherVisuals != null)
            {
                launcherVisuals.SetActive(true);
            }
            
            // Pusti zvuk animacije
            AudioHelper.Play2DSound(launchSound, soundVolume);
            
            // D. POKRENI ANIMACIJU
            if (animator != null)
            {
                animator.Play("ShootAnim", -1, 0f);
                animator.speed = 1f;
            }

            // E. FAZA DETEKCIJE - traje koliko god je potrebno (max activeLaunchTime ili dok se ne završi animacija)
            float elapsed = 0f;
            float detectionTime = Mathf.Min(activeLaunchTime, animationDuration);
            
            while (elapsed < detectionTime)
            {
                CheckAndLaunchPinball();
                elapsed += Time.deltaTime;
                yield return null;
            }

            // F. DEAKTIVIRAJ VIZUELNI OBJEKAT
            if (launcherVisuals != null)
            {
                launcherVisuals.SetActive(false);
            }

            // G. ČEKAJ PREOSTALI DEO ANIMACIJE (ako ga ima)
            float remainingTime = animationDuration - detectionTime;
            if (remainingTime > 0)
            {
                yield return new WaitForSeconds(remainingTime);
            }
            
            // H. ZAUSTAVI ANIMACIJU
            if (animator != null)
            {
                animator.speed = 0f;
            }
        }
    }

    void CheckAndLaunchPinball()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(box.bounds.center, box.bounds.size, 0f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Pinball"))
            {
                Rigidbody2D ballRb = hit.GetComponent<Rigidbody2D>();
                if (ballRb == null) continue;

                Vector2 bottomLeft = box.bounds.min;
                float distance = Vector2.Distance(hit.transform.position, bottomLeft);

                if (distance < activationDistance)
                {
                    Vector2 direction = new Vector2(-1, -1).normalized;

                    ballRb.linearVelocity = Vector2.zero;
                    ballRb.AddForce(direction * launchForce, ForceMode2D.Impulse);
                    
                    AudioHelper.Play2DSound(hitSound, soundVolume);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (GetComponent<BoxCollider2D>() == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetComponent<BoxCollider2D>().bounds.center, GetComponent<BoxCollider2D>().bounds.size);

        Gizmos.color = Color.yellow;
        Vector2 bottomLeft = GetComponent<BoxCollider2D>().bounds.min;
        Gizmos.DrawWireSphere(bottomLeft, activationDistance);
    }
}