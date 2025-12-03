using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class LeaderboardEntry
{
    public string user_id;
    public string username;
    public long highscore; // int8 = long
}

[Serializable]
public class LeaderboardResponse
{
    public LeaderboardEntry[] data;
}

public class SupabaseLeaderboard : MonoBehaviour
{
    private const string URL = "https://gcdfqzveobylyonwydxr.supabase.co";
    private const string TABLE = "Leaderboard";
    private const string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdjZGZxenZlb2J5bHlvbnd5ZHhyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MTc2NTgsImV4cCI6MjA4MDI5MzY1OH0.Qq8F6bScU_qy241N1UXcml1THbtxvuRSQ8vuhE00dPg"; // <-- ovde ubaci novi ključ

    // Generates UUID on client if you don't have login system
    public string GenerateUserId() => Guid.NewGuid().ToString();

    // Insert new user with initial score
    public IEnumerator AddUser(string userId, string username, long score)
    {
        string json = JsonUtility.ToJson(new LeaderboardEntry
        {
            user_id = userId,
            username = username,
            highscore = score
        });

        UnityWebRequest req = new UnityWebRequest($"{URL}/rest/v1/{TABLE}", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();

        SetHeaders(req);
        req.SetRequestHeader("Prefer", "return=minimal");

        yield return req.SendWebRequest();
        Debug.Log($"AddUser: {req.result}");
    }

    // Update user score
    public IEnumerator UpdateScore(string userId, long score)
    {
        string url = $"{URL}/rest/v1/{TABLE}?user_id=eq.{userId}";
        string json = "{\"highscore\": " + score + "}";

        UnityWebRequest req = UnityWebRequest.Put(url, json);
        req.method = "PATCH";

        SetHeaders(req);
        req.SetRequestHeader("Prefer", "return=minimal");

        yield return req.SendWebRequest();
        Debug.Log($"UpdateScore: {req.result}");
    }

    // Fetch Top 10 scores
    public IEnumerator GetTop10(Action<LeaderboardEntry[]> callback)
    {
        string url = $"{URL}/rest/v1/{TABLE}?select=*&order=highscore.desc&limit=10";

        UnityWebRequest req = UnityWebRequest.Get(url);
        SetHeaders(req);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string wrapped = "{\"data\":" + req.downloadHandler.text + "}";
            LeaderboardResponse res = JsonUtility.FromJson<LeaderboardResponse>(wrapped);
            callback(res.data);
        }
        else
        {
            Debug.LogError("GetTop10 error: " + req.error);
            callback(null);
        }
    }

    private void SetHeaders(UnityWebRequest req)
    {
        req.SetRequestHeader("apikey", API_KEY);
        req.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        req.SetRequestHeader("Content-Type", "application/json");
    }
}
