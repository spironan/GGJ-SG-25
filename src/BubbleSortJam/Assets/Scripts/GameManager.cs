using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SortData
{
    public int size;
    public int swaps;
    public List<int> pattern;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField]
    private string loadPath;
    private List<SortData> database;

    [SerializeField]
    private List<int> currentArray;

    public event Action OnSuccessfulSwap;
    public event Action OnFailedSwap;
    public event Action OnCompleteRow;
    public event Action OnCompleteAll;

    [SerializeField]
    private GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // DontDestroyOnLoad(gameObject);
    }

    void LoadDataFromCSV(string filePath)
    {
        database = new List<SortData>();

        string[] allLines = File.ReadAllLines(filePath);

        // Loop through each line in the CSV, skipping the header
        for (int i = 1; i < allLines.Length; i++)
        {
            var line = allLines[i];
            var columns = line.Split(',');

            // Ensure that the line contains the expected number of columns
            if (columns.Length > 2)
            {
                SortData data = new SortData();
                data.size = int.Parse(columns[0]);
                data.swaps = int.Parse(columns[1]);
                data.pattern = new List<int>();

                var count = columns[2].Split(" ");
                // Convert the remaining elements into integers and add to the pattern list
                for (int j = 0; j < count.Length; j++)
                {
                    data.pattern.Add(int.Parse(count[j]));
                }

                database.Add(data);
            }
        }

        Debug.Log("Database loaded " + database.Count + " entries");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Load all Possible Arrays
        LoadDataFromCSV(loadPath);
        GenerateArray(10, 10);
        player.GetComponent<Life>().OnDeath += ResetAll;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SortData GenerateArray(int size, int swaps)
    {
        var result = database.Find(data => data.size == size && data.swaps == swaps);

        Debug.Log("Generated array of size "  + result.size + " swaps: " + result.swaps);
        PrintArray(result.pattern);

        currentArray = result.pattern;

        ArrayCreatedGameplayEvent.BroadcastEvent(result.pattern);

        return result;
    }

    public bool AttemptSwap(int i, int j)
    {
        bool result = false;

        Debug.Log("Before");
        PrintArray(currentArray);

        if(currentArray[i] > currentArray[j])
        {
            int og = currentArray[i];
            currentArray[i] = currentArray[j];
            currentArray[j] = og;
            result = true;
        }

        Debug.Log("After");
        PrintArray(currentArray);

        if (result) 
        {
            OnSuccessfulSwap?.Invoke();
        } 
        else
        {
            OnFailedSwap?.Invoke();
        }

        return result;
    }

    private void PrintArray(List<int> result)
    {
        string outputVals = "";
        foreach (var val in result)
        {
            outputVals += val + " ";
        }
        Debug.Log("values " + outputVals);
    }

    private void ResetAll()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
