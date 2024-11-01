using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Import for scene management

public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    private float units;

    [SerializeField]
    private GameObject Prefab;

    [SerializeField]
    private Image fill;

    [SerializeField]
    private string nextSceneName; // Serialized field for the next scene's name

    private float fillAmount;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BuildUnits());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBar();
    }

    public IEnumerator BuildUnits()
    {
        for (int i = 0; i <= units; i++)
        {
            fillAmount = i / units;
            Instantiate(Prefab);
            yield return null;
        }
    }

    private void UpdateBar()
    {
        fill.fillAmount = fillAmount;

        // Check if the loading bar is full
        if (fillAmount >= 1f)
        {
            ChangeScene(); // Call to change scene
        }
    }

    // Method to change scenes
    private void ChangeScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName)) // Check if nextSceneName is set
        {
            SceneManager.LoadScene(nextSceneName); // Load the scene specified in the Inspector
        }
        else
        {
            Debug.LogWarning("Next scene name not set in the Inspector.");
        }
    }
}
