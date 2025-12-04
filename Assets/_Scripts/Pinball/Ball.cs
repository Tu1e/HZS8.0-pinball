using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public static event Action<Transform> OnBallInitialized;

    [SerializeField] private float maxSpeed = 20f; // maksimalna brzina loptice

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        OnBallInitialized?.Invoke(transform);
    }

    private void FixedUpdate()
    {
        LimitVelocity();
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
}
