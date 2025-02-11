using System.Collections.Generic;
using R3;
using UnityEngine;

public class BattleModel {
    public ReactiveProperty<bool> IsEnemyTurn { get; } = new();
    
    public void SetTurnIndicator(bool isEnemyTurn) {
        IsEnemyTurn.Value = isEnemyTurn;
    }
}
