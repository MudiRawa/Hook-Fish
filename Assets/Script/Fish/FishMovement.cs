using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Movement Area")]
    private BoxCollider movementArea;

    public float edgeMargin = 1.5f;

    [Header("Speed")]
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.8f;
    public float speedChangeRate = 0.8f;

    [Header("Direction")]
    public float turnSpeed = 2.5f;
    public float directionChangeMin = 1.5f;
    public float directionChangeMax = 3.5f;

    // Ukuran aktual ikan setelah spawn
    public float currentSizeMultiplier { get; private set; }

    private Vector3 desiredDirection;
    private float currentSpeed;
    private float targetSpeed;
    private float directionTimer;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        currentSpeed =
            Random.Range(
                minSpeed,
                maxSpeed
            );

        targetSpeed =
            Random.Range(
                minSpeed,
                maxSpeed
            );

        desiredDirection =
            transform.forward.normalized;

        directionTimer =
            Random.Range(
                directionChangeMin,
                directionChangeMax
            );
    }

    private void Update()
    {
        if (movementArea == null)
            return;

        // Kalau ikan sudah tertangkap,
        // jangan bergerak lagi.
        if (
            transform.parent != null &&
            transform.parent.CompareTag("FishingHook")
        )
        {
            return;
        }

        UpdateDirectionTimer();
        CheckBoundary();
        MoveFish();
    }

    public void SetMovementArea(BoxCollider area)
    {
        movementArea = area;
    }

    public void SetFishData(FishData data)
    {
        if (data == null)
            return;

        // Random ukuran berdasarkan FishData
        currentSizeMultiplier =
            Random.Range(
                data.minSizeMultiplier,
                data.maxSizeMultiplier
            );

        transform.localScale =
            originalScale *
            currentSizeMultiplier;

        Debug.Log(
            data.fishName +
            " | Size: " +
            currentSizeMultiplier.ToString("F2") +
            "x"
        );
    }

    public float GetSizeMultiplier()
    {
        return currentSizeMultiplier;
    }

    private void UpdateDirectionTimer()
    {
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0f)
        {
            ChooseNewDirection();

            directionTimer =
                Random.Range(
                    directionChangeMin,
                    directionChangeMax
                );
        }
    }

    private void ChooseNewDirection()
    {
        Vector3 randomDirection =
            new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-0.4f, 0.4f),
                Random.Range(-1f, 1f)
            );

        if (randomDirection.sqrMagnitude < 0.01f)
        {
            randomDirection =
                transform.forward;
        }

        desiredDirection =
            randomDirection.normalized;

        targetSpeed =
            Random.Range(
                minSpeed,
                maxSpeed
            );
    }

    private void CheckBoundary()
    {
        Bounds bounds =
            movementArea.bounds;

        Vector3 position =
            transform.position;

        Vector3 correction =
            Vector3.zero;

        if (position.x < bounds.min.x + edgeMargin)
            correction.x += 1f;

        if (position.x > bounds.max.x - edgeMargin)
            correction.x -= 1f;

        if (position.y < bounds.min.y + edgeMargin)
            correction.y += 1f;

        if (position.y > bounds.max.y - edgeMargin)
            correction.y -= 1f;

        if (position.z < bounds.min.z + edgeMargin)
            correction.z += 1f;

        if (position.z > bounds.max.z - edgeMargin)
            correction.z -= 1f;

        if (correction != Vector3.zero)
        {
            desiredDirection =
                (
                    desiredDirection +
                    correction * 2f
                ).normalized;
        }
    }

    private void MoveFish()
    {
        currentSpeed =
            Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                speedChangeRate *
                Time.deltaTime
            );

        Quaternion targetRotation =
            Quaternion.LookRotation(
                desiredDirection
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed *
                Time.deltaTime
            );

        transform.position +=
            transform.forward *
            currentSpeed *
            Time.deltaTime;
    }
}