using Core.ViewModels;
using DG.Tweening;
using Plugins.Modern.DI.Binding;
using System;
using UniRx;
using UnityEngine;

namespace Core.Views {
	public class SymbolView : ModernItemBinding<SymbolViewModel> {
		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[SerializeField]
		private GameObject _winningEffect;

		[SerializeField]
		private float _startDelay = 0.1f;

		[SerializeField]
		private float _defaultDelay = 0.05f;

		[SerializeField]
		private float _defaultDuration = 0.5f;

		private Sequence _winningAnimation;
		private Tween _liveAnimation;
		private AnimationSpeedService _animationSpeedService;

		private Subject<Unit> _winTimerSubject = new();

		public void InjectDependencies (AnimationSpeedService animationSpeedService) {
			_animationSpeedService = animationSpeedService;
		}

		protected override void RegisterInitialize () {
			base.RegisterInitialize();

			transform.SetParent(item.context.joint, false);
			
			var duration = _animationSpeedService?.GetAnimationDuration(_defaultDuration) ?? _defaultDuration;
			var startDelay = _animationSpeedService?.GetAnimationDelay(_startDelay) ?? _startDelay;
			var defaultDelay = _animationSpeedService?.GetAnimationDelay(_defaultDelay) ?? _defaultDelay;

			HideWinningEffect();

			item.isWinning
				.Subscribe(CheckWinningAnimation)
				.AddTo(bindingDisposable);

			item.expire
				.Skip(1)
				.Take(1)
				.Subscribe(_ => Destroy(gameObject))
				.AddTo(bindingDisposable);

			item.expire
				.Take(1)
				.Subscribe(_ => {
					_liveAnimation?.Complete();
					_winTimerSubject.OnCompleted();

					var expireDuration = _animationSpeedService?.GetAnimationDuration(_defaultDuration) ?? _defaultDuration;
					var expireDelay = _animationSpeedService?.GetAnimationDelay(startDelay + (defaultDelay * item.context.order)) ?? startDelay + (defaultDelay * item.context.order);

					_liveAnimation = transform.DOLocalMoveY(-10f, expireDuration)
						.SetEase(Ease.InOutBounce)
						.SetDelay(expireDelay);
				})
				.AddTo(bindingDisposable);

			_liveAnimation = transform.DOLocalMoveY(0f, duration)
				.SetEase(Ease.OutBounce)
				.SetDelay(startDelay + (defaultDelay * (item.context.order + item.context.rowOrdersLength)));
		}

		protected override void RegisterDestroy () {
			transform.DOKill();
			_winningAnimation?.Kill();

			_winTimerSubject.Dispose();

			base.RegisterDestroy();
		}

		private void ShowWinningEffect () {
			if (_winningEffect != null) _winningEffect.SetActive(true);

			_winningAnimation?.Kill();
			_winningAnimation = DOTween.Sequence();
			_winningAnimation.Append(transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack));
			_winningAnimation.Append(transform.DOScale(1f, 0.3f).SetEase(Ease.InBack));
			_winningAnimation.SetLoops(-1);

			_spriteRenderer.color = new Color(1.2f, 1.2f, 1.2f, 1f);
			_spriteRenderer.sortingOrder += 1;
		}

		private void HideWinningEffect () {
			if (_winningEffect != null) _winningEffect.SetActive(false);

			_winningAnimation?.Kill();
			transform.localScale = Vector3.one;

			_spriteRenderer.color = Color.white;
		}

		private void OnPostCreateAnimationCall () {
		}

		private void CheckWinningAnimation (bool isWinning) {
			if (!isWinning) return;

			var duration = _animationSpeedService?.GetAnimationDuration(_defaultDuration) ?? _defaultDuration;
			var startDelay = _animationSpeedService?.GetAnimationDelay(_startDelay) ?? _startDelay;
			var defaultDelay = _animationSpeedService?.GetAnimationDelay(_defaultDelay) ?? _defaultDelay;

			_winTimerSubject = new Subject<Unit>();

			Observable.Timer(TimeSpan.FromSeconds((duration * (item.context.totalOrdersLength)) + startDelay))
				.TakeUntil(_winTimerSubject)
				.Subscribe(_ => {}, ShowWinningEffect)
				.AddTo(bindingDisposable);
		}

		public void SetSprite (Sprite sprite) {
			_spriteRenderer.sprite = sprite;
		}
	}
}
