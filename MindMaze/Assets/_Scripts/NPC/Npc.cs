using UnityEngine;

public class Npc : MonoBehaviour
{
[Header("Player Interaction")]
private Transform player;
private SpriteRenderer spriteRenderer;
protected bool isPlayerNear = false;

[Header("Dialog Settings")]
public string[] dialogLines; // NPC-specific dialog lines
public float typingSpeed = 0.05f; // NPC-specific typing speed

protected virtual void Start()
{
    player = GameObject.FindGameObjectWithTag("Player").transform;
    spriteRenderer = GetComponent<SpriteRenderer>();
}

private void Update()
{
    // Handle sprite flipping based on player's position
    Vector3 directionToPlayer = player.position - transform.position;
    spriteRenderer.flipX = directionToPlayer.x < 0;

    // Check for interaction input (E key) and if Dialog is NOT active
    if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !DialogManager.Instance.IsDialogActive() && !ShopManager.Instance.isShopActive)
    {
        DialogManager.Instance.StartDialog(dialogLines, typingSpeed);
    }
}

private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        isPlayerNear = true;
    }
}

protected virtual void OnTriggerExit2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        isPlayerNear = false;
        DialogManager.Instance.EndDialog();
    }
}
}
