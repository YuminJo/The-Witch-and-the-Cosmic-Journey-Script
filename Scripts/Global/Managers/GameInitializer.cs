using System.Collections;
using UnityEngine;

public class GameInitializer : MonoBehaviour {
    private GameManager _gameManager;
    private DataManager _dataManager;
    void Start() => Initialize();

    void Initialize() {
        Init();
        ServiceRegistration();
        StartWaitforData();
        Debug.Log("GameInitializer Initialized");
    }
    
    public void Init() {
        _gameManager = new GameManager();
        _dataManager = new DataManager();
        
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this);
    }
    
    void ServiceRegistration() {
        ServiceLocator.Register<IGameManager>(_gameManager);
        ServiceLocator.Register(_dataManager);
        ServiceLocator.Register<IResourceManager>(new ResourceManager());
        ServiceLocator.Register<IUIManager>(new UIManager());
        //ServiceLocator.Register(new EventManager());
        //ServiceLocator.Register(new SceneManagerEx());
    }
    
    void StartWaitforData() => StartCoroutine(WaitforData());
    private IEnumerator WaitforData() {
        ServiceLocator.Get<IGameManager>().Init();
        StartCoroutine(ServiceLocator.Get<DataManager>().LoadData());
        
        Debug.Log("Loaded Finished");
        yield return new WaitForSeconds(1f);
    }
}