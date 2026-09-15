using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterDetector : MonoBehaviour
{
    private bool hasEnteredWater = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasEnteredWater)
            return;

        if (other.CompareTag("FishingHook"))
        {
            hasEnteredWater = true;

            SceneManager.LoadScene("Sea");
        }
    }
}