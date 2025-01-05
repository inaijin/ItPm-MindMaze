using UnityEngine;
using System.Collections;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] private GameObject notificationCanvas;
    [SerializeField] private float displayDuration = 2f; // Time to show the notification

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (notificationCanvas != null)
        {
            notificationCanvas.SetActive(false);
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
        // Assuming the canvas has a child Text or TMP_Text component for displaying messages
        var messageText = notificationCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (messageText != null)
        {
            messageText.text = message;
        }

        // Start a coroutine to hide the canvas after a delay
        StartCoroutine(HideNotificationAfterDelay());
    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (notificationCanvas != null)
        {
            notificationCanvas.SetActive(false);
        }
    }
}
