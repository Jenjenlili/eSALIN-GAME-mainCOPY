using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI TagalogText;
    public TextMeshProUGUI DefinitionText;
    public TextMeshProUGUI Choice1Text;
    public TextMeshProUGUI Choice2Text;
    public TextMeshProUGUI Choice3Text;

    private string dbPath;
    // Start is called before the first frame update
    void Start()
    {
        // TagalogText.text = "DI KO NA ALAM";
        // DefinitionText.text = "huhuhuhuhuhuhuhu";
        // Choice1Text.text = "yawqna";
        // Choice2Text.text = "shibal";
        // Choice3Text.text = "bosit";
 

        dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";

        // Get the correct answer and display words
        (string tagalog, string correctEnglish, string definition) = GetEasyWord();

        if (tagalog != null && correctEnglish != null && definition != null)
        {
            TagalogText.text = tagalog;
            DefinitionText.text = definition;

            // Get distractors
            List<string> choices = new List<string> { correctEnglish };
            choices.Add(GetRandomEnglishWord("easyWrds_tbl"));
            choices.Add(GetRandomEnglishWord("easyWrds_tbl"));

            // Shuffle choices and assign to choice texts
            choices = ShuffleList(choices);
            Choice1Text.text = choices[0];
            Choice2Text.text = choices[1];
            Choice3Text.text = choices[2];
        }
        else
        {
            Debug.Log("No words available for easy level.");
        }
    }

    // Method to get a random word with all columns
    private (string tagalog, string english, string definition) GetEasyWord()
    {
        return GetRandomWordByLevel("easyWrds_tbl");
    }

    // General method to get a word from the specified table with all columns
    private (string tagalog, string english, string definition) GetRandomWordByLevel(string tableName)
    {
        string tagalog = null;
        string english = null;
        string definition = null;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = $"SELECT tagalog, english, definition FROM {tableName} ORDER BY RANDOM() LIMIT 1";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tagalog = reader.GetString(0);
                        english = reader.GetString(1);
                        definition = reader.GetString(2);
                    }
                }
            }
            connection.Close();
        }
        return (tagalog, english, definition);
    }

    // Method to get a random English word from the easy level for distractors
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

    // Helper method to shuffle the list of choices
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
