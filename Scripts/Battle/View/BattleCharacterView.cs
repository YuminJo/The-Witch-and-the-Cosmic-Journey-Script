using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public interface IBattleCharacterView {
    void SetCharacterData(Character character);
}

public class BattleCharacterView : UI_Popup, IBattleCharacterView {
    enum GameObjects {
        EffectLayout
    }

    enum Images {
        CharacterImage,
        HpBar,
        TurnChecker
    }
    
    private Character _characterData;
    
    public override bool Init() {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        
        GetImage((int)Images.TurnChecker).gameObject.SetActive(false);
        
        return true;
    }

    public void OnDamaged(int value) {
        _characterData.OnDamage(value);
        
        /*
        Image hpBar = GetImage((int)Images.HpBar);
        DOTween.Kill(hpBar);
        DOTween.To(() => hpBar.fillAmount, x 
            => hpBar.fillAmount = x, Utils.GetHpByPercent(_characterData.Hp,_characterData.MaxHp), 0.5f);*/
    }
    
    public void SetCharacterData(Character character) => _characterData = character;
}
