﻿using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [Header("NPC Data")]
    public NpcDataSO[] npcsData; // Reference to the ScriptableObject containing NPC data

    [Header("Player Interaction")]
    private Transform player;
    private SpriteRenderer spriteRenderer;
    protected bool isPlayerNear = false;

    private static int id = 0;
    private int _id;

    [Header("Dialog Settings")]
    private string[] dialogLines; // NPC-specific dialog lines
    public float typingSpeed = 0.05f; // Default typing speed, can be overridden

    private void Awake()
    {
        id = 0;
    }
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        id++;
        _id = id;

        // Load data from the ScriptableObject
        if (npcsData != null)
        {
            NpcDataSO npcData = npcsData[_id % npcsData.Length];
            spriteRenderer.sprite = npcData.sprite; // Set the NPC's sprite
            dialogLines = npcData.dialogLines; // Load dialog lines
        }
        else
        {
            Debug.LogWarning($"NpcDataSO is not assigned for {gameObject.name}");
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
