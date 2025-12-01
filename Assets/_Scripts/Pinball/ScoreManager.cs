using UnityEngine;
using TMPro; // POTREBNO ZA RAD SA TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText2;

    public GameObject gameOverPanel;

    private int currentScore = 0;

    private void OnEnable()
    {
        GameManager.OnGameOver += GameOverScreen;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= GameOverScreen;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + currentScore.ToString();
    }

    void GameOverScreen()
    {
        gameOverPanel.SetActive(true);
        scoreText.gameObject.SetActive(false);   
        scoreText2.text = "Score: " + currentScore.ToString();
    }
}