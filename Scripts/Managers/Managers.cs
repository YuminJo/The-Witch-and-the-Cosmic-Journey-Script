using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers s_instance = null;
    public static Managers Instance => s_instance;
    private GameManager _game = new();
    private ResourceManager _resource = new();
    private UIManager _ui = new();
    private SceneManagerEx _scene = new();
    private EventManager _event = new();
    
    public static GameManager Game { get { Init(); return Instance?._game; } }
    public static ResourceManager Resource { get { Init(); return Instance?._resource; } }
    public static UIManager UI { get { Init(); return Instance?._ui; } }
    public static SceneManagerEx Scene { get { Init(); return Instance?._scene; } }
    public static EventManager Event { get { Init(); return Instance?._event; } }

    private void Start()
    {
        Init();
        StartCoroutine(WaitforData());
    }
    public static void Init() {
        if (s_instance == null) {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            s_instance = Utils.GetOrAddComponent<Managers>(go);
            DontDestroyOnLoad(go);

            Application.targetFrameRate = 60;
        }
    }

    public IEnumerator WaitforData() {
        s_instance._game.Init();
        
        Debug.Log("Loaded Finished");
        yield return new WaitForSeconds(1f);
    }
}
