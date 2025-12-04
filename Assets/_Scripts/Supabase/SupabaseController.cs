using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SupabaseController : MonoBehaviour
{
    public static SupabaseController Instance { get; private set; }
    public const string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdjZGZxenZlb2J5bHlvbnd5ZHhyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MTc2NTgsImV4cCI6MjA4MDI5MzY1OH0.Qq8F6bScU_qy241N1UXcml1THbtxvuRSQ8vuhE00dPg";
    public string userId;
    public string username;
    public int highscore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
