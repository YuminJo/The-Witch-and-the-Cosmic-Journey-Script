using UnityEngine;

public class BattleCharacterView : UI_Popup
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
        
        
        return true;
    }
}
