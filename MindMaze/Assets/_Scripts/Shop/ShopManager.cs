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

    private Player player; // Reference to the player

    public bool isShopActive = false;

    private void Awake()
    {
        // Ensure a single instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure the shop canvas is initially inactive
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
        // Get the player reference
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure the player has the correct tag.");
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
}
