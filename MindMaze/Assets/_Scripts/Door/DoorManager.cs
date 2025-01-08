using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private int requiredKeys = 3; // Minimum keys needed to unlock the door
    [SerializeField] private string[] insufficientKeyDialog; // Dialog for insufficient keys
    [SerializeField] private string[] sufficientKeyDialog;   // Dialog for sufficient keys

    [Header("Player Interaction")]
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerNear = false;
    private bool isDoorTriggered = false; // Prevent multiple interactions

    [Header("Dialog Settings")]
    public float typingSpeed = 0.05f; // Typing speed for dialog

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Handle sprite flipping based on player's position
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            spriteRenderer.flipX = directionToPlayer.x < 0;
        }

        // Check for interaction input (E key) and if Dialog is NOT active
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !DialogManager.Instance.IsDialogActive())
        {
            HandleDoorInteraction();
        }
    }

    private void HandleDoorInteraction()
    {
        if (isDoorTriggered) return; // Prevent multiple triggers

        Player playerComponent = player.GetComponent<Player>();

        if (playerComponent != null)
        {
            if (playerComponent.numberOfKey >= requiredKeys)
            {
                // Player has enough keys
                DialogManager.Instance.StartDialog(sufficientKeyDialog, typingSpeed);
                DialogManager.Instance.OnDialogEnd += EndGame; // Subscribe to end game when dialog ends
            }
            else
            {
                // Player doesn't have enough keys
                DialogManager.Instance.StartDialog(insufficientKeyDialog, typingSpeed);
            }
        }
    }

    private void EndGame()
    {
        isDoorTriggered = true; // Prevent further interactions
        DialogManager.Instance.OnDialogEnd -= EndGame; // Unsubscribe from the event
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
            DialogManager.Instance.EndDialog();
        }
    }
}
