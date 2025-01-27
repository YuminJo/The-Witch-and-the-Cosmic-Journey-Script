using System.Collections.Generic;
using UnityEngine;

public interface IBattleView {
    void CreateCharacterView(Character character);
}

public class BattleView : UI_Popup, IBattleView {
    enum Buttons {
        CardSelectButton,
        ItemSelectButton
    }
    
    enum GameObjects {
        CharacterGroup
    }
    
    private BattlePresenter presenter;
    private List<BattleCharacterView> characterViews = new();
    
    public override bool Init() {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        
        presenter = new BattlePresenter(this);
        
        GetButton((int)Buttons.CardSelectButton).gameObject.BindEvent(() => {
            TurnManager.OnClickSkillButton?.Invoke(); });
        return true;
    }
    
    /// <summary>
    /// Instantiates a new BattleCharacterView and sets its character data.
    /// </summary>
    /// <param name="character">The character data to set in the BattleCharacterView.</param>
    public void CreateCharacterView(Character character) {
        Managers.Resource.Instantiate(nameof(BattleCharacterView), GetObject((int)GameObjects.CharacterGroup).transform, (go) => {
            go.GetComponent<BattleCharacterView>().SetCharacterData(character); });
    }
}