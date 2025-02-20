using Entities.Base;

public enum EnemyType {
    Normal,
}

[System.Serializable]
public class Enemy : BaseEntity {
    public EnemyType Type { get; private set; }

    public Enemy(string templateId, int hp, int atk, EnemyType type)
        : base(templateId, hp,  atk) {
        Type = type;
    }
    
    

    // Character-specific methods can be added here
}