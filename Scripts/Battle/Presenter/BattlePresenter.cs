using System;
using Battle.View;
using Cysharp.Threading.Tasks;
using R3;
using Systems.BattleSystem;
using UnityEngine;

public class BattlePresenter {
    private readonly IBattleView view;
    private readonly BattleModel model;
    
    public BattlePresenter(BattleView battleView) {
        this.view = battleView;
        model = new BattleModel();
        model.IsEnemyTurn.Subscribe(view.IsEnemyTurn).AddTo(battleView);
    }
    
    public void OnClickEndTurnButton(Action action) {
        model.SetTurnIndicator(true);
        
        ServiceLocator.Get<IUIManager>().ShowPopupUI<BattleTurnIndicatorView>(callback: (popup) => {
            IBattleTurnIndicatorView turnIndicatorView = popup;
            turnIndicatorView?.SetTurnIndicator(true, BattleModel.TurnIndicatorDelay, action).Forget();
        });
    }

    public void UpdateCard(float turnDelayShort) {
        var cardSystem = ServiceLocator.Get<ICardSystem>();
        cardSystem.GetCardByCardBuffer(turnDelayShort).Forget();
    }
}
