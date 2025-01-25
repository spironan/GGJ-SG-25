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

    public GameplayEventListener()
    {
        GameplayEventManager.AddListener(this);
    }

    ~GameplayEventListener()
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
    private int unsortedCount = 0;
    public int UnsortedCount { get { return unsortedCount; } }

    public static void BroadcastEvent(int unsortedCount)
    {
        StartIterationGameplayEvent ev = new StartIterationGameplayEvent();
        ev.unsortedCount = unsortedCount;
        GameplayEventManager.BroadcastEvent(ev);
    }
}

public class StageCompleteGameplayEvent : BaseGameplayEvent
{
    public static void BroadcastEvent()
    {
        StageCompleteGameplayEvent ev = new StageCompleteGameplayEvent();
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