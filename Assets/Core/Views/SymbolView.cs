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
		private float _startDelay;
		
		[SerializeField]
		private float _defaultDelay;

		[SerializeField]
		private float _defaultDuration;

		protected override void RegisterInitialize () {
			base.RegisterInitialize();

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
			
			base.RegisterDestroy();
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
			// transform.DOLocalRotate(new Vector3(0f, 0f, item.order % 2 == 0 ? 10f : -10f), _defaultDuration)
			//transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(0, 3) % 2 == 0 ? 10f : -10f), _defaultDuration)
				//.SetEase(Ease.InOutBounce);
		}

		public void SetSprite (Sprite sprite) {
			_spriteRenderer.sprite = sprite;
		}
	}
}
