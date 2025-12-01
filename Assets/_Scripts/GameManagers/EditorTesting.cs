using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorTesting : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
