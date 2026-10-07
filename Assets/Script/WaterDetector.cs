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

        PlayWaterSplash(other.transform.position + Vector3.up * 0.35f);

        // Fade out BGM Dock (index 0)
        AudioManager.Instance.FadeOutBGM(1f);

        // Play splash SFX
        AudioManager.Instance.PlaySFX(1);

        // Play BGM ambient air (index 1)
        AudioManager.Instance.PlayBGM(1);

        Debug.Log("💧 Kail masuk air - BGM Dock fade out, BGM Air playing");

        StartCoroutine(LoadSeaScene());
    }

    private void PlayWaterSplash(Vector3 position)
    {
        if (waterSplashVFX == null)
        {
            Debug.LogWarning("Water Splash VFX belum diisi.");

            return;
        }

        GameObject vfx = Instantiate(waterSplashVFX, position, Quaternion.Euler(-90f, 0f, 0f));
        vfx.transform.localPosition = new Vector3(0f, -1.57f, 10.85f);

        Destroy(vfx, sceneLoadDelay);
    }

    private IEnumerator LoadSeaScene()
    {
        yield return new WaitForSeconds(
            sceneLoadDelay
        );

        SceneManager.LoadScene("Sea");
    }
}