using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    public TMP_Text coinText;

    private void Start()
    {
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        if (coinText == null)
            return;

        coinText.text = CoinManager.Instance.Coin.ToString();
    }
}