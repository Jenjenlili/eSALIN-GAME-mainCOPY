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
    public GameManager gameManager;

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
        enemy.gameObject.tag = "Enemy";

        // Set sprite for the enemy
        SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
        if (renderer != null && enemySprites.Length > 0) {
            int randomSpriteIndex = Random.Range(0, enemySprites.Length);
            renderer.sprite = enemySprites[randomSpriteIndex];
        }

        //  // Get the Enemy component to set the Tagalog word
        // Enemy enemyComponent = enemy.GetComponent<Enemy>();
        // if (enemyComponent != null && gameManager != null) {
        //     // Set the Tagalog word and choices for this enemy
        //     gameManager.SetupEnemyTagalogText(enemyComponent);
        // }
        // else {
        //     Debug.LogWarning("Enemy component or GameManager not found.");
        // }

        // Get the Enemy component to set the Tagalog word
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null && gameManager != null) {
            // Set the Tagalog word for this enemy
            gameManager.SetupEnemy(enemyComponent);
        }
        else {
            Debug.Log("Enemy component or GameManager not found.");
        }

        // PROBLEM: TAGALOG WORD IS SAME EACH ENEMY
        //Get the Enemy component (no need to set the Tagalog word here)
        // Enemy enemyComponent = enemy.GetComponent<Enemy>();
        // if (enemyComponent != null) {
        //     enemyComponent.SetTagalogText(gameManager.GetCurrentTagalogWord()); // Set the current Tagalog word
        // }


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