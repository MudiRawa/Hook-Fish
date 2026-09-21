using UnityEngine;
using UnityEngine.InputSystem;

public class SeaHookMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float idleForwardSpeed = 0.5f;
    public float verticalMultiplier = 1f;

    [Header("Fishing")]
    public FishingState fishingState;

    [Header("Depth Limit")]
    public DepthSystem depthSystem;

    private Vector2 moveInput;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        // Tidak bisa bergerak saat sedang memancing
        if (fishingState != null && fishingState.IsFishing)
            return;

        // Kecepatan maju / mundur
        float forwardSpeed =
            idleForwardSpeed +
            (moveInput.y * moveSpeed);

        // Gerakan kiri / kanan
        float horizontalSpeed =
            moveInput.x * moveSpeed;

        Vector3 movement = new Vector3(
            horizontalSpeed,
            -forwardSpeed * verticalMultiplier,
            forwardSpeed
        );

        Vector3 nextPosition =
            transform.position +
            movement * Time.deltaTime;

        // Batasi agar tidak melewati max depth
        if (depthSystem != null)
        {
            Vector3 startPosition =
                depthSystem.StartPosition;

            float nextDepth =
                Vector3.Distance(
                    startPosition,
                    nextPosition
                );

            if (nextDepth > depthSystem.maxDepth)
            {
                // Arah dari posisi awal menuju posisi berikutnya
                Vector3 direction =
                    nextPosition - startPosition;

                // Tempatkan tepat di batas maksimum
                nextPosition =
                    startPosition +
                    direction.normalized *
                    depthSystem.maxDepth;
            }
        }

        transform.position =
            nextPosition;
    }
}