using System;
using System.Collections.Generic;
using System.Linq;
using Battle.View;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using ValueType = Entities.Cards.ValueType;

namespace Systems.BattleSystem {
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
        [SerializeField] private int currentRound = 1;
        [SerializeField] private int currentTurn = 1;

        [Header("Properties")]
        [SerializeField] private ReactiveProperty<int> _currentAPCount = new();

        private enum ETurnMode { My, Enemy }
        private const float TURN_DELAY_SHORT = 0.05f;
        private const float TURN_DELAY_LONG = 0.7f;
    
        public List<Character> CharacterList { get; private set; } = new();
        public List<Enemy> EnemyList { get; private set; } = new();
        private Queue<AttackOrder> _attackOrder = new();
        
        private IBattleView _battleView;

        public void StartGame() => GameSetup();
    
        /// <summary>
        /// 게임의 초기 셋업
        /// </summary>
        private void GameSetup() {
            ServiceLocator.Get<ICardSystem>().SetTurnSystem(this);
            ServiceLocator.Get<ICardSystem>().SetupItemBuffer(cardLimit);
            NotProd();
            InitEditorData();
            InitializeCurrentBattleCharacterList();
            SetEnemyQueue();
            ShowBattleViewPopup().Forget();
        }

        private void InitEditorData() {
            _currentAPCount.Value = currentApCount;
        }
        
        public List<GameEntity> GetEntityList(bool isFindEnemy) {
            return isFindEnemy ? EnemyList.Cast<GameEntity>().ToList() : CharacterList.Cast<GameEntity>().ToList();
        }

        /// <summary>
        /// 테스트 데이터 로드
        /// </summary>
        private void NotProd() {
            ServiceLocator.Get<IUIManager>().PeekPopupUI<BattleView>();
        
            Enemy enemy01 = new Enemy("enemy01", 1000, 100, 40, 3, EnemyType.Normal);
            Enemy enemy02 = new Enemy("enemy02", 1000, 100, 50, 3, EnemyType.Normal);
            Enemy enemy03 = new Enemy("enemy03", 1000, 100, 60, 3, EnemyType.Normal);
        
            EnemyList.Add(enemy01);
            EnemyList.Add(enemy02);
            EnemyList.Add(enemy03);
        
            Character sample01 = new Character("sample01", 1000, 100, 10, 3, CharacterType.Tanker);
            Character sample02 = new Character("sample02", 1000, 100, 20, 3, CharacterType.Dealer);
            Character sample03 = new Character("sample03", 1000, 100, 30, 3, CharacterType.Supporter);
            CharacterList.Add(sample01);
            CharacterList.Add(sample02);
            CharacterList.Add(sample03);
        }
    
        /// <summary>
        /// 현재 선택된 캐릭터 리스트를 생성자 복사해서 가져온다.
        /// </summary>
        private void InitializeCurrentBattleCharacterList() {
            //var originalList = ServiceLocator.Get<IGameManager>().GetSelectedCharacters();
            //_characterList = originalList.Select(character => new Character(character)).ToList();
        }

        /// <summary>
        /// 적의 공격 순서를 정한다.
        /// </summary>
        private void SetEnemyQueue() {
            _attackOrder.Clear();
            var orderedAttackOrders = EnemyList
                .Select(enemy => new AttackOrder(enemy, SelectRandomCharacter(CharacterList)))
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

            ServiceLocator.Get<IUIManager>().ShowPopupUI<BattleView>(callback: (popup) => {
                _battleView = popup;
                isInit = popup.Init();
            });

            await UniTask.WaitUntil(() => isInit);

            _battleView.InitButton(TURN_DELAY_SHORT, EndTurn);
            _currentAPCount.Subscribe(value => _battleView.SetEnergy(value)).AddTo(this);
            CharacterList.ForEach(character => _battleView.CreateCharacterView(character));
            EnemyList.ForEach(CreateEnemyView);
        }

        private void CreateEnemyView(Enemy enemy) {
            var enemyGroup = gameObject;
            
            ServiceLocator.Get<IResourceManager>().Instantiate(nameof(BattleEnemyView), enemyGroup.transform, (go) => {
                var battleEnemyView = go.GetComponent<BattleEnemyView>();
                battleEnemyView.SetEnemyData(enemy);
            });
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
            Debug.Log("End Turn");
            ExecuteAttackOrder().Forget();
        }
        
        /// <summary>
        /// 공격 명령 처리
        /// </summary>
        private async UniTask ExecuteAttackOrder() {
            AttackOrder order = _attackOrder.Dequeue();
            
            // 캐릭터의 공격 애니메이션 처리
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            
            // 적의 공격 처리
            // 스킬이 있는 경우 별도의 처리 필요
            Debug.Log(order.Enemy.Atk);
            CardEffects.OnDamage(order.Character, order.Enemy.Atk, ValueType.Percent, 300);

            if (_attackOrder.Count != 0) { ExecuteAttackOrder().Forget(); }
            else { NextTurn(); }
        }

        private void NextTurn() {
            // 버프 및 디버프 처리

            currentTurn++;
            ServiceLocator.Get<ICardSystem>().SetupItemBuffer(cardLimit);
            
            _battleView.IsPlayerTurn(SetEnemyQueue);
        }
    }
}