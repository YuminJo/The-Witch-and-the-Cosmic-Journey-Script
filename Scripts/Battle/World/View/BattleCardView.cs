using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

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
    private bool _isCardClicked;
    
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
        GetText((int)Texts.Name).text = _cardData.templateId;
        GetText((int)Texts.Description).text = _cardData.description;
    }

    private void OnClickCard() {
        _isCardClicked = true;
        ServiceLocator.Get<ICardSystem>().UseOrSelectCard(this, _cardData);
        //await ServiceLocator.Get<ICardSystem>().MoveCardToCenter(this);
        //Destroy(gameObject);
    }
    
    void OnMouseUpAsButton() { 
        if (_isCardClicked) return;
        OnClickCard();
    }

    void OnMouseOver() { 
        if (_isCardClicked) return;
        ServiceLocator.Get<ICardSystem>().CardMouseOver(this, _cardData);
    }

    void OnMouseExit() { 
        if (_isCardClicked) return;
        ServiceLocator.Get<ICardSystem>().CardMouseExit(this);
    }

    void OnMouseDown() { 
        if (_isCardClicked) return;
        ServiceLocator.Get<ICardSystem>().ScaleCard(this, new Vector2(2.2f, 2.2f), 0.1f);
    }
}