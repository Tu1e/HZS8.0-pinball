using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
public class PointsPopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    [SerializeField] Transform pfPointsPopup;
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int score)
    {
        textMesh.SetText(score.ToString());
        Destroy(gameObject, 1);
    }

    public static PointsPopup Create(Vector2 position, int value)
    {
        Transform points = Instantiate(GameAssets.Instance.pfPointsPopup, position, Quaternion.identity);
        float randZ = Random.Range(-10.0f, 10.0f);
        points.Rotate(new Vector3(0,0,randZ));
        PointsPopup pointsPopup = points.GetComponent<PointsPopup>();
        pointsPopup.Setup(value);
        return pointsPopup;
    }
}
