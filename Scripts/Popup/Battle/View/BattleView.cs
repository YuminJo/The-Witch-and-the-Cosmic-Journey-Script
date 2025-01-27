using System.Collections.Generic;
using UnityEngine;

public interface IBattleView {

    
}

public class BattleView : UI_Popup, IBattleView {
    enum Buttons {
        CardSelectButton,
        ItemSelectButton
    }
    
    private BattlePresenter presenter;
    private List<BattleCharacterView> characterViews = new();
    
    public override bool Init() {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        
        presenter = new BattlePresenter(this, new BattleModel());
        
        GetButton((int)Buttons.CardSelectButton).gameObject.BindEvent(() => {
            TurnManager.OnClickSkillButton?.Invoke(); });

        return true;
    }
}
