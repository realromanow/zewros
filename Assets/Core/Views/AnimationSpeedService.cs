using UniRx;

namespace Core.Views {
	public class AnimationSpeedService {
		public IReadOnlyReactiveProperty<bool> isInstantMode => _isInstantMode;
		private readonly ReactiveProperty<bool> _isInstantMode = new(false);
        
		public void SetInstantMode(bool instant) {
			_isInstantMode.Value = instant;
		}
        
		public float GetAnimationDuration(float normalDuration) {
			return _isInstantMode.Value ? 0.01f : normalDuration;
		}
        
		public float GetAnimationDelay(float normalDelay) {
			return _isInstantMode.Value ? 0f : normalDelay;
		}
	}
}
