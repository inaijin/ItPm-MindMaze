using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IAgent, IHittable
{
    [SerializeField]
    private int maxHealth = 2;
    private int numberOfKey = 0;

    private int health;
    public int Health { 
        get => health;
        set 
        {
            health = Mathf.Clamp(value,0,maxHealth);
            uiHealth.UpdateUI(health);
        } 
    }

    private bool dead = false;

    private PlayerWeapon playeWeapon;

    [field: SerializeField]
    public UIHealth uiHealth { get; set; }

    [SerializeField]
    private UIKey uiKey = null;

    [field: SerializeField]
    public UnityEvent OnDie { get; set; }
    [field: SerializeField]
    public UnityEvent OnGetHit { get; set; }

    private void Awake()
    {
        playeWeapon = GetComponentInChildren<PlayerWeapon>();
    }
    private void Start()
    {
        numberOfKey = 0;
        uiKey.UdpateKeyText(numberOfKey);
        Health = maxHealth;
        uiHealth.Initialize(Health);
    }
    public void GetHit(int damage, GameObject damageDealer)
    {
        if(dead == false)
        {
            Health--;
            OnGetHit?.Invoke();
            if (Health <= 0)
            {
                OnDie?.Invoke();
                dead = true;
                
            }
        }
        
        
    }
    public void FindKey()
    {
        numberOfKey++;
        uiKey.UdpateKeyText(numberOfKey);
        Debug.Log("Key added! Total keys: " + numberOfKey);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Resource"))
        {
            var resource = collision.gameObject.GetComponent<Resource>();
            if (resource != null)
            {
                switch (resource.ResourceData.ResourceType)
                {
                    case ResourceTypeEnum.Health:
                        if(Health >= maxHealth)
                        {
                            return;
                        }
                        Health += resource.ResourceData.GetAmount();
                        resource.PickUpResource();
                        break;
                    case ResourceTypeEnum.Ammo:
                        if (playeWeapon.AmmoFull)
                        {
                            return;
                        }
                        playeWeapon.AddAmmo(resource.ResourceData.GetAmount());
                        resource.PickUpResource();
                        break;
                    default:
                        break;
                }
            }
        }
    }

}
