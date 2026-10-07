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

    [Header("Seabed")]
    public LayerMask seabedLayer;
    public float seabedClearance = 0.5f;
    public float seabedCheckHeight = 10f;
    public float seabedCheckDistance = 30f;

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
            bool isShiftPressed =
                Keyboard.current.leftShiftKey.isPressed ||
                Keyboard.current.rightShiftKey.isPressed;

            bool isSpacePressed =
                Keyboard.current.spaceKey.isPressed;

            // SHIFT = turun
            if (isShiftPressed)
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
            // BATAS KETINGGIAN AWAL
            // =================================================

            // Tidak boleh naik melewati titik awal
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

                if (direction != Vector3.zero)
                {
                    nextPosition =
                        startPosition +
                        direction.normalized *
                        depthSystem.maxDepth;
                }
            }
        }

        // =====================================================
        // SEABED CHECK
        // =====================================================

        nextPosition =
            ClampAboveSeabed(nextPosition);

        // Terapkan posisi akhir
        transform.position =
            nextPosition;
    }

    private Vector3 ClampAboveSeabed(
        Vector3 position
    )
    {
        Vector3 rayOrigin =
            position +
            Vector3.up *
            seabedCheckHeight;

        if (
            Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                seabedCheckDistance,
                seabedLayer
            )
        )
        {
            float minimumY =
                hit.point.y +
                seabedClearance;

            if (position.y < minimumY)
            {
                position.y =
                    minimumY;
            }
        }

        return position;
    }
}