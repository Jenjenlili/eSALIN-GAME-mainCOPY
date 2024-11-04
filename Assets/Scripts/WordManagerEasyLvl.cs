using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;

public class GameManager : MonoBehaviour
{
    private string dbPath;

    private struct WordData
    {
        public string Tagalog;
        public string English;
        public string Definition;
    }

    private List<WordData> enemyWordList = new List<WordData>();
    public TextMeshProUGUI DefinitionText;
    public TextMeshProUGUI Choice1Text;
    public TextMeshProUGUI Choice2Text;
    public TextMeshProUGUI Choice3Text;

    private void Start()
    {
        dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";
        
        // Load all unique words for enemies at the start
        LoadUniqueEnemyWords("easyWrds_tbl");

        // Example of setting up the first enemy (call this function when you spawn the enemy)
        SetupEnemy();
    }

    private void LoadUniqueEnemyWords(string tableName)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = $"SELECT tagalog, english, definition FROM {tableName}";

            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WordData wordData = new WordData
                        {
                            Tagalog = reader.GetString(0),
                            English = reader.GetString(1),
                            Definition = reader.GetString(2)
                        };

                        // Ensure each word combination is unique
                        if (!enemyWordList.Exists(w => w.Tagalog == wordData.Tagalog && w.English == wordData.English))
                        {
                            enemyWordList.Add(wordData);
                        }
                    }
                }
            }
            connection.Close();
        }
    }

    public (string tagalog, string english, string definition) GetRandomWordByLevel()
    {
        if (enemyWordList.Count == 0)
        {
            Debug.LogWarning("No more unique words available; consider loading more.");
            return (null, null, null);
        }

        // Select a random word from the list and remove it to ensure uniqueness
        int randomIndex = Random.Range(0, enemyWordList.Count);
        WordData selectedWord = enemyWordList[randomIndex];
        enemyWordList.RemoveAt(randomIndex); // Remove to ensure uniqueness for each enemy

        return (selectedWord.Tagalog, selectedWord.English, selectedWord.Definition);
    }

    // Setup method for an enemy
    public void SetupEnemy()
    {
        var (tagalog, english, definition) = GetRandomWordByLevel();

        // Ensure we have valid data before proceeding
        if (!string.IsNullOrEmpty(definition))
        {
            DefinitionText.text = definition;
        }
        else
        {
            Debug.LogWarning("Definition is empty!");
        }

        // Prepare choices
        List<string> choices = new List<string> { english }; // Start with the correct answer
        choices.Add(GetRandomEnglishWord("easyWrds_tbl")); // Add a random distractor
        choices.Add(GetRandomEnglishWord("easyWrds_tbl")); // Add another random distractor
        
        // Log choices before shuffling
        Debug.Log("Choices before shuffling: " + string.Join(", ", choices));

        // Shuffle choices to randomize their order
        choices = ShuffleList(choices);

        // Log choices after shuffling
        Debug.Log("Choices after shuffling: " + string.Join(", ", choices));

        // Set the choice texts
        Choice1Text.text = choices[0];
        Choice2Text.text = choices[1];
        Choice3Text.text = choices[2];
    }

    private string GetRandomEnglishWord(string tableName)
    {
        string randomWord = null;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = $"SELECT english FROM {tableName} ORDER BY RANDOM() LIMIT 1";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        randomWord = reader.GetString(0);
                    }
                }
            }
            connection.Close();
        }
        return randomWord;
    }

    private List<string> ShuffleList(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}       