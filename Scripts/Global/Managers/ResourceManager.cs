using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public interface IResourceManager
{
    UniTask<T> LoadAsync<T>(string key = null) where T : UnityEngine.Object;
    void Release(string key);
    void Clear();
    UniTask<GameObject> Instantiate(string key, Transform parent = null);
    void Destroy(GameObject go, float seconds = 0.0f);
}

public class ResourceManager : IResourceManager
{
    private readonly Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();
    private readonly Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();
    public int HandlesCount = 0;

    public async UniTask<T> LoadAsync<T>(string key = null) where T : UnityEngine.Object
    {
        if (_resources.TryGetValue(key, out Object resource))
        {
            return resource as T;
        }

        if (_handles.ContainsKey(key))
        {
            var handle = _handles[key];
            await handle.Task;
            return handle.Result as T;
        }

        var newHandle = Addressables.LoadAssetAsync<T>(key);
        _handles.Add(key, newHandle);
        HandlesCount++;

        var result = await newHandle.Task;
        _resources.Add(key, result as UnityEngine.Object);
        HandlesCount--;

        return result;
    }

    public void Release(string key)
    {
        if (_resources.TryGetValue(key, out Object resource))
        {
            _resources.Remove(key);
        }

        if (_handles.TryGetValue(key, out AsyncOperationHandle handle))
        {
            Addressables.Release(handle);
            _handles.Remove(key);
        }
    }

    public void Clear()
    {
        _resources.Clear();

        foreach (var handle in _handles.Values)
        {
            Addressables.Release(handle);
        }

        _handles.Clear();
    }

    public async UniTask<GameObject> Instantiate(string key, Transform parent = null)
    {
        var prefab = await LoadAsync<GameObject>(key);
        var go = UnityEngine.Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        go.transform.localPosition = prefab.transform.position;
        return go;
    }

    public void Destroy(GameObject go, float seconds = 0.0f)
    {
        if (seconds == 0.0f)
        {
            UnityEngine.Object.Destroy(go);
        }
        else
        {
            UnityEngine.Object.Destroy(go, seconds);
        }
    }
}