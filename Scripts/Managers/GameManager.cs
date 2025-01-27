// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager {
    List<Character> GetCharactersData();
}

// 치트, UI, 랭킹, 게임오버
public class GameManager : IGameManager {
    [Range(0,3)] private List<Character> _currentSelectedCharacters = new();

    public void Init() => InitData();
    void Update() {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    private void InitData() {
        _currentSelectedCharacters.Add(new Character("sample01", 100, 100, 10, 0, 10, 3, CharacterType.Tanker));
        _currentSelectedCharacters.Add(new Character("sample02", 100, 100, 10, 0, 10, 3, CharacterType.Dealer));
        _currentSelectedCharacters.Add(new Character("sample03", 100, 100, 10, 0, 10, 3, CharacterType.Supporter));
    }

    public List<Character> GetCharactersData() { return _currentSelectedCharacters; }

    private void InputCheatKey() {
        if (Input.GetKeyDown(KeyCode.Keypad3))
            TurnManager.Inst.EndTurn();

        if (Input.GetKeyDown(KeyCode.Space)) {
            TurnManager.OnAddCard?.Invoke();
        }
    }
}