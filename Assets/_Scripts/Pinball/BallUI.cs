using UnityEngine;

public class BallUI : MonoBehaviour
{
    [SerializeField] GameObject[] balls;
    int ballCounter = 0;
    private void OnEnable()
    {
        GameManager.OnBallUsed += RemoveBall;
    }

    private void OnDisable()
    {
        GameManager.OnBallUsed -= RemoveBall;
    }

    private void RemoveBall()
    {
        balls[ballCounter].SetActive(false);
        ballCounter++;
    }
}
