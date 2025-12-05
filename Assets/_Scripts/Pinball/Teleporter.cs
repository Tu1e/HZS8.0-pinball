using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    [Tooltip("Destinacija gde ?e loptica biti teleportovana")]
    public Transform destination;
    
    [Tooltip("Offset od destinacije (opciono)")]
    public Vector2 destinationOffset = Vector2.zero;
    
    [Header("Timing Settings")]
    [Tooltip("Koliko dugo je teleporter aktivan (u sekundama)")]
    public float activeTime = 3f;
    
    [Tooltip("Koliko dugo je teleporter neaktivan (u sekundama)")]
    public float inactiveTime = 5f;
    
    [Header("Audio Settings")]
    public AudioClip teleportSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    [Header("Visual Effects (Optional)")]
    public GameObject teleportEffect;
    
    [Header("Visual Feedback")]
    [Tooltip("SpriteRenderer koji ?e se ugasiti kada je teleporter neaktivan")]
    public SpriteRenderer teleporterVisual;
    
    [Tooltip("Boja kada je aktivan")]
    public Color activeColor = Color.green;
    
    [Tooltip("Boja kada je neaktivan")]
    public Color inactiveColor = Color.red;
    
    private bool isActive = true;
    private Collider2D teleporterCollider;
    
    private void Start()
    {
        teleporterCollider = GetComponent<Collider2D>();
        
        // Ako nema assignovanog SpriteRenderer-a, pokušaj na?i ga
        if (teleporterVisual == null)
        {
            teleporterVisual = GetComponent<SpriteRenderer>();
        }
        
        // Pokreni timer ciklus
        StartCoroutine(TeleporterCycle());
    }
    
    private IEnumerator TeleporterCycle()
    {
        while (true)
        {
            // AKTIVNA FAZA - 3 sekunde
            isActive = true;
            SetTeleporterState(true);
            Debug.Log("Teleporter AKTIVAN!");
            
            yield return new WaitForSeconds(activeTime);
            
            // NEAKTIVNA FAZA - 5 sekundi
            isActive = false;
            SetTeleporterState(false);
            Debug.Log("Teleporter NEAKTIVAN!");
            
            yield return new WaitForSeconds(inactiveTime);
        }
    }
    
    private void SetTeleporterState(bool active)
    {
        // Omogu?i/onemogu?i collider
        if (teleporterCollider != null)
        {
            teleporterCollider.enabled = active;
        }
        
        // Promeni boju sprite-a za vizuelni feedback
        if (teleporterVisual != null)
        {
            teleporterVisual.color = active ? activeColor : inactiveColor;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Proveri da li je teleporter aktivan
        if (!isActive)
        {
            Debug.Log("Teleporter je trenutno neaktivan!");
            return;
        }
        
        if (other.CompareTag("Pinball"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            
            if (rb != null && destination != null)
            {
                // Sa?uvaj trenutnu brzinu (direkciju i intenzitet)
                Vector2 currentVelocity = rb.linearVelocity;
                
                Debug.Log($"Teleporting ball from {other.transform.position} to {destination.position}");
                Debug.Log($"Preserving velocity: {currentVelocity}");
                
                // Teleportuj lopticu na destinaciju sa offsetom
                Vector2 targetPosition = (Vector2)destination.position + destinationOffset;
                other.transform.position = targetPosition;
                
                // Vrati istu brzinu (zadržava direkciju)
                rb.linearVelocity = currentVelocity;
                
                // Pusti zvuk teleportacije
                if (teleportSound != null)
                {
                    AudioHelper.Play2DSound(teleportSound, soundVolume);
                }
                
                // Spawn visual effect na oba mesta (opciono)
                if (teleportEffect != null)
                {
                    // Effect na ulazu (ovde)
                    Instantiate(teleportEffect, transform.position, Quaternion.identity);
                    
                    // Effect na izlazu (destinacija)
                    Instantiate(teleportEffect, targetPosition, Quaternion.identity);
                }
            }
            else
            {
                if (rb == null)
                {
                    Debug.LogWarning("Pinball nema Rigidbody2D komponentu!");
                }
                if (destination == null)
                {
                    Debug.LogWarning("Teleporter nema postavljenu destinaciju! Dodeli Transform u Inspector-u.");
                }
            }
        }
    }
    
    // Pomo?na funkcija za vizualizaciju u Scene view-u
    private void OnDrawGizmos()
    {
        if (destination != null)
        {
            // Linija od teleportera do destinacije (promenite boju u zavisnosti od stanja)
            Gizmos.color = isActive ? Color.cyan : Color.gray;
            Vector3 targetPos = (Vector2)destination.position + destinationOffset;
            Gizmos.DrawLine(transform.position, targetPos);
            
            // Krug na destinaciji
            Gizmos.color = isActive ? Color.green : Color.red;
            Gizmos.DrawWireSphere(targetPos, 0.5f);
            
            // Strelica za pokazivanje pravca
            if (isActive)
            {
                Vector3 direction = (targetPos - transform.position).normalized;
                Vector3 midPoint = Vector3.Lerp(transform.position, targetPos, 0.5f);
                Gizmos.DrawLine(midPoint, midPoint + direction * 0.3f);
            }
        }
    }
}
