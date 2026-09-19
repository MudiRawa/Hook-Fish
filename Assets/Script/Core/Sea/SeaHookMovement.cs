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

        // Kecepatan maju/mundur
        float forwardSpeed =
            idleForwardSpeed + (moveInput.y * moveSpeed);

        // Gerakan kiri/kanan
        float horizontalSpeed = moveInput.x * moveSpeed;

        Vector3 movement = new Vector3(
            horizontalSpeed,
            -forwardSpeed * verticalMultiplier,
            forwardSpeed
        );

        transform.position += movement * Time.deltaTime;
    }
}