using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIKey : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text = null;

    public void UdpateKeyText(int keyCount, bool foundSamurai)
    {
        if (foundSamurai)
        {
            text.SetText($"{keyCount}/10");
            if (keyCount < 10)
            {
                text.color = Color.red; // Set text color to red when keys are less than 10
            }
            else
            {
                text.color = Color.green; // Set text color to green when keys are 10
            }
        }
        else
        {
            text.SetText(keyCount.ToString());
            text.color = Color.white; // Default text color when Samurai is not found
        }
    }
}
