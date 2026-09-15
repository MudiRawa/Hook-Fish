using UnityEngine;
using UnityEngine.InputSystem;

public class SeaHookMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float idleForwardSpeed = 0.5f;

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

        Vector3 movement = Vector3.forward * idleForwardSpeed;

        movement += new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        ) * moveSpeed;

        transform.position += movement * Time.deltaTime;
    }
}