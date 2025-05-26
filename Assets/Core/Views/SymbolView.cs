using Core.Utils;
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
		private GlossySpriteEffect _glossySpriteEffect;
		
		[SerializeField]
		private ShatterEffect _shatterEffect;

		[SerializeField]
		private float _startDelay;

		[SerializeField]
		private float _defaultDelay;

		[SerializeField]
		private float _defaultDuration;

		[SerializeField]
		private float _winEffectDuration;

		[SerializeField]
		private float _winEffectScale;

		private Sequence _creationSequence;
		private Sequence _winningSequence;

		private Subject<Unit> _winTimerSubject = new();

		protected override void RegisterInitialize () {
			base.RegisterInitialize();

			_winningEffect.SetActive(false);

			PrepareWinAnimation(item.isWinner);

			transform.SetParent(item.context.joint, false);

			var duration = _defaultDuration;
			var startDelay = _startDelay;
			var defaultDelay = _defaultDelay;

			item.expire
				.Skip(1)
				.Take(1)
				.Subscribe(_ => { Destroy(gameObject); })
				.AddTo(bindingDisposable);

			item.expire
				.Take(1)
				.Subscribe(_ => {
					_creationSequence.Complete();
					_winTimerSubject.OnCompleted();

					var expireDuration = _defaultDuration;
					var expireDelay = item.isWinner ? startDelay + defaultDelay * item.context.columnOrder : startDelay + (defaultDelay * item.context.fieldOrder);
					
					var expireTween = transform.DOLocalMoveY(-10f, expireDuration)
						.SetEase(Ease.InOutBounce)
						.SetDelay(expireDelay)
						.OnComplete(() => Destroy(gameObject));
				})
				.AddTo(bindingDisposable);

			_creationSequence = DOTween.Sequence();

			var createTween = transform.DOLocalMoveY(0f, duration)
				.SetEase(Ease.OutBounce)
				.SetDelay(startDelay + (defaultDelay * (item.context.fieldOrder + item.context.columnLength)));

			_creationSequence.Append(createTween);
		}

		protected override void RegisterDestroy () {
			_winningSequence?.Kill();
			_creationSequence?.Kill();

			transform.DOKill();

			_winTimerSubject.Dispose();

			base.RegisterDestroy();
		}

		private void ShowWinningEffect () {
			if (_winningEffect != null) _winningEffect.SetActive(true);

			_glossySpriteEffect.PlayAnim();
			
			Observable.Timer(TimeSpan.FromSeconds(1f))
				.TakeUntil(_winTimerSubject)
				.Subscribe(_ => _shatterEffect.InitShader())
				.AddTo(bindingDisposable);
			
			transform.DOLocalMove(SpriteOffsetUtils.CalculateOffset(_spriteRenderer, _winEffectScale, OffsetCalculationType.GlobalCenter),
				_defaultDuration);

			_winningSequence = DOTween.Sequence();
			_winningSequence.Append(transform.DOScale(_winEffectScale, _winEffectDuration).SetEase(Ease.OutBack));
			// _winningSequence.Join(transform.DORotate(new Vector3(0f, 0f, 10f), _winEffectDuration).SetEase(Ease.InOutBack));
			// _winningSequence.Append(transform.DORotate(new Vector3(0f, 360f, 0f), _winEffectDuration * 5f, RotateMode.FastBeyond360).SetEase(Ease.InOutBack));
			// _winningSequence.Append(transform.DOScale(1f, _winEffectDuration).SetEase(Ease.InBack));
			// _winningSequence.SetLoops(-1);

			_spriteRenderer.sortingOrder = 10;
			_spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
		}

		private void OnPostCreateAnimationCall () {}

		private void PrepareWinAnimation (bool isWinning) {
			if (!isWinning) return;

			var duration = _defaultDuration;
			var startDelay = _startDelay;
			var defaultDelay = _defaultDelay;

			_winTimerSubject = new Subject<Unit>();

			var orderDelay = (duration * item.context.columnLength) + startDelay + (defaultDelay * (item.context.fieldLength + item.context.columnLength));
			var effectDelay = orderDelay + _defaultDelay;

			Observable.Timer(TimeSpan.FromSeconds(effectDelay))
				.TakeUntil(_winTimerSubject)
				.Subscribe(_ => {}, ShowWinningEffect)
				.AddTo(bindingDisposable);
		}

		public void SetSprite (Sprite sprite) {
			_spriteRenderer.sprite = sprite;
		}
	}
}
