using UnityEngine;

public class FishingState : MonoBehaviour
{
    public bool IsFishing { get; private set; }

    // Ikan yang sedang dipancing
    public FishData CurrentFish { get; private set; }

    // Object ikan yang sedang menempel di kail
    public Transform CurrentFishObject { get; private set; }

    // Ikan terakhir yang berhasil ditangkap
    // Tetap tersimpan walaupun pindah scene
    public static FishData LastCaughtFish { get; private set; }

    public void StartFishing(FishData fish, Transform fishObject)
    {
        if (IsFishing)
            return;

        IsFishing = true;
        CurrentFish = fish;
        CurrentFishObject = fishObject;

        // Hapus hasil tangkapan sebelumnya karena sekarang
        // sedang memulai tangkapan baru
        LastCaughtFish = null;

        Debug.Log("Ikan menyambar: " + fish.fishName);
    }

    public void CatchFish()
    {
        if (CurrentFish == null)
            return;

        LastCaughtFish = CurrentFish;

        Debug.Log(
            "Ikan berhasil ditangkap: " +
            LastCaughtFish.fishName
        );
    }

    public void EndFishing()
    {
        IsFishing = false;
        CurrentFish = null;
        CurrentFishObject = null;
    }
}