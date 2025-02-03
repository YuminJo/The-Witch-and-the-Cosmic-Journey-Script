using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Serialization;

interface ITurnSystem {
    void StartGame();
}

public class TurnSystem : MonoBehaviour, ITurnSystem
{
    Queue<GameEntity> _entityQueue = new();

    [Header("Develop")]
    [SerializeField] private ETurnMode eTurnMode;
    [SerializeField] private bool fastMode;
    [SerializeField] private int startCardCount;
    [SerializeField] private int currentAPCount;
    [SerializeField] private int currentRound;
    [SerializeField] private int currentTurn;

    [Header("Properties")]
    public bool IsLoading { get; private set; } // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭방지
    private bool _myTurn;

    enum ETurnMode { My, Enemy }

    private const float TURN_DELAY_SHORT = 0.05f;
    private const float TURN_DELAY_LONG = 0.7f;
    
    private WaitForSeconds delayShort;
    private WaitForSeconds delayLong;
    
    [Range(0,3)] private List<Character> _currentCharacters = new();
    [Range(0,3)] private List<Enemy> _currentTurnEnemys = new();

    public static Action OnClickSkillButton;
    public static Action OnAddCard;
    public static Action OnEndTurn;
    public static event Action<bool> OnTurnStarted;

    public void StartGame() => ShowBattleViewPopup();
    private void ShowBattleViewPopup() => ServiceLocator.Get<UIManager>().ShowPopupUI<BattleView>(callback: (popup) => {
        GameSetup();
    });
    
    private void GameSetup() {
        OnClickSkillButton += ClickSkillButton;
        OnEndTurn += EndTurn;
        
        ServiceLocator.Get<ICardSystem>().Init();
        ServiceLocator.Get<ICardSystem>().SetupItemBuffer();
        OnLoadTestData();
        
        // fastMode에 따라 delay 설정
        delayShort = new WaitForSeconds(fastMode ? TURN_DELAY_SHORT : TURN_DELAY_LONG);
        delayLong = new WaitForSeconds(TURN_DELAY_LONG);

        // 턴 설정
        switch (eTurnMode) {
            case ETurnMode.My:
                _myTurn = true;
                break;
            case ETurnMode.Enemy:
                _myTurn = false;
                break;
        }
    }

    private void OnLoadTestData() {
        ServiceLocator.Get<UIManager>().PeekPopupUI<BattleView>();
        
        Enemy enemy01 = new Enemy("enemy01", 100, 100, 10, 4, 3, EnemyType.Normal);
        Enemy enemy02 = new Enemy("enemy02", 100, 100, 20, 5, 3, EnemyType.Normal);
        Enemy enemy03 = new Enemy("enemy03", 100, 100, 30, 6, 3, EnemyType.Normal);
    }

    public List<Character> GetCurrentBattleCharacters() {
        return _currentCharacters;
    }

    /// <summary>
    /// Set the order of attack according to agi value
    /// </summary>
    private void SetEntityQueue() {
        _entityQueue.Clear();
        
        // 캐릭터와 적의 순서를 정하기 위해 agi값을 비교하여 큐에 삽입
        //_currentSelectedCharacters.ForEach(character => _entityQueue.Enqueue(character));
        //_currentEnemys.ForEach(enemy => _entityQueue.Enqueue(enemy));
        
        // agi값을 기준으로 정렬
        _entityQueue = new Queue<GameEntity>(_entityQueue.OrderBy(entity => entity.Agi));

        //var entityComponent = _entityQueue.Peek().GetComponent<YourComponentType>();
    }

    private void ClickSkillButton() { StartCoroutine(ClickSkillButtonCo()); }
    
    private IEnumerator ClickSkillButtonCo() {
        IsLoading = true;

        for (int i = 0; i < startCardCount; i++) {
            yield return delayShort;
            OnAddCard?.Invoke();
        }
        StartCoroutine(StartTurnCo());
    }

    private IEnumerator StartTurnCo()
    {
        IsLoading = true;
        yield return delayLong;
        IsLoading = false;
        OnTurnStarted?.Invoke(_myTurn);
    }
    

    private void EndTurn() {
        //TODO: 턴 종료전 모든 행동 처리 확인 로직 추가
        if (!_myTurn) return;
        _myTurn = !_myTurn;
        Debug.Log(_myTurn ? "TurnManager: 나의 턴" : "TurnManager: 적의 턴");
    }

    private void NextEntity() {
        
    }
}