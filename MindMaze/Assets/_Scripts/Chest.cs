using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private Player player;

    private void Start()
    {
        // Find the player in the scene
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (player != null)
            {
                player.FindKey();
                Debug.Log("Chest opened and key added!");
                Destroy(gameObject); // Destroy the chest after opening
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player is near the chest!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player left the chest!");
        }
    }
}