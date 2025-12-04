using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraUIController : MonoBehaviour
{
    bool sound = true;
    [SerializeField] Sprite soundOn, soundOff;
    [SerializeField] Image btnImage;
    public static event Action<bool> Sound;

    public void SoundButton()
    {
        sound = !sound;

        btnImage.sprite = sound ? soundOn : soundOff;

        Sound?.Invoke(sound);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

}
