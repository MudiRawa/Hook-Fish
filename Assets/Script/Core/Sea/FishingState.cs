using UnityEngine;

public class FishingState : MonoBehaviour
{
    public bool IsFishing { get; private set; }

    public FishData CurrentFish { get; private set; }

    public void StartFishing(FishData fish)
    {
        if (IsFishing)
            return;

        IsFishing = true;
        CurrentFish = fish;

        Debug.Log("Ikan menyambar: " + fish.fishName);
    }

    public void EndFishing()
    {
        IsFishing = false;
        CurrentFish = null;
    }
}