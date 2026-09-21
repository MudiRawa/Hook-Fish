using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FishingMinigame : MonoBehaviour
{
    [Header("References")]
    public FishingState fishingState;
    public DepthSystem depthSystem;
    public Transform seaHook;

    [Header("UI")]
    public GameObject fishingPanel;
    public RectTransform fishingBar;
    public RectTransform catchZone;
    public RectTransform fishMarker;
    public TMPro.TMP_Text fishingStatusText;

    [Header("Start")]
    public float startDelay = 1.5f;

    [Header("Catch Zone")]
    public float catchZoneMaxSpeed = 500f;
    public float catchZoneAcceleration = 700f;
    public float catchZoneDeceleration = 1000f;

    [Header("Result")]
    public float successMessageTime = 1f;

    [Header("Fight Depth")]
    public float pullDepthSpeed = 1f;
    public float escapeDepthSpeed = 1f;

    // =========================================================
    // INTERNAL VARIABLES
    // =========================================================

    private float catchZoneX;
    private float catchZoneVelocity;
    private float fishMarkerX;
    private float fishStateTimer;
    private float startingDepth;
    private float fightDepth;

    private Vector3 fightStartPosition;
    private Vector3 originalStartPosition;

    private bool minigameStarted;
    private bool fightStarted;
    private bool finishing;
    private bool maxDepthReached;

    private enum FishMotionState
    {
        Slow,
        Bursting,
        Slowing
    }

    private FishMotionState fishMotionState;

    private float fishVelocity;
    private float fishVelocitySmooth;
    private float fishDirection;

    private void Start()
    {
        fishingPanel.SetActive(false);
        maxDepthReached = false;
    }

    private void Update()
    {
        if (fishingState == null)
            return;

        if (fishingState.IsFishing && !minigameStarted)
        {
            StartCoroutine(StartMinigame());
        }
    }

    // =========================================================
    // START MINIGAME
    // =========================================================

    private IEnumerator StartMinigame()
    {
        minigameStarted = true;
        fightStarted = false;

        fishingPanel.SetActive(true);

        startingDepth = depthSystem.CurrentDepth;
        fightDepth = startingDepth;

        fightStartPosition = seaHook.position;
        originalStartPosition = depthSystem.StartPosition;

        catchZoneX = 0f;
        catchZoneVelocity = 0f;

        fishMarkerX = 0f;

        fishVelocity = 0f;
        fishVelocitySmooth = 0f;

        catchZone.anchoredPosition = new Vector2(
            catchZoneX,
            catchZone.anchoredPosition.y
        );

        fishMarker.anchoredPosition = new Vector2(
            fishMarkerX,
            fishMarker.anchoredPosition.y
        );

        fishingStatusText.text = "GET READY...";

        // Ikan langsung menyatu dengan kail
        AttachFishToHook();

        yield return new WaitForSeconds(startDelay);

        StartSlowPhase();

        fightStarted = true;

        fishingStatusText.text =
            "KEEP THE FISH IN THE ZONE!";

        Debug.Log(
            "Fishing dimulai! Depth awal: " +
            startingDepth.ToString("F1") +
            " m"
        );
    }

    // =========================================================
    // MAIN FIGHT LOOP
    // =========================================================

    private void LateUpdate()
    {
        if (!fishingState.IsFishing)
            return;

        if (!minigameStarted)
            return;

        if (!fightStarted)
            return;

        if (finishing)
            return;

        MoveCatchZone();
        MoveFishMarker();
        UpdateFightDepth();
    }

    // =========================================================
    // ATTACH FISH
    // =========================================================

    private void AttachFishToHook()
    {
        Transform fishObject =
            fishingState.CurrentFishObject;

        if (fishObject == null)
        {
            Debug.LogWarning(
                "CurrentFishObject tidak ditemukan."
            );

            return;
        }

        fishObject.SetParent(seaHook);

        fishObject.localPosition =
            Vector3.zero;

        fishObject.localRotation =
            Quaternion.identity;

        Collider fishCollider =
            fishObject.GetComponent<Collider>();

        if (fishCollider != null)
            fishCollider.enabled = false;
    }

    // =========================================================
    // CATCH ZONE
    // =========================================================

    private void MoveCatchZone()
    {
        if (Mouse.current == null)
            return;

        bool isHolding =
            Mouse.current.leftButton.isPressed;

        float inputDirection =
            isHolding ? 1f : -1f;

        // =====================================================
        // SAAT KLIK DITAHAN
        // =====================================================

        if (isHolding)
        {
            // Kalau sedang bergerak ke kiri,
            // jangan langsung balik ke kanan.
            // Perlambat dulu sampai 0.
            if (catchZoneVelocity < 0f)
            {
                catchZoneVelocity =
                    Mathf.MoveTowards(
                        catchZoneVelocity,
                        0f,
                        catchZoneDeceleration *
                        Time.deltaTime
                    );
            }
            else
            {
                // Setelah tidak punya momentum kiri,
                // mulai akselerasi ke kanan.
                catchZoneVelocity =
                    Mathf.MoveTowards(
                        catchZoneVelocity,
                        catchZoneMaxSpeed,
                        catchZoneAcceleration *
                        Time.deltaTime
                    );
            }
        }

        // =====================================================
        // SAAT KLIK DILEPAS
        // =====================================================

        else
        {
            // Kalau masih punya momentum ke kanan,
            // perlambat dulu sampai 0.
            if (catchZoneVelocity > 0f)
            {
                catchZoneVelocity =
                    Mathf.MoveTowards(
                        catchZoneVelocity,
                        0f,
                        catchZoneDeceleration *
                        Time.deltaTime
                    );
            }
            else
            {
                // Setelah berhenti, mulai akselerasi ke kiri.
                catchZoneVelocity =
                    Mathf.MoveTowards(
                        catchZoneVelocity,
                        -catchZoneMaxSpeed,
                        catchZoneAcceleration *
                        Time.deltaTime
                    );
            }
        }

        // Gerakkan CatchZone
        catchZoneX +=
            catchZoneVelocity *
            Time.deltaTime;

        // =====================================================
        // BATAS BAR
        // =====================================================

        float halfBarWidth =
            fishingBar.rect.width / 2f;

        float halfZoneWidth =
            catchZone.rect.width / 2f;

        float minX =
            -halfBarWidth +
            halfZoneWidth;

        float maxX =
            halfBarWidth -
            halfZoneWidth;

        catchZoneX =
            Mathf.Clamp(
                catchZoneX,
                minX,
                maxX
            );

        // Kalau sudah mentok kiri
        if (
            catchZoneX <= minX &&
            catchZoneVelocity < 0f
        )
        {
            catchZoneX = minX;
            catchZoneVelocity = 0f;
        }

        // Kalau sudah mentok kanan
        if (
            catchZoneX >= maxX &&
            catchZoneVelocity > 0f
        )
        {
            catchZoneX = maxX;
            catchZoneVelocity = 0f;
        }

        catchZone.anchoredPosition =
            new Vector2(
                catchZoneX,
                catchZone.anchoredPosition.y
            );
    }

    // =========================================================
    // FISH MARKER
    // =========================================================
    private float GetStrength01()
    {
        FishData fish =
            fishingState.CurrentFish;

        if (fish == null)
            return 0f;

        float normalizedStrength =
            Mathf.InverseLerp(
                1f,
                100f,
                fish.fightStrength
            );

        // Membuat strength kecil tetap cukup menantang.
        // 10 akan terasa lebih dekat ke tingkat kesulitan menengah.
        return Mathf.Pow(
            normalizedStrength,
            0.6f
        );
    }

    private void MoveFishMarker()
    {
        FishData fish =
            fishingState.CurrentFish;

        if (fish == null)
            return;

        float strength01 = GetStrength01();

        // Semakin tinggi strength,
        // semakin tinggi speed maksimal.
        //
        // Strength 1   = sekitar 120
        // Strength 50  = sekitar 380
        // Strength 100 = sekitar 650
        float strengthSpeedMultiplier =
            Mathf.Lerp(
                1f,
                1.8f,
                Mathf.Pow(
                    strength01,
                    0.8f
                )
            );

        float baseSpeed =
            120f *
            strengthSpeedMultiplier;

        // =====================================================
        // SLOW
        // =====================================================

        if (
            fishMotionState ==
            FishMotionState.Slow
        )
        {
            fishStateTimer -=
                Time.deltaTime;

            float slowMultiplier =
                Mathf.Lerp(
                    0.3f,
                    0.5f,
                    strength01
                );

            float targetSpeed =
                fishDirection *
                baseSpeed *
                slowMultiplier;

            float accelerationTime =
                Mathf.Lerp(
                    0.7f,
                    0.3f,
                    strength01
                );

            fishVelocity =
                Mathf.SmoothDamp(
                    fishVelocity,
                    targetSpeed,
                    ref fishVelocitySmooth,
                    accelerationTime
                );

            fishMarkerX +=
                fishVelocity *
                Time.deltaTime;

            ClampFishMarker();
            HandleFishEdge();

            if (fishStateTimer <= 0f)
            {
                StartBurstPhase();
            }

            return;
        }

        // =====================================================
        // BURST
        // =====================================================

        if (
            fishMotionState ==
            FishMotionState.Bursting
        )
        {
            fishStateTimer -=
                Time.deltaTime;

            float burstMultiplier =
                Mathf.Lerp(
                    1.1f,
                    3f,
                    strength01
                );

            float burstSpeed =
                baseSpeed *
                burstMultiplier;

            float accelerationTime =
                Mathf.Lerp(
                    0.55f,
                    0.12f,
                    strength01
                );

            float targetSpeed =
                fishDirection *
                burstSpeed;

            fishVelocity =
                Mathf.SmoothDamp(
                    fishVelocity,
                    targetSpeed,
                    ref fishVelocitySmooth,
                    accelerationTime
                );

            fishMarkerX +=
                fishVelocity *
                Time.deltaTime;

            ClampFishMarker();
            HandleFishEdge();

            if (fishStateTimer <= 0f)
            {
                fishMotionState =
                    FishMotionState.Slowing;
            }

            return;
        }

        // =====================================================
        // SLOWING
        // =====================================================

        if (
            fishMotionState ==
            FishMotionState.Slowing
        )
        {
            float slowMultiplier =
                Mathf.Lerp(
                    0.3f,
                    0.5f,
                    strength01
                );

            float slowSpeed =
                baseSpeed *
                slowMultiplier;

            float decelerationTime =
                Mathf.Lerp(
                    0.5f,
                    0.12f,
                    strength01
                );

            float targetSpeed =
                fishDirection *
                slowSpeed;

            fishVelocity =
                Mathf.SmoothDamp(
                    fishVelocity,
                    targetSpeed,
                    ref fishVelocitySmooth,
                    decelerationTime
                );

            fishMarkerX +=
                fishVelocity *
                Time.deltaTime;

            ClampFishMarker();
            HandleFishEdge();

            if (
                Mathf.Abs(fishVelocity) <=
                slowSpeed + 2f
            )
            {
                fishMotionState =
                    FishMotionState.Slow;

                fishStateTimer =
                    GetSlowDuration();

                // Jangan berhenti.
                fishVelocity =
                    fishDirection *
                    slowSpeed;
            }
        }
    }

    // =========================================================
    // START SLOW
    // =========================================================

    private void StartSlowPhase()
    {
        fishDirection =
            Random.value > 0.5f
                ? 1f
                : -1f;

        fishMotionState =
            FishMotionState.Slow;

        fishStateTimer =
            GetSlowDuration();
    }

    // =========================================================
    // START BURST
    // =========================================================

    private void StartBurstPhase()
    {
        // Burst selalu punya kemungkinan
        // berganti arah.
        fishDirection = Random.value > 0.5f ? 1f : -1f;
        fishMotionState = FishMotionState.Bursting;
        fishStateTimer = GetBurstDuration();
    }

    // =========================================================
    // SLOW DURATION
    // =========================================================

    private float GetSlowDuration()
    {
        FishData fish =
            fishingState.CurrentFish;

        if (fish == null)
            return 1f;

        float strength01 = GetStrength01();

        // Strength tinggi = fase slow lebih pendek.
        float duration =
            Mathf.Lerp(
                1.3f,
                0.25f,
                strength01
            );

        return duration *
               Random.Range(
                   0.75f,
                   1.25f
               );
    }

    // =========================================================
    // BURST DURATION
    // =========================================================

    private float GetBurstDuration()
    {
        FishData fish =
            fishingState.CurrentFish;

        if (fish == null)
            return 0.7f;

        float strength01 = GetStrength01();

        // Strength tinggi = burst lebih sering dan
        // dapat bertahan sedikit lebih lama.
        float duration =
            Mathf.Lerp(
                0.45f,
                0.9f,
                strength01
            );

        return duration *
               Random.Range(
                   0.75f,
                   1.25f
               );
    }

    // =========================================================
    // CLAMP MARKER
    // =========================================================

    private void ClampFishMarker()
    {
        float halfBarWidth =
            fishingBar.rect.width / 2f;

        float halfMarkerWidth =
            fishMarker.rect.width / 2f;

        float minX =
            -halfBarWidth +
            halfMarkerWidth;

        float maxX =
            halfBarWidth -
            halfMarkerWidth;

        fishMarkerX =
            Mathf.Clamp(
                fishMarkerX,
                minX,
                maxX
            );

        fishMarker.anchoredPosition =
            new Vector2(
                fishMarkerX,
                fishMarker.anchoredPosition.y
            );
    }

    // =========================================================
    // HANDLE EDGE
    // =========================================================

    private void HandleFishEdge()
    {
        float halfBarWidth =
            fishingBar.rect.width / 2f;

        float halfMarkerWidth =
            fishMarker.rect.width / 2f;

        float minX =
            -halfBarWidth +
            halfMarkerWidth;

        float maxX =
            halfBarWidth -
            halfMarkerWidth;

        // Sedikit menjauh dari sisi kiri
        if (
            fishMarkerX <= minX + 30f &&
            fishDirection < 0f
        )
        {
            fishDirection = 1f;
        }

        // Sedikit menjauh dari sisi kanan
        if (
            fishMarkerX >= maxX - 30f &&
            fishDirection > 0f
        )
        {
            fishDirection = -1f;
        }
    }

    // =========================================================
    // DEPTH FIGHT
    // =========================================================

    private void UpdateFightDepth()
    {
        bool fishInsideZone =
            IsFishInsideCatchZone();

        if (fishInsideZone)
        {
            // Menarik kail ke atas dengan kecepatan 1
            fightDepth -=
                pullDepthSpeed *
                Time.deltaTime;
        }
        else
        {
            // Ikan menarik kail ke bawah dengan kecepatan 1
            fightDepth +=
                escapeDepthSpeed *
                Time.deltaTime;
        }

        fightDepth =
            Mathf.Clamp(
                fightDepth,
                0f,
                depthSystem.maxDepth
            );

        // Cek apakah sudah mencapai batas maksimal
        if (fightDepth >= depthSystem.maxDepth && !maxDepthReached)
        {
            maxDepthReached = true;
            Debug.Log("Kalah mancing");
            SceneManager.LoadScene("Dock");
        }

        // Kalau sudah tidak berada di max depth,
        // izinkan log muncul lagi kalau nanti mencapai batas lagi.
        if (
            fightDepth < depthSystem.maxDepth
        )
        {
            maxDepthReached = false;
        }

        depthSystem.SetDepthOverride(
            fightDepth
        );

        MoveHookAccordingToDepth();

        if (fightDepth <= 0f)
        {
            StartCoroutine(
                FinishFishing()
            );
        }
    }

    // =========================================================
    // CHECK CATCH ZONE
    // =========================================================

    private bool IsFishInsideCatchZone()
    {
        float distance =
            Mathf.Abs(
                fishMarkerX -
                catchZoneX
            );

        float allowedDistance =
            (catchZone.rect.width / 2f) -
            (fishMarker.rect.width / 2f);

        return distance <=
               allowedDistance;
    }

    // =========================================================
    // HOOK MOVEMENT DURING FIGHT
    // =========================================================

    private void MoveHookAccordingToDepth()
    {
        if (startingDepth <= 0f)
            return;

        float progress = fightDepth / startingDepth;

        Vector3 newPosition = Vector3.LerpUnclamped(
                originalStartPosition,
                fightStartPosition,
                progress
            );

        seaHook.position = newPosition;
    }

    // =========================================================
    // FINISH
    // =========================================================

    private IEnumerator FinishFishing()
    {
        if (finishing)
            yield break;

        finishing = true;
        fightStarted = false;

        fishingStatusText.text = "FISH CAUGHT!";

        // Simpan ikan yang berhasil ditangkap
        fishingState.CatchFish();

        depthSystem.SetDepthOverride(0f);

        // Kembali tepat ke posisi awal.
        seaHook.position = originalStartPosition;

        yield return new WaitForSeconds(successMessageTime);

        if (fishingState.CurrentFishObject != null)
        {
            Destroy(fishingState.CurrentFishObject.gameObject);
        }

        fishingState.EndFishing();

        depthSystem.ResumeFromCurrentPosition();
        fishingPanel.SetActive(false);

        minigameStarted = false;
        fightStarted = false;
        finishing = false;

        SceneManager.LoadScene("Dock");
    }
}