using UnityEngine;

public class BattlePopupView : UI_Popup
{
    enum Buttons
    {
        CardSelectButton,
        ItemSelectButton
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        
        /*Managers.Resource.LoadAsync<Sprite>(presenter.GetCurrentCharacter().facehead,
            callback: (sprite) => { GetImage((int)Images.ProfileImage).sprite = sprite; });*/
        
        GetButton((int)Buttons.CardSelectButton).gameObject.BindEvent(() => {
            StartCoroutine(TurnManager.Inst.ClickSkillButton());
        });

        return true;
    }
}
