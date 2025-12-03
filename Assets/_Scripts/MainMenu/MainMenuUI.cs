using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;          // user_id
    [SerializeField] TMP_InputField inputFieldNickname;  // nickname
    [SerializeField] Button playButton, nextButton;
    [SerializeField] GameObject leaderboardPanel, mainPanel;

    private const string URL = "https://gcdfqzveobylyonwydxr.supabase.co";
    private const string TABLE = "Leaderboard";
    private const string API_KEY = "YOUR_NEW_ANON_KEY"; // <- ubaci regenerisani

    private void Start()
    {
        playButton.interactable = false;
        nextButton.interactable = false;
        inputFieldNickname.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        string id = Strip(inputField.text);

        if (id.Length < 4 || id.StartsWith(" "))
        {
            playButton.interactable = false;
            nextButton.interactable = false;
            return;
        }

        StartCoroutine(CheckUserId(id));
    }

    private string Strip(string s) => s.Trim();

    IEnumerator CheckUserId(string userId)
    {
        string url = $"{URL}/rest/v1/{TABLE}?select=username&user_id=eq.{userId}";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("apikey", API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + API_KEY);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            if (req.downloadHandler.text.Length < 5)
            {
                // user ne postoji
                playButton.interactable = false;
                nextButton.interactable = false;
                inputFieldNickname.gameObject.SetActive(false);
            }
            else
            {
                // parse username
                string json = req.downloadHandler.text;
                string username = ExtractUsername(json);

                SupabaseController.Instance.userId = userId;
                SupabaseController.Instance.username = username;

                if (username == "Unknown")
                {
                    // treba upisati username
                    inputFieldNickname.gameObject.SetActive(true);
                    nextButton.interactable = true;
                    playButton.interactable = false;
                }
                else
                {
                    // ima username - start allowed
                    inputFieldNickname.gameObject.SetActive(false);
                    nextButton.interactable = false;
                    playButton.interactable = true;
                }
            }
        }
        else
        {
            Debug.LogError("Supabase error: " + req.error);
            playButton.interactable = false;
        }
    }

    private string ExtractUsername(string json)
    {
        // o?ekivani format Supabase REST responsa:
        // [{"username":"Player","user_id":"..."}]
        int start = json.IndexOf("username\":\"") + 11;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }

    public void Next()
    {
        // sa?uvaj nickname u kontroleru
        SupabaseController.Instance.username = Strip(inputFieldNickname.text);
        StartCoroutine(UpdateNickname(SupabaseController.Instance.userId, SupabaseController.Instance.username));
    }

    IEnumerator UpdateNickname(string userId, string nickname)
    {
        string url = $"{URL}/rest/v1/{TABLE}?user_id=eq.{userId}";
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
            playButton.interactable = true;
            nextButton.interactable = false;
            inputFieldNickname.gameObject.SetActive(false);
        }
        else Debug.LogError(req.error);
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Leaderboard()
    {
        leaderboardPanel.SetActive(true);
        mainPanel.SetActive(false);
    }
}
