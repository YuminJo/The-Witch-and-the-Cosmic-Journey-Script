using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    CardView selectCard;
    List<Card> cardBuffer;

    void Start() {
        Managers.Resource.LoadAsync<GameObject>("CardPrefab", (result) => _cardPrefab = result);

#if UNITY_EDITOR
        if (myCardLeft == null) Utils.GlobalException("myCardLeft is null");
        if (myCardRight == null) Utils.GlobalException("myCardRight is null");
        if (cardSpawnPoint == null) Utils.GlobalException("cardSpawnPoint is null");
#endif
        TurnManager.OnAddCard += AddCard;
    }

    public void SetupItemBuffer() {
        cardBuffer = new List<Card>(100);
        for (int i = 0; i < Managers.Data.Cards.Count; i++) {
            Debug.Log(Managers.Data.Cards.Count);
            Card item = Managers.Data.Cards[i];
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

    public void UseCard(CardView card) {
        myCards.Remove(card);
        selectCard = null;
        SetOriginOrder();
        CardAlignment();
    }

    void SetOriginOrder() {
        for (int i = 0; i < myCards.Count; i++) {
            myCards[i]?.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    void CardAlignment() {
        List<PRS> originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * CardView.cardSize);

        var targetCards = myCards;
        for (int i = 0; i < targetCards.Count; i++) {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];
            MoveTransform(targetCard, targetCard.originPRS, true, 0.7f);
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

    public void CardMouseOver(CardView card) {
        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(CardView card) {
        EnlargeCard(false, card);
    }

    void EnlargeCard(bool isEnlarge, CardView card) {
        DOTween.Kill(card.transform);
        if (isEnlarge) {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, -4.8f, -10f);
            MoveTransform(card, new PRS(enlargePos, Utils.QI, Vector3.one * 2.5f), true, 0.5f);
            AdjustOtherCards(card, true);
        } else {
            MoveTransform(card, card.originPRS, true, 0.3f);
            AdjustOtherCards(card, false);
        }

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    void AdjustOtherCards(CardView centerCard, bool isEnlarge) {
        float offset = 3.0f;
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

            MoveTransform(myCards[i], new PRS(newPos, myCards[i].originPRS.rot, myCards[i].originPRS.scale), true, 0.5f);
        }
    }

    public void MoveTransform(CardView card, PRS prs, bool useDotween, float dotweenTime = 0) {
        DOTween.Kill(card.transform);
        if (useDotween) {
            card.transform.DOMove(prs.pos, dotweenTime).SetEase(Ease.OutQuart);
            card.transform.DORotateQuaternion(prs.rot, dotweenTime).SetEase(Ease.OutQuart);
            card.transform.DOScale(prs.scale, dotweenTime).SetEase(Ease.OutQuart);
        } else {
            card.transform.position = prs.pos;
            card.transform.rotation = prs.rot;
            card.transform.localScale = prs.scale;
        }
    }

    public IEnumerator MoveCardToCenter(CardView card) {
        MoveTransform(card, new PRS(Vector3.zero, Quaternion.identity, Vector3.one * CardView.cardSize), true, 0.5f);
        yield return new WaitForSeconds(1f);
    }

    public void ScaleCard(CardView card, Vector2 scale, float duration) {
        DOTween.Kill(card.transform);
        card.transform.DOScale(scale, duration).SetEase(Ease.InCirc);
    }
}