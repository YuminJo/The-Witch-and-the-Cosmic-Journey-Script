using System.Collections.Generic;
using Entities.Base;
using Entities.Cards;

public enum CharacterType {
    Tanker,
    Dealer,
    Supporter,
    None
}

public enum CharacterName {
    None,
    Mario,
    Luigi,
    Toad,
    Peach,
}

[System.Serializable]
public class Character : BaseEntity {
    public CharacterName Name { get; private set; }
    public CharacterType Type { get; private set; }
    public int Exp { get; private set; }
    public Character(CharacterName name,string templateId, int hp,  int atk, CharacterType type)
        : base(templateId, hp,  atk) {
        Name = name;
        Type = type;
    }
    
    public Character(Character character) : base(character.TemplateId, character.Hp.Value, character.Atk) {
        Type = character.Type;
    }
    
    
}