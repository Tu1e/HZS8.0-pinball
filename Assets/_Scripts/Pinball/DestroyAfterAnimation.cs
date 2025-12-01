using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    [Header("Destroy Settings")]
    [Tooltip("Koliko dugo traje animacija (u sekundama)")]
    public float lifetime = 1f;

    void Start()
    {
        // Automatski uništi objekat nakon što pro?e lifetime
        Destroy(gameObject, lifetime);
    }
}
