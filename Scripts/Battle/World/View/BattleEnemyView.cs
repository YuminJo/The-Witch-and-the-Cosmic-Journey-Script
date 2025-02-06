using UnityEngine;

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
        return true;
    }
    
    void OnMouseUpAsButton() { 
        bool isClicked = ServiceLocator.Get<ICardSystem>().SelectEnemy(_enemyData);
        if (isClicked)
            GetObject((int)GameObjects.TargetImage).SetActive(true);
    }
}
