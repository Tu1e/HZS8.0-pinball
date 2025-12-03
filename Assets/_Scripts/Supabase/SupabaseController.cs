using UnityEngine;

public class SupabaseController : MonoBehaviour
{
    public static SupabaseController Instance { get; private set; }

    public string userId;
    public string username;
    public long highscore;

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
