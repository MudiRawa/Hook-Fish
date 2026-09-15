using UnityEngine;

public class FishEncounter : MonoBehaviour
{
    public FishData fishData;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("FishingHook"))
            return;

        FishingState fishingState = FindAnyObjectByType<FishingState>();

        if (fishingState == null)
        {
            Debug.LogError("FishingState tidak ditemukan.");
            return;
        }

        fishingState.StartFishing(fishData);
    }
}