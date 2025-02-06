using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;

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
    [SerializeField] private int startCardCount;
    [SerializeField] private int currentAPCount;
    [SerializeField] private int currentRound;
    [SerializeField] private int currentTurn;

    [Header("Properties")]
    // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭방지
    public bool IsLoading { get; private set; }

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
        ServiceLocator.Get<ICardSystem>().SetupItemBuffer();
        NotProd();
        SetCurrentBattleCharacterList();
        SetEnemyQueue();
        ShowBattleViewPopup();
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
    private void ShowBattleViewPopup() 
        => ServiceLocator.Get<IUIManager>().ShowPopupUI<BattleView>(callback: (popup)
            => {
            popup.OnClickCardSelectButton(this).Forget();
            _characterList.ForEach(character => popup.CreateCharacterView(character).Forget());
        });
    
    public async UniTask ClickSkillButtonAsync() {
        IsLoading = true;
        
        var cardSystem = ServiceLocator.Get<ICardSystem>();
        for (int i = 0; i < startCardCount; i++) {
            await UniTask.Delay(TimeSpan.FromSeconds(TURN_DELAY_SHORT));
            cardSystem.AddCard();
        }
        
        //await StartTurnAsync();
    }

    /*private async UniTask StartTurnAsync() {
        IsLoading = true;
        await UniTask.Delay(TimeSpan.FromSeconds(TURN_DELAY_LONG));
        IsLoading = false;
    }*/
}