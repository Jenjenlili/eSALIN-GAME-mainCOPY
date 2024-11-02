using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    // Reference to the prefab to spawn
    public GameObject prefabToSpawn;

    // Time in seconds before the prefab is destroyed
    public float destroyAfterTime = 2f;

    // Interval between prefab spawns
    public float spawnInterval = 1f;

    // Button click tracking
    private bool isButtonClicked = false;
    private float timeSinceLastSpawn = 0f;

    void Update()
    {
        if (isButtonClicked)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnInterval)
            {
                SpawnPrefab();
                timeSinceLastSpawn = 0f;
            }
        }
    }

    // Call this method when the button is clicked
    public void OnButtonClick()
    {
        isButtonClicked = !isButtonClicked;
    }

    void SpawnPrefab()
    {
        // Instantiate the prefab at the spawner's position and rotation
        GameObject spawnedObject = Instantiate(prefabToSpawn, transform.position, transform.rotation);
        // Destroy the spawned object after the specified time
        Destroy(spawnedObject, destroyAfterTime);
    }
}
