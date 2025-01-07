using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private int requiredKeys = 3; // Minimum keys needed to unlock the door

    [Header("Player Interaction")]
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerNear = false;
    private bool isDoorOpen = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            HandleDoorInteraction();
        }

        // Handle sprite flipping based on player's position
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            spriteRenderer.flipX = directionToPlayer.x < 0;
        }
    }

    private void HandleDoorInteraction()
    {
        if (isDoorOpen) return; // Prevent multiple triggers if door is already open

        Player playerComponent = player.GetComponent<Player>();

        if (playerComponent != null)
        {
            if (playerComponent.numberOfKey >= requiredKeys)
            {
                Debug.Log("Door unlocked! Ending the game...");
                EndGame();
            }
            else
            {
                Debug.Log($"You need at least {requiredKeys} keys to unlock this door. Current keys: {playerComponent.numberOfKey}");
            }
        }
    }

    private void EndGame()
    {
        isDoorOpen = true;
        // Logic for ending the game, such as loading an end scene or showing a victory screen
        Debug.Log("Congratulations! You have completed the game!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
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
        }
    }
}
