using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버
public class GameManager
{
    List<Character> _currentSelectedCharacters = new();
    void Start() {
        InitData();
    }
    
    void InitData() {
        Character character1 = new Character {
            templateId = "char001",
            hp = 100,
            mp = 50,
            atk = 20,
            exp = 0,
            startAP = 5,
            type = CharacterType.Tanker
        };

        Character character2 = new Character {
            templateId = "char002",
            hp = 80,
            mp = 60,
            atk = 25,
            exp = 0,
            startAP = 4,
            type = CharacterType.Dealer
        };

        Character character3 = new Character {
            templateId = "char003",
            hp = 70,
            mp = 70,
            atk = 15,
            exp = 0,
            startAP = 6,
            type = CharacterType.Supporter
        };
        
        _currentSelectedCharacters.Add(character1);
        _currentSelectedCharacters.Add(character2);
        _currentSelectedCharacters.Add(character3);
    }

    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey()
    {

        if (Input.GetKeyDown(KeyCode.Keypad3))
            TurnManager.Inst.EndTurn();
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            TurnManager.OnAddCard?.Invoke();
        }

        /*if (Input.GetKeyDown(KeyCode.Keypad4))
            CardManager.Inst.TryPutCard(false);

        if (Input.GetKeyDown(KeyCode.Keypad5))
            EntityManager.Inst.DamageBoss(true, 19);

        if (Input.GetKeyDown(KeyCode.Keypad6))
            EntityManager.Inst.DamageBoss(false, 19);*/
    }

    public void Notification(string message)
    {
        //notificationPanel.Show(message);
    }

    /*public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return new WaitForSeconds(0.3f);

        TurnManager.Inst.isLoading = true;
        /*resultPanel.Show(isMyWin ? "승리" : "패배");
        cameraEffect.SetGrayScale(true);#1#
    }*/
}
