using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject sellButton;
    public GameObject priceButton;

    [Header("Price Info")]
    public TMP_Text priceText;

    [Header("References")]
    public CoinUI coinUI;
    public DockCaughtFish dockCaughtFish;

    private void Start()
    {
        priceText.gameObject.SetActive(false);

        CheckCaughtFish();
    }

    private void CheckCaughtFish()
    {
        FishData fish = FishingState.LastCaughtFish;

        bool hasFish = fish != null;

        sellButton.SetActive(hasFish);
        priceButton.SetActive(hasFish);
    }

    private int CalculateSellPrice(
        FishData fish,
        float sizeMultiplier
    )
    {
        if (fish == null)
            return 0;

        float price =
            fish.basePrice *
            sizeMultiplier;

        return Mathf.RoundToInt(price);
    }

    public void ShowPrice()
    {
        FishData fish =
            FishingState.LastCaughtFish;

        if (fish == null)
            return;

        float sizeMultiplier =
            FishingState.LastCaughtFishSize;

        int sellPrice =
            CalculateSellPrice(
                fish,
                sizeMultiplier
            );

        priceText.text =
            fish.fishName +
            "\nSize: " +
            sizeMultiplier.ToString("F2") +
            "x" +
            "\n" +
            sellPrice +
            " Coin";

        priceText.gameObject.SetActive(true);
    }

    public void HidePrice()
    {
        priceText.gameObject.SetActive(false);
    }

    public void SellFish()
    {
        FishData fish =
            FishingState.LastCaughtFish;

        if (fish == null)
        {
            Debug.Log(
                "Tidak ada ikan untuk dijual."
            );

            return;
        }

        float sizeMultiplier =
            FishingState.LastCaughtFishSize;

        int sellPrice =
            CalculateSellPrice(
                fish,
                sizeMultiplier
            );

        // Hapus data tangkapan terlebih dahulu
        // supaya ikan tidak bisa dijual berulang kali.
        FishingState.ClearLastCaughtFish();

        DockManager dockManager = FindAnyObjectByType<DockManager>();
        if (dockManager != null)
        {
            dockManager.UpdateFishingButton();
        }

        // Tambahkan coin
        CoinManager.Instance.AddCoin(
            sellPrice
        );

        Debug.Log(
            "Ikan terjual: " +
            fish.fishName +
            " | Size: " +
            sizeMultiplier.ToString("F2") +
            "x" +
            " | Harga: " +
            sellPrice +
            " Coin"
        );

        // Update UI coin
        coinUI.UpdateCoinUI();

        // Hapus ikan yang sedang dipegang
        dockCaughtFish.ClearHeldFish();

        // Sembunyikan info harga
        priceText.gameObject.SetActive(false);

        // Sembunyikan tombol
        sellButton.SetActive(false);
        priceButton.SetActive(false);
    }
}