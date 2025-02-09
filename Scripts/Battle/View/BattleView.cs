using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IBattleView {
}

public class BattleView : UI_Popup, IBattleView {
    enum Buttons {
        CardSelectButton,
        ItemSelectButton,
        EndTurnButton
    }
    
    enum GameObjects {
        CharacterGroup,
        EnergyGroup
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
    public void CreateCharacterView(Character character) {
        var characterGroup = GetObject((int)GameObjects.CharacterGroup);
    
        ServiceLocator.Get<IResourceManager>().Instantiate(nameof(BattleCharacterView), characterGroup.transform, (go) => {
            var battleCharacterView = go.GetComponent<BattleCharacterView>();
            battleCharacterView.SetCharacterData(character);
        });
    }
    
    public void SetEnergy(int value) {
        var energyGroup = GetObject((int)GameObjects.EnergyGroup);
        var children = energyGroup.transform.GetComponentsInChildren<Image>();
    
        // Deactivate all children first
        foreach (var child in children) {
            child.gameObject.SetActive(false);
        }

        // Activate only the required number of children
        for (int i = 0; i < value && i < children.Length; i++) {
            children[i].gameObject.SetActive(true);
        }
    }
    
    public void OnClickCardSelectButton(int startCardCount, float turnDelayShort) {
        GetButton((int)Buttons.CardSelectButton).gameObject.BindEvent(()
            => { UpdateCard(startCardCount, turnDelayShort).Forget(); });
    }

    private async UniTask UpdateCard(int startCardCount, float turnDelayShort) {
        var cardSystem = ServiceLocator.Get<ICardSystem>();
        
        for (int i = 0; i < startCardCount; i++) {
            await UniTask.Delay(TimeSpan.FromSeconds(turnDelayShort));
            cardSystem.AddCard();
        }
    }
}