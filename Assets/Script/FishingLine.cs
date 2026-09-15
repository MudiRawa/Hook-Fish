using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    [Header("References")]
    public Transform lineStart;
    public Transform hook;

    [Header("Line Settings")]
    public int pointCount = 12;
    public float sagAmount = 0.3f;
    public float movementEffect = 0.5f;

    private LineRenderer lineRenderer;
    private Vector3 previousHookPosition;
    private Vector3 hookVelocity;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCount;

        if (hook != null)
            previousHookPosition = hook.position;
    }

    private void Update()
    {
        if (lineStart == null || hook == null)
            return;

        CalculateHookVelocity();
        UpdateLine();
    }

    private void CalculateHookVelocity()
    {
        Vector3 currentVelocity =
            (hook.position - previousHookPosition) / Time.deltaTime;

        hookVelocity = Vector3.Lerp(
            hookVelocity,
            currentVelocity,
            Time.deltaTime * 8f
        );

        previousHookPosition = hook.position;
    }

    private void UpdateLine()
    {
        Vector3 startPosition = lineStart.position;
        Vector3 endPosition = hook.position;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);

            Vector3 position = Vector3.Lerp(
                startPosition,
                endPosition,
                t
            );

            // Lengkungan dasar senar
            float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;
            position.y -= sag;

            // Pengaruh gerakan kail
            float movementCurve = Mathf.Sin(t * Mathf.PI) * movementEffect;

            Vector3 movementOffset = -hookVelocity * movementCurve * 0.01f;

            position += movementOffset;

            lineRenderer.SetPosition(i, position);
        }
    }
}