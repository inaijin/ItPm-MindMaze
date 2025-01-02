using UnityEngine;

public class ShopNpc : Npc
{
    protected override void Start()
    {
        base.Start();
        // Subscribe to the OnDialogEnd event
        DialogManager.Instance.OnDialogEnd += HandleDialogEnd;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnDialogEnd event to avoid memory leaks
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnDialogEnd -= HandleDialogEnd;
        }
    }

    private void HandleDialogEnd()
    {
        // Check if the player just finished interacting with this ShopNpc
        if (isPlayerNear) 
        {
            ShopManager.Instance?.OpenShop();
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (collision.CompareTag("Player"))
        {
            // Close the shop UI when the player exits the interaction range
            ShopManager.Instance?.CloseShop();
        }
    }
}
