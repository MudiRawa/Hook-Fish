using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RodUpgradeUI : MonoBehaviour
{
    [Header("Information")]
    public TMP_Text currentLevelText;
    public TMP_Text currentDepthText;
    public TMP_Text nextLevelText;
    public TMP_Text nextDepthText;
    public TMP_Text upgradePriceText;
    public TMP_Text upgradeStatusText;

    [Header("Button")]
    public Button upgradeButton;

    [Header("References")]
    public CoinUI coinUI;

    private void Start()
    {
        RefreshUI();
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (RodUpgradeManager.Instance == null)
        {
            Debug.LogError(
                "RodUpgradeManager tidak ditemukan."
            );

            return;
        }

        RodUpgradeManager manager =
            RodUpgradeManager.Instance;

        // Informasi level sekarang
        currentLevelText.text =
            "Rod Level: " +
            manager.CurrentLevel;

        currentDepthText.text =
            "Maximum Depth: " +
            manager.CurrentMaxDepth +
            " m";

        upgradeStatusText.text = "";

        // Cek apakah sudah mencapai level maksimal
        if (manager.IsMaxLevel)
        {
            nextLevelText.text =
                "MAX LEVEL";

            nextDepthText.text =
                "Maximum upgrade reached";

            upgradePriceText.text =
                "-";

            upgradeButton.interactable =
                false;

            return;
        }

        // Informasi upgrade berikutnya
        nextLevelText.text =
            "Next Level: " +
            (manager.CurrentLevel + 1);

        nextDepthText.text =
            "New Maximum Depth: " +
            manager.NextMaxDepth +
            " m";

        upgradePriceText.text =
            "Price: " +
            manager.NextUpgradePrice +
            " Coin";

        upgradeButton.interactable =
            true;
    }

    public void BuyUpgrade()
    {
        if (RodUpgradeManager.Instance == null)
            return;

        RodUpgradeManager manager =
            RodUpgradeManager.Instance;

        if (manager.IsMaxLevel)
        {
            upgradeStatusText.text =
                "Rod sudah mencapai level maksimal.";

            RefreshUI();
            return;
        }

        bool success =
            manager.BuyUpgrade();

        if (success)
        {
            upgradeStatusText.text =
                "UPGRADE BERHASIL!";

            // Perbarui tampilan Coin
            if (coinUI != null)
            {
                coinUI.UpdateCoinUI();
            }

            Debug.Log(
                "Rod berhasil di-upgrade!"
            );
        }
        else
        {
            upgradeStatusText.text =
                "Coin tidak cukup!";
        }

        RefreshUI();
    }
}