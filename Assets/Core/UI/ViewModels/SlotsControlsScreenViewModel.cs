using UniRx;

namespace Core.UI.ViewModels {
	public class SlotsControlsScreenViewModel {
		public ReactiveCommand spin { get; }
		
		public SlotsControlsScreenViewModel (ReactiveCommand spin) {
			this.spin = spin;
		}
	}
}
