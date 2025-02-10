using System;
using DG.Tweening;
using R3;
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
        
        _characterData.Hp.Subscribe(HpBarAnimation).AddTo(this);
        return true;
    }
    
    private void HpBarAnimation(int value) {
        GetImage((int)Images.HpBar).DOFillAmount(Utils.GetHpByPercent(value, _characterData.MaxHp),0.5f);
    }
    
    public void SetCharacterData(Character character) => _characterData = character;
}
