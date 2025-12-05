using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public static event Action<Transform> OnBallInitialized;

    [SerializeField] private float maxSpeed = 20f; // maksimalna brzina loptice
    int collisionCounter = 0;
    private Rigidbody2D rb;

    [SerializeField] float timer = 2f;
    float pTimer = 0f;

    [SerializeField] Collider2D collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        OnBallInitialized?.Invoke(transform);
    }

    private void Update()
    {
        pTimer += Time.deltaTime;
        if (pTimer >= timer)
        {
            pTimer = 0f;
            collisionCounter = 0;
            if (!collider.enabled)
            {
                Debug.Log(">>> COLLIDER ENABLED");
                collider.enabled = true;
            }
        }
    }

    private void FixedUpdate()
    {
        LimitVelocity();

        if(collisionCounter >= 12)
        {
            collisionCounter = 0;
            Debug.Log(">>> COLLIDER DISABLED");
            collider.enabled = false;
            pTimer = 1.95f;
        }
    }

    /// <summary>
    /// Ograničava brzinu loptice na maxSpeed.
    /// Ako je brža, usporava je na dozvoljenu brzinu.
    /// Ako je sporija, normalno ubrzava dok ne dostigne maxSpeed.
    /// </summary>
    private void LimitVelocity()
    {
        float currentSpeed = rb.linearVelocity.magnitude;

        if (currentSpeed > maxSpeed)
        {
            // Normalizuj vektor i množi sa maxSpeed
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionCounter++;
    }
}
