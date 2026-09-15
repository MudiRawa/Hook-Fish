using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DepthUI : MonoBehaviour
{
    [Header("References")]
    public DepthSystem depthSystem;
    public Slider depthSlider;
    public TMP_Text depthText;

    private void Start()
    {
        if (depthSystem == null)
        {
            Debug.LogError("DepthSystem belum diisi.");
            return;
        }

        depthSlider.minValue = 0f;
        depthSlider.maxValue = depthSystem.maxDepth;
        depthSlider.value = depthSystem.CurrentDepth;
    }

    private void Update()
    {
        if (depthSystem == null)
            return;

        depthSlider.value = depthSystem.CurrentDepth;

        depthText.text =
            depthSystem.CurrentDepth.ToString("F1") + " m";
    }
}