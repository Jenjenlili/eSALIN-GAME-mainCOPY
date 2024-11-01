using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnpoints;        // Array of spawn points
    public GameObject[] enemyPrefabs;      // Array of enemy prefabs to spawn different types of enemies
    public Sprite[] enemySprites;          // Array of sprites for the enemies
    public Vector3[] customScales;         // Array of custom scales for each enemy prefab
    public float[] spawnIntervals;         // Array of custom spawn intervals for each enemy prefab

    private bool isSpawning = true;        // Control spawning state

    private void Start() {
        // Ensure the array sizes match
        if (enemyPrefabs.Length != customScales.Length || enemyPrefabs.Length != spawnIntervals.Length) {
            Debug.LogError("The number of enemy prefabs must match the number of custom scales and spawn intervals.");
            return;
        }

        // Start spawning
        StartCoroutine(SpawnEnemiesWithIntervals());
    }

    IEnumerator SpawnEnemiesWithIntervals() {
        while (isSpawning) {
            // Choose a random enemy prefab and corresponding spawn interval
            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            float spawnInterval = spawnIntervals[randomEnemyIndex];

            // Spawn the enemy
            SpawnEnemy(randomEnemyIndex);

            // Wait for the next spawn based on the prefab's spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(int randomEnemyIndex) {
        // Choose a random spawn point
        int randomSpawnIndex = Random.Range(0, spawnpoints.Length);
        Transform spawnPoint = spawnpoints[randomSpawnIndex];

        // Get the enemy prefab
        GameObject selectedEnemyPrefab = enemyPrefabs[randomEnemyIndex];

        // Spawn the enemy at the selected spawn point
        GameObject enemy = Instantiate(selectedEnemyPrefab, spawnPoint.position, Quaternion.identity);

        // Assign a random sprite from the array if needed
        SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
        if (renderer != null && enemySprites.Length > 0) {
            int randomSpriteIndex = Random.Range(0, enemySprites.Length);
            renderer.sprite = enemySprites[randomSpriteIndex];  // Assign a random sprite
        }

        // Check for TextMeshPro component and activate it if found
        TextMeshPro textMeshPro = enemy.GetComponentInChildren<TextMeshPro>();
        if (textMeshPro != null) {
            textMeshPro.gameObject.SetActive(true);  // Ensure it's active
            textMeshPro.text = "Enemy Spawned!";     // Example text to show on spawn
            textMeshPro.sortingOrder = 1;            // Adjust sorting order to ensure visibility

            // Debugging information
            Debug.Log("TextMeshPro component found and activated for spawned enemy.");
            Debug.Log($"TextMeshPro Position: {textMeshPro.transform.position}");
            Debug.Log($"TextMeshPro Local Scale: {textMeshPro.transform.localScale}");
            Debug.Log($"TextMeshPro Font Size: {textMeshPro.fontSize}");

            // Set position and scale to ensure visibility (optional)
            textMeshPro.transform.position = spawnPoint.position + new Vector3(0, 0.5f, 0); // Adjust as needed
            textMeshPro.transform.localScale = Vector3.one;   // Reset scale for testing visibility
            textMeshPro.fontSize = 5; // Set a larger font size to make it more visible
        } else {
            Debug.LogWarning("TextMeshPro component not found in spawned enemy.");
        }

        // Scale the enemy based on its corresponding custom scale
        ScaleEnemy(enemy, randomEnemyIndex);
    }

    // Method to scale the spawned enemy based on the specific prefab index
    void ScaleEnemy(GameObject enemy, int prefabIndex) {
        // Set the enemy's scale to the corresponding custom scale for that prefab
        enemy.transform.localScale = customScales[prefabIndex];
    }

    // Method to start spawning
    public void StartSpawning() {
        isSpawning = true;
        StartCoroutine(SpawnEnemiesWithIntervals());
    }

    // Method to stop spawning
    public void StopSpawning() {
        isSpawning = false;
    }
}
