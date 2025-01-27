using UnityEngine;

public class BattlePresenter {
    private readonly IBattleView view;
    private readonly BattleModel model;
    
    public BattlePresenter(IBattleView view, BattleModel model) {
        this.view = view;
        this.model = model;
    }
}
