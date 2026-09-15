using UnityEngine;

public class DockManager : MonoBehaviour
{
    public HookThrow hook;
    public void StartFishing()
    {
        Debug.Log("Fishing started!");
        hook.ThrowHook();
    }
}
