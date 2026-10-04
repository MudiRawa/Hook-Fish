using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public int Coin { get; private set; }

    private const string CoinKey = "Coin";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCoin();
    }

    public void AddCoin(int amount)
    {
        if (amount <= 0)
            return;

        Coin += amount;
        SaveCoin();

        Debug.Log("Coin bertambah: +" + amount + " | Total: " + Coin);
    }

    public bool SpendCoin(int amount)
    {
        if (amount <= 0)
            return false;

        if (Coin < amount)
            return false;

        Coin -= amount; 
        SaveCoin();

        return true;
    }

    private void SaveCoin()
    {
        PlayerPrefs.SetInt(CoinKey, Coin);
        PlayerPrefs.Save();
    }

    private void LoadCoin()
    {
        Coin = PlayerPrefs.GetInt(CoinKey, 0);

        Debug.Log("Coin dimuat: " + Coin);
    }
}