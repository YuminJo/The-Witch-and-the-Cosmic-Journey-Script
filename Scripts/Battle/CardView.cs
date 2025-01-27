using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;
    //[SerializeField] ECardState eCardState;
    
    public Card item;
    bool isFront;
    public PRS originPRS;


    public void Setup(Card item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;

        if (this.isFront)
        {
            character.sprite = this.item.sprite;
            nameTMP.text = this.item.name;
            attackTMP.text = this.item.attack.ToString();
        }
        else
        {
            card.sprite = cardBack;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
        }
    }
    
    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime).SetEase(Ease.OutQuart);
            transform.DORotateQuaternion(prs.rot, dotweenTime).SetEase(Ease.OutQuart);
            transform.DOScale(prs.scale, dotweenTime).SetEase(Ease.OutQuart);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }
    
    void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
        isFront = true; //test
        if (isFront) CardManager.Inst.CardMouseOver(this);
    }

    void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
        isFront = true; //test
        if (isFront) CardManager.Inst.CardMouseExit(this);
    }

    /*void OnMouseDown()
    {
        if (isFront) CardManager.Inst.CardMouseDown();
    }

    void OnMouseUp()
    {
        if (isFront) CardManager.Inst.CardMouseUp();
    }*/
}