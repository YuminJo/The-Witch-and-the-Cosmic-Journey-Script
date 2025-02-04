using UnityEngine;

public class UI_Popup : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        ServiceLocator.Get<IUIManager>().SetCanvas(gameObject, true);
        return true;
    }

    public virtual void ClosePopupUI()
    {
        ServiceLocator.Get<IUIManager>().ClosePopupUI(this);
    }
    
    public virtual void SetCanvas(GameObject go, bool sort = true , bool canvascamera = false)
    {
        ServiceLocator.Get<IUIManager>().SetCanvas(go, sort, canvascamera);
    }
}
