using UnityEngine;
using System.Collections.Generic;

public class AnimationManager : MonoBehaviour
{
    private static AnimationManager instance = null;
    public static AnimationManager Instance { get { return instance; } }

    [Header("World Space Settings")]
    [SerializeField] private Vector2 GridSlotSize;
    [SerializeField] private float GameplayBPM;

    [Header("Player")]
    [SerializeField] private PlayerAnimator player;

    public PlayerAnimator Player { get { return player; } }

    [Header("Number Arrays")]
    [SerializeField] private GameObject numberArrayPrefab;

    private List<NumberArrayAnimator> numberArrays = new List<NumberArrayAnimator>();

    private GameplayEventListener eventListener = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance == this)
        {
            Destroy(this);
        }

        eventListener = new GameplayEventListener();

        eventListener.AddCallback(typeof(GameStartGameplayEvent), OnGameStart);
        eventListener.AddCallback(typeof(GameEndGameplayEvent), OnGameEnd);

        eventListener.AddCallback(typeof(ArrayCreatedGameplayEvent), OnArrayCreated);
        eventListener.AddCallback(typeof(StartIterationGameplayEvent), OnIterationStart);
        eventListener.AddCallback(typeof(StageCompleteGameplayEvent), OnStageCompleted);
        eventListener.AddCallback(typeof(BPMChangedGameplayEvent), OnBPMChanged);

        eventListener.AddCallback(typeof(SwapElementGameplayEvent), OnElementSwapped);
    }

    private void OnDestroy()
    {
        eventListener = null;
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
    }

    private void OnGameEnd(BaseGameplayEvent baseEvent)
    {
        GameEndGameplayEvent usableEvent = (GameEndGameplayEvent)baseEvent;

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
        Player.QueueAnimation(PlayerAnimationPresetType.MoveNextStart);
        Player.QueueAnimation(PlayerAnimationPresetType.MoveNextEnd);
    }

    private void OnBPMChanged(BaseGameplayEvent baseEvent)
    {
        BPMChangedGameplayEvent usableEvent = (BPMChangedGameplayEvent)baseEvent;
        Player.SetGameplayBPM(usableEvent.BPM);
    }

    private void OnElementSwapped(BaseGameplayEvent baseEvent)
    {
        SwapElementGameplayEvent usableEvent = (SwapElementGameplayEvent)baseEvent;

        if(usableEvent.StageIndex < 0 && usableEvent.StageIndex >= numberArrays.Count)
        {
            Debug.LogError("Invalid Stage Index");
            return;
        }

        NumberArrayAnimator numberArrayAnimator = numberArrays[usableEvent.StageIndex];
        numberArrayAnimator.PlaySwapAnimation(usableEvent.ElementIndex);
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