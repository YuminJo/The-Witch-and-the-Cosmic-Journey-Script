// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버
public class GameManager {

    List<Character> _selectedCharacters = new();
    
    public void Init() => OnLoadTestData();
    void Update() {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    private void OnLoadTestData() {
        Character sample01 = new Character("sample01", 100, 100, 10, 1, 3, CharacterType.Tanker);
        Character sample02 = new Character("sample02", 100, 100, 20, 2, 3, CharacterType.Dealer);
        Character sample03 = new Character("sample03", 100, 100, 30, 3, 3, CharacterType.Supporter);
        _selectedCharacters.Add(sample01);
        _selectedCharacters.Add(sample02);
        _selectedCharacters.Add(sample03);
    }
    
    public List<Character> GetSelectedCharacters() => _selectedCharacters;
    
    private void InputCheatKey() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TurnSystem.OnAddCard?.Invoke();
        }
    }
}