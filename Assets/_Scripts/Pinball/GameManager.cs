using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Podešavanja Života")]
    public int maxLives = 5;
    public Transform respawnPosition;
    public StopperController launcherStopper;
    public StopperSensor stopperSensor;
    [Header("Reference")]
    public GameObject ballPrefab;
    private GameObject currentBall;
    public AudioClip gameOver;
    private int lives;

    public static event Action OnGameOver;
    public static event Action OnBallUsed;

    void Start()
    {
        lives = maxLives;

        SpawnBall();
    }

    void SpawnBall()
    {
        if (lives <= 0)
        {
            if(gameOver != null)
            {
                AudioSource.PlayClipAtPoint(gameOver, Camera.main.transform.position);
            }

            OnBallUsed?.Invoke();
            OnGameOver?.Invoke();
            Debug.Log("GAME OVER! Nema više života.");
            return;
        }

        if (currentBall == null)
        {
            currentBall = Instantiate(ballPrefab, respawnPosition.position, Quaternion.identity);
            currentBall.tag = "Pinball";

        }
        else
        {
            currentBall.transform.position = respawnPosition.position;

            Rigidbody2D ballRb = currentBall.GetComponent<Rigidbody2D>();
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
            ballRb.isKinematic = true;

            Collider2D ballCol = currentBall.GetComponent<Collider2D>();
            if (ballCol != null) ballCol.enabled = true;

            currentBall.transform.parent = null;
        }
        if (launcherStopper != null)
        {
            launcherStopper.ResetGate();
        }
        if (stopperSensor != null)
        {
            stopperSensor.ResetSensor();
        }
            if (lives < 5)
                OnBallUsed?.Invoke();

        Debug.Log("Loptica respawnana. Životi preostali: " + lives);
    }

    public void LoseLife()
    {
        if (currentBall != null && lives > 0)
        {
            lives--;
            SpawnBall();
        }
        else if (lives <= 0)
        {
            Debug.Log("GAME OVER! (Konačna provera)");

            OnBallUsed?.Invoke();
            OnGameOver?.Invoke();
        }
    }
}