using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnpoints;
    public GameObject[] enemyPrefabs;
    public Sprite[] enemySprites;
    public Vector3[] customScales;
    public float[] spawnIntervals;
    [SerializeField]
    private int numberOfSpawns = 10;

    private bool isSpawning = true;
    private int spawnCount = 0;

    private void Start() {
        if (enemyPrefabs.Length != customScales.Length || enemyPrefabs.Length != spawnIntervals.Length) {
            Debug.LogError("The number of enemy prefabs must match the number of custom scales and spawn intervals.");
            return;
        }
        StartCoroutine(SpawnEnemiesWithIntervals());
    }

    IEnumerator SpawnEnemiesWithIntervals() {
        while (isSpawning && spawnCount < numberOfSpawns) {
            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            float spawnInterval = spawnIntervals[randomEnemyIndex];
            SpawnEnemy(randomEnemyIndex);
            spawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(int randomEnemyIndex) {
        int randomSpawnIndex = Random.Range(0, spawnpoints.Length);
        Transform spawnPoint = spawnpoints[randomSpawnIndex];
        GameObject selectedEnemyPrefab = enemyPrefabs[randomEnemyIndex];
        GameObject enemy = Instantiate(selectedEnemyPrefab, spawnPoint.position, Quaternion.identity);

        // Set sprite for the enemy
        SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
        if (renderer != null && enemySprites.Length > 0) {
            int randomSpriteIndex = Random.Range(0, enemySprites.Length);
            renderer.sprite = enemySprites[randomSpriteIndex];
        }

        // Set Tagalog text from the database
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null) {
            string tagalogWord = FindObjectOfType<GameManager>().GetRandomTagalogWord(); // Get a random Tagalog word
            enemyComponent.SetTagalogText(tagalogWord); // Set the word on the enemy
        } else {
            Debug.LogWarning("Enemy component not found on spawned enemy.");
        }

        ScaleEnemy(enemy, randomEnemyIndex);
    }

    void ScaleEnemy(GameObject enemy, int prefabIndex) {
        enemy.transform.localScale = customScales[prefabIndex];
    }

    public void StartSpawning() {
        isSpawning = true;
        StartCoroutine(SpawnEnemiesWithIntervals());
    }

    public void StopSpawning() {
        isSpawning = false;
    }
}