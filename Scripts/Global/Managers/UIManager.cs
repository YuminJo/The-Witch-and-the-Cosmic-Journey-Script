using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IUIManager {
    void SetCanvas(GameObject go, bool sort = true, bool canvascamera = false);
    void MakeSubItem<T>(Transform parent = null, string key = null, Action<T> callback = null) where T : UI_Base;
    void ShowSceneUI<T>(string key = null, Action<T> callback = null) where T : UI_Scene;
    void ShowPopupUI<T>(string key = null, Transform parent = null, Action<T> callback = null) where T : UI_Popup;
    T FindPopup<T>() where T : UI_Popup;
    T PeekPopupUI<T>() where T : UI_Popup;
    void ClosePopupUI(UI_Popup popup);
    void ClosePopupUI();
    void CloseAllPopupUI();
    void Clear();
}

public class UIManager : IUIManager
{
    int _order = 20;
    
    Stack<UI_Popup> _popupStack = new();
    private UI_Scene SceneUI { get; set; }
    private GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true , bool canvascamera = false)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
        if (canvascamera)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
        }

        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public void MakeSubItem<T>(Transform parent = null, string key = null, Action<T> callback = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        ServiceLocator.Get<IResourceManager>().Instantiate(key, parent, (go) =>
        {
            T subItem = Utils.GetOrAddComponent<T>(go);
            callback?.Invoke(subItem);
        });
    }

    public void ShowSceneUI<T>(string key = null, Action<T> callback = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        ServiceLocator.Get<IResourceManager>().Instantiate(key, Root.transform, (go) =>
        {
            T sceneUI = Utils.GetOrAddComponent<T>(go);
            SceneUI = sceneUI;
            callback?.Invoke(sceneUI);
        });
    }

    public void ShowPopupUI<T>(string key = null, Transform parent = null, Action<T> callback = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        ServiceLocator.Get<IResourceManager>().Instantiate(key, null, (go) =>
        {
            T popup = Utils.GetOrAddComponent<T>(go);
            _popupStack.Push(popup);

            if (parent != null)
                go.transform.SetParent(parent);
            else
                go.transform.SetParent(Root.transform);

            callback?.Invoke(popup);
        });
    }

    public T FindPopup<T>() where T : UI_Popup
    {
        return _popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    public T PeekPopupUI<T>() where T : UI_Popup
    {
        if (_popupStack.Count == 0) {
            return null;
        }

        return _popupStack.Peek() as T;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        ServiceLocator.Get<IResourceManager>().Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        SceneUI = null;
    }
}
