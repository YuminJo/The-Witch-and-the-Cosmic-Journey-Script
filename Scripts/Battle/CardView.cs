using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;

    public Card item;
    public PRS originPRS;
    private bool isCardClicked = false;

    public const float cardSize = 1.5f;

    public void Setup(Card item) {
        this.item = item;
        character.sprite = this.item.sprite;
        nameTMP.text = this.item.templateId;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            isCardClicked = false;
        }
    }

    private IEnumerator OnClickCard() {
        isCardClicked = true;
        CardManager.Inst.UseCard(this);
        yield return CardManager.Inst.MoveCardToCenter(this);
        Destroy(gameObject);
    }

    void OnMouseOver() {
        if (isCardClicked) return;
        Debug.Log("OnMouseOver");
        CardManager.Inst.CardMouseOver(this);
    }

    void OnMouseExit() {
        if (isCardClicked) return;
        Debug.Log("OnMouseExit");
        CardManager.Inst.CardMouseExit(this);
    }

    void OnMouseDown() {
        if (isCardClicked) return;
        Debug.Log("OnMouseDown");
        CardManager.Inst.ScaleCard(this, new Vector2(2.2f, 2.2f), 0.1f);
    }

    void OnMouseUpAsButton() {
        if (isCardClicked) return;
        Debug.Log("OnMouseUp");
        StartCoroutine(OnClickCard());
    }
}