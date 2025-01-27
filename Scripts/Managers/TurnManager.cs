using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Inst { get; private set; }
    void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField] ETurnMode eTurnMode;
    [SerializeField] bool fastMode;
    [SerializeField] int startCardCount;
    [SerializeField] int currentAPCount;
    [SerializeField] int currentRound;
    [SerializeField] int currentTurn;

    [Header("Properties")]
    public bool isLoading; // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭방지
    private bool _myTurn;

    enum ETurnMode { My, Enemy }
    
    WaitForSeconds delay05 = new(0.5f);
    WaitForSeconds delay07 = new(0.7f);

    public static Action OnAddCard;
    public static event Action<bool> OnTurnStarted;

    void GameSetup() {
        CardManager.Inst.SetupItemBuffer();
        if (fastMode) delay05 = new WaitForSeconds(0.05f);

        switch (eTurnMode) {
            case ETurnMode.My:
                _myTurn = true;
                break;
            case ETurnMode.Enemy:
                _myTurn = false;
                break;
        }
    }
    
    public void StartGame() => GameSetup();
    

    public IEnumerator ClickSkillButton() {
        isLoading = true;

        for (int i = 0; i < startCardCount; i++) {
            yield return delay05;
            OnAddCard?.Invoke();
        }
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;
        yield return delay07;
        isLoading = false;
        OnTurnStarted?.Invoke(_myTurn);
    }
    

    public void EndTurn() {
        _myTurn = !_myTurn;
        Debug.Log(_myTurn ? "나의 턴" : "적의 턴");
    }
}