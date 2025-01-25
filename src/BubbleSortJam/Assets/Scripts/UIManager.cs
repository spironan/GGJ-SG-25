using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    [Header("UI")]
    [SerializeField] public GameObject TitleScreen;
    [SerializeField] public GameObject HUD;

    private List<GameObject> allUI = new List<GameObject>();
    private GameplayEventListener eventListener = null;

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

        eventListener = new GameplayEventListener();

        eventListener.AddCallback(typeof(GameStartGameplayEvent), OnGameStart);
        eventListener.AddCallback(typeof(GameEndGameplayEvent), OnGameEnd);
    }

    private void OnDestroy()
    {
        eventListener = null;
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

    }

    private void SetActiveUI(GameObject targetUI)
    {
        foreach(GameObject ui in allUI)
        {
            ui?.SetActive(ui == targetUI);
        }
    }
}