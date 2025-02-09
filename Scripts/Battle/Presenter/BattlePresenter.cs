using UnityEngine;

public class BattlePresenter {
    private readonly IBattleView view;
    private readonly BattleModel model;
    
    public BattlePresenter(IBattleView view) {
        this.view = view;
        model = new BattleModel();
    }
}
