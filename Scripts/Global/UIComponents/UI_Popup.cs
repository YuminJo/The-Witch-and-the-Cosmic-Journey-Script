using UnityEngine;

public class UI_Popup : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Managers.UI.SetCanvas(gameObject, true);
        return true;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
    
    public virtual void SetCanvas(GameObject go, bool sort = true , bool canvascamera = false)
    {
        Managers.UI.SetCanvas(go, sort, canvascamera);
    }
}
