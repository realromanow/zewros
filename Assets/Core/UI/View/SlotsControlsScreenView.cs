using Core.UI.ViewModels;
using Plugins.Modern.DI.Binding;

namespace Core.UI.View {
	public class SlotsControlsScreenView : ModernItemBinding<SlotsControlsScreenViewModel> {
		public void Spin () {
			item.spin.Execute();
		}
	}
}
