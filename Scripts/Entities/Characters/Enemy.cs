public enum EnemyType {
    Normal,
}

[System.Serializable]
public class Enemy : GameEntity {
    public EnemyType Type { get; private set; }

    public Enemy(string templateId, int hp, int mp, int atk, int agi, int startAP, EnemyType type)
        : base(templateId, hp, mp, atk , agi, startAP) {
        Type = type;
    }

    // Character-specific methods can be added here
}