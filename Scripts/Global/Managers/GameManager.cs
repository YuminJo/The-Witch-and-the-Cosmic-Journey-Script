using System.Collections.Generic;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버

public interface IGameManager {
    void Init();
}

public class GameManager : IGameManager {

    public void Init() {
    }

    void Update() {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }
    
    private void InputCheatKey() {
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            var turnSystem = ServiceLocator.Get<TurnSystem>();
            turnSystem.OnAddCard.OnNext(Unit.Default);
        }*/
    }
}