using UnityEngine;
using DG.Tweening;

public class Chest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private ChestNotificationData notificationData;

    [Header("Animation Settings")]
    [SerializeField] private float keyFloatDuration = 1f;
    [SerializeField] private float keyFloatHeight = 1f;
    [SerializeField] private Vector3 keyRotation = new Vector3(0, 360, 0);

    private bool isPlayerNearby = false;
    private Player player;
    private Animator animator;
    private bool isOpen = false;
    private AudioSource chestAudioSource;

    private static int idCounter = 0;
    private int chestId;

    private void Awake()
    {
        idCounter = 0; // Reset counter when the scene is loaded
    }

    private void Start()
    {
        idCounter++;
        chestId = idCounter;
        Debug.Log("Chest ID: " + chestId);

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

            // Get the notification message
            string notificationMessage = GetNotificationMessage();
            NotificationManager.Instance.ShowNotification(notificationMessage);
        }
    }

    private string GetNotificationMessage()
    {
        if (notificationData != null && notificationData.messages.Length > 0)
        {
            // Get message by ID (ensure it's within bounds)
            int index = (chestId - 1) % notificationData.messages.Length;
            return notificationData.messages[index];
        }

        return "You opened a chest, but there's no message assigned!";
    }

    private void SpawnAndAnimateKey()
    {
        GameObject key = Instantiate(keyPrefab, transform.position, Quaternion.identity);

        Sequence keySequence = DOTween.Sequence();

        keySequence.Append(key.transform.DOMoveY(transform.position.y + keyFloatHeight, keyFloatDuration)
            .SetEase(Ease.OutQuad));
        keySequence.Join(key.transform.DORotate(keyRotation, keyFloatDuration, RotateMode.FastBeyond360));

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
