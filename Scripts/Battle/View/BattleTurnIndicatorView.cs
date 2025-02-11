using System;
using Cysharp.Threading.Tasks;

namespace Battle.View {
    public interface IBattleTurnIndicatorView {
        public UniTask<bool> SetTurnIndicator(bool isEnemyTurn, Action action);
    }
    public class BattleTurnIndicatorView : UI_Popup, IBattleTurnIndicatorView
    {
        enum GameObjects {
            TurnIndicator
        }
        enum Texts {
            TurnIndicatorText
        }
        
        private const int TurnIndicatorDelay = 5000;
        
        public override bool Init() {
            if (base.Init() == false)
                return false;
            
            BindObject(typeof(GameObjects));
            BindText(typeof(Texts));

            return true;
        }
    
        /// <summary>
        /// 턴 UI를 표시합니다.
        /// </summary>
        /// <param name="isEnemyTurn"></param>
        /// <param name="turnDelayTime"></param>
        public async UniTask<bool> SetTurnIndicator(bool isEnemyTurn, Action action) {
            await UniTask.WaitUntil(() => _init);
            
            GetObject((int)GameObjects.TurnIndicator).SetActive(true);
            GetText((int)Texts.TurnIndicatorText).text = isEnemyTurn ? "Enemy Turn" : "Player Turn";
        
            await UniTask.Delay(TurnIndicatorDelay);
            GetObject((int)GameObjects.TurnIndicator).SetActive(false);
            
            ClosePopupUI();
            action();
            return isEnemyTurn;
        }
    }
}