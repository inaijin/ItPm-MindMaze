using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Header("NPC Data")]
    public NpcDataSO[] npcsData; // Reference to the ScriptableObject containing NPC data

    [Header("Player Interaction")]
    private Transform player;
    private GameObject playerItself;
    private SpriteRenderer spriteRenderer;
    protected bool isPlayerNear = false;

    private static int id = 0;
    private bool isSamurai = false;
    private int _id;

    [Header("Dialog Settings")]
    private string[] dialogLines; // NPC-specific dialog lines
    public float typingSpeed = 0.05f; // Default typing speed, can be overridden

    private EenemySpawner enemySpawner; // Reference to EnemySpawner

    private void Awake()
    {
        id = 0;
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerItself = GameObject.FindGameObjectsWithTag("Player")[0];
        spriteRenderer = GetComponent<SpriteRenderer>();

        id++;
        _id = id;

        // Load data from the ScriptableObject
        if (npcsData != null)
        {
            NpcDataSO npcData = npcsData[_id % npcsData.Length];
            if(_id % npcsData.Length == 1) { isSamurai = true; }
            spriteRenderer.sprite = npcData.sprite; // Set the NPC's sprite
            dialogLines = npcData.dialogLines; // Load dialog lines
        }
        else
        {
            Debug.LogWarning($"NpcDataSO is not assigned for {gameObject.name}");
        }

        // Find the EnemySpawner in the scene
        enemySpawner = FindObjectOfType<EenemySpawner>();
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found in the scene!");
        }
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

            if (isSamurai) { 
                playerItself.GetComponent<Player>().markSamuraiAsSeen();
                playerItself.GetComponent<Player>().updateKeyUIOWO();
            }

            // Disable enemy spawning
            if (enemySpawner != null)
            {
                enemySpawner.SetSpawningState(false);
            }
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

            // Enable enemy spawning
            if (enemySpawner != null)
            {
                enemySpawner.SetSpawningState(true);
            }
        }
    }
}
