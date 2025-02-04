using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public interface IBattleView {
}

public class BattleView : UI_Popup, IBattleView {
    enum Buttons {
        CardSelectButton,
        ItemSelectButton,
        EndTurnButton
    }
    
    enum GameObjects {
        CharacterGroup
    }
    
    private BattlePresenter presenter;
    
    public override bool Init() {
        if (base.Init() == false)
            return false;
        
        presenter = new BattlePresenter(this);
        
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        
        SetCanvas(gameObject, false, true);
        return true;
    }
    
    /// <summary>
    /// 캐릭터의 창을 만들고 데이터를 설정합니다.
    /// </summary>
    /// <param name="character">캐릭터의 데이터값</param>
    public async UniTask CreateCharacterView(Character character) {
        await UniTask.WaitUntil(() => _init);
    
        var characterGroup = GetObject((int)GameObjects.CharacterGroup);
    
        ServiceLocator.Get<IResourceManager>().Instantiate(nameof(BattleCharacterView), characterGroup.transform, (go) => {
            var battleCharacterView = go.GetComponent<BattleCharacterView>();
            battleCharacterView.SetCharacterData(character);
        });
    }
    
    public async UniTask OnClickCardSelectButton(TurnSystem turnSystem) {
        await UniTask.WaitUntil(() => _init);
        
        GetButton((int)Buttons.CardSelectButton).gameObject.BindEvent(()
            => { turnSystem.ClickSkillButtonAsync().Forget(); });
    }
}