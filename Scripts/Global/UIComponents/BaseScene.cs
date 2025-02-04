using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseScene : MonoBehaviour
{
    public SceneType SceneType = SceneType.Unknown;
    protected bool _init = false;

    public void Awake()
    {
        Init();
    }

    protected virtual bool Init() {
        if (_init)
            return false;

        _init = true;
        
        GameObject initalizerObject = GameObject.Find("@GameInitializer");
        GameObject eventObject = GameObject.Find("EventSystem");

        if (initalizerObject == null) {
            if (initalizerObject == null)
                initalizerObject = new GameObject { name = "@GameInitializer" };

            Utils.GetOrAddComponent<GameInitializer>(initalizerObject);
            DontDestroyOnLoad(initalizerObject);
        }

        if (eventObject == null) {
            ServiceLocator.Get<IResourceManager>().Instantiate("EventSystem", null, (go) =>
            {
                go.name = "@EventSystem";
            });
        }

        return true;
    }
    

    public virtual void Clear()
    {

    }
}
