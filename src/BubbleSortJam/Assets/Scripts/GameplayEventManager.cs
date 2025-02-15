using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameplayEventManager
{
    private static List<GameplayEventListener> listeners = new List<GameplayEventListener>();

    public static void AddListener(GameplayEventListener listener)
    {
        listeners.Add(listener);
    }

    public static void RemoveListener(GameplayEventListener listener)
    {
        listeners.Remove(listener);
    }

    public static void BroadcastEvent(BaseGameplayEvent ev)
    {
        foreach(GameplayEventListener listener in listeners)
        {
            listener.RecieveEvent(ev);
        }
    }
}

public class GameplayEventListener
{
    public delegate void CallbackFunc(BaseGameplayEvent ev);
    private Dictionary<System.Type, CallbackFunc> callbacks = new Dictionary<System.Type, CallbackFunc>();

    public void Activate()
    {
        GameplayEventManager.AddListener(this);
    }

    public void Deactivate()
    {
        GameplayEventManager.RemoveListener(this);
    }

    public void AddCallback(System.Type eventType, CallbackFunc callback)
    {
        callbacks.Add(eventType, callback);
    }

    public void RemoveCallback(System.Type eventType)
    {
        callbacks.Remove(eventType);
    }

    public void RecieveEvent(BaseGameplayEvent ev)
    {
        CallbackFunc callback;
        if(callbacks.TryGetValue(ev.GetType(), out callback))
        {
            callback.Invoke(ev);
        }
    }
}

public class BaseGameplayEvent
{
}

public class ArrayCreatedGameplayEvent : BaseGameplayEvent
{
    private List<int> values;
    public List<int> Values { get { return values; } }

    public static void BroadcastEvent(List<int> values)
    {
        ArrayCreatedGameplayEvent ev = new ArrayCreatedGameplayEvent();
        ev.values = values;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class GameStartGameplayEvent : BaseGameplayEvent
{
    public static void BroadcastEvent()
    {
        GameStartGameplayEvent ev = new GameStartGameplayEvent();
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class GameEndGameplayEvent : BaseGameplayEvent
{
    private bool isWin;
    public bool IsWin { get { return isWin; } }

    public static void BroadcastEvent(bool isWin)
    {
        GameEndGameplayEvent ev = new GameEndGameplayEvent();
        ev.isWin = isWin;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class StartIterationGameplayEvent : BaseGameplayEvent
{
    private int levelIndex = 0;
    private int sortedCount = 0;
    private int unsortedCount = 0;

    public int LevelIndex { get { return levelIndex; } }
    public int SortedCount { get { return sortedCount; } }
    public int UnsortedCount { get { return unsortedCount; } }

    public static void BroadcastEvent(int levelIndex, int sortedCount, int unsortedCount)
    {
        StartIterationGameplayEvent ev = new StartIterationGameplayEvent();
        ev.levelIndex = levelIndex;
        ev.sortedCount = sortedCount;
        ev.unsortedCount = unsortedCount;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class StageCompleteGameplayEvent : BaseGameplayEvent
{
    private int stageIndex;
    public int StageIndex { get { return stageIndex; } }

    public static void BroadcastEvent(int stageIndex)
    {
        StageCompleteGameplayEvent ev = new StageCompleteGameplayEvent();
        ev.stageIndex = stageIndex;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class BPMChangedGameplayEvent : BaseGameplayEvent
{
    private float bpm;
    public float BPM { get { return bpm; } }

    public static void BroadcastEvent(float bpm)
    {
        BPMChangedGameplayEvent ev = new BPMChangedGameplayEvent();
        ev.bpm = bpm;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class SwapElementGameplayEvent : BaseGameplayEvent
{
    private int stageIndex;
    private int elementIndex;

    public int StageIndex {  get { return stageIndex; } }
    public int ElementIndex { get { return elementIndex; } }

    public static void BroadcastEvent(int stageIndex, int elementIndex)
    {
        SwapElementGameplayEvent ev = new SwapElementGameplayEvent();
        ev.stageIndex = stageIndex;
        ev.elementIndex = elementIndex;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class LifeChangedGameplayEvent : BaseGameplayEvent
{
    private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }

    public static void BroadcastEvent(int currentHealth)
    {
        LifeChangedGameplayEvent ev = new LifeChangedGameplayEvent();
        ev.currentHealth = currentHealth;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class BeatLeniencyGameplayEvent : BaseGameplayEvent
{
    private bool isOpen;
    public bool IsOpen { get { return isOpen; } }

    public static void BroadcastEvent(bool isOpen)
    {
        BeatLeniencyGameplayEvent ev = new BeatLeniencyGameplayEvent();
        ev.isOpen = isOpen;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class ArrayElementStateChangedEvent : BaseGameplayEvent
{
    private int stageIndex;
    private int elementIndex;
    private NumberElementState newState;

    public int StageIndex { get { return stageIndex; } }
    public int ElementIndex { get { return elementIndex; } }
    public NumberElementState NewState { get { return newState; } }

    public static void BroadcastEvent(int stageIndex, int elementIndex, NumberElementState newState)
    {
        ArrayElementStateChangedEvent ev = new ArrayElementStateChangedEvent();
        ev.stageIndex = stageIndex;
        ev.elementIndex = elementIndex;
        ev.newState = newState;
        GameplayEventManager.BroadcastEvent(ev);
    }

    public static void BroadcastEvent(int stageIndex, int currentIndex)
    {
        //if (currentIndex - 1 >= 0)
        //{
        //    ArrayElementStateChangedEvent ev = new ArrayElementStateChangedEvent();
        //    ev.stageIndex = stageIndex;
        //    ev.elementIndex = currentIndex - 1;
        //    ev.newState = NumberElementState.Neutral;
        //    GameplayEventManager.BroadcastEvent(ev);
        //}

        {
            ArrayElementStateChangedEvent ev = new ArrayElementStateChangedEvent();
            ev.stageIndex = stageIndex;
            ev.elementIndex = currentIndex;
            ev.newState = NumberElementState.Involved;
            GameplayEventManager.BroadcastEvent(ev);
        }

        {
            ArrayElementStateChangedEvent ev = new ArrayElementStateChangedEvent();
            ev.stageIndex = stageIndex;
            ev.elementIndex = currentIndex + 1;
            ev.newState = NumberElementState.Involved;
            GameplayEventManager.BroadcastEvent(ev);
        }
    }
}