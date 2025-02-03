using System.Collections;
using UnityEngine;

public class GameInitializer : MonoBehaviour {
    public static GameManager GameManager;
    public static DataManager DataManager;
    void Start() => Initialize();

    void Initialize() {
        Init();
        ServiceRegistration();
        StartWaitforData();
        Debug.Log("GameInitializer Initialized");
    }
    
    public void Init() {
        GameManager = new GameManager();
        DataManager = new DataManager();
        
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this);
    }
    
    void ServiceRegistration() {
        ServiceLocator.Register(GameManager);
        ServiceLocator.Register(DataManager);
        ServiceLocator.Register<IResourceManager>(new ResourceManager());
        ServiceLocator.Register<IUIManager>(new UIManager());
        //ServiceLocator.Register(new EventManager());
        //ServiceLocator.Register(new SceneManagerEx());
    }
    
    void StartWaitforData() => StartCoroutine(WaitforData());
    private IEnumerator WaitforData() {
        ServiceLocator.Get<GameManager>().Init();
        StartCoroutine(ServiceLocator.Get<DataManager>().LoadData());
        
        Debug.Log("Loaded Finished");
        yield return new WaitForSeconds(1f);
    }
}