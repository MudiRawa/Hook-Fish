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
                "fishPrefab untuk " +
                caughtFish.fishName +
                " belum diisi."
            );

            return;
        }

        currentFishObject =
            Instantiate(
                caughtFish.fishPrefab,
                fishHoldPoint
            );

        currentFishObject.transform.localPosition = Vector3.zero;
        currentFishObject.transform.localRotation = Quaternion.Euler(-70, 0, 0);
        Debug.Log("Menampilkan ikan di Dock: " + caughtFish.fishName);
    }
}