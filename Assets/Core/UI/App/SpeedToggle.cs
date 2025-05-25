using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.App {
	public class SpeedToggle : MonoBehaviour {
		[SerializeField] private Toggle _instantModeToggle;
		private AnimationSpeedService _animationSpeedService;
        
		public void Initialize(AnimationSpeedService animationSpeedService) {
			_animationSpeedService = animationSpeedService;
            
			if (_instantModeToggle != null) {
				_instantModeToggle.onValueChanged.AddListener(OnToggleChanged);
			}
		}
        
		private void OnToggleChanged(bool isOn) {
			_animationSpeedService?.SetInstantMode(isOn);
		}
        
		private void OnDestroy() {
			if (_instantModeToggle != null) {
				_instantModeToggle.onValueChanged.RemoveListener(OnToggleChanged);
			}
		}
	}
}
