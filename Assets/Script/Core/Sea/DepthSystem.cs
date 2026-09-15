using UnityEngine;

public class DepthSystem : MonoBehaviour
{
    [Header("References")]
    public Transform seaHook;

    [Header("Depth Settings")]
    public float maxDepth = 100f;

    private Vector3 startPosition;

    public float CurrentDepth { get; private set; }

    private void Start()
    {
        if (seaHook == null)
        {
            Debug.LogError("SeaHook belum diisi pada DepthSystem.");
            return;
        }

        // Menyimpan posisi awal SeaHook
        startPosition = seaHook.position;
    }

    private void Update()
    {
        if (seaHook == null)
            return;

        CalculateDepth();
    }

    private void CalculateDepth()
    {
        // Hanya menghitung jarak pada bidang horizontal
        Vector3 currentPosition = seaHook.position;

        float distance = Vector3.Distance(
            startPosition,
            currentPosition
        );

        CurrentDepth = Mathf.Clamp(distance, 0f, maxDepth);

        // Debug.Log("Depth: " + CurrentDepth.ToString("F1") + " m");
    }
}