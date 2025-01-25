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