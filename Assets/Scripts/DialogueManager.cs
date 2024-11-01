using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Reference to your TextMeshProUGUI component
    public float typingSpeed = 0.05f; // Speed of typing effect
    public DialogueData dialogueData; // Reference to the DialogueData asset

    private int currentDialogueIndex = 0;
    private string currentText;
    private bool isTyping;
    private bool cancelTyping;

    private void Start()
    {
        dialogueText.text = "";
        if (dialogueData != null && dialogueData.dialogueTexts.Count > 0)
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        currentDialogueIndex = 0;
        ShowNextDialogue();
    }

    private void ShowNextDialogue()
    {
        if (currentDialogueIndex < dialogueData.dialogueTexts.Count)
        {
            currentText = dialogueData.dialogueTexts[currentDialogueIndex];
            dialogueText.text = "";
            cancelTyping = false;
            StartCoroutine(TypeText());
        }
        else
        {
            EndDialogue(); // No more dialogue left
        }
    }

    private IEnumerator TypeText()
    {
        isTyping = true;

        foreach (char letter in currentText.ToCharArray())
        {
            if (cancelTyping)
            {
                dialogueText.text = currentText;
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void OnClick()
    {
        if (isTyping)
        {
            cancelTyping = true; // Skip to the end of the current line
        }
        else
        {
            // Move to the next dialogue
            currentDialogueIndex++; 
            if (currentDialogueIndex < dialogueData.dialogueTexts.Count)
            {
                ShowNextDialogue();
            }
            else
            {
                EndDialogue(); // No more dialogues left
            }
        }
    }

    private void EndDialogue()
    {
        // Implement your logic for ending or progressing the dialogue
        Debug.Log("Dialogue ended or progressed.");
        dialogueText.text = ""; // Clear the text when dialogue ends
    }
}
