using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorTesting : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}

