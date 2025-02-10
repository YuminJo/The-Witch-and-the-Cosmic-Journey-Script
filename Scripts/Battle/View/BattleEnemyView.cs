using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnemyView : Object_Base
{
    enum GameObjects {
        Hpbar,
        TargetImage
    }
    
    private Enemy _enemyData;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        GetObject((int)GameObjects.TargetImage).SetActive(false);
        
        NotProd();
        _enemyData.Hp.Subscribe(HpBarAnimation).AddTo(this);
        return true;
    }

    private void NotProd() {
        _enemyData = new Enemy("enemy01", 1030, 1300, 4, 3, EnemyType.Normal);
    }
    
    private void OnMouseUpAsButton() { 
        bool isClicked = ServiceLocator.Get<ICardSystem>().SelectEnemy(_enemyData,this);
        if (isClicked) TargetSelected(true);
    }
    
    public void TargetSelected(bool isSelected) => GetObject((int)GameObjects.TargetImage).SetActive(isSelected);
    
    private void HpBarAnimation(int value) {
        Image hpBar = GetObject((int)GameObjects.Hpbar).GetComponent<Image>();
        hpBar.DOFillAmount(Utils.GetHpByPercent(value, _enemyData.MaxHp),0.5f);
    }
}
