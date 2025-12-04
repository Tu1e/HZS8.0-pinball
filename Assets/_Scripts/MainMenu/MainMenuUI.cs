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
    [SerializeField] GameObject homeButton;
    [SerializeField] CanvasGroup mCG, lCG;
    [SerializeField] GameObject nicknameWarningText; // UI warning text
    [SerializeField] TextAsset badWordsFile;       // lokalna lista


    private const string URL = "https://gcdfqzveobylyonwydxr.supabase.co";
    private const string TABLE = "users";
    private const string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdjZGZxenZlb2J5bHlvbnd5ZHhyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MTc2NTgsImV4cCI6MjA4MDI5MzY1OH0.Qq8F6bScU_qy241N1UXcml1THbtxvuRSQ8vuhE00dPg"; // ubaci key

    [Header("Score Display Settings")]
    public int scoreDigits = 7;
    public Color normalColor = Color.white;
    public Color scoreColor = Color.green;

    private void Start()
    {
        playButton.interactable = false;
        nextButton.interactable = false;
        inputFieldNickname.gameObject.SetActive(false);
    }

    private void Update()
    {
        string code = inputField.text.Trim();

        // NEXT aktivan samo ako je kod validan
        nextButton.interactable = code.Length >= 4 && !code.StartsWith(" ");

        // PLAY aktivan samo ako nickname ima smisla
        if (inputFieldNickname.gameObject.activeSelf)
            playButton.interactable = inputFieldNickname.text.Trim().Length >= 3;
    }

    public void Next()
    {
        StartCoroutine(CheckCode(inputField.text.Trim()));
    }

    IEnumerator CheckCode(string code)
    {
        nextButton.interactable = false;

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

        string id = Extract(json, "id\":\"");
        string username = Extract(json, "username\":\"");
        string highscore = Extract(json, "highscore\":");

        SupabaseController.Instance.userId = id;
        SupabaseController.Instance.username = username;
        SupabaseController.Instance.highscore = int.Parse(highscore);

        Debug.Log($"USER LOADED -> ID:{id} | USERNAME:{username} | HS:{highscore}");

        inputFieldNickname.gameObject.SetActive(true);

        inputFieldNickname.text =
            (string.IsNullOrEmpty(username) || username == "EMPTY") ? "" : username;

        playButton.gameObject.SetActive(true);
        playButton.interactable = inputFieldNickname.text.Trim().Length >= 3;

        nextButton.gameObject.SetActive(false);
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
        string newNick = inputFieldNickname.text.Trim();
        string oldNick = SupabaseController.Instance.username;

        playButton.interactable = false; // blok dok proveravamo

        // prvo proveri blacklist
        if (IsBadWord(newNick))
        {
            nicknameWarningText.SetActive(true);
            playButton.interactable = false;
            return;
        }

        if (newNick != oldNick)
            StartCoroutine(CheckNicknameAvailability(newNick));
        else
        {
            nicknameWarningText.SetActive(false);
            SceneManager.LoadScene(1);
        }
            
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
        homeButton.SetActive(true);

        mCG.blocksRaycasts = false;
        lCG.blocksRaycasts = true;

        StartCoroutine(LoadLeaderboard());
        StartCoroutine(LoadUserRank());
    }

    public void Back()
    {
        leaderboardPanel.SetActive(false);
        mainPanel.SetActive(true);
        homeButton.SetActive(false);
        mCG.blocksRaycasts = true;
        lCG.blocksRaycasts = false;
    }

    IEnumerator LoadLeaderboard()
    {
        string url = $"{URL}/rest/v1/{TABLE}?select=id,username,highscore&order=highscore.desc&limit=10";

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

        foreach (Transform child in leaderboardContent)
            Destroy(child.gameObject);

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
            int scoreValue = int.Parse(json.Substring(hsKey, hsEnd - hsKey));

            int idKey = json.IndexOf("\"id\":\"", userEnd) + 6;
            int idEnd = json.IndexOf("\"", idKey);
            string userId = json.Substring(idKey, idEnd - idKey);

            GameObject prefab = userId == SupabaseController.Instance.userId
                ? leaderboardCurrentPlaceItemPrefab
                : leaderboardItemPrefab;

            GameObject item = Instantiate(prefab, leaderboardContent);

            TMP_Text[] fields = item.GetComponentsInChildren<TMP_Text>();
            fields[0].text = rank.ToString();
            fields[1].text = username;
            fields[2].text = FormatScoreWithColor(scoreValue);

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

        fields[1].color = Color.yellow;
    }

    string FormatScoreWithColor(int score)
    {
        string scoreString = score.ToString().PadLeft(scoreDigits, '0');

        if (score == 0)
            return $"<color=#{ColorUtility.ToHtmlStringRGB(normalColor)}>{scoreString}</color>";

        int firstNonZero = scoreString.Length;
        for (int i = 0; i < scoreString.Length; i++)
        {
            if (scoreString[i] != '0')
            {
                firstNonZero = i;
                break;
            }
        }

        string zeros = scoreString[..firstNonZero];
        string active = scoreString[firstNonZero..];

        return $"<color=#{ColorUtility.ToHtmlStringRGB(normalColor)}>{zeros}</color>" +
               $"<color=#{ColorUtility.ToHtmlStringRGB(scoreColor)}>{active}</color>";
    }

    IEnumerator CheckNicknameAvailability(string nickname)
    {
        string url = $"{URL}/rest/v1/{TABLE}?select=id&username=eq.{nickname}";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("apikey", API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + API_KEY);

        yield return req.SendWebRequest();

        // ako je nesto doslo → vec postoji
        if (req.downloadHandler.text.Length > 5)
        {
            nicknameWarningText.SetActive(true);
            playButton.interactable = false;
            yield break;
        }

        StartCoroutine(UpdateNickname(nickname));
    }

    bool IsBadWord(string nickname)
    {
        if (badWordsFile == null) return false;

        string[] badWords = badWordsFile.text.Split('\n');

        foreach (string word in badWords)
        {
            if (nickname.ToLower().Contains(word.Trim().ToLower()))
                return true;
        }

        return false;
    }


}
