using System.Collections.Generic;
using R3;
using UnityEngine;

public class BattleModel {
    public ReactiveProperty<bool> IsEnemyTurn { get; } = new();
    public const int TurnIndicatorDelay = 5000; // Corrected initialization
    
    public void SetTurnIndicator(bool isEnemyTurn) {
        IsEnemyTurn.Value = isEnemyTurn;
    }
}
