// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class EnemySpawner : MonoBehaviour
// {
//     public Transform[] spawnpoints;        // Array of spawn points
//     public GameObject[] enemyPrefabs;      // Array of enemy prefabs to spawn different types of enemies
//     public Sprite[] enemySprites;          // Array of sprites for the enemies
//     public Vector3[] customScales;         // Array of custom scales for each enemy prefab
//     public float[] spawnIntervals;         // Array of custom spawn intervals for each enemy prefab

//     public TextMeshProUGUI Text;

//     [SerializeField]
//     private int numberOfSpawns = 10;       // Number of enemies to spawn

//     private bool isSpawning = true;        // Control spawning state
//     private int spawnCount = 0;            // Tracks the number of enemies spawned

//     private void Start() {
//         // Ensure the array sizes match
//         if (enemyPrefabs.Length != customScales.Length || enemyPrefabs.Length != spawnIntervals.Length) {
//             Debug.LogError("The number of enemy prefabs must match the number of custom scales and spawn intervals.");
//             return;
//         }

//         // Start spawning
//         StartCoroutine(SpawnEnemiesWithIntervals());

//         Text.text = "Text";
//     }

//     IEnumerator SpawnEnemiesWithIntervals() {
//         while (isSpawning && spawnCount < numberOfSpawns) {
//             // Choose a random enemy prefab and corresponding spawn interval
//             int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
//             float spawnInterval = spawnIntervals[randomEnemyIndex];

//             // Spawn the enemy
//             SpawnEnemy(randomEnemyIndex);
//             spawnCount++; // Increase the spawn count

//             // Wait for the next spawn based on the prefab's spawn interval
//             yield return new WaitForSeconds(spawnInterval);
//         }
//     }

//     void SpawnEnemy(int randomEnemyIndex) {
//         // Choose a random spawn point
//         int randomSpawnIndex = Random.Range(0, spawnpoints.Length);
//         Transform spawnPoint = spawnpoints[randomSpawnIndex];

//         // Get the enemy prefab
//         GameObject selectedEnemyPrefab = enemyPrefabs[randomEnemyIndex];

//         // Spawn the enemy at the selected spawn point
//         GameObject enemy = Instantiate(selectedEnemyPrefab, spawnPoint.position, Quaternion.identity);

//         // Assign a random sprite from the array if needed
//         SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
//         if (renderer != null && enemySprites.Length > 0) {
//             int randomSpriteIndex = Random.Range(0, enemySprites.Length);
//             renderer.sprite = enemySprites[randomSpriteIndex];
//         }

//         // Check for TextMeshPro component and activate it if found
//         TextMeshPro textMeshPro = enemy.GetComponentInChildren<TextMeshPro>();
//         if (textMeshPro != null) {
//             textMeshPro.gameObject.SetActive(true);
//             textMeshPro.text = "Enemy Spawned!";
//             textMeshPro.sortingOrder = 1;

//             // Set position and scale to ensure visibility (optional)
//             textMeshPro.transform.position = spawnPoint.position + new Vector3(0, 0.5f, 0);
//             textMeshPro.transform.localScale = Vector3.one;
//             textMeshPro.fontSize = 5;
//         } else {
//             Debug.LogWarning("TextMeshPro component not found in spawned enemy.");
//         }

//         // Scale the enemy based on its corresponding custom scale
//         ScaleEnemy(enemy, randomEnemyIndex);
//     }

//     // Method to scale the spawned enemy based on the specific prefab index
//     void ScaleEnemy(GameObject enemy, int prefabIndex) {
//         // Set the enemy's scale to the corresponding custom scale for that prefab
//         enemy.transform.localScale = customScales[prefabIndex];
//     }

//     // Method to start spawning
//     public void StartSpawning() {
//         isSpawning = true;
//         StartCoroutine(SpawnEnemiesWithIntervals());
//     }

//     // Method to stop spawning
//     public void StopSpawning() {
//         isSpawning = false;
//     }
// }

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

    public TextMeshProUGUI Text;

    [SerializeField]
    private int numberOfSpawns = 10;       // Number of enemies to spawn

    private bool isSpawning = true;        // Control spawning state
    private int spawnCount = 0;            // Tracks the number of enemies spawned

    private void Start() {
        // Ensure the array sizes match
        if (enemyPrefabs.Length != customScales.Length || enemyPrefabs.Length != spawnIntervals.Length) {
            Debug.LogError("The number of enemy prefabs must match the number of custom scales and spawn intervals.");
            return;
        }

        // Start spawning
        StartCoroutine(SpawnEnemiesWithIntervals());

        Text.text = "Text";
    }

    IEnumerator SpawnEnemiesWithIntervals() {
        while (isSpawning && spawnCount < numberOfSpawns) {
            // Choose a random enemy prefab and corresponding spawn interval
            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            float spawnInterval = spawnIntervals[randomEnemyIndex];

            // Spawn the enemy
            SpawnEnemy(randomEnemyIndex);
            spawnCount++; // Increase the spawn count

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
            renderer.sprite = enemySprites[randomSpriteIndex];
        }

        // Check for TextMeshPro component and activate it if found
        TextMeshPro textMeshPro = enemy.GetComponentInChildren<TextMeshPro>();
        if (textMeshPro != null) {
            textMeshPro.gameObject.SetActive(true);           // Ensure the TMPro component is active
            textMeshPro.text = "Enemy Spawned!";              // Set the text content
            textMeshPro.alignment = TextAlignmentOptions.Center; // Center-align the text
            textMeshPro.fontSize = 5;                         // Set a readable font size
            textMeshPro.sortingOrder = 1;                     // Ensure text renders above other elements

            // Position the text just above the enemy
            textMeshPro.transform.position = spawnPoint.position + new Vector3(0, 0.5f, 0);
            textMeshPro.transform.localScale = Vector3.one;
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
