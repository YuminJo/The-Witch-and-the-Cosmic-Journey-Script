using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Object_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new();

    protected bool _init = false;
    
    public virtual bool Init()
    {
        if (_init)
            return false;

        Managers.Init();

        _init = true;
        return true;
    }
    
    private void Start()
    {
        Init();
    }
    
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.LogWarning($"Failed to bind({names[i]})");
        }
    }


    protected void BindObject(Type type) { Bind<GameObject>(type); }
    protected void BindSprite(Type type) { Bind<SpriteRenderer>(type); }
    protected void BindText(Type type) { Bind<TextMeshPro>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected SpriteRenderer GetSprite(int idx) { return Get<SpriteRenderer>(idx); }
    protected TextMeshPro GetText(int idx) { return Get<TextMeshPro>(idx); }
}
