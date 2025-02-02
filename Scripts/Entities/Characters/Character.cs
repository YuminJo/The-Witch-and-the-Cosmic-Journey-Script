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
    public Character(string templateId, int hp, int mp, int atk, int agi, int startAP, CharacterType type)
        : base(templateId, hp, mp, atk , agi, startAP) {
        Type = type;
    }

    // Character-specific methods can be added here
}