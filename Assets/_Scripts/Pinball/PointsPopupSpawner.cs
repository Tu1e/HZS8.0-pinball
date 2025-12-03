using UnityEngine;

public class PointsPopupSpawner : MonoBehaviour
{
    [SerializeField] Transform ball;

    void Start()
    {
    }

    private void OnEnable()
    {
        ScoreManager.ScoreChanged += ShowPPopup;
        Ball.OnBallInitialized += SetBallTransofrm;
    }

    private void OnDisable()
    {
        ScoreManager.ScoreChanged -= ShowPPopup;
        Ball.OnBallInitialized -= SetBallTransofrm;
    }

    private void SetBallTransofrm(Transform t) => ball = t;
    private void ShowPPopup(int value)
    {
        PointsPopup.Create(ball.position, value);

    }

}
