using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

interface ITurnManager {
    void StartGame();
    void EndTurn();
}

public class TurnManager : MonoBehaviour, ITurnManager
{
    public static TurnManager Inst { get; private set; }
    void Awake() {
        if (Inst != null && Inst != this) {
            Destroy(gameObject); // 중복된 인스턴스가 생성되는 것 방지
        } else {
            Inst = this;
        }
    }

    [Header("Develop")]
    [SerializeField] private ETurnMode eTurnMode;
    [SerializeField] private bool fastMode;
    [SerializeField] private int startCardCount;
    [SerializeField] private int currentAPCount;
    [SerializeField] private int currentRound;
    [SerializeField] private int currentTurn;

    [Header("Properties")]
    public bool IsLoading { get; private set; } // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭방지
    private bool _myTurn;

    enum ETurnMode { My, Enemy }

    private const float TURN_DELAY_SHORT = 0.05f;
    private const float TURN_DELAY_LONG = 0.7f;
    
    private WaitForSeconds delayShort;
    private WaitForSeconds delayLong;

    public static Action OnClickSkillButton;
    public static Action OnAddCard;
    public static event Action<bool> OnTurnStarted;

    private void GameSetup() {
        OnClickSkillButton += ClickSkillButton;
        CardManager.Inst.SetupItemBuffer();
        
        // fastMode에 따라 delay 설정
        delayShort = new WaitForSeconds(fastMode ? TURN_DELAY_SHORT : TURN_DELAY_LONG);
        delayLong = new WaitForSeconds(TURN_DELAY_LONG);

        // 턴 설정
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
    
    private void ClickSkillButton() { StartCoroutine(ClickSkillButtonCo()); }
    
    private IEnumerator ClickSkillButtonCo() {
        IsLoading = true;

        for (int i = 0; i < startCardCount; i++) {
            yield return delayShort;
            OnAddCard?.Invoke();
        }
        StartCoroutine(StartTurnCo());
    }

    private IEnumerator StartTurnCo()
    {
        IsLoading = true;
        yield return delayLong;
        IsLoading = false;
        OnTurnStarted?.Invoke(_myTurn);
    }
    

    public void EndTurn() {
        _myTurn = !_myTurn;
        Debug.Log(_myTurn ? "나의 턴" : "적의 턴");
    }
}