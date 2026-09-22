using UnityEngine;

public enum DockState
{
    ReadyToFish,
    Shop
}

public class DockManager : MonoBehaviour
{
    [Header("Fishing")]
    public HookThrow hook;

    [Header("Dock State")]
    public DockState currentState = DockState.ReadyToFish;

    public bool IsInShop => currentState == DockState.Shop;

    public bool IsReadyToFish => currentState == DockState.ReadyToFish;

    public void StartFishing()
    {
        if (!IsReadyToFish)
            return;

        Debug.Log("Fishing started!");

        hook.ThrowHook();
    }

    public void OpenShop()
    {
        currentState = DockState.Shop;

        Debug.Log("Dock State: SHOP");
    }

    public void CloseShop()
    {
        currentState = DockState.ReadyToFish;

        Debug.Log("Dock State: READY TO FISH");
    }
}