using System;
using UniRx;

namespace Core.UI.ViewModels {
	public class SlotsControlsScreenViewModel : IDisposable {
		public ReactiveCommand spin { get; } = new();

		public void Spin () {
			spin.Execute();
		}
		
		public void Dispose () {
			spin.Dispose();
		}
	}
}
