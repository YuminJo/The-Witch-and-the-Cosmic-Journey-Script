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
    
    public void CreateCharacterView(Character character, Transform characterGroup) {
        ServiceLocator.Get<IResourceManager>().Instantiate(nameof(BattleCharacterView), characterGroup).ContinueWith(go => {
            var battleCharacterView = go.GetComponent<BattleCharacterView>();
            battleCharacterView.SetCharacterData(character);
        }).Forget();
    }
    
    public void OnClickEndTurnButton(Action action) => TurnChange(true, action);
    public void IsPlayerTurn(Action action) => TurnChange(false, action);

    private void TurnChange(bool isEnemyTurn, Action action) {
        model.SetTurnIndicator(isEnemyTurn);
        action();
    }

    public void UpdateCard(float turnDelayShort) {
        var cardSystem = ServiceLocator.Get<ICardSystem>();
        cardSystem.GetCardByCardBuffer(turnDelayShort).Forget();
    }
}
