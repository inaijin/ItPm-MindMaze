using UnityEngine;
using System.Collections;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] private GameObject notificationCanvas;
    [SerializeField] private float displayDuration = 2f; // Time to show the notification

    private Coroutine activeNotificationCoroutine; // To track the currently running coroutine

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (notificationCanvas != null)
        {
            notificationCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Notification canvas is not assigned.");
        }
    }

    private void Update()
    {
        // Check for the 'E' key to cancel the notification
        if (notificationCanvas.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            EndNotification();
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationCanvas == null)
        {
            Debug.LogWarning("Notification canvas is not assigned.");
            return;
        }

        // Activate canvas and display the message
        notificationCanvas.SetActive(true);
        var messageText = notificationCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (messageText != null)
        {
            messageText.text = message;
        }

        // Cancel any active notification coroutine
        if (activeNotificationCoroutine != null)
        {
            StopCoroutine(activeNotificationCoroutine);
        }

        // Start a new coroutine to hide the notification after a delay
        activeNotificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        EndNotification();
    }

    private void EndNotification()
    {
        if (notificationCanvas != null)
        {
            notificationCanvas.SetActive(false);
        }

        // Stop the active coroutine (if any)
        if (activeNotificationCoroutine != null)
        {
            StopCoroutine(activeNotificationCoroutine);
            activeNotificationCoroutine = null;
        }
    }
}
