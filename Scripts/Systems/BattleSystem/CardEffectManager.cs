using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Entities.Cards;
using UnityEngine;

namespace Systems.BattleSystem {
    public interface ICardEffectManager {
        void UseCost(Card card, Enemy selectedEnemy = null);
    }
    public class CardEffectManager : ICardEffectManager {
        private readonly CardSystem _cardSystem;
        private readonly ITurnSystem _turnSystem;
        private Enemy _enemy;
        
        public CardEffectManager(ITurnSystem turnSystem, CardSystem cardSystem) {
            _cardSystem = cardSystem;
            _turnSystem = turnSystem;
        }
        
        // 카드 사용 시 코스트 처리
        public void UseCost(Card card,Enemy selectedEnemy = null) {
            if (!_turnSystem.UseAPCost(card.Ap)) return;
            // 카드 효과 실행
            _enemy = selectedEnemy;
            EffectByType(card);
        }

        /// <summary>
        /// 카드의 타입에 따라 효과를 실행합니다.
        /// </summary>
        private async void EffectByType(Card card) {
            _cardSystem.UseCard();
            
            List<Enemy> targetEntities = GetTargetEntities(card);
            
            // 효과 적용
            foreach (var entity in targetEntities) {
                RunEffect(card, entity);
                await UniTask.Delay(1000); // 예시: 지연 시간
            }
            
            _cardSystem.InitSelectedEnemyAndCard();
        }
        
        // 카드에 따른 타겟 목록 반환
        private List<Enemy> GetTargetEntities(Card card) {
            // Range가 -1 ( 전체 대상 )인 경우
            List<Enemy> targetEntities = card.IsTargetAll()
                ? _turnSystem.EnemyList
                : Utils.GetTargetEntitiesByRange(card.Range, _turnSystem.EnemyList, _enemy);
            
            return targetEntities;
        }
        
        private void RunEffect(Card card, Enemy targetEnemy) {
            Character attacker = _turnSystem.GetCharacter(card.Character);
            foreach (var effect in card.Effects) {
                CardEffects.OnBuff(attacker, targetEnemy, effect);
            }
        }
    }
}