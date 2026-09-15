using UnityEngine;

public class HookThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    public Transform target;
    public float throwDuration = 2f;
    public float arcHeight = 5f;

    private Vector3 startPosition;
    private float timer;
    private bool isThrowing;

    public bool IsThrowing => isThrowing;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void ThrowHook()
    {
        startPosition = transform.position;
        timer = 0f;
        isThrowing = true;
    }

    private void Update()
    {
        if (!isThrowing)
            return;

        timer += Time.deltaTime;

        float progress = timer / throwDuration;
        progress = Mathf.Clamp01(progress);

        Vector3 position = Vector3.Lerp(
            startPosition,
            target.position,
            progress
        );

        float arc = 4f * arcHeight * progress * (1f - progress);

        position.y += arc;

        transform.position = position;

        if (progress >= 1f)
        {
            isThrowing = false;
        }
    }
}