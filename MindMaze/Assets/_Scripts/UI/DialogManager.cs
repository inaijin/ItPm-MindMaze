using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogManager : MonoBehaviour
{
    public event Action OnDialogEnd;
    public static DialogManager Instance; // Singleton instance for global access

    [Header("Dialog UI Elements")]
    [SerializeField] private Canvas dialogCanvas; // Dialog UI Canvas
    [SerializeField] private TextMeshProUGUI dialogText; // TextMeshProUGUI for dialog text
    [SerializeField] private Button continueButton; // Continue button

    [Header("Gameplay Elements")]
    [SerializeField] private GameObject weapon; // Weapon GameObject to deactivate/reactivate
    [SerializeField] private GameManager gameManager; // Reference to GameManager for cursor management

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // AudioSource for playing typing sound
    [SerializeField] private AudioClip typingSound; // AudioClip for typing sound
    [SerializeField] private float typingSoundInterval = 0.1f; // Time between typing sound plays
    [Range(0f, 1f)]
    [SerializeField] private float typingSoundVolume = 0.5f; // Volume of the typing sound

    private bool isTyping = false;
    private float typingSpeed = 0.05f; // Default typing speed
    private float normalTypingSpeed = 0.05f; // Store the default typing speed
    private float fastTypingSpeed = 0.025f; // 2x speed when holding Spacebar

    private string[] currentDialogLines; // Current dialog lines to display
    private int currentDialogIndex = 0;

    private bool isDialogActive = false;

    private int currentNpcId = -1;


    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initially hide the entire dialog canvas
        dialogCanvas.gameObject.SetActive(false);

        // Attach the continue button's click listener
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueDialog);
        }
        else
        {
            Debug.LogError("Continue button is not assigned in DialogManager!");
        }

        // Validate AudioSource
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned in DialogManager!");
        }
    }

    public void StartDialog(int npcId, string[] dialogLines, float typingSpeed = 0.05f)
    {
        if (isDialogActive || currentNpcId != -1) return; // Prevent starting a dialog if one is already active

        currentNpcId = npcId;
        isDialogActive = true;
        this.typingSpeed = typingSpeed;
        normalTypingSpeed = typingSpeed; // Save the normal speed
        currentDialogLines = dialogLines;
        currentDialogIndex = 0;

        dialogCanvas.gameObject.SetActive(true); // Show the entire dialog canvas

        // Disable weapon and set default cursor
        if (weapon != null)
            weapon.SetActive(false);

        if (gameManager != null)
            gameManager.SetCursorDefault();

        DisplayNextLine();
    }

    private void Update()
    {
        // Check if Space is held to increase typing speed
        if (isDialogActive && isTyping)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                typingSpeed = fastTypingSpeed; // Increase speed when holding Spacebar
                audioSource.pitch = 1.5f; // Increase pitch to simulate faster sound
            }
            else
            {
                typingSpeed = normalTypingSpeed; // Reset speed when releasing Spacebar
                audioSource.pitch = 1.0f; // Reset pitch
            }
        }

        // Check if Space is pressed to continue dialog
        if (isDialogActive && !isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            ContinueDialog();
        }
    }

    private void DisplayNextLine()
    {
        if (currentDialogIndex < currentDialogLines.Length)
        {
            StartCoroutine(TypeLine(currentDialogLines[currentDialogIndex]));
        }
        else
        {
            EndDialog(currentNpcId);
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = ""; // Clear previous text
        continueButton.gameObject.SetActive(false);

        float soundTimer = 0f; // Timer to control sound playback

        foreach (char letter in line)
        {
            if (isDialogActive)
            {
                dialogText.text += letter; // Append letter by letter

                // Play typing sound at intervals
                soundTimer += typingSpeed;
                if (soundTimer >= typingSoundInterval)
                {
                    PlayTypingSound();
                    soundTimer = 0f;
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        continueButton.gameObject.SetActive(true); // Enable the continue button
        audioSource.pitch = 1.0f; // Reset pitch when typing finishes
    }

    private void PlayTypingSound()
    {
        if (audioSource != null && typingSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(typingSound, typingSoundVolume); // Use the slider value for volume
        }
    }

    private void ContinueDialog()
    {
        if (!isTyping)
        {
            currentDialogIndex++;
            DisplayNextLine();
        }
    }

    public void EndDialog(int npcId)
    {
        if (currentNpcId != npcId) {
            return;
        }
        currentNpcId = -1;
        isDialogActive = false;
        typingSpeed = normalTypingSpeed; // Reset typing speed
        audioSource.pitch = 1.0f; // Reset pitch
        dialogCanvas.gameObject.SetActive(false); // Hide the entire dialog canvas

        // Reactivate weapon and set gameplay cursor
        if (weapon != null)
            weapon.SetActive(true);

        if (gameManager != null)
            gameManager.SetCursorIcon();

        OnDialogEnd?.Invoke(); // Trigger the dialog end event
    }

    public bool IsDialogActive()
    {
        return isDialogActive;
    }
}
