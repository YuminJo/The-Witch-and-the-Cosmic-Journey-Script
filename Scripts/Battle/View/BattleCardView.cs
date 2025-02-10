using Cysharp.Threading.Tasks;
using Entities.Cards;
using Systems.BattleSystem;
using UnityEngine;

public class BattleCardView : Object_Base {
    enum GameObjects {
    }
    
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
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindSprite(typeof(Sprites));
        BindText(typeof(Texts));
        
        return true;
    }
    
    public async UniTask SetCardData(Card card) {
        _cardData = card;
        await UniTask.WaitUntil(() => _init);
        
        ServiceLocator.Get<IResourceManager>().LoadAsync<Sprite>("samplecard", (sprite) 
            => { GetSprite((int)Sprites.Character).sprite = sprite; });
        GetText((int)Texts.Name).text = _cardData.TemplateId;
        GetText((int)Texts.Description).text = _cardData.Description;
    }
    
    void IsCardControllable(bool isControllable) {
        GetComponent<PolygonCollider2D>().enabled = isControllable;
    }
    
    void OnMouseUpAsButton() { 
        IsCardControllable(false);
        ServiceLocator.Get<ICardSystem>().UseOrSelectCard(this, _cardData);
        //await ServiceLocator.Get<ICardSystem>().MoveCardToCenter(this);
        //Destroy(gameObject);
    }

    void OnMouseOver() { 
        ServiceLocator.Get<ICardSystem>().CardMouseOver(this);
    }

    void OnMouseExit() { 
        ServiceLocator.Get<ICardSystem>().CardMouseExit(this);
    }
    
    //TODO: 카드 클릭시 확대되는 기능 추가
    /*void OnMouseDown() { 
        ServiceLocator.Get<ICardSystem>().ScaleCard(this, new Vector2(2.2f, 2.2f), 0.1f);
    }*/ 
}