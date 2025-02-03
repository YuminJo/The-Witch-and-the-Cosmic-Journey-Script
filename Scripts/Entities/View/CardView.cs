using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class CardView : Object_Base
{
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

    private Card cardData;
    public PRS originPRS;
    private bool isCardClicked = false;

    public const float cardSize = 1.5f;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindSprite(typeof(Sprites));
        BindText(typeof(Texts));
        
        //TODO: Set character image and hp bar
        
        return true;
    }
    
    public void SetCardData(Card card) {
        cardData = card;
        StartCoroutine(Setup());
    }

    private IEnumerator Setup() {
        while (_init == false) {
            yield return null; }
        
        Debug.Log("CardView.Setup: " + cardData.templateId);
        Debug.Log("CardView.Setup: " + cardData.description);
        Debug.Log("CardView.Setup: " + cardData.sprite);
        
        ServiceLocator.Get<ResourceManager>().LoadAsync<Sprite>("samplecard", (sprite) => {
            GetSprite((int)Sprites.Character).sprite = sprite;
        });
        GetText((int)Texts.Name).text = cardData.templateId;
        GetText((int)Texts.Description).text = cardData.description;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            isCardClicked = false;
        }
    }

    private IEnumerator OnClickCard() {
        isCardClicked = true;
        ServiceLocator.Get<ICardSystem>().UseCard(this);
        yield return ServiceLocator.Get<ICardSystem>().MoveCardToCenter(this);
        Destroy(gameObject);
    }

    void OnMouseOver() {
        if (isCardClicked) return;
        ServiceLocator.Get<ICardSystem>().CardMouseOver(this);
    }

    void OnMouseExit() {
        if (isCardClicked) return;
        ServiceLocator.Get<ICardSystem>().CardMouseExit(this);
    }

    void OnMouseDown() {
        if (isCardClicked) return;
        ServiceLocator.Get<ICardSystem>().ScaleCard(this, new Vector2(2.2f, 2.2f), 0.1f);
    }

    void OnMouseUpAsButton() {
        if (isCardClicked) return;
        Debug.Log("OnMouseUp");
        StartCoroutine(OnClickCard());
    }
}