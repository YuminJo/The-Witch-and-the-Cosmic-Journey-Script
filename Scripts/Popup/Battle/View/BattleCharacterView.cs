using UnityEngine;

public interface IBattleCharacterView {
    void SetCharacterData(Character character);
}

public class BattleCharacterView : UI_Popup, IBattleCharacterView
{
    enum GameObjects {
        EffectLayout
    }

    enum Images {
        CharacterImage,
        HpBar
    }
    
    private Character _characterData;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        
        //TODO: Set character image and hp bar
        
        return true;
    }
    
    public void SetCharacterData(Character character) {
        _characterData = character;
    }
}
