using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ValueType = Entities.Cards.ValueType;


[System.Serializable]
public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}

public class Utils
{
    public static Quaternion QI => Quaternion.identity;

    public static Vector3 MousePos
    {
        get
        {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            result.z = -10;
            return result;
        }
    }
    
    public static T ParseEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object {
        if (go == null)
            return null;

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }
    
    public static float GetHpByPercent(int currentHp, int maxHp) => (float)currentHp / maxHp;
    
    /// <summary>
    /// ValueType에 따라서 Value를 계산하여 반환한다.
    /// </summary>
    /// <param name="valueType">값의 타입</param>
    /// <param name="entityStatValue">Entity의 ATK 스탯</param>
    /// <param name="value">카드의 값</param>
    /// <returns></returns>
    public static int GetValueByValueType(ValueType valueType, int entityStatValue, int value) {
        float result;
        
        if (valueType == ValueType.Percent) result = entityStatValue * ((float)value / 100);
        else result = value + entityStatValue;
        
        return (int)result;
    }

    public static void GlobalException(string msg) => Debug.LogError(":::GLOBAL EXCEPTION::: " + msg);
    
    public static List<T> GetTargetEntitiesByRange<T>(int range, List<T> allEntities, T selectedEntity) where T : class {
        List<T> targetEntities = new();
        int selectEntityIndex = allEntities.IndexOf(selectedEntity);
        
        for (int i = 0; i <= range / 2; i++) {
            int leftIndex = selectEntityIndex - i;
            int rightIndex = selectEntityIndex + i;

            if (leftIndex >= 0) {
                targetEntities.Add(allEntities[leftIndex]);
            }
            if (rightIndex < allEntities.Count && rightIndex != leftIndex) {
                targetEntities.Add(allEntities[rightIndex]);
            }
        }

        return targetEntities;
    }
}