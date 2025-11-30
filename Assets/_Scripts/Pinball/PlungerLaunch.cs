using UnityEngine;

public class PlungerScript : MonoBehaviour
{
    public float snagaLansiranja = 1000f;
    public float maxPovlacenje = 1.0f;
    public float brzinaPovlacenja = 2.0f;

    private Rigidbody2D rb;
    private Vector3 startPozicija;
    private float trenutnoPovlacenje = 0f;
    private GameObject loptica = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Ovo je na Plunger_Pivot
        startPozicija = transform.position;
    }

    void Update()
    {
        // 1. Spuštanje
        if (Input.GetKey(KeyCode.Space))
        {
            trenutnoPovlacenje += brzinaPovlacenja * Time.deltaTime;
            trenutnoPovlacenje = Mathf.Clamp(trenutnoPovlacenje, 0, maxPovlacenje);
        }

        // 2. Lansiranje
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Lansiraj();
        }

        // Pomeramo Pivot (a on nosi i grafiku i lopticu)
        rb.MovePosition(startPozicija - new Vector3(0, trenutnoPovlacenje, 0));
    }

    void Lansiraj()
    {
        if (loptica != null)
        {
            // A. Odvajamo lopticu
            loptica.transform.parent = null;

            // B. Vraćamo fiziku loptici! (KLJUČNO ZA POPRAVKU PENJANJA)
            Rigidbody2D ballRb = loptica.GetComponent<Rigidbody2D>();
            ballRb.isKinematic = false; // Vracamo da bude Dynamic

            // C. Lansiramo
            float procenatSnage = trenutnoPovlacenje / maxPovlacenje;
            ballRb.AddForce(Vector2.up * snagaLansiranja * procenatSnage, ForceMode2D.Impulse);
        }

        trenutnoPovlacenje = 0f;
        loptica = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball"))
        {
            loptica = other.gameObject;
            Rigidbody2D ballRb = loptica.GetComponent<Rigidbody2D>();

            // 1. GASIMO FIZIKU (KLJUČNO)
            // Loptica postaje duh - ne reaguje na zidove ni gravitaciju
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
            ballRb.isKinematic = true;

            // 2. Lepimo je za Pivot (koji ima skalu 1,1,1)
            loptica.transform.parent = transform;

            // 3. Centriramo lopticu (da ne bude ukrivo) - Opciono
            // loptica.transform.localPosition = new Vector3(0, 0.5f, 0); 
        }
    }
}