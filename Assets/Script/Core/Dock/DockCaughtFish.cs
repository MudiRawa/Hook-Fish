using UnityEngine;

public class DockCaughtFish : MonoBehaviour
{
    [Header("Fish Hold Point")]
    public Transform fishHoldPoint;

    private GameObject currentFishObject;

    private void Start()
    {
        ShowCaughtFish();
    }

    private void ShowCaughtFish()
    {
        FishData caughtFish =
            FishingState.LastCaughtFish;

        if (caughtFish == null)
        {
            Debug.LogWarning(
                "Tidak ada ikan hasil tangkapan."
            );

            return;
        }

        if (caughtFish.fishPrefab == null)
        {
            Debug.LogWarning(
                "FishPrefab untuk " +
                caughtFish.fishName +
                " belum diisi."
            );

            return;
        }

        // Spawn ikan di tangan
        currentFishObject =
            Instantiate(
                caughtFish.fishPrefab,
                fishHoldPoint
            );

        currentFishObject.transform.localPosition =
            Vector3.zero;

        currentFishObject.transform.localRotation =
            Quaternion.Euler(-70f, 0f, 0f);

        // =====================================================
        // PAKAI UKURAN IKAN YANG SAMA DENGAN HASIL TANGKAPAN
        // =====================================================

        float sizeMultiplier =
            FishingState.LastCaughtFishSize;

        currentFishObject.transform.localScale =
            caughtFish.fishPrefab.transform.localScale *
            sizeMultiplier;

        Debug.Log(
            "Menampilkan ikan di Dock: " +
            caughtFish.fishName +
            " | Size: " +
            sizeMultiplier.ToString("F2") +
            "x"
        );
    }

    public void ClearHeldFish()
    {
        if (currentFishObject != null)
        {
            Destroy(currentFishObject);

            currentFishObject = null;
        }
    }
}