using System;
using UniRx;

namespace Core.UI.ViewModels {
	public class SlotsControlsScreenViewModel : IDisposable {
		public ReactiveCommand spin { get; }

		public SlotsControlsScreenViewModel (IObservable<bool> canSpin) {
			spin = new ReactiveCommand(canSpin);
		}

		public void Spin () {
			spin.Execute();
		}

		public void Dispose () {
			spin.Dispose();
		}
	}
}
