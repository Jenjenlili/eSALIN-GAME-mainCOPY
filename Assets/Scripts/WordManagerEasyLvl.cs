//PROBLEM: DEFINITION AND CHOICES CHANGING EACH SPAWN (THE BEST CODE SO FAR)
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

    private string correctAnswer;
    static List<Enemy> enemies = new List<Enemy>(); // Track active enemies
    private bool isFirstEnemy = true;
    private bool isNextQuestion = false;
    private void Start()
    {
        dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";
        LoadUniqueEnemyWords("easyWrds_tbl");
        // Dynamically add listeners to pass the choice text when each button is clicked
        Choice1Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice1Text.text));
        Choice2Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice2Text.text));
        Choice3Text.GetComponentInParent<Button>().onClick.AddListener(() => OnChoiceSelected(Choice3Text.text)); 

    }
    // private void Update()
    // {
    //     if (isFirstEnemy)
    //     {   
    //         Enemy currentEnemy = enemies[0];
    //         Debug.Log("Current Enemy: " + currentEnemy.GetID()); 
    //         SetupQuestion(currentEnemy);
    //         isFirstEnemy = false;
    //     }else if (isNextQuestion)
    //     {
    //         SetupQuestion(enemies[0]);
    //         Debug.Log("Current Enemy: " + enemies[0].GetID());
    //     }
    // }

    private void LoadUniqueEnemyWords(string tableName)
    {
        enemies.Clear();
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
                        }
                    }
                }
            }
            connection.Close();
        }
    }

    // Setup method for an enemy to set Tagalog word
    public void SetupEnemy(Enemy enemy)
    {
        if (enemyWordList.Count == 0)
        {
            Debug.LogWarning("No more words available.");
            return;
        }

        WordData wordData = enemyWordList[0];
        enemyWordList.RemoveAt(0);
        enemy.SetDefinition(wordData.Definition); // Set the definition in the enemy
        enemy.SetTagalogText(wordData.Tagalog);
        enemy.SetID(wordData.wordID);
        enemy.SetEnglish(wordData.English);
        //correctAnswer = wordData.English; // Save the correct answer

        // Track the active enemy
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }

        // Set the definition and choices only for the first enemy
        if (isFirstEnemy)
        {   
            Enemy currentEnemy = enemies[0];
            Debug.Log("Current Enemy: " + currentEnemy.GetID()); 
            SetupQuestion(currentEnemy);
            isFirstEnemy = false;
        }else if (isNextQuestion)
        {
            SetupQuestion(enemies[0]);
            Debug.Log("Current Enemy: " + enemies[0].GetID());
        }
    }

    // Setup method to set definition and choices after an enemy is set
    public void SetupQuestion(Enemy enemy)
    { 
        string definition = enemy.GetDefinition();
        DefinitionText.text = definition;
        
        correctAnswer = enemy.GetEnglish();
        //SET UP CHOICE
        List<string> choices = new List<string> { correctAnswer };

        // Add two unique distractors
        while (choices.Count < 3)
        {
            string distractor = GetRandomEnglishWord("easyWrds_tbl");
            if (!choices.Contains(distractor))
            {
                choices.Add(distractor);
            }
        }

        // Shuffle choices to randomize order
        choices = ShuffleList(choices);

        // Set the choice texts
        Choice1Text.text = choices[0];
        Choice2Text.text = choices[1];
        Choice3Text.text = choices[2];
        Debug.Log("Choices set to: " + Choice1Text.text + ", " + Choice2Text.text + ", " + Choice3Text.text);

    }

    // Check the selected choice
    public void OnChoiceSelected(string selectedChoice)
    {
        Debug.Log("Something was Selected");
        var enm =GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Current number of enemies with tag Enemy:" + enm.Length);
        Debug.Log("Current number of enemies in List: "+ enemies.Count);
        Debug.Log("Button clicked: " + selectedChoice); // check if the button registers correctly

        //Debug.Log(OnChoiceSelected);
        if (enemies.Count > 0)
        {
            Enemy currentEnemy = enemies[0]; // Get the current enemy
            //enemies.RemoveAt(0); // Remove the current enemy from the list
            //Destroy(currentEnemy.gameObject); // Destroy the current enemy's GameObject
        }
        if (selectedChoice == correctAnswer)
        {
            ProceedToNextWord();
        }
        else
        {
            HandleIncorrectAnswer();
        }
    }

    private void ProceedToNextWord()
    {
        if (enemies.Count > 0)
        {
            Enemy currentEnemy = enemies[0]; // Get the current enemy
            enemies.RemoveAt(0); // Remove the current enemy from the list
            
            // Set up the next enemy with a new word
            SetupEnemy(currentEnemy);
            SetupQuestion(currentEnemy); // Pass the definition for the UI
        }
    }

    private void HandleIncorrectAnswer()
    {
         if (enemies.Count > 0)
        {
            Enemy currentEnemy = enemies[0]; // Get the current enemy
            enemies.RemoveAt(0); // Remove the current enemy from the list
            
            // Set up the next enemy with a new word
            SetupEnemy(currentEnemy);
            SetupQuestion(currentEnemy); // Pass the definition for the UI
        }
        // Logic to handle incorrect answer (e.g., notify player, destroy enemy, etc.)
        Debug.Log("Incorrect answer selected.");
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

    // Helper method to shuffle a list of strings
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



//-------------------------------------------------------------------------------------------------------
//MAS UPDATED VERSION
//WRKING NA PERO NAGDODOBLE COUNT 1 SA CURRENET ENEMY

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
//         dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";
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
//     public void SetupEnemyTagalogText(Enemy enemy)
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

// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using System.Data;
// using Mono.Data.Sqlite;

// // Define WordData struct outside of the GameManager class
// public struct WordData
// {
//     public int wordID;
//     public string Tagalog;
//     public string English;
//     public string Definition;
// }

// public class GameManager : MonoBehaviour
// {
//     private string dbPath;

//     private List<WordData> wordList = new List<WordData>(); // List of all word data
//     private Queue<WordData> wordQueue = new Queue<WordData>();
//     public TextMeshProUGUI DefinitionText;
//     public TextMeshProUGUI Choice1Text;
//     public TextMeshProUGUI Choice2Text;
//     public TextMeshProUGUI Choice3Text;

//     private WordData currentWordData;
//     private bool isAnswerPending = true;

//     private void Start()
//     {
//         dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";
//         LoadUniqueEnemyWords("easyWrds_tbl");

//         // Set up the first question
//         SetNewQuestion();
//     }

//     private void LoadUniqueEnemyWords(string tableName)
//     {
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

//                         if (!wordList.Exists(w => w.Tagalog == wordData.Tagalog && w.English == wordData.English))
//                         {
//                             wordList.Add(wordData);
//                             wordQueue.Enqueue(wordData); // Enqueue the word data
//                         }
//                     }
//                 }
//             }
//             connection.Close();
//         }
//     }

//     public bool TryGetNextWordData(out WordData wordData)
//     {
//         if (wordQueue.Count > 0)
//         {
//             wordData = wordQueue.Dequeue();
//             return true;
//         }
//         else
//         {
//             wordData = default;
//             return false;
//         }
//     }

//     public void OnChoiceSelected(string selectedChoice)
//     {
//         if (selectedChoice == currentWordData.English)
//         {
//             // Correct answer, proceed to the next question
//             OnCorrectAnswer();
//         }
//         else
//         {
//             // Handle incorrect answer (optional: feedback to player)
//             Debug.Log("Incorrect answer! Try again.");
//         }
//     }

//     private void OnCorrectAnswer()
//     {
//         // Set up the next question
//         SetNewQuestion();
//     }

//     private void SetNewQuestion()
//     {
//         if (wordQueue.Count > 0)
//         {
//             currentWordData = wordQueue.Peek(); // Get the next word without removing it

//             // Display the new question
//             DefinitionText.text = currentWordData.Definition;

//             // Set up choices
//             List<string> choices = new List<string> { currentWordData.English };
//             while (choices.Count < 3)
//             {
//                 string distractor = GetRandomEnglishWord("easyWrds_tbl");
//                 if (!choices.Contains(distractor))
//                 {
//                     choices.Add(distractor);
//                 }
//             }

//             // Shuffle choices to randomize their order
//             choices = ShuffleList(choices);

//             // Set the choice texts
//             Choice1Text.text = choices[0];
//             Choice2Text.text = choices[1];
//             Choice3Text.text = choices[2];
//         }
//         else
//         {
//             Debug.LogWarning("No more questions available.");
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

//     private List<string> ShuffleList(List<string> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             string temp = list[i];
//             int randomIndex = Random.Range(i, list.Count);
//             list[i] = list[randomIndex];
//             list[randomIndex] = temp;
//         }
//         return list;
//     }
// }




//PROBLEM: TAGALOG WORD NOT CHANGING even if clicked the right answer
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using System.Data;
// using Mono.Data.Sqlite;
// using UnityEngine.UI;

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

//     private WordData currentWordData;

//     private void Start()
//     {
//         dbPath = "URI=file:" + Application.dataPath + "/Plugins/eSALINdatabase.db";
        
//         // Load all unique words for enemies at the start
//         LoadUniqueEnemyWords("easyWrds_tbl");

//         // Set up the first question
//         SetNewQuestion();
//     }

//     private void LoadUniqueEnemyWords(string tableName)
//     {
//         using (var connection = new SqliteConnection(dbPath))
//         {
//             connection.Open();
//             string query = $"SELECT easy_ID, tagalog, english, definition FROM {tableName}";

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

//                         // Ensure each word combination is unique
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
//      public void OnSelectChoice(string selectedChoice)
//     {
//         // Check if the selected choice is correct
//         if (selectedChoice == currentWordData.English)
//         {
//             // Correct answer, proceed to the next question
//             OnCorrectAnswer();
//         }
//         else
//         {
//             // Handle incorrect answer (optional: feedback to player)
//             Debug.Log("Incorrect answer! Try again.");
//         }
//     }
//      public void SetupChoiceButtons()
//     {
//         // Link the buttons to the OnSelectChoice method
//         Choice1Text.GetComponent<Button>().onClick.AddListener(() => OnSelectChoice(Choice1Text.text));
//         Choice2Text.GetComponent<Button>().onClick.AddListener(() => OnSelectChoice(Choice2Text.text));
//         Choice3Text.GetComponent<Button>().onClick.AddListener(() => OnSelectChoice(Choice3Text.text));
//     }
//     private void SetNewQuestion()
//     {
//         // Get a random word for the new question
//         if (enemyWordList.Count > 0)
//         {
//             int randomIndex = Random.Range(0, enemyWordList.Count);
//             currentWordData = enemyWordList[randomIndex];
//             enemyWordList.RemoveAt(randomIndex); // Ensure it’s unique for this round

//             // Display the new question
//             DefinitionText.text = currentWordData.Definition;

//             // Set up choices
//             List<string> choices = new List<string> { currentWordData.English };
//             while (choices.Count < 3)
//             {
//                 string distractor = GetRandomEnglishWord("easyWrds_tbl");
//                 if (!choices.Contains(distractor))
//                 {
//                     choices.Add(distractor);
//                 }
//             }
//             choices = ShuffleList(choices);

//             // Update choice buttons
//             Choice1Text.text = choices[0];
//             Choice2Text.text = choices[1];
//             Choice3Text.text = choices[2];
//         }
//         else
//         {
//             Debug.LogWarning("No more words available in the list.");
//         }
//         SetupChoiceButtons();
//     }

//     public string GetCurrentTagalogWord()
//     {
//         return currentWordData.Tagalog;
//     }

//     public void OnCorrectAnswer()
//     {
//         // Set a new question only when the player answers correctly
//         SetNewQuestion();
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

//     private List<string> ShuffleList(List<string> list)
//     {
//         for (int i = 0; i < list.Count; i++)
//         {
//             string temp = list[i];
//             int randomIndex = Random.Range(i, list.Count);
//             list[i] = list[randomIndex];
//             list[randomIndex] = temp;
//         }
//         return list;
//     }
//}
