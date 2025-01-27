using System.Collections.Generic;
using UnityEngine;

public class BattleModel {
    public List<Character> CurrentCharacters = new();

    public BattleModel() {
        Managers.Game.GetCharactersData().ForEach(character => CurrentCharacters.Add(character));
    }
}
