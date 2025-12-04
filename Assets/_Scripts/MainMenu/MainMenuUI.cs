using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;          // Code
    [SerializeField] TMP_InputField inputFieldNickname;  // Nickname
    [SerializeField] Button playButton, nextButton;
    [SerializeField] GameObject leaderboardPanel, mainPanel;
    [SerializeField] Transform leaderboardContent;
    [SerializeField] GameObject leaderboardItemPrefab;
    [SerializeField] GameObject leaderboardCurrentPlaceItemPrefab;

    private const string URL = "https://gcdfqzveobylyonwydxr.supabase.co";
    private const string TABLE = "users";
    private const string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdjZGZxenZlb2J5bHlvbnd5ZHhyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MTc2NTgsImV4cCI6MjA4MDI5MzY1OH0.Qq8F6bScU_qy241N1UXcml1THbtxvuRSQ8vuhE00dPg";


    [Header("Score Display Settings")]
    public int scoreDigits = 7; // Broj cifara za prikaz (0000000)
    public Color normalColor = Color.white; // Boja za vodece nule
    public Color scoreColor = Color.green; // Boja za aktivan skor
    private void Start()
    {
        playButton.interactable = false;
        nextButton.interactable = false;

        inputFieldNickname.gameObject.SetActive(false);
    }

    private void Update()
    {
        string code = inputField.text.Trim();

        // NEXT dugme je aktivno samo ako ima smislen unos
        nextButton.interactable = code.Length >= 4 && !code.StartsWith(" ");

        // PLAY dugme je aktivno samo ako je nickname validan ili vec postoji
        if (inputFieldNickname.gameObject.activeSelf)
            playButton.interactable = inputFieldNickname.text.Trim().Length >= 3;
    }

    public void Next()
    {
        StartCoroutine(CheckCode(inputField.text.Trim()));
    }

    IEnumerator CheckCode(string code)
    {
        nextButton.interactable = false; // Zaključaj dok se proverava

        string url = $"{URL}/rest/v1/{TABLE}?select=id,username,highscore&code=eq.{code}";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("apikey", API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + API_KEY);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Supabase error: " + req.error);
            nextButton.interactable = true;
            yield break;
        }

        string json = req.downloadHandler.text;

        if (json.Length < 5)
        {
            Debug.Log("CODE ne postoji u bazi.");
            nextButton.interactable = true;
            yield break;
        }

        // Extract values
        string id = Extract(json, "id\":\"");
        string username = Extract(json, "username\":\"");
        string highscore = Extract(json, "highscore\":");

        // SAVE TO CONTROLLER
        SupabaseController.Instance.userId = id;
        SupabaseController.Instance.username = username;
        SupabaseController.Instance.highscore = int.Parse(highscore);

        Debug.Log($"USER LOADED -> ID:{id} | USERNAME:{username} | HS:{highscore}");
        playButton.gameObject.SetActive(true);
        if (string.IsNullOrEmpty(username) || username == "EMPTY")
        {
            // USER POSTOJI ali nema nickname
            inputFieldNickname.gameObject.SetActive(true);
            playButton.interactable = false;
        }
        else
        {
            // USER VEĆ IMA NICK
            inputFieldNickname.gameObject.SetActive(false);
            playButton.interactable = true;
        }

        nextButton.gameObject.SetActive(false); // sakrij NEXT zauvek
    }

    private string Extract(string json, string key)
    {
        int start = json.IndexOf(key);
        if (start < 0) return "";
        start += key.Length;
        int end = json.IndexOf("\"", start);
        if (end < 0) end = json.IndexOf("}", start);
        return json.Substring(start, end - start);
    }

    public void Play()
    {
        string nickname = inputFieldNickname.text.Trim();

        if (inputFieldNickname.gameObject.activeSelf)
            StartCoroutine(UpdateNickname(nickname));
        else
            SceneManager.LoadScene(1);
    }

    IEnumerator UpdateNickname(string nickname)
    {
        string id = SupabaseController.Instance.userId;
        string url = $"{URL}/rest/v1/{TABLE}?id=eq.{id}";
        string json = "{\"username\":\"" + nickname + "\"}";

        UnityWebRequest req = UnityWebRequest.Put(url, json);
        req.method = "PATCH";
        req.SetRequestHeader("apikey", API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Prefer", "return=minimal");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            SupabaseController.Instance.username = nickname;
            SceneManager.LoadScene(1);
        }
        else Debug.LogError("Nickname update error: " + req.error);
    }

    public void Leaderboard()
    {
        leaderboardPanel.SetActive(true);
        mainPanel.SetActive(false);

        StartCoroutine(LoadLeaderboard());
        StartCoroutine(LoadUserRank());
    }

    public void Back()
    {
        leaderboardPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    IEnumerator LoadLeaderboard()
    {
        string url = $"{URL}/rest/v1/{TABLE}?select=username,highscore&order=highscore.desc&limit=10";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("apikey", API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + API_KEY);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Leaderboard error: " + req.error);
            yield break;
        }

        string json = req.downloadHandler.text;
        Debug.Log("LEADERBOARD JSON: " + json);

        // Obrisi stari sadržaj
        foreach (Transform child in leaderboardContent)
            Destroy(child.gameObject);

        // JSON format od Supabase:  
        // [{"username":"Ana","highscore":221},{"username":"Mika","highscore":100}]
        int index = 0;
        int rank = 1;

        while (true)
        {
            int userIndex = json.IndexOf("\"username\":\"", index);
            if (userIndex < 0) break;
            userIndex += 12;

            int userEnd = json.IndexOf("\"", userIndex);
            string username = json.Substring(userIndex, userEnd - userIndex);

            int hsKey = json.IndexOf("\"highscore\":", userEnd) + 12;
            int hsEnd = json.IndexOf("}", hsKey);
            string highscore = json.Substring(hsKey, hsEnd - hsKey);

            // Napravi UI element
            GameObject prefabToUse = username == SupabaseController.Instance.username
                ? leaderboardCurrentPlaceItemPrefab
                : leaderboardItemPrefab;

            GameObject item = Instantiate(prefabToUse, leaderboardContent);

            TMP_Text[] fields = item.GetComponentsInChildren<TMP_Text>();
            fields[0].text = rank.ToString();         // #1
            fields[1].text = username;                // Player

            int scoreValue = int.Parse(highscore);
            fields[2].text = FormatScoreWithColor(scoreValue); // Score

            rank++;
            index = hsEnd;
        }
    }

    IEnumerator LoadUserRank()
    {
        string userId = SupabaseController.Instance.userId;
        if (string.IsNullOrEmpty(userId))
            yield break;

        string url = $"{URL}/rest/v1/rpc/get_user_rank";

        string json = "{\"uid\":\"" + userId + "\"}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("apikey", API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Rank error: " + req.error + "\n" + req.downloadHandler.text);
            yield break;
        }

        int rank = int.Parse(req.downloadHandler.text);

        GameObject item = Instantiate(leaderboardCurrentPlaceItemPrefab, leaderboardContent);
        TMP_Text[] fields = item.GetComponentsInChildren<TMP_Text>();
        fields[0].text = rank.ToString();
        fields[1].text = SupabaseController.Instance.username;
        fields[2].text = FormatScoreWithColor(SupabaseController.Instance.highscore);

        fields[1].color = Color.yellow; // highlight
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

}
