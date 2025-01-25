using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    [Header("UI")]
    [SerializeField] public GameObject TitleScreen;
    [SerializeField] public GameObject HUD;
    [SerializeField] public GameObject WinScreen;
    [SerializeField] public GameObject LoseScreen;

    private List<GameObject> allUI = new List<GameObject>();
    private GameplayEventListener eventListener = new GameplayEventListener();

    private void Awake()
    {
        if(instance == null)
        {  
            instance = this; 
        }
        else if(instance != this)
        {
            Destroy(this);
            return;
        }

        allUI.Add(TitleScreen);
        allUI.Add(HUD);
        allUI.Add(WinScreen);
        allUI.Add(LoseScreen);

        eventListener.Activate();

        eventListener.AddCallback(typeof(GameStartGameplayEvent), OnGameStart);
        eventListener.AddCallback(typeof(GameEndGameplayEvent), OnGameEnd);
    }

    private void OnDestroy()
    {
        eventListener.Deactivate();
    }

    private void Start()
    {
        SetActiveUI(TitleScreen);
    }

    private void OnGameStart(BaseGameplayEvent baseEvent)
    {
        SetActiveUI(HUD);
    }

    private void OnGameEnd(BaseGameplayEvent baseEvent)
    {
        GameEndGameplayEvent usableEvent = (GameEndGameplayEvent)baseEvent;
        if(usableEvent.IsWin)
        {
            SetActiveUI(WinScreen);
        }
        else
        {
            SetActiveUI(LoseScreen);
        }
    }

    private void SetActiveUI(GameObject targetUI)
    {
        foreach(GameObject ui in allUI)
        {
            ui?.SetActive(ui == targetUI);
        }
    }
}