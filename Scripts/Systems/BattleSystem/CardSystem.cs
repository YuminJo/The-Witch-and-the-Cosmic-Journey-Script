using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.Cards;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.BattleSystem {
    public interface ICardSystem {
        void SetTurnSystem(TurnSystem turnSystem);
        void SetupItemBuffer(int startCardCount = 5);
        void CardMouseOver(BattleCardView battleCard);
        void CardMouseExit(BattleCardView battleCard);
        void UseOrSelectCard(BattleCardView battleCard, Card cardData);
        void ScaleCard(BattleCardView battleCard, Vector2 scale, float duration);
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
    
        //TODO: 나중에 카드 데이터를 불러오는 방식을 바꿔야함
        private TurnSystem _turnSystem;
        private List<Card> _handCardBuffer = new();
        private ObservableList<BattleCardView> _myCards = new();
        private GameObject _cardPrefab;
    
        private KeyValuePair<BattleCardView, Card> _selectedCard;
        private KeyValuePair<BattleEnemyView, Enemy> _selectedEnemy;
    
        private bool _isCardUsing;
    
        public override bool Init() {
            if (base.Init() == false)
                return false;
        
            BindObject(typeof(GameObjects));

            ServiceLocator.Register<ICardSystem>(this);
            ServiceLocator.Get<IResourceManager>().LoadAsync<GameObject>(nameof(BattleCardView), (result) => _cardPrefab = result);
        
            // 카드 추가/제거 이벤트
            _myCards.ObserveAdd().Subscribe(_ => UpdateCardAlignment()).AddTo(this);
            //_myCards.ObserveRemove().Subscribe(_ => UpdateCardAlignment()).AddTo(this);
        
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
    
        public void SetTurnSystem(TurnSystem turnSystem) => _turnSystem = turnSystem;
        public void UnRegisterService() => ServiceLocator.UnRegister<ICardSystem>();

        /// <summary>
        /// Data Manager에서 카드 데이터를 불러와 섞어서 버퍼에 저장합니다.
        /// _handCardBuffer에는 기본적으로 5장의 카드가 들어가 있습니다.
        /// 턴이 끝나면 기존에 남아있는 카드는 유지하고 새로운 카드를 추가합니다.
        /// </summary>
        /// <param name="limitCardCount">카드 제한 개수</param>
        public void SetupItemBuffer(int limitCardCount = 5) {
            var dataManager = ServiceLocator.Get<DataManager>();
            int cardsToAdd = limitCardCount - _handCardBuffer.Count;

            if (cardsToAdd > 0) {
                _handCardBuffer.AddRange(dataManager.Cards.Values
                    .OrderBy(_ => Random.value)
                    .Take(cardsToAdd));
            }
        }
    
        /// <summary>
        /// 카드를 추가합니다.
        /// </summary>
        private void AddCard() {
            var cardObject = Instantiate(_cardPrefab, GetObject((int)GameObjects.CardSpawnPoint).transform.position, Utils.QI);
            var battleCardView = cardObject.GetComponent<BattleCardView>();
        
            cardObject.transform.SetParent(GetObject((int)GameObjects.CardGroup).transform);
            battleCardView.SetCardData(_handCardBuffer[_myCards.Count]).Forget();
            _myCards.Add(battleCardView);  // 리스트에 추가하면 자동으로 반응
        }
    
        /// <summary>
        /// 카드 버퍼에서 카드를 가져와서 카드 Object를 생성합니다.
        /// </summary>
        /// <param name="turnDelayShort">카드를 가져오는 지연 시간</param>
        public async UniTask GetCardByCardBuffer(float turnDelayShort) {
            if (_handCardBuffer.Count == 0) return;
        
            while (_myCards.Count < _handCardBuffer.Count) {
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
            ResetBattleCardView();
        
            //Target 판별
            _selectedCard = new KeyValuePair<BattleCardView, Card>(battleCard, cardData);
            if (cardData.IsTargetAll) { UseCost(); }
        }

        private void TargetSelect() {
            if ( _selectedEnemy.Value == null || _selectedCard.Value == null) return;
            UseCost();
            DeactiveTargetSelect();
        }
        
        /// <summary>
        /// 코스트 사용 처리
        /// </summary>
        private void UseCost() {
            if (!_turnSystem.UseAPCost(_selectedCard.Value.Ap)) { return; }
            EffectByType().Forget();
        }

        private void UseCard() => _handCardBuffer.Remove(_selectedCard.Value);
    
        private void ResetBattleCardView() {
            //TODO 나중에 Destroy 대신에 Object Pooling으로 변경
            _myCards.ForEach(card => Destroy(card.gameObject));
            _myCards.Clear();
        }

        /// <summary>
        /// 카드의 타입에 따라 효과를 실행합니다.
        /// </summary>
        private async UniTask EffectByType() {
            // 카드 사용 처리
            UseCard();
            
            // 전체공격 선택공격 판별
            

            // 효과 맵핑
            var effectActions = new Dictionary<EffectType, Action<Effect>> {
                { EffectType.Attack, effect => CardEffects.OnDamage(_selectedEnemy.Value, 50, effect.ValueType, effect.Value) },
                { EffectType.Heal, effect => CardEffects.OnHeal(50, _selectedEnemy.Value, effect) }
            };

            // 효과 적용
            foreach (var effect in _selectedCard.Value.Effects) {
                if (!effectActions.TryGetValue(effect.Type, out var action)) {
                    action = e => CardEffects.OnBuff(e, _selectedEnemy.Value);
                }
                action(effect);
                await UniTask.Delay(1000);
            }

            InitSelectedEnemyAndCard();
            _isCardUsing = false;
        }
    
        /// <summary>
        /// 선택된 적과 카드를 초기화합니다.
        /// </summary>
        private void InitSelectedEnemyAndCard() {
            _selectedEnemy = new KeyValuePair<BattleEnemyView, Enemy>(null, null);
            _selectedCard = new KeyValuePair<BattleCardView, Card>(null, null);
        }

        private void DeactiveTargetSelect() => _selectedEnemy.Key.TargetSelected(false);
    
        /// <summary>
        /// 선택된 적을 저장합니다.
        /// </summary>
        /// <param name="enemy">적 데이터</param>
        public bool SelectEnemy(Enemy enemy,BattleEnemyView enemyView) {
            if (_selectedCard.Value == null) return false;
            _selectedEnemy = new KeyValuePair<BattleEnemyView, Enemy>(enemyView, enemy);
            return true;
        }

        public void CardMouseOver(BattleCardView battleCard) {
            if (_isCardUsing) return;
            EnlargeCard(true, battleCard);
        }

        public void CardMouseExit(BattleCardView battleCard) {
            if (_isCardUsing) return;
            EnlargeCard(false, battleCard);
        }
    
        public void ScaleCard(BattleCardView battleCard, Vector2 scale, float duration) {
            if (_isCardUsing) return;
            DOTween.Kill(battleCard.transform);
            battleCard.transform.DOScale(scale, duration).SetEase(Ease.InCirc);
        }

        #region Card Alignment
        void SetOriginOrder() {
            for (int i = 0; i < _myCards.Count; i++) {
                _myCards[i]?.GetComponent<Order>().SetOriginOrder(i);
            }
        }
    
        private void UpdateCardAlignment() {
            SetOriginOrder();
            CardAlignment();
        }
    
        void CardAlignment() {
            List<PRS> originCardPRSs = RoundAlignment(_myCards.Count, 0.5f, Vector3.one * BattleCardView.CardSize);

            var targetCards = _myCards;
            for (int i = 0; i < targetCards.Count; i++) {
                var targetCard = targetCards[i];

                targetCard.originPrs = originCardPRSs[i];
                MoveTransform(targetCard, targetCard.originPrs, true, 0.7f);
            }
        }

        List<PRS> RoundAlignment(int objCount, float height, Vector3 scale) {
            float[] objLerps = new float[objCount];
            List<PRS> results = new List<PRS>(objCount);

            switch (objCount) {
                case 1:
                    objLerps = new float[] { 0.5f };
                    break;
                case 2:
                    objLerps = new float[] { 0.27f, 0.73f };
                    break;
                case 3:
                    objLerps = new float[] { 0.1f, 0.5f, 0.9f };
                    break;
                default:
                    float interval = 1f / (objCount - 1);
                    for (int i = 0; i < objCount; i++)
                        objLerps[i] = interval * i;
                    break;
            }
        
            var leftTr = GetObject((int)GameObjects.MyCardLeft).transform;
            var rightTr = GetObject((int)GameObjects.MyCardRight).transform;
            for (int i = 0; i < objCount; i++) {
                var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
                var targetRot = Utils.QI;
                if (objCount >= 4) {
                    float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                    curve = height >= 0 ? curve : -curve;
                    targetPos.y += curve;
                    targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
                }

                results.Add(new PRS(targetPos, targetRot, scale));
            }

            return results;
        }

        void EnlargeCard(bool isEnlarge, BattleCardView battleCard) {
            DOTween.Kill(battleCard.transform);
            if (isEnlarge) {
                Vector3 enlargePos = new Vector3(battleCard.originPrs.pos.x, -4.8f, -10f);
                MoveTransform(battleCard, new PRS(enlargePos, Utils.QI, Vector3.one * 2.5f), true, 0.5f);
                AdjustOtherCards(battleCard, true);
            } else {
                MoveTransform(battleCard, battleCard.originPrs, true, 0.3f);
                AdjustOtherCards(battleCard, false);
            }

            battleCard.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
        }

        void AdjustOtherCards(BattleCardView centerBattleCard, bool isEnlarge) {
            float offset = 3.0f;
            for (int i = 0; i < _myCards.Count; i++) {
                if (_myCards[i] == centerBattleCard) continue;

                Vector3 newPos = _myCards[i].originPrs.pos;
                if (isEnlarge) {
                    if (_myCards[i].originPrs.pos.x < centerBattleCard.originPrs.pos.x) {
                        newPos.x -= offset;
                    } else {
                        newPos.x += offset;
                    }
                }

                MoveTransform(_myCards[i], new PRS(newPos, _myCards[i].originPrs.rot, _myCards[i].originPrs.scale), true, 0.5f);
            }
        }

        public void MoveTransform(BattleCardView battleCard, PRS prs, bool useDotween, float dotweenTime = 0) {
            DOTween.Kill(battleCard.transform);
            if (useDotween) {
                battleCard.transform.DOLocalMove(prs.pos, dotweenTime).SetEase(Ease.OutQuart);
                battleCard.transform.DORotateQuaternion(prs.rot, dotweenTime).SetEase(Ease.OutQuart);
                battleCard.transform.DOScale(prs.scale, dotweenTime).SetEase(Ease.OutQuart);
            } else {
                battleCard.transform.position = prs.pos;
                battleCard.transform.rotation = prs.rot;
                battleCard.transform.localScale = prs.scale;
            }
        }
        #endregion
    }
}