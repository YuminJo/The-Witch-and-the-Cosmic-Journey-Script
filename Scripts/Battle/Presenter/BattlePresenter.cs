using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattlePresenter {
    private readonly IBattleView view;
    private readonly BattleModel model;
    
    public BattlePresenter(IBattleView view) {
        this.view = view;
        model = new BattleModel();
    }
    
    public void OnClickEndTurnButton() {
        view.IsEnemyTurn(true);
    }

    public void UpdateCard(float turnDelayShort) {
        var cardSystem = ServiceLocator.Get<ICardSystem>();
        cardSystem.GetCardByCardBuffer(turnDelayShort).Forget();
    }
}
