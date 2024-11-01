using UnityEngine;
using TMPro;

public class TextMeshProSortingFix : MonoBehaviour
{
    public Renderer spriteRenderer; // Reference to the SpriteRenderer
    public int sortingOrderOffset = 1; // Sorting order difference

    private TextMeshPro textMeshPro;

    void Start()
    {
        // Get the TextMeshPro component
        textMeshPro = GetComponent<TextMeshPro>();

        // Set the sorting layer and sorting order
        if (spriteRenderer != null && textMeshPro != null)
        {
            Renderer textRenderer = textMeshPro.GetComponent<Renderer>();

            // Make sure both use the same sorting layer
            textRenderer.sortingLayerID = spriteRenderer.sortingLayerID;

            // Make sure TextMeshPro is in front of the sprite
            textRenderer.sortingOrder = spriteRenderer.sortingOrder + sortingOrderOffset;
        }
    }
}
