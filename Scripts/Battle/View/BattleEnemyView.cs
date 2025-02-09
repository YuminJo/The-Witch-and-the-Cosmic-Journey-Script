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

    void NotProd() {
        _enemyData = new Enemy("enemy01", 1030, 1300, 4, 3, EnemyType.Normal);
    }
    
    void OnMouseUpAsButton() { 
        bool isClicked = ServiceLocator.Get<ICardSystem>().SelectEnemy(_enemyData);
        if (isClicked) GetObject((int)GameObjects.TargetImage).SetActive(true);
    }

    public void OnDamage(int value) => _enemyData.OnDamage(value);
    public void OnHeal(int value) => _enemyData.OnHeal(value);
    public void AddShield(int value) => _enemyData.AddShield(value); 
    
    private void HpBarAnimation(int value) {
        Image hpBar = GetObject((int)GameObjects.Hpbar).GetComponent<Image>();
        hpBar.DOFillAmount(Utils.GetHpByPercent(value, _enemyData.MaxHp),0.5f);
    }
}
