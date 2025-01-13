using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private int requiredKeys = 3; // Minimum keys needed to unlock the door
    [SerializeField] private string[] insufficientKeyDialog; // Dialog for insufficient keys
    [SerializeField] private string[] sufficientKeyDialog;   // Dialog for sufficient keys

    [Header("Dialog Settings")]
    [SerializeField] private float typingSpeed = 0.05f; // Typing speed for dialog

    [Header("Player Interaction")]
    private Transform playerTransform;
    private Player playerComponent;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerNear = false;
    private bool isDoorTriggered = false; // Prevent multiple interactions

    const int ID = -2;

    private void Start()
    {
        // Cache references to player and sprite renderer
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerComponent = playerObject.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Flip sprite to face the player
            spriteRenderer.flipX = (playerTransform.position.x < transform.position.x);

            // Handle interaction input
            if (!playerComponent.dead && isPlayerNear && Input.GetKeyDown(KeyCode.E) && !DialogManager.Instance.IsDialogActive())
            {
                HandleDoorInteraction();
            }
        }
    }

    private void HandleDoorInteraction()
    {
        if (isDoorTriggered) return; // Prevent multiple interactions

        if (playerComponent != null)
        {
            if (playerComponent.numberOfKey >= requiredKeys)
            {
                // Player has enough keys
                DialogManager.Instance.StartDialog(ID,sufficientKeyDialog, typingSpeed);
                DialogManager.Instance.OnDialogEnd += CompleteGame; // Subscribe to dialog end event
            }
            else
            {
                // Player doesn't have enough keys
                DialogManager.Instance.StartDialog(ID,insufficientKeyDialog, typingSpeed);
            }
        }
    }

    private void CompleteGame()
    {
        isDoorTriggered = true; // Mark door as triggered
        DialogManager.Instance.OnDialogEnd -= CompleteGame; // Unsubscribe from the event
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
            DialogManager.Instance.EndDialog(ID);
        }
    }
}
