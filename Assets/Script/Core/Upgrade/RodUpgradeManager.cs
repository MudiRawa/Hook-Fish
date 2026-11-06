using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RodUpgradeLevel
{
    public float maxDepth = 20f;
    public int upgradePrice;
}

public class RodUpgradeManager : MonoBehaviour
{
    public static RodUpgradeManager Instance { get; private set; }

    [Header("Rod Upgrade Levels")]
    public List<RodUpgradeLevel> upgradeLevels =
        new List<RodUpgradeLevel>();

    private const string RodLevelKey = "RodLevel";

    private int currentLevelIndex;

    public int CurrentLevel =>
        currentLevelIndex + 1;

    public float CurrentMaxDepth =>
        upgradeLevels[currentLevelIndex].maxDepth;

    public bool IsMaxLevel =>
        currentLevelIndex >= upgradeLevels.Count - 1;

    public int NextUpgradePrice =>
        IsMaxLevel
            ? 0
            : upgradeLevels[currentLevelIndex + 1].upgradePrice;

    public float NextMaxDepth =>
        IsMaxLevel
            ? CurrentMaxDepth
            : upgradeLevels[currentLevelIndex + 1].maxDepth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadRodLevel();
    }

    private void LoadRodLevel()
    {
        if (upgradeLevels == null || upgradeLevels.Count == 0)
        {
            Debug.LogError("Rod Upgrade Levels masih kosong!");
            return;
        }

        currentLevelIndex =
            PlayerPrefs.GetInt(RodLevelKey, 0);

        currentLevelIndex =
            Mathf.Clamp(
                currentLevelIndex,
                0,
                upgradeLevels.Count - 1
            );

        Debug.Log(
            "Rod Level: " + CurrentLevel +
            " | Max Depth: " + CurrentMaxDepth + " m"
        );
    }

    public bool BuyUpgrade()
    {
        if (IsMaxLevel)
        {
            Debug.Log("Rod sudah mencapai level maksimal.");
            return false;
        }

        int price = NextUpgradePrice;

        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager tidak ditemukan.");
            return false;
        }

        if (!CoinManager.Instance.SpendCoin(price))
        {
            Debug.Log("Coin tidak cukup untuk upgrade.");
            return false;
        }

        currentLevelIndex++;

        PlayerPrefs.SetInt(
            RodLevelKey,
            currentLevelIndex
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Rod berhasil di-upgrade ke Level " +
            CurrentLevel +
            " | Max Depth: " +
            CurrentMaxDepth + " m"
        );

        return true;
    }
}