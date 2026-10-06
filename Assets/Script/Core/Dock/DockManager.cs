using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum DockState
{
    ReadyToFish,
    Shop
}

public class DockManager : MonoBehaviour
{
    [Header("Fishing")]
    public HookThrow hook;
    public Button startFishingButton;

    [Header("Dock State")]
    public DockState currentState = DockState.ReadyToFish;

    private void Start()
    {
        UpdateFishingButton();
    }

    public void StartFishing()
    {
        // Tidak bisa mancing saat Shop
        if (currentState != DockState.ReadyToFish)
            return;

        // Tidak bisa mancing kalau masih membawa ikan
        if (FishingState.LastCaughtFish != null)
        {
            Debug.Log(
                "Tidak bisa mancing. Ikan masih dibawa."
            );

            return;
        }

        Debug.Log("Fishing started!");

        hook.ThrowHook();
        StartCoroutine(WaitForSeconds(2.5f));
    }

    public void OpenShop()
    {
        currentState = DockState.Shop;

        UpdateFishingButton();

        Debug.Log("Dock State: SHOP");
    }

    public void CloseShop()
    {
        currentState = DockState.ReadyToFish;

        UpdateFishingButton();

        Debug.Log("Dock State: READY TO FISH");
    }

    public void UpdateFishingButton()
    {
        if (startFishingButton == null)
            return;

        bool hasCaughtFish =
            FishingState.LastCaughtFish != null;

        bool canFish =
            currentState == DockState.ReadyToFish &&
            !hasCaughtFish;

        startFishingButton.gameObject.SetActive(
            canFish
        );
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("Sea");
    }
}