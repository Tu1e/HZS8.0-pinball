using UnityEngine;

public class ScoringObject : MonoBehaviour
{
    public int points = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Ball"))
        {
            ScoreManager.Instance.AddScore(points);

            Debug.Log(gameObject.name + " je pogođen! Dodato " + points + " bodova.");
        }
    }
}