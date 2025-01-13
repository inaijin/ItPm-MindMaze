using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    public float detectionRadius = 5.0f; // Radius for detecting the player
    [SerializeField] private GameObject hitBox;

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        // Find all colliders in the radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        Debug.Log($"Detected {hitColliders.Length} colliders.");

        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log($"Object in range: {collider.gameObject.name}");

            if (collider.CompareTag("Player"))
            {
                animator.SetBool("isNearPlayer", true);
                Debug.Log("Player detected!");
                return;
            }
        }

        animator.SetBool("isNearPlayer", false);
        Debug.Log("No player detected.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void EnabeleHitBox()
    {
        hitBox.SetActive(true);
    }

    public void DisableHitBox()
    {
        hitBox.SetActive(false);
    }
}
