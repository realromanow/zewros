using Core.UI;
using Core.UI.ViewModels;
using System;
using System.Collections.Generic;
using UniRx;

namespace Core.App {
	public class SlotsGameScreenService {
		private readonly CoreUIService _uiService;

		public SlotsGameScreenService (CoreUIService uiService) {
			_uiService = uiService;
		}

		public void ShowSlotsControlsScreen (Action<Unit> onSpin, ICollection<IDisposable> disposables) {
			var viewModel = new SlotsControlsScreenViewModel();
			viewModel.spin.Subscribe(onSpin)
				.AddTo(disposables);

			_uiService.ShowControlsScreen(viewModel);
		}
	}
}
