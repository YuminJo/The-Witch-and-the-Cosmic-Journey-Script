using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CardManager : MonoBehaviour {
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;
    
    [Required] private GameObject _cardPrefab;
    
    [Required] [SerializeField] [FoldoutGroup("Card Env")] [ReadOnly]
    List<CardView> myCards;

    [Required] [SerializeField] [FoldoutGroup("Card Env")]
    Transform myCardLeft;

    [Required] [SerializeField] [FoldoutGroup("Card Env")]
    Transform myCardRight;

    [Required] [SerializeField] [FoldoutGroup("Card Env")]
    Transform cardSpawnPoint;

    private List<Card> cardBuffer;

    void Start() {
        Managers.Resource.LoadAsync<GameObject>("CardPrefab", (result) => _cardPrefab = result);

#if UNITY_EDITOR
        if (myCardLeft == null) Utils.GlobalException("myCardLeft is null");
        if (myCardRight == null) Utils.GlobalException("myCardRight is null");
        if (cardSpawnPoint == null) Utils.GlobalException("cardSpawnPoint is null");
#endif
        TurnManager.OnAddCard += AddCard;
    }

    public Card PopCard() {
        if (cardBuffer.Count == 0)
            SetupItemBuffer();

        Card item = cardBuffer[0];
        cardBuffer.RemoveAt(0);
        return item;
    }

    public void SetupItemBuffer() {
        //TODO: 플레이어가 가지고 있는 카드로 나중에 변경
        cardBuffer = new List<Card>(100);
        for (int i = 0; i < Managers.Data.Cards.Count; i++) {
            Card item = Managers.Data.Cards[i];
            //for (int j = 0; j < item.percent; j++)
                cardBuffer.Add(item);
        }

        for (int i = 0; i < cardBuffer.Count; i++) {
            int rand = Random.Range(i, cardBuffer.Count);
            Card temp = cardBuffer[i];
            cardBuffer[i] = cardBuffer[rand];
            cardBuffer[rand] = temp;
        }
    }

    void AddCard() {
        var cardObject = Instantiate(_cardPrefab, cardSpawnPoint.position, Utils.QI);
        myCards.Add(cardObject.GetComponent<CardView>());
        SetOriginOrder();
        CardAlignment();
    }

    void SetOriginOrder() {
        for (int i = 0; i < myCards.Count; i++) {
            myCards[i]?.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    void CardAlignment() {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 1.5f);

        var targetCards = myCards;
        for (int i = 0; i < targetCards.Count; i++) {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale) {
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

    #region MyCard

    public void CardMouseOver(CardView card) {
        /*if (eCardState == ECardState.Nothing)
            return;

        selectCard = card;*/
        EnlargeCard(true, card);
    }

    public void CardMouseExit(CardView card) {
        EnlargeCard(false, card);
    }

    /*public void CardMouseDown()
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        isMyCardDrag = true;
    }

    public void CardMouseUp()
    {
        isMyCardDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (onMyCardArea)
            EntityManager.Inst.RemoveMyEmptyEntity();
        else
            TryPutCard(true);
    }

    void CardDrag()
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (!onMyCardArea)
        {
            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false);
            EntityManager.Inst.InsertMyEmptyEntity(Utils.MousePos.x);
        }
    }

    void DetectCardArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("MyCardArea");
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }*/

    void EnlargeCard(bool isEnlarge, CardView card) {
        DOTween.Kill(card.transform);
        if (isEnlarge) {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, -4.8f, -10f);
            card.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 2.5f), true, 0.5f);
            AdjustOtherCards(card, true);
        } else {
            card.MoveTransform(card.originPRS, true, 0.3f);
            AdjustOtherCards(card, false);
        }

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    void AdjustOtherCards(CardView centerCard, bool isEnlarge) {
        float offset = 3.0f; // Adjust this value to control how much other cards move
        for (int i = 0; i < myCards.Count; i++) {
            if (myCards[i] == centerCard) continue;

            Vector3 newPos = myCards[i].originPRS.pos;
            if (isEnlarge) {
                if (myCards[i].originPRS.pos.x < centerCard.originPRS.pos.x) {
                    newPos.x -= offset;
                } else {
                    newPos.x += offset;
                }
            }

            myCards[i].MoveTransform(new PRS(newPos, myCards[i].originPRS.rot, myCards[i].originPRS.scale), true, 0.5f);
        }
    }

    /*void SetECardState()
    {
        if (TurnManager.Inst.isLoading)
            eCardState = ECardState.Nothing;

        else if (!TurnManager.Inst.myTurn || myPutCount == 1 || EntityManager.Inst.IsFullMyEntities)
            eCardState = ECardState.CanMouseOver;

        else if (TurnManager.Inst.myTurn && myPutCount == 0)
            eCardState = ECardState.CanMouseDrag;
    }*/

    #endregion
}