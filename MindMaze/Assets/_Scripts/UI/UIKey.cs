using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIKey : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text = null;

    public void UdpateKeyText(int keyCount)
    {
        text.SetText(keyCount.ToString());
    }
}
