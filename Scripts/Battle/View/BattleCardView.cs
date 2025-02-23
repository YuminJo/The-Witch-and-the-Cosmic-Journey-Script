using Cysharp.Threading.Tasks;
using Entities.Cards;
using Systems.BattleSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battle.View {
    public class BattleCardView : Object_Base, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        enum Sprites {
            Card,
            Character
        }
    
        enum Texts {
            Name,
            Description
        }

        private Card _cardData;
        public PRS originPrs;
        public const float CardSize = 1.5f;

        public override bool Init() {
            if (!base.Init()) return false;

            BindSprite(typeof(Sprites));
            BindText(typeof(Texts));

            return true;
        }
    
        public async UniTask SetCardData(Card card) {   
            _cardData = card;
            await UniTask.WaitUntil(() => _init);

            // LoadAsync를 UniTask로 처리
            Sprite sprite = await ServiceLocator.Get<IResourceManager>().LoadAsync<Sprite>("samplecard");
            
            if (sprite != null) {
                GetSprite((int)Sprites.Character).sprite = sprite;
            }

            // sprite가 로드되었을 경우에만 설정
            if (sprite != null) {
                GetSprite((int)Sprites.Character).sprite = sprite;
            }

            GetText((int)Texts.Name).text = _cardData.TemplateId;
            GetText((int)Texts.Description).text = _cardData.Description;
        }
    
        public void IsCardControllable(bool isControllable) {
            GetComponent<PolygonCollider2D>().enabled = isControllable;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            // MouseOver 상태를 자동으로 처리하지 않고, 각 이벤트마다 호출
            ServiceLocator.Get<ICardSystem>().CardMouseOver(this, _cardData.Ap);
        }

        public void OnPointerExit(PointerEventData eventData) {
            ServiceLocator.Get<ICardSystem>().CardMouseExit(this);
        }

        public void OnPointerClick(PointerEventData eventData) {
            IsCardControllable(false);
            ServiceLocator.Get<ICardSystem>().UseOrSelectCard(this, _cardData);
        }
    }
}
