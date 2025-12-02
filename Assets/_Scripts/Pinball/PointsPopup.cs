using UnityEngine;
using TMPro;
public class PointsPopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int score)
    {
        textMesh.SetText(score.ToString());
    }

    /*public static PointsPopup Create()
    {
        Transform points = Instantiate(pfPointsPopup, Vector3.zero, Quaternion.identity, transform);
        PointsPopup pointsPopup = points.GetComponent<PointsPopup>();
        pointsPopup.Setup(10);
    }*/
}
