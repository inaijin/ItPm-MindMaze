using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance; // Singleton instance for global access

    [Header("Gameplay Elements")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameManager gameManager;

    [Header("Shop UI Elements")]
    [SerializeField] private Canvas shopCanvas; // Reference to the shop canvas

    [Header("Shop Settings")]
    [SerializeField] private int healthPrice = 100;
    [SerializeField] private int ammoPrice = 50;
    [SerializeField] private int damagePrice = 200; // Price for increasing damage

    private Player player; // Reference to the player
    private Enemy[] enemies; // To update all enemies' multipliers
    private Weapon playerWeapon;

    public bool isShopActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (shopCanvas != null)
        {
            shopCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Shop Canvas is not assigned in ShopManager!");
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure the player has the correct tag.");
        }

        // Get the weapon reference
        playerWeapon = player.GetComponentInChildren<Weapon>();
        if (playerWeapon == null)
        {
            Debug.LogError("Weapon not found on the player! Ensure the weapon is a child of the player.");
        }
    }

    public void OpenShop()
    {
        isShopActive = true;
        if (shopCanvas != null)
        {
            shopCanvas.gameObject.SetActive(true);
        }
        if (weapon != null)
            weapon.SetActive(false);

        if (gameManager != null)
            gameManager.SetCursorDefault();
    }

    public void CloseShop()
    {
        isShopActive = false;
        if (shopCanvas != null)
        {
            shopCanvas.gameObject.SetActive(false);
        }
        if (weapon != null)
            weapon.SetActive(true);

        if (gameManager != null)
            gameManager.SetCursorIcon();
    }

    public void BuyHealth()
    {
        if (player != null && player.coin >= healthPrice && player.maxHealth < 5)
        {
            player.coin -= healthPrice;
            player.AddMaxHealth();
            player.Health += 1;
            Debug.Log("One Health purchased.");
        }
        else
        {
            Debug.Log("Cannot purchase health. Insufficient coins or health is full.");
        }
    }

    public void BuyMaxAmmo()
    {
        if (player != null && player.coin >= ammoPrice && playerWeapon != null)
        {
            player.coin -= ammoPrice;
            playerWeapon.weaponData.AmmoCapacity += 10;
            playerWeapon.Ammo += 10; // Give additional ammo immediately
            Debug.Log("Max Ammo increased by 10.");
        }
        else
        {
            Debug.Log("Cannot purchase Max Ammo. Insufficient coins.");
        }
    }

    public void BuyIncreaseDamage()
    {
        if (player != null && player.coin >= damagePrice)
        {
            player.coin -= damagePrice;

            playerWeapon.IncreaseDamage();
        }
        else
        {
            Debug.Log("Cannot purchase Damage increase. Insufficient coins.");
        }
    }
}
