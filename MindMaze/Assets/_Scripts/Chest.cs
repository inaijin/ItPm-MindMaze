using UnityEngine;
using DG.Tweening;

public class Chest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject keyPrefab;

    [Header("Animation Settings")]
    [SerializeField] private float keyFloatDuration = 1f;
    [SerializeField] private float keyFloatHeight = 1f;
    [SerializeField] private Vector3 keyRotation = new Vector3(0, 360, 0);

    private bool isPlayerNearby = false;
    private Player player;
    private Animator animator;
    private bool isOpen = false;
    private AudioSource chestAudioSource;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        chestAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (player != null)
        {
            if (chestAudioSource != null)
            {
                chestAudioSource.Play();
            }
            animator.SetTrigger("Open");
            isOpen = true;
            SpawnAndAnimateKey();
        }
    }

    private void SpawnAndAnimateKey()
    {
        // Spawn the key at chest position
        GameObject key = Instantiate(keyPrefab, transform.position, Quaternion.identity);

        // Create a sequence of animations
        Sequence keySequence = DOTween.Sequence();

        // First float up and rotate
        keySequence.Append(key.transform.DOMoveY(transform.position.y + keyFloatHeight, keyFloatDuration)
            .SetEase(Ease.OutQuad));
        keySequence.Join(key.transform.DORotate(keyRotation, keyFloatDuration, RotateMode.FastBeyond360));

        // When complete, destroy the key object and update the player's key count
        keySequence.OnComplete(() => {
            player.FindKey();
            Destroy(key);
            Debug.Log("Chest opened and key added!");
        });
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