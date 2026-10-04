using UnityEngine;
using UnityEngine.InputSystem;

public class SeaHookMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Vertical Movement")]
    public float downSpeed = 2f;
    public float upSpeed = 2f;
    public float idleDownSpeed = 0.3f;

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

        float horizontalSpeed =
            moveInput.x * moveSpeed;

        float forwardSpeed =
            moveInput.y * moveSpeed;

        float verticalSpeed = 0f;

        // =====================================================
        // VERTICAL MOVEMENT
        // =====================================================

        if (Keyboard.current != null)
        {
            bool isCtrlPressed =
                Keyboard.current.leftCtrlKey.isPressed ||
                Keyboard.current.rightCtrlKey.isPressed;

            bool isSpacePressed =
                Keyboard.current.spaceKey.isPressed;

            // CTRL = turun
            if (isCtrlPressed)
            {
                verticalSpeed =
                    -downSpeed;
            }
            // SPACE = naik
            else if (isSpacePressed)
            {
                verticalSpeed =
                    upSpeed;
            }
            // Tidak ada input gerakan = turun pelan
            else if (moveInput == Vector2.zero)
            {
                verticalSpeed =
                    -idleDownSpeed;
            }
        }

        Vector3 movement = new Vector3(
            horizontalSpeed,
            verticalSpeed,
            forwardSpeed
        );

        Vector3 nextPosition =
            transform.position +
            movement * Time.deltaTime;

        if (depthSystem != null)
        {
            Vector3 startPosition =
                depthSystem.StartPosition;

            // =================================================
            // BATAS BELAKANG
            // =================================================

            // Tidak boleh melewati titik awal ke belakang
            if (nextPosition.z < startPosition.z)
            {
                nextPosition.z =
                    startPosition.z;
            }

            // =================================================
            // BATAS KETINGGIAN
            // =================================================

            // Tidak boleh naik melewati ketinggian awal
            if (nextPosition.y > startPosition.y)
            {
                nextPosition.y =
                    startPosition.y;
            }

            // =================================================
            // MAX DEPTH
            // =================================================

            float nextDepth =
                Vector3.Distance(
                    startPosition,
                    nextPosition
                );

            if (nextDepth > depthSystem.maxDepth)
            {
                Vector3 direction =
                    nextPosition - startPosition;

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