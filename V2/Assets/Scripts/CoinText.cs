using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CoinText : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    int coinCount;

    private void OnEnable(){
        Coin.OnCoinCollected += IncrementCoinCount;
    }
    private void OnDisable(){
        Coin.OnCoinCollected -= IncrementCoinCount;
    }

    public void IncrementCoinCount(){
        coinCount++;
        coinText.text = $"Trybiki: {coinCount}/5";
    }
}
