using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.View {
    public interface IBattleView {
        bool Init();
        void SetButtonRaycastByTurnValue(bool value);
        void EndEnemyTurn(Action action);
        void SetEnergy(int value);
        void OnClickCardSelectButton(float turnDelayShort);
        void OnClickEndTurnButton(Action action);
        void OnClickCardCancelButton(Action action);
        void CreateCharacterView(Character character);
        void OverLayEnergy(int value, BattleCardView battleCardView);
        void DeOverLayEnergy();
    }

    public class BattleView : UI_Popup, IBattleView {
        enum Buttons {
            CardSelectButton,
            ItemSelectButton,
            EndTurnButton,
            CardCancelButton
        }

        enum GameObjects {
            CharacterGroup,
            BackgroundEnergyGroup,
            EnergyGroup,
        }

        enum Texts {
            EnergyCountText
        }

        private BattlePresenter _presenter;
        private List<Image> _energyImages;
        private List<Image> _backgroundEnergyImages;
        private BattleCardView _selectedCard;
        
        private Vector2 _animationVector = new Vector2(150, 250);
        private List<Vector3> _originPosition = new();
        
        public override bool Init() {
            if (!base.Init())
                return false;

            BindButton(typeof(Buttons));
            BindObject(typeof(GameObjects));
            BindText(typeof(Texts));
            SetCanvas(gameObject, false, true);
            
            GetButton((int)Buttons.CardCancelButton).gameObject.SetActive(false);

            var energyGroup = GetObject((int)GameObjects.EnergyGroup);
            var backgroundEnergyGroup = GetObject((int)GameObjects.BackgroundEnergyGroup);

            _energyImages = new List<Image>(energyGroup.GetComponentsInChildren<Image>());
            _backgroundEnergyImages = new List<Image>(backgroundEnergyGroup.transform.GetComponentsInChildren<Image>());
            
            _presenter = new BattlePresenter(this);
            return true;
        }
        
        public void SetButtonRaycastByTurnValue(bool value) {
            bool isInteractable = !value;
            SetButtonRaycast((int)Buttons.EndTurnButton, isInteractable);
            SetButtonRaycast((int)Buttons.CardSelectButton, isInteractable);
            SetButtonRaycast((int)Buttons.ItemSelectButton, isInteractable);
        }
        
        private void SetButtonRaycast(int buttonIndex, bool value) 
            => GetButton(buttonIndex).RaycastTarget(value);
        
        public void CreateCharacterView(Character character) 
            => _presenter.CreateCharacterView(character, GetObject((int)GameObjects.CharacterGroup).transform);
        
        public void EndEnemyTurn(Action action) 
            => _presenter.IsPlayerTurn(action);
        
        public void OverLayEnergy(int value, BattleCardView battleCardView) {
            if (battleCardView == _selectedCard) return;

            DeOverLayEnergy();
            _selectedCard = battleCardView;

            for (int i = 0; i < value; i++) {
                AnimateEnergyImage(_energyImages[i].isActiveAndEnabled ? _energyImages[i] : _backgroundEnergyImages[i], Color.red);
            }
        }
        
        private void AnimateEnergyImage(Image image, Color color) 
            => image.DOColor(color, 0.5f).SetLoops(-1, LoopType.Yoyo);
        

        public void DeOverLayEnergy() {
            if (_selectedCard == null) return;
            _selectedCard = null;
            
            foreach (var image in _energyImages) ResetEnergyImage(image);
            foreach (var image in _backgroundEnergyImages) ResetEnergyImage(image);
        }

        private void ResetEnergyImage(Image image) {
            image.color = Color.white;
            DOTween.Kill(image);
        }
        
        private void SetOriginPosition() {
            var battleCharacterViews = GetBattleCharacterViews();
            _originPosition = new List<Vector3>(battleCharacterViews.Count);

            foreach (var view in battleCharacterViews) {
                _originPosition.Add(view.GetComponent<RectTransform>().localPosition);
            }
        }
        
        private List<BattleCharacterView> GetBattleCharacterViews() {
            var characterGroup = GetObject((int)GameObjects.CharacterGroup);
            return new List<BattleCharacterView>(characterGroup.GetComponentsInChildren<BattleCharacterView>());
        }

        private async UniTask BattleCharacterViewAnimation() {
            var characterGroup = GetObject((int)GameObjects.CharacterGroup);
            characterGroup.GetComponent<HorizontalLayoutGroup>().enabled = false;
            
            var battleCharacterViews = GetBattleCharacterViews();
            int count = battleCharacterViews.Count;

            for (int i = 0; i < count; i++) {
                var rectTransform = battleCharacterViews[i].GetComponent<RectTransform>();
                rectTransform.DOLocalMove(new Vector2(_animationVector.x, _animationVector.y - (i - 1) * 50), 0.5f).SetEase(Ease.OutQuart);
                await UniTask.Delay(100);
            }
        }

        private async UniTask ResetCharacterView() {
            GetButton((int)Buttons.CardCancelButton).gameObject.SetActive(false);
            
            var battleCharacterViews = GetBattleCharacterViews();
            int count = battleCharacterViews.Count -1;

            for (int i = count; i >= 0; i--) {
                var rectTransform = battleCharacterViews[i].GetComponent<RectTransform>();
                rectTransform.DOLocalMove(_originPosition[i], 0.5f).SetEase(Ease.OutQuart);
                await UniTask.Delay(100);
            }
        }

        public void SetEnergy(int value) {
            value = Math.Min(value, 6);

            for (int i = 0; i < _energyImages.Count; i++) {
                _energyImages[i].gameObject.SetActive(i < value);
            }

            GetText((int)Texts.EnergyCountText).text = $"{value} / 6";
        }

        public void OnClickCardSelectButton(float turnDelayShort) {
            BindButtonEvent((int)Buttons.CardSelectButton, () => {
                //TODO : 수정 해야됨.
                if (_originPosition.Count == 0) SetOriginPosition();
                
                _presenter.UpdateCard(turnDelayShort);
                GetButton((int)Buttons.CardCancelButton).gameObject.SetActive(true);
                BattleCharacterViewAnimation().Forget();
            });
        }
        
        public void OnClickEndTurnButton(Action action) {
            BindButtonEvent((int)Buttons.EndTurnButton, () => {
                _presenter.OnClickEndTurnButton(action);
                ResetCharacterView().Forget();
            });
        }
        
        public void OnClickCardCancelButton(Action action) {
            BindButtonEvent((int)Buttons.CardCancelButton, () => {
                action?.Invoke();
                ResetCharacterView().Forget();
            });
        }

        private void BindButtonEvent(int buttonIndex, Action action) {
            GetButton(buttonIndex).gameObject.BindEvent(action);
        }
    }
}