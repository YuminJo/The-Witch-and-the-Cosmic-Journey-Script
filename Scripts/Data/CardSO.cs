using UnityEngine;

public enum CardType {
    Attack,
    Defense,
    Buff,
    Debuff
}

[System.Serializable]
public class Card {
    public string name;
    public CardType type;
    public int attack;
    public Sprite sprite;
    public float percent;
    
    /*public void Setup(Item item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;

        if (this.isFront)
        {
            character.sprite = this.item.sprite;
            nameTMP.text = this.item.name;
            attackTMP.text = this.item.attack.ToString();
            healthTMP.text = this.item.health.ToString();
        }
        else
        {
            card.sprite = cardBack;
            nameTMP.text = "";
            attackTMP.text = "";
            healthTMP.text = "";
        }
    }*/
}

[CreateAssetMenu(fileName = "New Card", menuName = "Scripable Object/CardSO")]
public class CardSO : ScriptableObject {
    public Card[] cards;
}
