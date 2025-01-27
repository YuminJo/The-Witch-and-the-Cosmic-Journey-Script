using System;
using System.Collections.Generic;

public class EventManager
{
    private static readonly Dictionary<string, Delegate> eventDictionary = new();

    // 리스너 추가 (제너릭으로 개선)
    public void AddListener<T>(string eventName, Action<T> listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = null;
        }

        eventDictionary[eventName] = (Action<T>)eventDictionary[eventName] + listener;
    }

    // 리스너 제거 (제너릭으로 개선)
    public void RemoveListener<T>(string eventName, Action<T> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = (Action<T>)eventDictionary[eventName] - listener;
        }
    }

    // 이벤트 호출 (제너릭으로 개선)
    public void Invoke<T>(string eventName, T arg)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            var action = eventDictionary[eventName] as Action<T>;
            action?.Invoke(arg);
        }
    }
}