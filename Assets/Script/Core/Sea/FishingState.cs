using UnityEngine;

public class FishingState : MonoBehaviour
{
    public bool IsFishing { get; private set; }

    public FishData CurrentFish { get; private set; }

    public Transform CurrentFishObject { get; private set; }

    // Ukuran aktual ikan yang sedang dipancing
    public float CurrentFishSize { get; private set; }

    // Ikan terakhir yang berhasil ditangkap
    public static FishData LastCaughtFish { get; private set; }

    // Ukuran aktual ikan terakhir
    public static float LastCaughtFishSize { get; private set; }

    [Header("Catch VFX")]
    public GameObject catchVFXPrefab;

    public void StartFishing(FishData fish, Transform fishObject)
    {
        if (IsFishing)
            return;

        IsFishing = true;

        CurrentFish = fish;
        CurrentFishObject = fishObject;

        // =====================================================
        // CATCH VFX
        // =====================================================

        if (catchVFXPrefab != null)
        {
            GameObject vfx = Instantiate(catchVFXPrefab, fishObject.position, Quaternion.identity);

            Destroy(vfx, 2f);
        }

        // =====================================================
        // AMBIL UKURAN IKAN
        // =====================================================

        FishMovement fishMovement =
            fishObject.GetComponent<FishMovement>();

        if (fishMovement != null)
        {
            CurrentFishSize =
                fishMovement.currentSizeMultiplier;
        }
        else
        {
            CurrentFishSize = 1f;

            Debug.LogWarning(
                "FishMovement tidak ditemukan pada ikan."
            );
        }

        // Hapus hasil tangkapan sebelumnya
        LastCaughtFish = null;
        LastCaughtFishSize = 0f;

        Debug.Log("Ikan menyambar: " + fish.fishName + " | Size: " + CurrentFishSize.ToString("F2") + "x");
    }

    public void CatchFish()
    {
        if (CurrentFish == null)
            return;

        LastCaughtFish =
            CurrentFish;

        LastCaughtFishSize =
            CurrentFishSize;

        Debug.Log(
            "Ikan berhasil ditangkap: " +
            LastCaughtFish.fishName +
            " | Size: " +
            LastCaughtFishSize.ToString("F2") +
            "x"
        );
    }

    public void EndFishing()
    {
        IsFishing = false;

        CurrentFish = null;
        CurrentFishObject = null;
        CurrentFishSize = 0f;
    }

    public static void ClearLastCaughtFish()
    {
        LastCaughtFish = null;
        LastCaughtFishSize = 0f;
    }
}