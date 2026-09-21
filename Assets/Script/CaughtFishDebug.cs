using UnityEngine;

public class CaughtFishDebug : MonoBehaviour
{
    private void Start()
    {
        if (FishingState.LastCaughtFish != null)
        {
            Debug.Log(
                "Dock menerima ikan: " +
                FishingState.LastCaughtFish.fishName
            );
        }
        else
        {
            Debug.LogWarning(
                "Dock tidak menerima ikan."
            );
        }
    }
}