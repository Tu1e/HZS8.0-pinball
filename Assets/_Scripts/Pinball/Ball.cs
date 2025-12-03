using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static event Action<Transform> OnBallInitialized;

    private void Start()
    {
        OnBallInitialized?.Invoke(transform);
    }
}
