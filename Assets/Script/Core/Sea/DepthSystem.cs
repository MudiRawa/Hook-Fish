using UnityEngine;

public class DepthSystem : MonoBehaviour
{
    [Header("References")]
    public Transform seaHook;

    [Header("Depth Settings")]
    public float maxDepth;

    private Vector3 startPosition;

    private bool overrideDepth;

    public float CurrentDepth { get; private set; }
    public Vector3 StartPosition => startPosition;

    private void Start()
    {
        if (seaHook == null)
        {
            Debug.LogError("SeaHook belum diisi pada DepthSystem.");
            return;
        }

        startPosition = seaHook.position;
    }

    private void Update()
    {
        if (seaHook == null)
            return;

        if (overrideDepth)
            return;

        CalculateDepth();
    }

    private void CalculateDepth()
    {
        float distance = Vector3.Distance(startPosition, seaHook.position);

        CurrentDepth = Mathf.Clamp(distance, 0f, maxDepth
        );
    }

    public void SetDepthOverride(float depth)
    {
        overrideDepth = true;

        CurrentDepth = Mathf.Clamp(depth, 0f, maxDepth);
    }

    public void ResumeFromCurrentPosition()
    {
        startPosition = seaHook.position;
        CurrentDepth = 0f;
        overrideDepth = false;
    }
}