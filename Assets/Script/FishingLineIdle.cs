using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLineIdle : MonoBehaviour
{
    [Header("References")]
    public Transform lineStart;
    public Transform hook;

    [Header("Line Settings")]
    public int pointCount = 12;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer =
            GetComponent<LineRenderer>();

        lineRenderer.positionCount =
            pointCount;
    }

    private void Update()
    {
        if (lineStart == null || hook == null)
            return;

        UpdateLine();
    }

    private void UpdateLine()
    {
        Vector3 startPosition =
            lineStart.position;

        Vector3 endPosition =
            hook.position;

        for (int i = 0; i < pointCount; i++)
        {
            float t =
                i / (float)(pointCount - 1);

            Vector3 position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            lineRenderer.SetPosition(
                i,
                position
            );
        }
    }
}