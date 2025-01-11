using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance; // Singleton instance for global access

    [Header("Gameplay Elements")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameManager gameManager;

    [Header("Shop UI Elements")]
    [SerializeField] private Canvas shopCanvas; // Reference to the shop canvas
    [SerializeField]
    private UIDamage uiKey = null;

    [Header("Shop Notifications")]
    [SerializeField] private TextMeshProUGUI insufficientCoinsText; // Notification for insufficient coins
    [SerializeField] private TextMeshProUGUI upgradeLimitReachedText; // Notification for upgrade limit reached

    [Header("Shop Settings")]
    [SerializeField] private int healthPrice = 100;
    [SerializeField] private int ammoPrice = 50;
    [SerializeField] private int damagePrice = 200; // Price for increasing damage
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int maxDamage = 10;
    [SerializeField] private int maxAmmo = 500;

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

        if (insufficientCoinsText != null)
        {
            insufficientCoinsText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Insufficient Coins Text is not assigned!");
        }

        if (upgradeLimitReachedText != null)
        {
            upgradeLimitReachedText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Upgrade Limit Reached Text is not assigned!");
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
        if (player != null && player.coin >= healthPrice && player.maxHealth < maxHealth)
        {
            player.coin -= healthPrice;
            player.AddMaxHealth();
            player.Health += 1;
            Debug.Log("One Health purchased.");
        }
        else
        {
            if (player.coin < healthPrice)
            {
                DisplayNotification(insufficientCoinsText);
            }
            else if (player.maxHealth >= maxHealth)
            {
                DisplayNotification(upgradeLimitReachedText);
            }
        }
    }

    public void BuyMaxAmmo()
    {
        if (player != null && player.coin >= ammoPrice && playerWeapon != null && playerWeapon.Ammo < maxAmmo)
        {
            player.coin -= ammoPrice;
            playerWeapon.weaponData.AmmoCapacity += 10;
            playerWeapon.Ammo += 10; // Give additional ammo immediately
            Debug.Log("Max Ammo increased by 10.");
        }
        else
        {
            if (playerWeapon.Ammo >= maxAmmo)
                DisplayNotification(upgradeLimitReachedText);
            else
                DisplayNotification(insufficientCoinsText);
        }
    }

    public void BuyIncreaseDamage()
    {
        if (player != null && player.coin >= damagePrice && playerWeapon.additionalDamage < maxDamage)
        {
            player.coin -= damagePrice;

            playerWeapon.IncreaseDamage();
            uiKey.UdpateDamageText(playerWeapon.additionalDamage);
            Debug.Log("Weapon Damage Increased.");
        }
        else
        {
            if (playerWeapon.additionalDamage >= maxDamage)
                DisplayNotification(upgradeLimitReachedText);
            else
                DisplayNotification(insufficientCoinsText);
        }
    }

    private void DisplayNotification(TextMeshProUGUI text)
    {
        if (text != null)
        {
            text.gameObject.SetActive(true);
            Invoke(nameof(HideNotification), 2f);
        }
    }

    private void HideNotification()
    {
        if (insufficientCoinsText != null)
            insufficientCoinsText.gameObject.SetActive(false);

        if (upgradeLimitReachedText != null)
            upgradeLimitReachedText.gameObject.SetActive(false);
    }
}
