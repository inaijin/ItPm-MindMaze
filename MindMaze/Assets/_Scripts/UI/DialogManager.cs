using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance; // Singleton instance for global access

    [Header("Dialog UI Elements")]
    [SerializeField] private Canvas dialogCanvas; // Dialog UI Canvas
    [SerializeField] private TextMeshProUGUI dialogText; // TextMeshProUGUI for dialog text
    [SerializeField] private Button continueButton; // Continue button

    [Header("Gameplay Elements")]
    [SerializeField] private GameObject weapon; // Weapon GameObject to deactivate/reactivate
    [SerializeField] private GameManager gameManager; // Reference to GameManager for cursor management

    private bool isTyping = false;
    private float typingSpeed = 0.05f; // Default typing speed
    private string[] currentDialogLines; // Current dialog lines to display
    private int currentDialogIndex = 0;

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
    }

    public void StartDialog(string[] dialogLines, float typingSpeed = 0.05f)
    {
        this.typingSpeed = typingSpeed;
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

    private void DisplayNextLine()
    {
        if (currentDialogIndex < currentDialogLines.Length)
        {
            StartCoroutine(TypeLine(currentDialogLines[currentDialogIndex]));
        }
        else
        {
            EndDialog();
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = ""; // Clear previous text
        continueButton.gameObject.SetActive(false);

        foreach (char letter in line)
        {
            dialogText.text += letter; // Append letter by letter
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        continueButton.gameObject.SetActive(true); // Enable the continue button
    }

    private void ContinueDialog()
    {
        if (!isTyping)
        {
            currentDialogIndex++;
            DisplayNextLine();
        }
    }

    public void EndDialog()
    {
        dialogCanvas.gameObject.SetActive(false); // Hide the entire dialog canvas

        // Reactivate weapon and set gameplay cursor
        if (weapon != null)
            weapon.SetActive(true);

        if (gameManager != null)
            gameManager.SetCursorIcon();
    }
}
