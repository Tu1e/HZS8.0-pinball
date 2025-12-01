using UnityEngine;

public static class AudioHelper
{
    private static float globalVolume = 1f; // Globalna ja?ina svih zvukova

    /// <summary>
    /// Pušta 2D audio clip sa kontrolisanom ja?inom
    /// </summary>
    public static void Play2DSound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        
        audioSource.clip = clip;
        audioSource.volume = volume * globalVolume;
        audioSource.spatialBlend = 0f; // 2D zvuk
        audioSource.Play();
        
        Object.Destroy(tempAudio, clip.length);
    }

    /// <summary>
    /// Postavlja globalnu ja?inu za sve zvukove
    /// </summary>
    public static void SetGlobalVolume(float volume)
    {
        globalVolume = Mathf.Clamp01(volume);
    }
}
