using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleView {
}

public class BattleView : UI_Popup, IBattleView {
    enum Buttons {
        CardSelectButton,
        ItemSelectButton,
        EndTurnButton
    }
    
    enum GameObjects {
        CharacterGroup
    }
    
    private BattlePresenter presenter;
    
    public override bool Init() {
        if (base.Init() == false)
            return false;
        
        base.SetCanvas(gameObject, false, true);

        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        
        presenter = new BattlePresenter(this);
        
        GetButton((int)Buttons.EndTurnButton).gameObject.BindEvent(OnClickEndTurn);
        GetButton((int)Buttons.CardSelectButton).gameObject.BindEvent(() => {
            TurnSystem.OnClickSkillButton?.Invoke(); });

        foreach (var character in Managers.Game.GetSelectedCharacters()) {
            CreateCharacterView(character); }
        
        return true;
    }
    
    /// <summary>
    /// Instantiates a new BattleCharacterView and sets its character data.
    /// </summary>
    /// <param name="character">The character data to set in the BattleCharacterView.</param>
    private void CreateCharacterView(Character character) {
        var characterGroup = GetObject((int)GameObjects.CharacterGroup);
        
        Managers.Resource.Instantiate(nameof(BattleCharacterView), characterGroup.transform, (go) => {
            var battleCharacterView = go.GetComponent<BattleCharacterView>();
            if (battleCharacterView == null) {
                Debug.LogError("BattleCharacterView component is missing on the instantiated object");
                return;
            }
            battleCharacterView.SetCharacterData(character);
        });
    }

    public void OnClickEndTurn() {
        TurnSystem.OnEndTurn?.Invoke();
    }
}