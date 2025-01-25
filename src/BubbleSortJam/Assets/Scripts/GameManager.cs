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
public class StageData
{
    public int arraySize;
    public int swapsRequired;
}
public class PlayData
{
    public int stageSize;
    public int currentIteration;
    public int currentIndex;

    public int startBeatsToIgnoreLeft;
};

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public bool LevelIsWon { get; private set; } = false;

    [SerializeField]
    private string sortLoadPath;
    private List<SortData> sortData = new List<SortData>();

    [SerializeField]
    private string levelLoadPath;
    private List<StageData> levelData = new List<StageData>();

    private bool hasStartedGame = false;
    private bool hasEndedGame = false;
    private int currentStage = 0;
    [SerializeField]
    private List<int> currentArray = new List<int>();

    // takes into account beats that should be ignored because animation
    public bool BeatWindow { get; private set; } = false;

    public event Action OnSuccessfulSwap;
    public event Action OnFailedSwap;
    public event Action OnCompleteRow;
    public event Action OnCompleteAll;

    [SerializeField]
    private GameObject player;

    //[SerializeField]
    //private int currentId = 0;
    [SerializeField]
    private PlayData playData = new PlayData();

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
        sortData = new List<SortData>();

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

                sortData.Add(data);
            }
        }

        Debug.Log("Database loaded " + sortData.Count + " entries");
    }

    void LoadLevelFromCSV(string filePath)
    {
        levelData = new List<StageData>();

        string[] allLines = File.ReadAllLines(filePath);

        // Loop through each line in the CSV, skipping the header
        for (int i = 1; i < allLines.Length; i++)
        {
            var line = allLines[i];
            var columns = line.Split(',');
            // Ensure that the line contains the expected number of columns
            if (columns.Length > 1)
            {
                StageData data = new StageData();
                data.arraySize = int.Parse(columns[0]);
                data.swapsRequired = int.Parse(columns[1]);
                levelData.Add(data);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //OnSuccessfulSwap += CheckIfProceed;

        // Load all Possible Arrays
        LoadDataFromCSV(sortLoadPath);

        // Load level data
        LoadLevelFromCSV(levelLoadPath);

        // Start Level
        LoadStage();

        player.GetComponent<Life>().OnDeath += OnPlayerDeath;

        // engage bpm tracker increment bpm
        BpmTracker.instance.OnWindowOpen += OnBeatWindowOpen;
        BpmTracker.instance.OnWindowClose += OnBeatWindowClosed;
    }

    public bool HasStartedGame()
    {
        return hasStartedGame;
    }

    public bool HasEndedGame()
    {
        return hasEndedGame;
    }

    public void StartGame()
    {
        hasStartedGame = true;

        MasterAudioController.instance.OnGameStart();
        BpmTracker.instance.OnGameStart();

        GameStartGameplayEvent.BroadcastEvent();
        StartIterationGameplayEvent.BroadcastEvent(playData.stageSize - playData.currentIteration + 1);
    }

    private void OnPlayerDeath()
    {
        hasEndedGame = true;

        BpmTracker.instance.OnGameEnd();
        MasterAudioController.instance.OnGameLost();
        GameEndGameplayEvent.BroadcastEvent(false);
    }

    private void OnBeatWindowOpen()
    {
        if (playData.startBeatsToIgnoreLeft > 0)
        {
            return;
        }

        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex, NumberElementState.Involved);
        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex + 1, NumberElementState.Involved);
        BeatWindow = true;
    }

    private void OnBeatWindowClosed()
    {
        if(playData.startBeatsToIgnoreLeft > 0)
        {
            playData.startBeatsToIgnoreLeft--;
            return;
        }

        BeatWindow = false;

        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex, NumberElementState.Neutral);
        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex + 1, NumberElementState.Neutral);

        if (SwapIsNeeded())
        {
            Debug.Log("Player should have swapped!");

            // player failed to swap.
            OnFailedSwap?.Invoke();

            BroadcastMistakeAtCurrent();

            // additionally: we need to help them do it for them
            // this should always pass
            AttemptSwap();
        }

        Debug.Log("iteration " + playData.currentIteration + " index " + (playData.currentIndex + 1));

        if (++playData.currentIndex >= playData.stageSize - playData.currentIteration)
        {
            ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex, NumberElementState.Sorted);
            //ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex - 1, NumberElementState.Neutral);

            // increment iteration & reset current index
            playData.currentIndex = 0;
            ++playData.currentIteration;


            if (StageIsSorted())
            {
                Debug.Log("Auto proceeding to the next stage");
                NextStage();
            }
            else if (playData.currentIteration + 1 >= playData.stageSize) // iter ends at n-1 
            {
                NextStage();
            }
            else
            {
                StartIterationGameplayEvent.BroadcastEvent(playData.stageSize - playData.currentIteration + 1);
                playData.startBeatsToIgnoreLeft = 3;
            }
        }
        else
        {
            //ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex);
        }
    }

    private bool SwapIsNeeded()
    {
        int i = playData.currentIndex;
        int j = playData.currentIndex + 1;
        Debug.Log("swap needed between " +  i + ", " + j);
        return (i < currentArray.Count && currentArray[i] > currentArray[j]);
    }

    // Update is called once per frame
    void Update()
    {
        // testing code
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    NextStage();
        //}
    }

    public bool AttemptSwap()
    {
        int i = playData.currentIndex;
        int j = playData.currentIndex + 1;

        bool result = false;

        Debug.Log("Before");
        PrintArray(currentArray);

        // check if we should swap
        if (currentArray[i] > currentArray[j])
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
            SwapElementGameplayEvent.BroadcastEvent(currentStage, playData.currentIndex);
        }
        else
        {
            OnFailedSwap?.Invoke();
        }

        return result;
    }

    public void BroadcastMistakeAtCurrent()
    {
        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex, NumberElementState.Mistake);
        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex + 1, NumberElementState.Mistake);
    }

    public void BroadcastCorrectAtCurrent()
    {
        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex, NumberElementState.Correct);
        ArrayElementStateChangedEvent.BroadcastEvent(currentStage, playData.currentIndex + 1, NumberElementState.Correct);
    }

    private void NextStage()
    {
        if (++currentStage >= levelData.Count)
        {
            // we finished the level! broadcast on game finish event or something
            hasEndedGame = true;
            LevelIsWon = true;

            BpmTracker.instance.OnGameEnd();
            GameEndGameplayEvent.BroadcastEvent(true);
            Debug.Log("We have won the game!");
        }
        else
        {
            LoadStage();
            playData.startBeatsToIgnoreLeft = 5;
            StageCompleteGameplayEvent.BroadcastEvent(currentStage - 1);
            StartIterationGameplayEvent.BroadcastEvent(playData.stageSize - playData.currentIteration + 1);
        }

        Debug.Log("Current Stage is now " + currentStage);
    }

    private void LoadStage()
    {
        // generate the stage based on requirements.
        GenerateArray(levelData[currentStage].arraySize, levelData[currentStage].swapsRequired);
        
        // reload playdata
        playData.currentIndex = 0;
        playData.currentIteration = 1;
        playData.stageSize = levelData[currentStage].arraySize;
        playData.startBeatsToIgnoreLeft = 1;
    }

    private SortData GenerateArray(int size, int swaps)
    {
        var result = sortData.Find(data => data.size == size && data.swaps == swaps);

        Debug.Log("Generated array of size "  + result.size + " swaps: " + result.swaps);
        PrintArray(result.pattern);

        currentArray = result.pattern;

        ArrayCreatedGameplayEvent.BroadcastEvent(result.pattern);

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

    public void ResetAll()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private bool StageIsSorted()
    {
        for (int i = 1; i < currentArray.Count; i++)
        {
            if(currentArray[i] > currentArray[i - 1])
                return false;
        }
        return true;
    }

    //private void CheckIfProceed()
    //{
    //    if (StageIsSorted())
    //    {
    //        Debug.Log("Auto proceeding to the next stage");
    //        NextStage();
    //    }
    //}
}
