using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";
        
        // Load the easy level word on start
        (string correctEnglish, string definition) = GetEasyWord();
        
        if (correctEnglish != null && definition != null) {
            DefinitionText.text = definition;

            List<string> choices = new List<string> { correctEnglish };
            choices.Add(GetRandomEnglishWord("easyWrds_tbl"));
            choices.Add(GetRandomEnglishWord("easyWrds_tbl"));

            choices = ShuffleList(choices);
            Choice1Text.text = choices[0];
            Choice2Text.text = choices[1];
            Choice3Text.text = choices[2];
        } else {
            Debug.Log("No words available for easy level.");
        }
    }

    private (string english, string definition) GetEasyWord()
    {
        return GetRandomWordByLevel("easyWrds_tbl");
    }

    private (string english, string definition) GetRandomWordByLevel(string tableName)
    {
        string english = null;
        string definition = null;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = $"SELECT english, definition FROM {tableName} ORDER BY RANDOM() LIMIT 1";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        english = reader.GetString(0);
                        definition = reader.GetString(1);
                    }
                }
            }
            connection.Close();
        }
        return (english, definition);
    }

    // Method to get a Tagalog word only, for assigning to enemies
    public string GetRandomTagalogWord()
    {
        string tagalog = null;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = "SELECT tagalog FROM easyWrds_tbl ORDER BY RANDOM() LIMIT 1";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tagalog = reader.GetString(0);
                    }
                }
            }
            connection.Close();
        }
        return tagalog;
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