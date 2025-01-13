using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICoin : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text = null;

    public void UpdateCoinText(int coinCount)
    {
        text.SetText(coinCount.ToString());
    }
}
