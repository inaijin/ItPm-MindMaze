using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDamage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text = null;

    public void UdpateDamageText(int additionalDmgCount)
    {
        text.color = Color.magenta;
        text.SetText("+" + additionalDmgCount.ToString());
    }
}
