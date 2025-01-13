using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Header("NPC Data")]
    public NpcDataSO[] npcsData; // Reference to ScriptableObject containing NPC data

    [Header("Player Interaction")]
    private Transform playerTransform;
    private Player playerComponent;
    private SpriteRenderer spriteRenderer;
    protected bool isPlayerNear = false;

    private static int globalId = 0;
    private int npcId;
    private bool isSamurai = false;

    [Header("Dialog Settings")]
    public float typingSpeed = 0.05f; // Default typing speed, can be overridden
    private string[] dialogLines; // NPC-specific dialog lines

    private EenemySpawner enemySpawner; // Reference to EnemySpawner

    private void Awake()
    {
        globalId++; // Increment global ID counter for each NPC instance
        npcId = globalId;
    }

    protected virtual void Start()
    {
        // Cache player-related references
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

        // Cache SpriteRenderer reference
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Load NPC data if available
        if (npcsData != null && npcsData.Length > 0)
        {
            NpcDataSO npcData = npcsData[npcId % npcsData.Length];
            isSamurai = (npcId % npcsData.Length == 1);
            spriteRenderer.sprite = npcData.sprite;
            dialogLines = npcData.dialogLines;
        }
        else
        {
            Debug.LogWarning($"NpcDataSO is not assigned for {gameObject.name}");
        }

        // Find and cache EnemySpawner reference
        enemySpawner = FindObjectOfType<EenemySpawner>();
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found in the scene!");
        }
    }

    private void Update()
    {
        // Flip sprite to face the player
        if (playerTransform != null)
        {
            spriteRenderer.flipX = (playerTransform.position.x < transform.position.x);
        }

        // Handle interaction input when the player is near
        if (!playerComponent.dead && isPlayerNear && Input.GetKeyDown(KeyCode.E) && !DialogManager.Instance.IsDialogActive() && !ShopManager.Instance.isShopActive)
        {
            StartInteraction();
        }
    }

    private void StartInteraction()
    {
        // Start dialog
        DialogManager.Instance.StartDialog(npcId,dialogLines, typingSpeed);

        // Perform Samurai-specific logic
        if (isSamurai && playerComponent != null)
        {
            playerComponent.markSamuraiAsSeen();
            playerComponent.updateKeyUIOWO();
        }

        // Disable enemy spawning during interaction
        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningState(false);
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
            DialogManager.Instance.EndDialog(npcId);

            // Enable enemy spawning after interaction ends
            if (enemySpawner != null)
            {
                enemySpawner.SetSpawningState(true);
            }
        }
    }
}
