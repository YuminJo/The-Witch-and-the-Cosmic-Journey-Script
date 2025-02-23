using System.Collections.Generic;
using Battle.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Systems.BattleSystem {
    public interface ICardViewManager {
        ObservableList<BattleCardView> MyCards { get; }
        int GetCardCount { get; }
        void Init(GameObject myCardLeft, GameObject myCardRight);
        void AddCard(BattleCardView battleCardView);
        void ResetBattleCardView();
        void ShowOverLayEnergy(int value, BattleCardView battleCardView);
        void DeOverLayEnergy();
        GameObject GetCardPrefab();
        void UpdateCardAlignment();
        void EnlargeCard(bool isEnlarge, BattleCardView battleCard);
    }
    
    public class CardViewManager : ICardViewManager {
        private GameObject _cardPrefab;
        public ObservableList<BattleCardView> MyCards { get; } = new();
        
        private GameObject _myCardLeft;
        private GameObject _myCardRight;

        public void Init(GameObject myCardLeft, GameObject myCardRight) {
            _myCardLeft = myCardLeft;
            _myCardRight = myCardRight;
            
            InitCardPrefab();
            // 카드 추가/제거 이벤트
            MyCards.ObserveAdd().Subscribe(_ => UpdateCardAlignment()).AddTo(myCardLeft);
            //_myCards.ObserveRemove().Subscribe(_ => UpdateCardAlignment()).AddTo(this);
        }
        
        private void InitCardPrefab() {
            ServiceLocator.Get<IResourceManager>().LoadAsync<GameObject>(nameof(BattleCardView)).ContinueWith(
                result => _cardPrefab = result).Forget();
        }
        
        public int GetCardCount => MyCards.Count;
        
        /// <summary>
        /// 카드 추가
        /// 리스트에 추가하면 자동으로 정렬
        /// </summary>
        /// <param name="battleCardView"></param>
        public void AddCard(BattleCardView battleCardView) 
            => MyCards.Add(battleCardView);
        
        public void ResetBattleCardView() {
            //TODO 나중에 Destroy 대신에 Object Pooling으로 변경
            MyCards.ForEach(card => DOTween.Kill(card.transform));
            MyCards.ForEach(card => Object.Destroy(card.gameObject));
            MyCards.Clear();
        }

        // 카드 에너지 오버레이 표시
        public void ShowOverLayEnergy(int value, BattleCardView battleCardView) {
            var battleView = ServiceLocator.Get<IUIManager>().FindPopup<BattleView>();
            battleView.OverLayEnergy(value, battleCardView);
        }

        // 카드 에너지 오버레이 제거
        public void DeOverLayEnergy() {
            var battleView = ServiceLocator.Get<IUIManager>().FindPopup<BattleView>();
            battleView.DeOverLayEnergy();
        }
    
        public GameObject GetCardPrefab() {
            return _cardPrefab;
        }
    
        #region Card Alignment
        void SetOriginOrder() {
            for (int i = 0; i < MyCards.Count; i++) {
                MyCards[i]?.GetComponent<Order>().SetOriginOrder(i);
            }
        }
    
        public void UpdateCardAlignment() {
            SetOriginOrder();
            CardAlignment();
        }
    
        void CardAlignment() {
            List<PRS> originCardPRSs = RoundAlignment(MyCards.Count, 0.5f, Vector3.one * BattleCardView.CardSize);

            var targetCards = MyCards;
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
        
            var leftTr = _myCardLeft.transform;
            var rightTr = _myCardRight.transform;
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

        public void EnlargeCard(bool isEnlarge, BattleCardView battleCard) {
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
            for (int i = 0; i < MyCards.Count; i++) {
                if (MyCards[i] == centerBattleCard) continue;

                Vector3 newPos = MyCards[i].originPrs.pos;
                if (isEnlarge) {
                    if (MyCards[i].originPrs.pos.x < centerBattleCard.originPrs.pos.x) {
                        newPos.x -= offset;
                    } else {
                        newPos.x += offset;
                    }
                }

                MoveTransform(MyCards[i], new PRS(newPos, MyCards[i].originPrs.rot, MyCards[i].originPrs.scale), true, 0.5f);
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