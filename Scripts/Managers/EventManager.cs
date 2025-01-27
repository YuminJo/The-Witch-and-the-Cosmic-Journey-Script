using System;
using System.Collections.Generic;

public class EventManager
{
    private static readonly Dictionary<string, Action> eventDictionary = new();

    public void AddListener(string eventName, Action listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = null;
        }

        eventDictionary[eventName] += listener;
    }

    public void RemoveListener(string eventName, Action listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }

    public void Invoke(string eventName)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName]?.Invoke();
        }
    }
}
