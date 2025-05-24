using Core.ViewModels;
using DG.Tweening;
using Plugins.Modern.DI.Binding;
using UniRx;
using UnityEngine;

namespace Core.Views {
    public class SymbolView : ModernItemBinding<SymbolViewModel> {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        [SerializeField]
        private GameObject _winningEffect; // Префаб эффекта выигрыша
        
        [SerializeField]
        private float _startDelay;
        
        [SerializeField]
        private float _defaultDelay;

        [SerializeField]
        private float _defaultDuration;
        
        private Sequence _winningAnimation;

        protected override void RegisterInitialize () {
            base.RegisterInitialize();

            // Подписка на изменение статуса выигрыша
            item.isWinning
                .Subscribe(isWinning => {
                    if (isWinning) {
                        ShowWinningEffect();
                    } else {
                        HideWinningEffect();
                    }
                })
                .AddTo(bindingDisposable);

            item.expire
                .Skip(1)
                .Take(1)
                .Subscribe(_ => Destroy(this.gameObject))
                .AddTo(bindingDisposable);
            
            item.expire
                .Take(1)
                .Subscribe(_ => {
                    transform.DOComplete();

                    transform.DOLocalMoveY(-10f, _defaultDuration)
                        .SetEase(Ease.InOutBounce)
                        .SetDelay(_startDelay + (_defaultDelay * item.context.order));
                })
                .AddTo(bindingDisposable);

            transform.DOLocalMoveY(0f, _defaultDuration)
                .SetEase(Ease.OutBounce)
                .SetDelay(_startDelay + (_defaultDelay * (item.context.order + item.context.ordersLength)))
                .OnComplete(OnPostCreateAnimationCall);
        }

        protected override void RegisterDestroy () {
            transform.DOKill();
            _winningAnimation?.Kill();
            
            base.RegisterDestroy();
        }
        
        private void ShowWinningEffect() {
            // Визуальный эффект выигрыша
            if (_winningEffect != null) {
                _winningEffect.SetActive(true);
            }
            
            // Анимация пульсации
            _winningAnimation?.Kill();
            _winningAnimation = DOTween.Sequence();
            _winningAnimation.Append(transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack));
            _winningAnimation.Append(transform.DOScale(1f, 0.3f).SetEase(Ease.InBack));
            _winningAnimation.SetLoops(-1);
            
            // Подсветка спрайта
            _spriteRenderer.color = new Color(1.2f, 1.2f, 1.2f, 1f);
        }
        
        private void HideWinningEffect() {
            // Скрываем эффект
            if (_winningEffect != null) {
                _winningEffect.SetActive(false);
            }
            
            // Останавливаем анимацию
            _winningAnimation?.Kill();
            transform.localScale = Vector3.one;
            
            // Возвращаем нормальный цвет
            _spriteRenderer.color = Color.white;
        }

        private void OnPostExpireAnimationCall () {
            transform.DOLocalMoveY(-10f, _defaultDuration)
                .SetEase(Ease.InBounce)
                .OnComplete(OnExpire);
        }

        private void OnExpire () {
            Destroy(gameObject);
        }

        private void OnPostCreateAnimationCall () {
            // Анимация после создания
        }

        public void SetSprite (Sprite sprite) {
            _spriteRenderer.sprite = sprite;
        }
    }
}
