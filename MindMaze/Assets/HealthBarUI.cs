using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] int showTime = 2;

    private Coroutine lastCoroutine = null;

    private void Start()
    {
        healthBar.gameObject.SetActive(false);
    }
    public void Initialized(int maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }

    public void UpdateHealth(int health)
    {
        healthBar.value = health;
        if(lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
        }
        lastCoroutine = StartCoroutine(ShowHealthCoroutine());
    }


    private IEnumerator ShowHealthCoroutine()
    {
        healthBar.gameObject.SetActive(true);

        yield return new WaitForSeconds(showTime);

        healthBar.gameObject.SetActive(false);
        lastCoroutine = null;

    }

}
