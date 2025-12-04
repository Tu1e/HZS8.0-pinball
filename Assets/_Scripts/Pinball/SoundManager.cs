using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private bool isMuted = false;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ExtraUIController.Sound += ToggleSound;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ExtraUIController.Sound -= ToggleSound;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMuteState();
    }

    private void ToggleSound(bool sound)
    {
        isMuted = !sound; 
        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>(true);

        foreach (AudioSource src in sources)
            src.mute = isMuted; 
    }

    public bool IsMuted() => isMuted;
}
