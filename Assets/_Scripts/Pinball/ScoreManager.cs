using System; // POTREBNO ZA RAD SA TextMeshPro
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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

    public static event Action<int> ScoreChanged;

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
        ScoreChanged?.Invoke(points);
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

        string formattedScore = FormatScoreWithColor(currentScore);
        scoreText2.text = formattedScore;

        // 🟢 PROVERA HIGHSCORE-A
        if (currentScore > SupabaseController.Instance.highscore)
        {
            Debug.Log($"Novi highscore! Stari: {SupabaseController.Instance.highscore}, Novi: {currentScore}");

            // Update u kontroleru
            SupabaseController.Instance.highscore = currentScore;

            // Update u Supabase bazi
            StartCoroutine(UpdateHighscoreInDatabase(currentScore));
        }
        else
        {
            Debug.Log("Score nije veći od postojeće vrednosti. Highscore ostaje isti.");
        }
    }

    IEnumerator UpdateHighscoreInDatabase(int newScore)
    {
        string url = $"https://gcdfqzveobylyonwydxr.supabase.co/rest/v1/users?id=eq.{SupabaseController.Instance.userId}";
        string json = "{\"highscore\":" + newScore + "}";

        UnityWebRequest req = UnityWebRequest.Put(url, json);
        req.method = "PATCH";
        req.SetRequestHeader("apikey", SupabaseController.API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + SupabaseController.API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Prefer", "return=minimal");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            Debug.Log($"[SUPABASE] Novi highscore upisan: {newScore}");
        else
            Debug.LogError("[SUPABASE] Greška pri upisu: " + req.error);
    }


}