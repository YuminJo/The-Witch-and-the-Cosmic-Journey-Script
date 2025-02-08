using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    public static IDictionary<Type, object> Services { get => _services; }
    private static Dictionary<Type, object> _services = new();
    // ICardSystemProvider...
    // TurnStartServiceProvider...
    // 공변성 반공병성 제네릭 인터페이스
    // https://roseline.oopy.io/dev/what-is-variance
    
    // 서비스 등록
    public static void Register<T>(T service)
    {
        if (!_services.ContainsKey(typeof(T)))
        {
            _services[typeof(T)] = service;
        }
        else
        {
            throw new InvalidOperationException($"Service of type {typeof(T)} is already registered.");
        }
    }

    // 서비스 가져오기
    public static T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        else
        {
            throw new KeyNotFoundException($"Service of type {typeof(T)} is not registered.");
        }
    }
    
    // 서비스 제거
    public static void UnRegister<T>()
    {
        if (_services.ContainsKey(typeof(T)))
        {
            _services.Remove(typeof(T));
        }
        else
        {
            throw new KeyNotFoundException($"Service of type {typeof(T)} is not registered.");
        }
    }
}