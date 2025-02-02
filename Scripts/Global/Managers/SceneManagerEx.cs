using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class SceneManagerEx
{
    private SceneType _curSceneType = SceneType.Unknown;

    public void Init()
    {
    }

    public SceneType CurrentSceneType
    {
        get
        {
            if (_curSceneType != SceneType.Unknown)
                return _curSceneType;
            return CurrentScene.SceneType;
        }
        set {  _curSceneType = value; }
    }

    public BaseScene CurrentScene => Object.FindAnyObjectByType<BaseScene>();

    public void ChangeScene(SceneType type)
    {
        if(CurrentScene != null) CurrentScene.Clear();
        _curSceneType = type;
        
        Debug.Log($"ChangeScene: {_curSceneType}");
        SceneManager.LoadScene(GetSceneName(CurrentSceneType));
    }

    string GetSceneName(SceneType type) => System.Enum.GetName(typeof(SceneType), type);
}