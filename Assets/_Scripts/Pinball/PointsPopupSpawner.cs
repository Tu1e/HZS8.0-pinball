using UnityEngine;

public class PointsPopupSpawner : MonoBehaviour
{
    [SerializeField] Transform pfPointsPopup;
    void Start()
    {
        Transform points = Instantiate(pfPointsPopup, Vector3.zero, Quaternion.identity, transform);
        PointsPopup pointsPopup = points.GetComponent<PointsPopup>();
        pointsPopup.Setup(10);
    }

}
