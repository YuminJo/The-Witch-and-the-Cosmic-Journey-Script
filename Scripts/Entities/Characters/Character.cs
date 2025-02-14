using System.Collections.Generic;
using Entities.Cards;

public enum CharacterType {
    Tanker,
    Dealer,
    Supporter,
    None
}

[System.Serializable]
public class Character : GameEntity {
    public CharacterType Type { get; private set; }
    public int Exp { get; private set; }
    public Character(string templateId, int hp, int mp, int atk, int startAP, CharacterType type)
        : base(templateId, hp, mp, atk , startAP) {
        Type = type;
    }
    
    public Character(Character character) : base(character.TemplateId, character.Hp.Value, character.Mp.Value, character.Atk, character.StartAP) {
        Type = character.Type;
    }
    
    
}