using System;
using UniRx;

namespace Core.UI.ViewModels {
	public class SlotsControlsScreenViewModel : IDisposable {
		public ReactiveCommand spin { get; }
		public ReactiveCommand toggleTestMode { get; }
		public IReadOnlyReactiveProperty<bool> isTestMode => _isTestMode;
		public IReadOnlyReactiveProperty<string> lastWinInfo => _lastWinInfo;
        
		private readonly ReactiveProperty<bool> _isTestMode = new(false);
		private readonly ReactiveProperty<string> _lastWinInfo = new("No spins yet");

		public SlotsControlsScreenViewModel (IObservable<bool> canSpin) {
			spin = new ReactiveCommand(canSpin);
			toggleTestMode = new ReactiveCommand();
            
			toggleTestMode.Subscribe(_ => {
				_isTestMode.Value = !_isTestMode.Value;
			});
		}

		public void Spin () {
			spin.Execute();
		}
        
		public void ToggleTestMode() {
			toggleTestMode.Execute();
		}
        
		public void UpdateWinInfo(string info) {
			_lastWinInfo.Value = info;
		}

		public void Dispose () {
			spin.Dispose();
			toggleTestMode.Dispose();
			_isTestMode.Dispose();
			_lastWinInfo.Dispose();
		}
	}
}
