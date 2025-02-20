using System.Collections.Generic;
using System.Linq;
using Entities.Cards;
using UnityEngine;

namespace Systems.BattleSystem {
    public interface ICardBufferManager {
        void SetupItemBuffer(List<Character> characterList, int limitCardCount = 5);
        List<Card> GetHandCardBuffer();
    }
    
    public class CardBufferManager : ICardBufferManager {
        private readonly List<Card> _handCardBuffer = new();

        /// <summary>
        /// Data Manager에서 카드 데이터를 불러와 섞어서 버퍼에 저장합니다.
        /// _handCardBuffer에는 기본적으로 5장의 카드가 들어가 있습니다.
        /// 턴이 끝나면 기존에 남아있는 카드는 유지하고 새로운 카드를 추가합니다.
        /// 모든 카드는 AGI를 기준으로 배포됩니다.
        /// </summary>
        /// <param name="limitCardCount">카드 제한 개수</param>
        public void SetupItemBuffer(List<Character> characterList, int limitCardCount = 5) {
            int cardsToAdd = limitCardCount - _handCardBuffer.Count;
            
            Queue<Character> charactersByAgi = new(characterList.OrderBy(c => c.Agi));
            while (cardsToAdd > 0) {
                if (charactersByAgi.Count == 0) { break; }

                Character currentCharacter = charactersByAgi.Dequeue();
                GetCharacterCard(currentCharacter);
                charactersByAgi.Enqueue(currentCharacter);
                
                cardsToAdd--;
            }
        }
        
        private void GetCharacterCard(Character character) {
            var dataManager = ServiceLocator.Get<DataManager>();
            
            // 캐릭터에 맞는 카드를 뽑아서 버퍼에 추가합니다.
            Card card = dataManager.Cards.Values
                .Where(c => c.Character == character.Name)
                .OrderBy(_ => Random.value)
                .FirstOrDefault();
            
            // 만약 cards가 null이라면 랜덤 카드를 추가합니다.
            if (card == null) {
                card = dataManager.Cards.Values
                    .OrderBy(_ => Random.value)
                    .FirstOrDefault();
            }

            _handCardBuffer.Add(card);
        }

        public List<Card> GetHandCardBuffer() {
            return _handCardBuffer;
        }
    }
}