using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour
{
    [Header("Bumper Settings")]
    public float silaUdarca = 200f;
    public float vizuelniTrzaj = 0.1f;
    public float brzinaVracanja = 15f; // This is now used to control shake speed
    public float shakeDuration = 0.2f; // How long the bumper shakes
    public int bodovi = 100;
    
    [Header("Audio Settings")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private Vector3 startPozicija;
    private Coroutine shakeCoroutine;

    void Start()
    {
        startPozicija = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pinball"))
        {
            // Pusti 2D zvuk
            AudioHelper.Play2DSound(hitSound, soundVolume);
            
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                Vector2 pravacOdbijanja = -collision.contacts[0].normal;
                ballRb.AddForce(pravacOdbijanja * silaUdarca, ForceMode2D.Impulse);
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(bodovi);
            }

            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }
            shakeCoroutine = StartCoroutine(ShakeBumper());
        }
    }

    IEnumerator ShakeBumper()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Generate a random direction and move the bumper
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 targetPosition = startPozicija + randomDirection * vizuelniTrzaj;
            
            // Move towards the random position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * brzinaVracanja);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After shaking, ensure it returns to the start position
        transform.position = startPozicija;
    }
}