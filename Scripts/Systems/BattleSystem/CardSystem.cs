using System;
using System.Collections.Generic;
using System.Linq;
using Battle.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.Cards;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.BattleSystem {
    public interface ICardSystem {
        void InitializeDependencies(ITurnSystem turnSystem, ICardBufferManager cardBufferManager
        , ICardViewManager cardViewManager);
        void CardMouseOver(BattleCardView battleCard, int apCount);
        void CardMouseExit(BattleCardView battleCard);
        void UseOrSelectCard(BattleCardView battleCard, Card cardData);
        UniTask GetCardByCardBuffer(float turnDelayShort);
        bool SelectEnemy(Enemy enemy, BattleEnemyView enemyView);
    }

    public class CardSystem : Object_Base, ICardSystem {
        enum GameObjects {
            CardSpawnPoint,
            MyCardLeft,
            MyCardRight,
            CardGroup
        }
        
        private ITurnSystem _turnSystem;
        private ICardBufferManager _cardBufferManager;
        private ICardViewManager _cardViewManager;
        private ICardEffectManager _cardEffectManager;
    
        private KeyValuePair<BattleCardView, Card> _selectedCard;
        private KeyValuePair<BattleEnemyView, Enemy> _selectedEnemy;
    
        private bool _isCardUsing;
        
        public void InitializeDependencies(ITurnSystem turnSystem, ICardBufferManager cardBufferManager
        , ICardViewManager cardViewManager) {
            _turnSystem = turnSystem;
            _cardBufferManager = cardBufferManager;
            _cardEffectManager = new CardEffectManager(_turnSystem, this);
            _cardViewManager = cardViewManager;
            
        }
    
        public override bool Init() {
            if (base.Init() == false)
                return false;
        
            BindObject(typeof(GameObjects));

            ServiceLocator.Register<ICardSystem>(this);
            
            _cardViewManager.Init(
                GetObject((int)GameObjects.MyCardLeft), 
                GetObject((int)GameObjects.MyCardRight));
        
            // 적 선택 완료 이벤트
            var enemySelectStream = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyUp(KeyCode.Space)) // 스페이스바를 눌렀을 때
                .ThrottleFirst(TimeSpan.FromMilliseconds(300)); // 300ms 동안 쓰로틀링

            // 선택된 카드가 있을 때만 처리
            enemySelectStream
                .Where(_ => _selectedEnemy.Value != null) // 선택된 적이 있을 때
                .Subscribe(_ => TargetSelect())  // 공격 함수 호출
                .AddTo(this); // 구독 해제 처리
            
        
            return true;
        }
        
        public void UnRegisterService() => ServiceLocator.UnRegister<ICardSystem>();
    
        /// <summary>
        /// 카드를 추가합니다.
        /// </summary>
        private void AddCard() {
            var cardObject = Instantiate(_cardViewManager.GetCardPrefab(), GetObject((int)GameObjects.CardSpawnPoint).transform.position, Utils.QI);
            var battleCardView = cardObject.GetComponent<BattleCardView>();
        
            cardObject.transform.SetParent(GetObject((int)GameObjects.CardGroup).transform);
            battleCardView.SetCardData(_cardBufferManager.GetHandCardBuffer()[_cardViewManager.GetCardCount]).Forget();
            _cardViewManager.AddCard(battleCardView);
        }
    
        /// <summary>
        /// 카드 버퍼에서 카드를 가져와서 카드 Object를 생성합니다.
        /// </summary>
        /// <param name="turnDelayShort">카드를 가져오는 지연 시간</param>
        public async UniTask GetCardByCardBuffer(float turnDelayShort) {
            if (_cardBufferManager.GetHandCardBuffer().Count == 0) return;
        
            while (_cardViewManager.GetCardCount < _cardBufferManager.GetHandCardBuffer().Count) {
                await UniTask.Delay(TimeSpan.FromSeconds(turnDelayShort));
                AddCard();
            }
        }
    
        /// <summary>
        /// 카드가 isTargetAll에 따라 전체 대상인지 아닌지에 따라 카드를 사용하거나 선택합니다.
        /// </summary>
        /// <param name="battleCard"></param>
        /// <param name="cardData"></param>
        public void UseOrSelectCard(BattleCardView battleCard, Card cardData) {
            if (_isCardUsing) return; _isCardUsing = true;
        
            //TODO: 카드 사용 로직
            //Card Disappear Logic
            _cardViewManager.ResetBattleCardView();
            _cardViewManager.DeOverLayEnergy();
        
            //Target 판별
            _selectedCard = new KeyValuePair<BattleCardView, Card>(battleCard, cardData);
            if (_selectedCard.Value.IsTargetAll()) { _cardEffectManager.UseCost(_selectedCard.Value); }
        }

        private void TargetSelect() {
            if ( _selectedEnemy.Value == null || _selectedCard.Value == null) return;
            //선택 대상 표시 초기화
            _selectedEnemy.Key.TargetSelected(false);
            // 카드 사용 로직
            _cardEffectManager.UseCost(_selectedCard.Value, _selectedEnemy.Value);
        }

        public void UseCard() => _cardBufferManager.GetHandCardBuffer().Remove(_selectedCard.Value);
    
        /// <summary>
        /// 선택된 적과 카드를 초기화합니다.
        /// </summary>
        public void InitSelectedEnemyAndCard() {
            _selectedEnemy = new KeyValuePair<BattleEnemyView, Enemy>(null, null);
            _selectedCard = new KeyValuePair<BattleCardView, Card>(null, null);
            _isCardUsing = false;
        }
        
        /// <summary>
        /// 선택된 적을 저장합니다.
        /// </summary>
        /// <param name="enemy">적 데이터</param>
        /// <param name="enemyView">적 View</param>
        public bool SelectEnemy(Enemy enemy,BattleEnemyView enemyView) {
            if (_selectedCard.Value == null) return false;
            if (_selectedEnemy.Key != null) _selectedEnemy.Key.TargetSelected(false);
            
            _selectedEnemy = new KeyValuePair<BattleEnemyView, Enemy>(enemyView, enemy);
            return true;
        }

        public void CardMouseOver(BattleCardView battleCardView, int apCount) {
            if (_isCardUsing) return;
            _cardViewManager.EnlargeCard(true, battleCardView);
            _cardViewManager.ShowOverLayEnergy(apCount, battleCardView);
        }

        public void CardMouseExit(BattleCardView battleCard) {
            if (_isCardUsing) return;
            _cardViewManager.EnlargeCard(false, battleCard);
            _cardViewManager.DeOverLayEnergy();
        }
    }
}