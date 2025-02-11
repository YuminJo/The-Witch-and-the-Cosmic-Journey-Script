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
        model.IsEnemyTurn.Subscribe(view.SetButtonRaycastByTurnValue).AddTo(battleView);
    }
    
    public void OnClickEndTurnButton(Action action) => TurnChange(true, action);
    public void IsPlayerTurn(Action action) => TurnChange(false, action);

    private void TurnChange(bool isEnemyTurn, Action action) {
        model.SetTurnIndicator(isEnemyTurn);
        
        ServiceLocator.Get<IUIManager>().ShowPopupUI<BattleTurnIndicatorView>(callback: (popup) => {
            IBattleTurnIndicatorView turnIndicatorView = popup;
            turnIndicatorView?.SetTurnIndicator(isEnemyTurn, action).Forget();
        });
    }

    public void UpdateCard(float turnDelayShort) {
        var cardSystem = ServiceLocator.Get<ICardSystem>();
        cardSystem.GetCardByCardBuffer(turnDelayShort).Forget();
    }
}
