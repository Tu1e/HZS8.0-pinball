using UnityEngine;
using TMPro; // POTREBNO ZA RAD SA TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText2;

    public GameObject gameOverPanel;

    [Header("Score Display Settings")]
    public int scoreDigits = 7; // Broj cifara za prikaz (0000000)
    public Color normalColor = Color.white; // Boja za vodece nule
    public Color scoreColor = Color.green; // Boja za aktivan skor

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
        string formattedScore = FormatScoreWithColor(currentScore);
        scoreText.text = formattedScore;
    }

    string FormatScoreWithColor(int score)
    {
        // Konvertuj skor u string sa vode?im nulama
        string scoreString = score.ToString().PadLeft(scoreDigits, '0');
        
        // Ako je skor 0, sve su nule normalne boje
        if (score == 0)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(normalColor)}>{scoreString}</color>";
        }
        
        // Prona?i prvu cifru koja nije nula
        int firstNonZeroIndex = scoreString.Length;
        for (int i = 0; i < scoreString.Length; i++)
        {
            if (scoreString[i] != '0')
            {
                firstNonZeroIndex = i;
                break;
            }
        }
        
        // Podeli string na vodece nule i aktivan skor
        string leadingZeros = scoreString.Substring(0, firstNonZeroIndex);
        string activeScore = scoreString.Substring(firstNonZeroIndex);
        
        // Formatiraj sa bojama
        string normalColorHex = ColorUtility.ToHtmlStringRGB(normalColor);
        string scoreColorHex = ColorUtility.ToHtmlStringRGB(scoreColor);
        
        return $"<color=#{normalColorHex}>{leadingZeros}</color><color=#{scoreColorHex}>{activeScore}</color>";
    }

    void GameOverScreen()
    {
        gameOverPanel.SetActive(true);
        scoreText.gameObject.SetActive(false);
        
        // Prikazi formatiran skor na game over ekranu
        string formattedScore = FormatScoreWithColor(currentScore);
        scoreText2.text = formattedScore;
    }
}