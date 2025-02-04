using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public interface ICardSystem {
    void SetupItemBuffer();
    void CardMouseOver(BattleCardView battleCard, Card cardData);
    void CardMouseExit(BattleCardView battleCard);
    void UseOrSelectCard(BattleCardView battleCard, Card cardData);
    void ScaleCard(BattleCardView battleCard, Vector2 scale, float duration);
    void AddCard();
}

public class CardSystem : Object_Base, ICardSystem {
    enum GameObjects {
        CardSpawnPoint,
        MyCardLeft,
        MyCardRight
    }

    //TODO: 나중에 카드 데이터를 불러오는 방식을 바꿔야함
    private List<Card> _cardBuffer;
    private ReactiveCollection<BattleCardView> _myCards = new();
    private GameObject _cardPrefab;
    private Card _selectCard;
    
    public override bool Init() {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));

        ServiceLocator.Register<ICardSystem>(this);
        ServiceLocator.Get<IResourceManager>().LoadAsync<GameObject>("CardPrefab", (result) => _cardPrefab = result);
        
        // 카드 추가 이벤트
        _myCards.ObserveAdd().Subscribe(addEvent => {
                SetOriginOrder();
                CardAlignment();
            })
            .AddTo(this);
        
        return true;
    }
    
    public void UnRegisterService() => ServiceLocator.UnRegister<ICardSystem>();

    /// <summary>
    /// 카드 버퍼를 세팅합니다. 랜덤으로 섞어줍니다.
    /// </summary>
    public void SetupItemBuffer() {
        _cardBuffer = new List<Card>(100);

        ServiceLocator.Get<DataManager>().Cards
            .ToObservable()
            .Subscribe(card => _cardBuffer.Add(card.Value))
            .AddTo(this); // 메모리 누수 방지

        _cardBuffer = _cardBuffer.OrderBy(_ => Random.value).ToList();
    }

    
    /// <summary>
    /// 카드를 추가합니다.
    /// </summary>
    public void AddCard() {
        var cardObject = Instantiate(_cardPrefab, GetObject((int)GameObjects.CardSpawnPoint).transform.position, Utils.QI);
        var battleCardView = cardObject.GetComponent<BattleCardView>();
        
        battleCardView.SetCardData(_cardBuffer[_myCards.Count]);
        _myCards.Add(battleCardView);  // 리스트에 추가하면 자동으로 반응
    }
    
    /// <summary>
    /// 카드가 isTargetAll에 따라 전체 대상인지 아닌지에 따라 카드를 사용하거나 선택합니다.
    /// </summary>
    /// <param name="battleCard"></param>
    public void UseOrSelectCard(BattleCardView battleCard, Card cardData) {
        _selectCard = null;
        SetOriginOrder();
        CardAlignment();
    }

    void SetOriginOrder() {
        for (int i = 0; i < _myCards.Count; i++) {
            _myCards[i]?.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    void CardAlignment() {
        List<PRS> originCardPRSs = RoundAlignment(_myCards.Count, 0.5f, Vector3.one * BattleCardView.CardSize);

        var targetCards = _myCards;
        for (int i = 0; i < targetCards.Count; i++) {
            var targetCard = targetCards[i];

            targetCard.originPrs = originCardPRSs[i];
            MoveTransform(targetCard, targetCard.originPrs, true, 0.7f);
        }
    }

    List<PRS> RoundAlignment(int objCount, float height, Vector3 scale) {
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
        
        var leftTr = GetObject((int)GameObjects.MyCardLeft).transform;
        var rightTr = GetObject((int)GameObjects.MyCardRight).transform;
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

    public void CardMouseOver(BattleCardView battleCard, Card cardData) {
        _selectCard = cardData;
        EnlargeCard(true, battleCard);
    }

    public void CardMouseExit(BattleCardView battleCard) {
        EnlargeCard(false, battleCard);
    }

    void EnlargeCard(bool isEnlarge, BattleCardView battleCard) {
        DOTween.Kill(battleCard.transform);
        if (isEnlarge) {
            Vector3 enlargePos = new Vector3(battleCard.originPrs.pos.x, -4.8f, -10f);
            MoveTransform(battleCard, new PRS(enlargePos, Utils.QI, Vector3.one * 2.5f), true, 0.5f);
            AdjustOtherCards(battleCard, true);
        } else {
            MoveTransform(battleCard, battleCard.originPrs, true, 0.3f);
            AdjustOtherCards(battleCard, false);
        }

        battleCard.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    void AdjustOtherCards(BattleCardView centerBattleCard, bool isEnlarge) {
        float offset = 3.0f;
        for (int i = 0; i < _myCards.Count; i++) {
            if (_myCards[i] == centerBattleCard) continue;

            Vector3 newPos = _myCards[i].originPrs.pos;
            if (isEnlarge) {
                if (_myCards[i].originPrs.pos.x < centerBattleCard.originPrs.pos.x) {
                    newPos.x -= offset;
                } else {
                    newPos.x += offset;
                }
            }

            MoveTransform(_myCards[i], new PRS(newPos, _myCards[i].originPrs.rot, _myCards[i].originPrs.scale), true, 0.5f);
        }
    }

    public void MoveTransform(BattleCardView battleCard, PRS prs, bool useDotween, float dotweenTime = 0) {
        DOTween.Kill(battleCard.transform);
        if (useDotween) {
            battleCard.transform.DOMove(prs.pos, dotweenTime).SetEase(Ease.OutQuart);
            battleCard.transform.DORotateQuaternion(prs.rot, dotweenTime).SetEase(Ease.OutQuart);
            battleCard.transform.DOScale(prs.scale, dotweenTime).SetEase(Ease.OutQuart);
        } else {
            battleCard.transform.position = prs.pos;
            battleCard.transform.rotation = prs.rot;
            battleCard.transform.localScale = prs.scale;
        }
    }

    /*public async UniTask MoveCardToCenter(BattleCardView battleCard) {
        MoveTransform(battleCard, new PRS(Vector3.zero, Quaternion.identity, Vector3.one * BattleCardView.cardSize), true, 0.5f);
        await UniTask.Delay(1000);
    }*/

    public void ScaleCard(BattleCardView battleCard, Vector2 scale, float duration) {
        DOTween.Kill(battleCard.transform);
        battleCard.transform.DOScale(scale, duration).SetEase(Ease.InCirc);
    }
}