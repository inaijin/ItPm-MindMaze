using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopNPC : MonoBehaviour
{
    [Header("Player Interaction")]
    private Transform player;
    private bool isPlayerNear = false;

    [Header("Shop UI Elements")]
    private Canvas shopCanvas; // UI for the shop
    private TextMeshProUGUI messageText; // Text for messages
    private Button heartsButton;
    private Button projectileSpeedButton;
    private Button maxAmmoButton;

    [Header("Upgrade Costs")]
    [SerializeField] private int heartUpgradeCost = 50;
    [SerializeField] private int projectileSpeedUpgradeCost = 75;
    [SerializeField] private int maxAmmoUpgradeCost = 100;

    private Player playerScript;
    private PlayerWeapon playerWeapon;
    //private PlayerInventory playerInventory;

    private void Start()
    {
        // Find the Player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<Player>();
        playerWeapon = player.GetComponentInChildren<PlayerWeapon>();
        //playerInventory = player.GetComponent<PlayerInventory>();

        // Find the Shop UI in the Scene (by tag or direct reference)
        shopCanvas = GameObject.Find("ShopCanvas").GetComponent<Canvas>();
        messageText = shopCanvas.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();
        heartsButton = shopCanvas.transform.Find("HeartsButton").GetComponent<Button>();
        projectileSpeedButton = shopCanvas.transform.Find("ProjectileSpeedButton").GetComponent<Button>();
        maxAmmoButton = shopCanvas.transform.Find("MaxAmmoButton").GetComponent<Button>();

        // Initialize UI State
        shopCanvas.gameObject.SetActive(false);

        // Attach button listeners
        heartsButton.onClick.AddListener(BuyHeartUpgrade);
        projectileSpeedButton.onClick.AddListener(BuyProjectileSpeedUpgrade);
        maxAmmoButton.onClick.AddListener(BuyMaxAmmoUpgrade);
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

    private void ToggleShop()
    {
        shopCanvas.gameObject.SetActive(!shopCanvas.gameObject.activeSelf);
        Time.timeScale = shopCanvas.gameObject.activeSelf ? 0 : 1; // Pause game while in the shop
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            shopCanvas.gameObject.SetActive(false);
            Time.timeScale = 1; // Resume game when leaving the shop
        }
    }

    #region Upgrade Functions

    private void BuyHeartUpgrade()
    {
        if (SpendCurrency(heartUpgradeCost))
        {
            playerScript.uiHealth.Initialize(++playerScript.uiHealth.heartCount);
            messageText.text = "Heart upgrade purchased!";
        }
        else
        {
            messageText.text = "Not enough money for Heart upgrade!";
        }
    }

    private void BuyProjectileSpeedUpgrade()
    {
        if (SpendCurrency(projectileSpeedUpgradeCost))
        {
            //playerWeapon.projectileSpeed += 1.0f; // Increase projectile speed
            messageText.text = "Projectile speed upgraded!";
        }
        else
        {
            messageText.text = "Not enough money for Projectile upgrade!";
        }
    }

    private void BuyMaxAmmoUpgrade()
    {
        if (SpendCurrency(maxAmmoUpgradeCost))
        {
            //playerWeapon.maxAmmo += 5; // Increase max ammo capacity
            messageText.text = "Max ammo capacity increased!";
        }
        else
        {
            messageText.text = "Not enough money for Ammo upgrade!";
        }
    }

    #endregion

    private bool SpendCurrency(int cost)
    {
        return true;
        //if (playerInventory != null && playerInventory.currency >= cost)
        //{
        //    playerInventory.currency -= cost;
        //    return true;
        //}
        //return false;
    }
}
