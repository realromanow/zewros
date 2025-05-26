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
			
			transform.SetParent(item.context.Value.joint, false);
			
			_winningEffect.SetActive(false);

			item.isWinner
				.Subscribe(PrepareWinAnimation)
				.AddTo(bindingDisposable);

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
					
					if (item.isWinner.Value) _shatterEffect.InitShader();

					var expireDuration = _defaultDuration;
					var expireDelay = item.isWinner.Value ? startDelay + defaultDelay * item.context.Value.columnOrder : startDelay + (defaultDelay * item.context.Value.fieldOrder);
					
					transform.DOLocalMoveY(-10f, expireDuration)
						.SetEase(Ease.InOutBounce)
						.SetDelay(expireDelay)
						.OnComplete(() => Destroy(gameObject));
				})
				.AddTo(bindingDisposable);
			
			_creationSequence = DOTween.Sequence();

			var createAnimDelay = startDelay + (defaultDelay * (item.context.Value.fieldOrder + item.context.Value.columnLength));
			var generationDelay = duration + startDelay + _winEffectDuration + duration + startDelay + (defaultDelay * (item.context.Value.fieldLength + item.context.Value.columnLength * 2));
			
			item.context
				.Skip(1)
				.Subscribe(context => {
					Observable.Timer(TimeSpan.FromSeconds(generationDelay))
						.Subscribe(_ => {
							transform.SetParent(context.joint);
							transform.DOLocalMoveY(0, duration)
								.SetEase(Ease.OutBounce);
						})
						.AddTo(bindingDisposable);
				})
				.AddTo(bindingDisposable);

			
			var createTween = transform.DOLocalMoveY(0f, duration)
				.SetEase(Ease.OutBounce)
				.SetDelay(item.generation <= 0 ? createAnimDelay : duration + duration + startDelay + _winEffectDuration + duration + startDelay + (defaultDelay * (item.context.Value.fieldLength + item.context.Value.columnLength * 2)));

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
			
			Observable.Timer(TimeSpan.FromSeconds(_winEffectDuration))
				.TakeUntil(_winTimerSubject)
				.Subscribe(_ => item.Expire())
				.AddTo(bindingDisposable);
			
			transform.DOLocalMove(SpriteOffsetUtils.CalculateOffset(_spriteRenderer, _winEffectScale, OffsetCalculationType.GlobalCenter),
				_defaultDuration);

			_winningSequence = DOTween.Sequence();
			_winningSequence.Append(transform.DOScale(_winEffectScale, 0.2f).SetEase(Ease.OutBack));

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

			var orderDelay = startDelay + (defaultDelay * (item.context.Value.fieldLength + item.context.Value.columnLength));
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
