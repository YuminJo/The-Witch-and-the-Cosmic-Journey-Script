using UnityEngine;

public class EnemyView : Object_Base
{
    enum GameObjects {
        Hpbar
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));

        return true;
    }
}
