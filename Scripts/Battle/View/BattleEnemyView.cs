using DG.Tweening;
using R3;
using Systems.BattleSystem;
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
        
        _enemyData.Hp.Subscribe(HpBarAnimation).AddTo(this);
        return true;
    }
    
    public void SetEnemyData(Enemy enemy) => _enemyData = enemy;
    
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
