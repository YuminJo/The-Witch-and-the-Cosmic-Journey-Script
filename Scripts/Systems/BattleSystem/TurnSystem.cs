using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.Serialization;

public class AttackOrder {
    public Enemy Enemy { get; set; }
    public Character Character { get; set; }

    public AttackOrder(Enemy enemy, Character character)
    {
        Enemy = enemy;
        Character = character;
    }
}

public class TurnSystem : UnitaskBase {
    [Header("Develop")]
    [SerializeField] private bool fastMode;
    [SerializeField] private int currentApCount; // Editor용 AP 카운트
    [SerializeField] private int cardLimit;
    [SerializeField] private int currentRound;
    [SerializeField] private int currentTurn;

    [Header("Properties")]
    // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭방지
    public bool IsLoading { get; private set; }
    
    [SerializeField] private ReactiveProperty<int> _currentAPCount = new();

    private enum ETurnMode { My, Enemy }
    private const float TURN_DELAY_SHORT = 0.05f;
    private const float TURN_DELAY_LONG = 0.7f;
    
    private List<Character> _characterList = new();
    private Queue<Enemy> _enemyQueue = new();
    private Queue<AttackOrder> _attackOrder = new();

    public void StartGame() => GameSetup();
    
    /// <summary>
    /// 게임의 초기 셋업
    /// </summary>
    private void GameSetup() {
        ServiceLocator.Get<ICardSystem>().SetTurnSystem(this);
        ServiceLocator.Get<ICardSystem>().SetupItemBuffer(cardLimit);
        NotProd();
        InitEditorData();
        SetCurrentBattleCharacterList();
        SetEnemyQueue();
        ShowBattleViewPopup().Forget();
    }

    private void InitEditorData() {
        _currentAPCount.Value = currentApCount;
    }

    /// <summary>
    /// 테스트 데이터 로드
    /// </summary>
    private void NotProd() {
        ServiceLocator.Get<IUIManager>().PeekPopupUI<BattleView>();
        
        Enemy enemy01 = new Enemy("enemy01", 100, 100, 4, 3, EnemyType.Normal);
        Enemy enemy02 = new Enemy("enemy02", 100, 100, 5, 3, EnemyType.Normal);
        Enemy enemy03 = new Enemy("enemy03", 100, 100, 6, 3, EnemyType.Normal);
        
        _enemyQueue.Enqueue(enemy01);
        _enemyQueue.Enqueue(enemy02);
        _enemyQueue.Enqueue(enemy03);
    }
    
    /// <summary>
    /// 현재 선택된 캐릭터 리스트를 생성자 복사해서 가져온다.
    /// </summary>
    private void SetCurrentBattleCharacterList() {
        var originalList = ServiceLocator.Get<IGameManager>().GetSelectedCharacters();
        _characterList = originalList.Select(character => new Character(character)).ToList();
    }

    /// <summary>
    /// 적의 공격 순서를 정한다.
    /// </summary>
    private void SetEnemyQueue() {
        _attackOrder.Clear();
        var orderedAttackOrders = _enemyQueue
            .Select(enemy => new AttackOrder(enemy, SelectRandomCharacter(_characterList)))
            .OrderBy(order => order.Enemy.Agi);
        _attackOrder = new Queue<AttackOrder>(orderedAttackOrders);
        _attackOrder.ToList().ForEach(order => Debug.Log($"Enemy: {order.Enemy.TemplateId}, Character: {order.Character.TemplateId}"));
    }
    private Character SelectRandomCharacter(List<Character> characters) 
        => characters[UnityEngine.Random.Range(0, characters.Count)];

    /// <summary>
    /// 전투 뷰 팝업을 보여준다.
    /// </summary>
    private async UniTask ShowBattleViewPopup() {
        bool isInit = false;
        BattleView battleView = null;

        ServiceLocator.Get<IUIManager>().ShowPopupUI<BattleView>(callback: (popup) => {
            battleView = popup;
            isInit = popup.Init();
        });

        await UniTask.WaitUntil(() => isInit);

        battleView.InitButton(TURN_DELAY_SHORT, EndTurn);
        _currentAPCount.Subscribe(value => battleView.SetEnergy(value)).AddTo(this);
        _characterList.ForEach(character => battleView.CreateCharacterView(character));
    }

    /// <summary>
    /// AP 코스트 사용
    /// </summary>
    public bool UseAPCost(int cost) {
        if (_currentAPCount.CurrentValue < cost) return false;
        _currentAPCount.Value -= cost;
        
        Debug.Log($"AP Cost: {cost}");
        return true;
    }
    
    /// <summary>
    /// 턴 종료 처리
    /// </summary>
    private void EndTurn() {
        IsLoading = true;
        
        Debug.Log("End Turn");
    }
}