using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_InputField inputFieldNickname;
    [SerializeField] Button playButton;
    [SerializeField] GameObject leaderboardPanel, mainPanel;

    private void FixedUpdate()
    {
        if(inputField == null || inputField.text.Length == 0 || inputField.text.StartsWith(" "))
        {
            playButton.interactable = false;
        }
        else
        {
            playButton.interactable = true;
        }
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
