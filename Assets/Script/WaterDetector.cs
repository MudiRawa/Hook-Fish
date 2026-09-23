using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterDetector : MonoBehaviour
{
    [Header("Water Splash VFX")]
    public GameObject waterSplashVFX;
    public float sceneLoadDelay = 1f;

    private bool hasEnteredWater = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasEnteredWater)
            return;

        if (!other.CompareTag("FishingHook"))
            return;

        hasEnteredWater = true;

        PlayWaterSplash(other.transform.position);

        StartCoroutine(
            LoadSeaScene()
        );
    }

    private void PlayWaterSplash(Vector3 position)
    {
        if (waterSplashVFX == null)
        {
            Debug.LogWarning(
                "Water Splash VFX belum diisi."
            );

            return;
        }

        GameObject vfx =
            Instantiate(
                waterSplashVFX,
                position,
                Quaternion.identity
            );

        Destroy(
            vfx,
            sceneLoadDelay
        );
    }

    private IEnumerator LoadSeaScene()
    {
        yield return new WaitForSeconds(
            sceneLoadDelay
        );

        SceneManager.LoadScene("Sea");
    }
}