//PROBLEM: DEFINITION AND CHOICES CHANGING EACH SPAWN (THE BEST CODE SO FAR)
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Data;
// using Mono.Data.Sqlite;
// using System;

// public class GameManager : MonoBehaviour
// {
//     private string dbPath;

//     private struct WordData
//     {
//         public int wordID;
//         public string Tagalog;
//         public string English;
//         public string Definition;
//     }

//     private List<WordData> enemyWordList = new List<WordData>();
//     public TextMeshProUGUI DefinitionText;
//     public TextMeshProUGUI Choice1Text;
//     public TextMeshProUGUI Choice2Text;
//     public TextMeshProUGUI Choice3Text;

//     private string correctAnswer;
//     static List<Enemy> enemies = new List<Enemy>(); // Track active enemies
//     private bool isFirstEnemy = true;
//     private bool isNextQuestion = false;
//     private void Start()
//     {
//         dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";
//         LoadUniqueEnemyWords("easyWrds_tbl");
//         // // Dynamically add listeners to pass the choice text when each button is clicked
//         // Choice1Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice1Text.text));
//         // Choice2Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice2Text.text));
//         // Choice3Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice3Text.text)); 

//     }
//     // private void Update()
//     // {
//     //     if (isFirstEnemy)
//     //     {   
//     //         Enemy currentEnemy = enemies[0];
//     //         Debug.Log("Current Enemy: " + currentEnemy.GetID()); 
//     //         SetupQuestion(currentEnemy);
//     //         isFirstEnemy = false;
//     //     }else if (isNextQuestion)
//     //     {
//     //         SetupQuestion(enemies[0]);
//     //         Debug.Log("Current Enemy: " + enemies[0].GetID());
//     //     }
//     // }

//     private void LoadUniqueEnemyWords(string tableName)
//     {
//         enemies.Clear();
//         using (var connection = new SqliteConnection(dbPath))
//         {
//             connection.Open();
//             string query = $"SELECT easy_ID, tagalog, english, definition FROM {tableName} ORDER BY RANDOM()";

//             using (var command = new SqliteCommand(query, connection))
//             {
//                 using (IDataReader reader = command.ExecuteReader())
//                 {
//                     while (reader.Read())
//                     {
//                         WordData wordData = new WordData
//                         {
//                             wordID = reader.GetInt32(0),
//                             Tagalog = reader.GetString(1),
//                             English = reader.GetString(2),
//                             Definition = reader.GetString(3)
//                         };

//                         if (!enemyWordList.Exists(w => w.Tagalog == wordData.Tagalog && w.English == wordData.English))
//                         {
//                             enemyWordList.Add(wordData);
//                         }
//                     }
//                 }
//             }
//             connection.Close();
//         }
//     }

//     // Setup method for an enemy to set Tagalog word
//     public void SetupEnemy(Enemy enemy)
//     {
//         if (enemyWordList.Count == 0)
//         {
//             Debug.LogWarning("No more words available.");
//             return;
//         }

//         WordData wordData = enemyWordList[0];
//         enemyWordList.RemoveAt(0);
//         enemy.SetDefinition(wordData.Definition); // Set the definition in the enemy
//         enemy.SetTagalogText(wordData.Tagalog);
//         enemy.SetID(wordData.wordID);
//         enemy.SetEnglish(wordData.English);
//         //correctAnswer = wordData.English; // Save the correct answer

//         // Track the active enemy
//         if (!enemies.Contains(enemy))
//         {
//             enemies.Add(enemy);
//         }

//         // Set the definition and choices only for the first enemy
//         if (isFirstEnemy)
//         {   
//             Enemy currentEnemy = enemies[0];
//             Debug.Log("Current Enemy: " + currentEnemy.GetID()); 
//             SetupQuestion(currentEnemy);
//             isFirstEnemy = false;
//         }else if (isNextQuestion)
//         {
//             SetupQuestion(enemies[0]);
//             Debug.Log("Current Enemy: " + enemies[0].GetID());
//         }
//     }

//     // Setup method to set definition and choices after an enemy is set
//     public void SetupQuestion(Enemy enemy)
//     { 
//         string definition = enemy.GetDefinition();
//         DefinitionText.text = definition;
        
//         correctAnswer = enemy.GetEnglish();
//         //SET UP CHOICE
//         List<string> choices = new List<string> { correctAnswer };

//         // Add two unique distractors
//         while (choices.Count < 3)
//         {
//             string distractor = GetRandomEnglishWord("easyWrds_tbl");
//             if (!choices.Contains(distractor))
//             {
//                 choices.Add(distractor);
//             }
//         }

//         // Shuffle choices to randomize order
//         choices = ShuffleList(choices);

//         // Set the choice texts
//         Choice1Text.text = choices[0];
//         Choice2Text.text = choices[1];
//         Choice3Text.text = choices[2];
//         Debug.Log("Choices set to: " + Choice1Text.text + ", " + Choice2Text.text + ", " + Choice3Text.text);

//     }

//     // Check the selected choice
//     public void OnChoiceSelected(string selectedChoice)
//     {
//         Debug.Log("Something was Selected");
//         var enm =GameObject.FindGameObjectsWithTag("Enemy");
//         Debug.Log("Current number of enemies with tag Enemy:" + enm.Length);
//         Debug.Log("Current number of enemies in List: "+ enemies.Count);
//         Debug.Log("Button clicked: " + selectedChoice); // check if the button registers correctly

//         //Debug.Log(OnChoiceSelected);
//         if (enemies.Count > 0)
//         {
//             Enemy currentEnemy = enemies[0]; // Get the current enemy
//             //enemies.RemoveAt(0); // Remove the current enemy from the list
//             //Destroy(currentEnemy.gameObject); // Destroy the current enemy's GameObject
//         }
//         if (selectedChoice == correctAnswer)
//         {
//             ProceedToNextWord();
//         }
//         else
//         {
//             HandleIncorrectAnswer();
//         }
//     }

//     private void ProceedToNextWord()
//     {
//         if (enemies.Count > 0)
//         {
//             Enemy currentEnemy = enemies[0]; // Get the current enemy
//             enemies.RemoveAt(0); // Remove the current enemy from the list
            
//             // Set up the next enemy with a new word
//             SetupEnemy(currentEnemy);
//             SetupQuestion(currentEnemy); // Pass the definition for the UI
//         }
//     }

//     private void HandleIncorrectAnswer()
//     {
//         if (enemies.Count > 0)
//         {
//             Enemy currentEnemy = enemies[0]; // Get the current enemy
//             enemies.RemoveAt(0); // Remove the current enemy from the list
            
//             // Set up the next enemy with a new word
//             SetupEnemy(currentEnemy);
//             SetupQuestion(currentEnemy); // Pass the definition for the UI
//         }
//         // Logic to handle incorrect answer (e.g., notify player, destroy enemy, etc.)
//         Debug.Log("Incorrect answer selected.");
//     }

//     private string GetRandomEnglishWord(string tableName)
//     {
//         string randomWord = null;
//         using (var connection = new SqliteConnection(dbPath))
//         {
//             connection.Open();
//             string query = $"SELECT english FROM {tableName} ORDER BY RANDOM() LIMIT 1";
//             using (var command = new SqliteCommand(query, connection))
//             {
//                 using (IDataReader reader = command.ExecuteReader())
//                 {
//                     if (reader.Read())
//                     {
//                         randomWord = reader.GetString(0);
//                     }
//                 }
//             }
//             connection.Close();
//         }
//         return randomWord;
//     }

//     // Helper method to shuffle a list of strings
//     private List<string> ShuffleList(List<string> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             string temp = list[i];
//             int randomIndex = UnityEngine.Random.Range(i, list.Count);
//             list[i] = list[randomIndex];
//             list[randomIndex] = temp;
//         }
//         return list;
//     }
// }



//-------------------------------------------------------------------------------------------------------
//MAS UPDATED VERSION

//FINAAAALLLL ATLEAST NALALARO KASO MAY MALI PA YAWQNA
//WRKING NA PERO NAGDODOBLE COUNT 1 SA CURRENTT ENEMY

// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Data;
// using Mono.Data.Sqlite;
// using System;

// public class GameManager : MonoBehaviour
// {
//     private string dbPath;

//     private struct WordData
//     {
//         public int wordID;
//         public string Tagalog;
//         public string English;
//         public string Definition;
//     }

//     private List<WordData> enemyWordList = new List<WordData>();
//     public TextMeshProUGUI DefinitionText;
//     public TextMeshProUGUI Choice1Text;
//     public TextMeshProUGUI Choice2Text;
//     public TextMeshProUGUI Choice3Text;

//     static string correctAnswer;
//     static List<int> enemies = new List<int>(); // Track enemies
//     static List<string> english = new List<string>(); // Track english correct answer
//     static List<string> definition = new List<string>(); // Track
//     static List<string> choices = new List<string>();
//     private bool isFirstEnemy = true;
//     public int currentEnemy;
//     private void Start()
//     {
//         dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabaseFinal.db";
//         LoadUniqueEnemyWords("easyWrds_tbl");
//         // enemies.Clear();
//         // english.Clear();
//         // definition.Clear();
//         // Dynamically add listeners to pass the choice text when each button is clicked
//         Choice1Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice1Text.text));
//         Choice2Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice2Text.text));
//         Choice3Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice3Text.text)); 
//     }

//     private void LoadUniqueEnemyWords(string tableName)
//     {
//         enemies.Clear();
//         english.Clear();
//         definition.Clear();
        
//         using (var connection = new SqliteConnection(dbPath))
//         {
//             connection.Open();
//             string query = $"SELECT easy_ID, tagalog, english, definition FROM {tableName} ORDER BY RANDOM()";

//             using (var command = new SqliteCommand(query, connection))
//             {
//                 using (IDataReader reader = command.ExecuteReader())
//                 {
//                     while (reader.Read())
//                     {
//                         WordData wordData = new WordData
//                         {
//                             wordID = reader.GetInt32(0),
//                             Tagalog = reader.GetString(1),
//                             English = reader.GetString(2),
//                             Definition = reader.GetString(3)
//                         };

//                         if (!enemyWordList.Exists(w => w.Tagalog == wordData.Tagalog && w.English == wordData.English))
//                         {
//                             enemyWordList.Add(wordData);
//                         }
//                     }
//                 }
//             }
//             connection.Close();
//         }
//     }

//     // Setup method for an enemy to set Tagalog word
//     public void SetupEnemy(Enemy enemy)
//     {
//         if (enemyWordList.Count == 0)
//         {
//             Debug.LogWarning("No more words available.");
//             return;
//         }

//         WordData wordData = enemyWordList[0];
//         enemyWordList.RemoveAt(0);
//         // enemy.SetDefinition(wordData.Definition); // Set the definition in the enemy
//         enemy.SetTagalogText(wordData.Tagalog);
//         enemy.SetID(wordData.wordID);
//         enemies.Add(wordData.wordID); 
//         english.Add(wordData.English); // Save the correct answer
//         definition.Add(wordData.Definition);
//         // Track enemylist added
//         for(int i = 0; i < enemies.Count; i++)
//         {
//             Debug.Log("Added enemy: " + enemies[i]);
//         }
        
//         // Set the definition and choices only for the first enemy
//         if (isFirstEnemy)
//         {
//             SetupChoicesAndDefinition(definition[0]);
//             isFirstEnemy = false;
//             Debug.Log("firstEnemy");
//         }
//         //enemyWordList.RemoveAt(0);//Only remove after setup
//         Debug.Log("Current number of enemies in List: "+ enemies.Count);
//         Debug.Log("Current number of Word in List: "+ enemyWordList.Count);
//     }

//     // Setup method to set definition and choices after an enemy is set
//     public void SetupChoicesAndDefinition(string definition)
//     {
//         DefinitionText.text = definition;
//         Debug.Log("definition: " + definition);
//         SetupChoices();
        
//     }

//     // Set up choices for the UI based on the correct answer
//     private void SetupChoices()
//     {
//         choices.Clear();
//         correctAnswer = english[0];
//         choices.Add(correctAnswer);
//         Debug.Log("correctAnswer: " + correctAnswer);
        
//         // Add two unique distractors
//         while (choices.Count < 3)
//         {
//             string distractor = GetRandomEnglishWord("easyWrds_tbl");
//             if (!choices.Contains(distractor))
//             {
//                 choices.Add(distractor);
//             }
//         }

//         // Shuffle choices to randomize order
//         choices = ShuffleList(choices);

//         // Set the choice texts
//         Choice1Text.text = choices[0];
//         Choice2Text.text = choices[1];
//         Choice3Text.text = choices[2];
//     }

//     // Check the selected choice
//     public void OnChoiceSelected(string selectedChoice)
//     {
//         Debug.Log("Something was Selected");
//         // var enm =GameObject.FindGameObjectsWithTag("Enemy");
//         // Debug.Log("Current number of enemies with tag Enemy:" + enm.Length);
//         // Debug.Log("Current number of enemies in List: "+ enemies.Count);
//         Debug.Log("selected choice (onclick): " + selectedChoice);
        
        
//         if (enemies.Count > 0)
//         {
//             // currentEnemy = enemies[0]; // Get the current enemy
//             // Debug.Log("OnClick Current enemy: " + currentEnemy);
//             // Debug.Log("Choice: " + english[0]);
//             // Debug.Log("CorrectChoice: " + correctAnswer);
//             // Debug.Log("selected choice (condition): " + selectedChoice);
//             //enemies.RemoveAt(0); // Remove the current enemy from the list
//             //Destroy(currentEnemy.gameObject); // Destroy the current enemy's GameObject
//             if (selectedChoice == correctAnswer)
//             {
//                 Debug.Log("Correct answer");
//                 ProceedToNextWord();
//                 DestroyCurrentEnemy();
    
//             } else
//             {
//                 Debug.Log("Incorrect answer");
//                 HandleIncorrectAnswer();
//                 DestroyCurrentEnemy();
//             }
//         } 
//     }
//     private void DestroyCurrentEnemy()
// {
//     if (enemies.Count > 0)
//     {
//         int enemyID = enemies[0]; // Get the ID of the current enemy
//         Enemy enemyToDestroy = null;

//         // Find the enemy GameObject with the matching ID
//         foreach (Enemy enemy in FindObjectsOfType<Enemy>())
//         {
//             if (enemy.ID == enemyID)
//             {
//                 enemyToDestroy = enemy;
//                 break;
//             }
//         }

//         if (enemyToDestroy != null)
//         {
//              // Remove the current enemy from the list
//             Destroy(enemyToDestroy.gameObject); // Destroy the current enemy's GameObject
//             enemies.RemoveAt(0);
//             currentEnemy = 0;
//         }
//         else
//         {
//             Debug.LogWarning("Enemy with ID " + enemyID + " not found.");
//         }
//     }
// }

//     private void ProceedToNextWord()
//     {
//         Debug.Log("Proceed to next word Activated");

//         if (enemies.Count > 0 && english[0] != null && definition[0] != null)
//         {
//             //currentEnemy = enemies[0]; // Get the current enemy
//             //enemies.RemoveAt(0); // Remove the current enemy from the list
//             english.RemoveAt(0);
//             definition.RemoveAt(0);
//             Debug.Log("Current enemy removed");
            
//             //SetupChoicesAndDefinition(definition[0]);

//              // Ensure we have the updated definition and correct answer for the new enemy
//             if (definition.Count > 0)
//             {
//                 SetupChoicesAndDefinition(definition[0]); // Display new definition and choices
//                 Debug.Log("Next Word definition and choices Activated");
//             }
//             // Set up the next enemy with a new word
//             // SetupEnemyTagalogText(currentEnemy);
//             // SetupChoicesAndDefinition(currentEnemy.GetDefinition()); // Pass the definition for the UI
//             // Destroy(currentEnemy.gameObject);
//         }
//         else{
//             Debug.Log("Next Word Failed");
//         }
//     }

//     private void HandleIncorrectAnswer()
//     {
//         Debug.Log("Handle Incorrect Answer Activated");

//         if (enemies.Count > 0 && english[0] != null && definition[0] != null)
//         {
//             //currentEnemy = enemies[0]; // Get the current enemy
//             //enemies.RemoveAt(0); // Remove the current enemy from the list
//             english.RemoveAt(0);
//             definition.RemoveAt(0);
//             Debug.Log("Current enemy removed");
            
//             //SetupChoicesAndDefinition(definition[0]);

//              // Ensure we have the updated definition and correct answer for the new enemy
//             if (definition.Count > 0)
//             {
//                 SetupChoicesAndDefinition(definition[0]); // Display new definition and choices
//                 Debug.Log("Next Word definition and choices Activated");
//             }
//             // Set up the next enemy with a new word
//             // SetupEnemyTagalogText(currentEnemy);
//             // SetupChoicesAndDefinition(currentEnemy.GetDefinition()); // Pass the definition for the UI
//             // Destroy(currentEnemy.gameObject);
//         }
//         else{
//             Debug.Log("Next Word Failed");
//         }
//     }

//     private string GetRandomEnglishWord(string tableName)
//     {
//         string randomWord = null;
//         using (var connection = new SqliteConnection(dbPath))
//         {
//             connection.Open();
//             string query = $"SELECT english FROM {tableName} ORDER BY RANDOM() LIMIT 1";
//             using (var command = new SqliteCommand(query, connection))
//             {
//                 using (IDataReader reader = command.ExecuteReader())
//                 {
//                     if (reader.Read())
//                     {
//                         randomWord = reader.GetString(0);
//                     }
//                 }
//             }
//             connection.Close();
//         }
//         return randomWord;
//     }

//     // Helper method to shuffle a list of strings
//     private List<string> ShuffleList(List<string> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             string temp = list[i];
//             int randomIndex = UnityEngine.Random.Range(i, list.Count);
//             list[i] = list[randomIndex];
//             list[randomIndex] = temp;
//         }
//         return list;
//     }
// }


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;
using System;

public class GameManager : MonoBehaviour
{
    private string dbPath;

    private struct WordData
    {
        public int wordID;
        public string Tagalog;
        public string English;
        public string Definition;
    }

    private List<WordData> enemyWordList = new List<WordData>();
    public TextMeshProUGUI DefinitionText;
    public TextMeshProUGUI Choice1Text;
    public TextMeshProUGUI Choice2Text;
    public TextMeshProUGUI Choice3Text;

    static string correctAnswer;
    static List<int> enemies = new List<int>(); // Track enemies
    static List<string> english = new List<string>(); // Track english correct answer
    static List<string> definition = new List<string>(); // Track definitions
    static List<string> choices = new List<string>();
    private bool isFirstEnemy = true;

    private void Start()
    {
        dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabaseFinal.db";
        LoadUniqueEnemyWords("easyWrds_tbl");

        Choice1Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice1Text.text));
        Choice2Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice2Text.text));
        Choice3Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice3Text.text)); 
    }

    private void LoadUniqueEnemyWords(string tableName)
    {
        enemies.Clear();
        english.Clear();
        definition.Clear();
        
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = $"SELECT easy_ID, tagalog, english, definition FROM {tableName} ORDER BY RANDOM()";

            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WordData wordData = new WordData
                        {
                            wordID = reader.GetInt32(0),
                            Tagalog = reader.GetString(1),
                            English = reader.GetString(2),
                            Definition = reader.GetString(3)
                        };

                        if (!enemyWordList.Exists(w => w.Tagalog == wordData.Tagalog && w.English == wordData.English))
                        {
                            enemyWordList.Add(wordData);
                            Debug.Log($"Loaded word: Tagalog='{wordData.Tagalog}', English='{wordData.English}', Definition='{wordData.Definition}', ID={wordData.wordID}");
                        }
                    }
                }
            }
            connection.Close();
        }
    }

    public void SetupEnemy(Enemy enemy)
    {
        if (enemyWordList.Count == 0)
        {
            Debug.LogWarning("No more words available.");
            return;
        }

        WordData wordData = enemyWordList[0];
        enemyWordList.RemoveAt(0);
        enemy.SetTagalogText(wordData.Tagalog);
        enemy.SetID(wordData.wordID);
        enemies.Add(wordData.wordID); 
        english.Add(wordData.English);
        definition.Add(wordData.Definition);
        
        Debug.Log($"Enemy setup: Tagalog='{wordData.Tagalog}', English='{wordData.English}', Definition='{wordData.Definition}', ID={wordData.wordID}");

        // Set the definition and choices only for the first enemy
        if (isFirstEnemy)
        {
            SetupChoicesAndDefinition();
            isFirstEnemy = false;
        }

        Debug.Log($"-----Current number of enemies in List: {enemies.Count}, ID: {enemies[0]}");
    }

    private void SetupChoicesAndDefinition()
    {
        if (definition.Count > 0 && english.Count > 0)
        {
            DefinitionText.text = definition[0];
            correctAnswer = english[0];
            Debug.Log($"Setting definition and correct answer: Definition='{definition[0]}', Correct Answer='{correctAnswer}'");
            SetupChoices();
        }
        else
        {
            Debug.LogWarning("No definition or correct answer available.");
        }
    }

    private void SetupChoices()
    {
        choices.Clear();
        choices.Add(correctAnswer);
        Debug.Log("Adding correct answer to choices list.");

        // Add two unique distractors
        while (choices.Count < 3)
        {
            string distractor = GetRandomEnglishWord("easyWrds_tbl");
            if (!choices.Contains(distractor))
            {
                choices.Add(distractor);
            }
        }

        choices = ShuffleList(choices);
        Debug.Log($"Final choices: {choices[0]}, {choices[1]}, {choices[2]}");

        Choice1Text.text = choices[0];
        Choice2Text.text = choices[1];
        Choice3Text.text = choices[2];
    }

    public void OnChoiceSelected(string selectedChoice)
    {
        Debug.Log("Enemy on the list:" + enemies.Count);
        Debug.Log("Enemy ID:" + enemies[0]);
        Debug.Log($"Selected choice: '{selectedChoice}', Correct answer: '{correctAnswer}'");

        if (enemies.Count > 0)
            // Log what is in the first index of the enemies list
            Debug.Log($"First enemy ID in list: {enemies[0]}");
        {
            if (selectedChoice == correctAnswer)
            {
                Debug.Log("Correct answer selected.");
                ProceedToNextWord();
                DestroyCurrentEnemy();
            }
            else
            {
                Debug.Log("Incorrect answer.");
                HandleIncorrectAnswer();
                DestroyCurrentEnemy();
            }
        }
    }

    private void DestroyCurrentEnemy()
    {
        if (enemies.Count > 0)
        {
            int enemyID = enemies[0];
            Enemy enemyToDestroy = null;
            Debug.Log($"Attempting to destroy enemy with ID={enemyID}");


            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if (enemy.ID == enemyID)
                {
                    enemyToDestroy = enemy;
                    break;
                }
            }

            if (enemyToDestroy != null)
            {
                Debug.Log($"Destroying enemy with ID={enemyID}");
                Destroy(enemyToDestroy.gameObject);
                Debug.Log($"Destroyed enemy with ID={enemyID}");
                enemies.RemoveAt(0);
                english.RemoveAt(0);
                definition.RemoveAt(0);
            }
            else
            {
                Debug.LogWarning("Enemy with ID " + enemyID + " not found.");
            }
        }
    }

    private void ProceedToNextWord()
    {
        if (enemies.Count > 0 && english.Count > 0 && definition.Count > 0)
        {
            Debug.Log("Proceeding to next word.");
            SetupChoicesAndDefinition();
        }
        else
        {
            Debug.LogWarning("No more words to display.");
        }
    }

    private void HandleIncorrectAnswer()
    {
        Debug.Log("Handling incorrect answer.");
        ProceedToNextWord();
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
                        Debug.Log($"Fetched random distractor: '{randomWord}'");
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
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}

