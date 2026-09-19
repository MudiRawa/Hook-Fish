using UnityEngine;

public enum FishRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(
    fileName = "NewFish",
    menuName = "Fishing/Fish Data"
)]
public class FishData : ScriptableObject
{
    [Header("Basic Info")]
    public string fishName;

    [TextArea(2, 5)]
    public string description;

    [Header("Fish")]
    public GameObject fishPrefab;

    [Header("Rarity")]
    public FishRarity rarity;

    [Header("Fight")]
    [Range(1f, 100f)]
    public float fightStrength = 1f;
}