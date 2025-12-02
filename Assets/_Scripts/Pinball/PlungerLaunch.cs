using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))] // Automatski dodaje AudioSource ako ga nema
public class PlungerScript : MonoBehaviour
{
    [Header("Launch Settings")]
    public float snagaLansiranja = 1000f;
    public float maxPovlacenje = 1.0f;
    public float brzinaPovlacenja = 2.0f;

    [Header("References")]
    public Animator springAnimator;
    public string pullAnimationName = "SpringPull";

    [Header("Input Settings")]
    public bool useKeyboard = true;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip pullClip;      // Zvuk dok se nateže (npr. krckanje opruge)
    public AudioClip releaseClip;   // Zvuk kad se pusti (npr. BOING)
    [Range(0.5f, 3.0f)]
    public float minPitch = 0.8f;   // Pitch na početku povlačenja
    [Range(0.5f, 3.0f)]
    public float maxPitch = 1.5f;   // Pitch na maksimalnom povlačenju

    private Rigidbody2D rb;
    private Vector3 startPozicija;
    [SerializeField] private float trenutnoPovlacenje = 0f;
    private GameObject loptica = null;
    private bool isButtonPressed = false;
    private bool wasPressingLastFrame = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPozicija = transform.position;

        // Audio setup
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Default za pull zvuk je loop

        // Animator setup
        if (springAnimator == null) springAnimator = GetComponentInChildren<Animator>();

        if (springAnimator != null)
        {
            springAnimator.speed = 0;
        }
    }

    void Update()
    {
        bool isPulling = false;

        // --- INPUT ---
        if (useKeyboard && Input.GetKey(KeyCode.Space)) isPulling = true;
        if (isButtonPressed) isPulling = true;

        // --- 1. LOGIKA POVLAČENJA I ZVUKA ---
        // Proveravamo da li vučemo I da li nismo stigli do kraja
        if (isPulling && trenutnoPovlacenje < maxPovlacenje)
        {
            // Pomeranje
            trenutnoPovlacenje += brzinaPovlacenja * Time.deltaTime;
            trenutnoPovlacenje = Mathf.Clamp(trenutnoPovlacenje, 0, maxPovlacenje);

            // -- AUDIO LOGIKA ZA POVLAČENJE --
            if (pullClip != null)
            {
                if (!audioSource.isPlaying || audioSource.clip != pullClip)
                {
                    audioSource.clip = pullClip;
                    audioSource.loop = true; // Želimo da se ponavlja dok držimo
                    audioSource.Play();
                }

                // Dinamički menjamo pitch na osnovu procenta zategnutosti
                float progress = trenutnoPovlacenje / maxPovlacenje;
                // Lerp: Ako je progress 0 -> minPitch, ako je 1 -> maxPitch
                audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, progress);
            }
        }
        else
        {
            // Ako smo stali sa povlačenjem (pustili taster ILI stigli do maxa), 
            // a zvuk natezanja još ide -> ugasi ga.
            if (audioSource.isPlaying && audioSource.clip == pullClip)
            {
                audioSource.Stop();
            }
        }

        // --- SINHRONIZACIJA ANIMACIJE ---
        float normalizedPull = trenutnoPovlacenje / maxPovlacenje;
        UpdateSpringAnimation(normalizedPull);

        // --- FIZIKA ---
        if (rb != null)
        {
            Vector3 targetPos = startPozicija - new Vector3(0, trenutnoPovlacenje, 0);
            rb.MovePosition(targetPos);
        }

        // --- 2. LANSIRANJE ---
        bool justReleased = false;
        if (useKeyboard && Input.GetKeyUp(KeyCode.Space)) justReleased = true;
        if (wasPressingLastFrame && !isButtonPressed) justReleased = true;

        if (justReleased && trenutnoPovlacenje > 0)
        {
            Lansiraj();
        }

        wasPressingLastFrame = isButtonPressed;
    }

    void UpdateSpringAnimation(float normalizedPull)
    {
        if (springAnimator != null)
        {
            // Clamp01 osigurava da ostane na poslednjem frejmu ako je >= 1
            float finalTime = Mathf.Clamp01(normalizedPull);
            springAnimator.Play(pullAnimationName, 0, finalTime);
            springAnimator.speed = 0f;
        }
    }

    void Lansiraj()
    {
        if (releaseClip != null && audioSource != null)
        {
            audioSource.Stop();

            audioSource.loop = false;

            audioSource.pitch = 1.0f;

            audioSource.PlayOneShot(releaseClip);
        }

        // A. Izbaci lopticu
        if (loptica != null)
        {
            loptica.transform.parent = null;
            Rigidbody2D ballRb = loptica.GetComponent<Rigidbody2D>();

            if (ballRb != null)
            {
                ballRb.isKinematic = false;
                float procenatSnage = trenutnoPovlacenje / maxPovlacenje;
                ballRb.AddForce(Vector2.up * snagaLansiranja * procenatSnage, ForceMode2D.Impulse);
            }
            loptica = null;
        }

        // B. Vrati oprugu
        StopAllCoroutines();
        StartCoroutine(ReleaseRoutine());
    }

    IEnumerator ReleaseRoutine()
    {
        float startValue = trenutnoPovlacenje;
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            trenutnoPovlacenje = Mathf.Lerp(startValue, 0f, elapsed / duration);

            if (rb != null)
                rb.MovePosition(startPozicija - new Vector3(0, trenutnoPovlacenje, 0));

            UpdateSpringAnimation(trenutnoPovlacenje / maxPovlacenje);

            yield return null;
        }

        trenutnoPovlacenje = 0f;
        if (rb != null) rb.MovePosition(startPozicija);
        UpdateSpringAnimation(0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            loptica = other.gameObject;
            Rigidbody2D ballRb = loptica.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
                ballRb.isKinematic = true;
            }
            loptica.transform.parent = transform;
        }
    }

    public void OnButtonDown() => isButtonPressed = true;
    public void OnButtonUp() => isButtonPressed = false;
}