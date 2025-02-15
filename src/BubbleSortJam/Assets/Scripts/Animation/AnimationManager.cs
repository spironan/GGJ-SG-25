using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class AnimationManager : MonoBehaviour
{
    private static AnimationManager instance = null;
    public static AnimationManager Instance { get { return instance; } }

    [Header("World Space Settings")]
    [SerializeField] private Vector2 GridSlotSize;
    [SerializeField] private float GameplayBPM;

    [Header("Other Managers")]
    [SerializeField] private PlayerAnimator player;
    [SerializeField] private TutorialUIManager tutorialManager;

    private bool isInTutorial = true;

    public PlayerAnimator Player { get { return player; } }

    [Header("Number Arrays")]
    [SerializeField] private GameObject numberArrayPrefab;

    private List<NumberArrayAnimator> numberArrays = new List<NumberArrayAnimator>();

    private GameplayEventListener eventListener = new GameplayEventListener();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
        }

        eventListener.Activate();

        eventListener.AddCallback(typeof(GameStartGameplayEvent), OnGameStart);
        eventListener.AddCallback(typeof(GameEndGameplayEvent), OnGameEnd);

        eventListener.AddCallback(typeof(ArrayCreatedGameplayEvent), OnArrayCreated);
        eventListener.AddCallback(typeof(StartIterationGameplayEvent), OnIterationStart);
        eventListener.AddCallback(typeof(StageCompleteGameplayEvent), OnStageCompleted);
        eventListener.AddCallback(typeof(BPMChangedGameplayEvent), OnBPMChanged);

        eventListener.AddCallback(typeof(SwapElementGameplayEvent), OnElementSwapped);

        eventListener.AddCallback(typeof(BeatLeniencyGameplayEvent), OnBeatLeniencyChanged);

        eventListener.AddCallback(typeof(ArrayElementStateChangedEvent), OnArrayElementStateChanged);
    }

    private void OnDestroy()
    {
        eventListener.Deactivate();
    }

    private void Start()
    {
        Player.SetGameplayDistanceToTravel(new Vector2(GridSlotSize.x, GridSlotSize.y * 2));
    }

    public NumberArrayAnimator CreateNumberArray(List<int> values)
    {
        NumberArrayAnimator array = Instantiate(numberArrayPrefab).GetComponent<NumberArrayAnimator>();
        array.name = "ArrayAnimator_" + numberArrays.Count;

        array.transform.SetParent(transform);
        array.transform.localPosition = new Vector2(0.0f, numberArrays.Count * GridSlotSize.y * -2);

        array.Initialize(values, GridSlotSize.x);

        numberArrays.Add(array);
        return array;
    }

    private void OnGameStart(BaseGameplayEvent baseEvent)
    {
        TryShowTutorial(0);
    }

    private void OnGameEnd(BaseGameplayEvent baseEvent)
    {
        GameEndGameplayEvent usableEvent = (GameEndGameplayEvent)baseEvent;

        if (usableEvent.IsWin)
        {
            NumberArrayAnimator numberArrayAnimator = numberArrays[numberArrays.Count - 1];
            numberArrayAnimator.SetAllElementsState(NumberElementState.Sorted);
            numberArrayAnimator.PlayArrayCompleteParticles();

            Player.ClearAnimationQueue();
            Player.QueueAnimation(PlayerAnimationPresetType.FinishStart);
            Player.QueueAnimation(PlayerAnimationPresetType.FinishEnd, () =>
            {
                UIManager.Instance.ShowWinScreen();
            });
        }
        else
        {
            Player.StopAllAnimations();
            Player.PlayDeathAnimation();
        }
    }

    private void OnArrayCreated(BaseGameplayEvent baseEvent)
    {
        ArrayCreatedGameplayEvent usableEvent = (ArrayCreatedGameplayEvent)baseEvent;
        CreateNumberArray(usableEvent.Values);
    }

    private void OnIterationStart(BaseGameplayEvent baseEvent)
    {
        StartIterationGameplayEvent usableEvent = (StartIterationGameplayEvent)baseEvent;

        Player.QueueAnimation(PlayerAnimationPresetType.Start);

        for (int i = 0; i < usableEvent.UnsortedCount - 1; ++i)
        {
            Player.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
        }

        Player.QueueAnimation(PlayerAnimationPresetType.FinishStart);
        Player.QueueAnimation(PlayerAnimationPresetType.FinishEnd);
    }

    private void OnStageCompleted(BaseGameplayEvent baseEvent)
    {
        StageCompleteGameplayEvent usableEvent = (StageCompleteGameplayEvent)baseEvent;
        if (usableEvent.StageIndex >= 0 && usableEvent.StageIndex < numberArrays.Count)
        {
            NumberArrayAnimator numberArrayAnimator = numberArrays[usableEvent.StageIndex];
            numberArrayAnimator.SetAllElementsState(NumberElementState.Sorted);
            numberArrayAnimator.PlayArrayCompleteParticles();
        }

        Player.QueueAnimation(PlayerAnimationPresetType.MoveNextStart);
        Player.QueueAnimation(PlayerAnimationPresetType.MoveNextEnd, () =>
        {
            TryShowTutorial(usableEvent.StageIndex + 1);
        });
    }

    private void OnBPMChanged(BaseGameplayEvent baseEvent)
    {
        BPMChangedGameplayEvent usableEvent = (BPMChangedGameplayEvent)baseEvent;
        Player.SetGameplayBPM(usableEvent.BPM);
    }

    private void OnElementSwapped(BaseGameplayEvent baseEvent)
    {
        SwapElementGameplayEvent usableEvent = (SwapElementGameplayEvent)baseEvent;

        if (usableEvent.StageIndex < 0 || usableEvent.StageIndex >= numberArrays.Count)
        {
            Debug.LogError("Invalid Stage Index");
            return;
        }

        NumberArrayAnimator numberArrayAnimator = numberArrays[usableEvent.StageIndex];
        numberArrayAnimator.PlaySwapAnimation(usableEvent.ElementIndex);
    }

    private void OnBeatLeniencyChanged(BaseGameplayEvent baseEvent)
    {
        BeatLeniencyGameplayEvent usableEvent = (BeatLeniencyGameplayEvent)baseEvent;
        if (usableEvent.IsOpen)
        {
            Player.PlayLeniencyWindowActivated();
        }
        else
        {
            Player.PlayLeniencyWindowDeactivated();
        }
    }

    private void OnArrayElementStateChanged(BaseGameplayEvent baseEvent)
    {
        ArrayElementStateChangedEvent usableEvent = (ArrayElementStateChangedEvent)baseEvent;

        if (usableEvent.StageIndex < 0 || usableEvent.StageIndex >= numberArrays.Count)
        {
            Debug.LogError("Invalid Stage Index");
            return;
        }

        NumberArrayAnimator arrayAnimator = numberArrays[usableEvent.StageIndex];
        NumberElementAnimator numberAnimator = arrayAnimator.FindElement(usableEvent.ElementIndex);
        numberAnimator.UpdateState(usableEvent.NewState);
    }

    private void TryShowTutorial(int levelIndex)
    {
        NumberArrayAnimator arrayAnimator = numberArrays[levelIndex];

        Vector2 boundsSize = new Vector2(GridSlotSize.x * arrayAnimator.Count, GridSlotSize.y);
        Vector2 boundsCenter = new Vector2(arrayAnimator.transform.position.x - (GridSlotSize.x / 2.0f) + (boundsSize.x / 2.0f), arrayAnimator.transform.position.y);
        Bounds bounds = new Bounds(boundsCenter, boundsSize);

        tutorialManager.TryCreateMessage(levelIndex, bounds);
        bool isTutorial = tutorialManager.HasTutorial(levelIndex);
        if (isInTutorial != isTutorial)
        {
            if (!isTutorial)
            {
                MasterAudioController.instance.OnFullGameStart();
            }
            isInTutorial = isTutorial;
        }
    }

    public Vector2 GetGridSlotSize()
    {
        return GridSlotSize;
    }
}

[System.Serializable]
public class SimpleAnimationData
{
    public AnimationCurve Curve;
    public float StartValue;
    public float EndValue;
    public float Duration;
}