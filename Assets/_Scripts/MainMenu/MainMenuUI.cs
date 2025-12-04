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

    private const string URL = "https://gcdfqzveobylyonwydxr.supabase.co";
    private const string TABLE = "users";
    private const string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdjZGZxenZlb2J5bHlvbnd5ZHhyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MTc2NTgsImV4cCI6MjA4MDI5MzY1OH0.Qq8F6bScU_qy241N1UXcml1THbtxvuRSQ8vuhE00dPg";

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
        SupabaseController.Instance.highscore = long.Parse(highscore);

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
    }
}
