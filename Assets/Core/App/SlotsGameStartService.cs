using System;
using UniRx;

namespace Core.App {
	public class SlotsGameStartService : IDisposable {
		private readonly SlotsGameScreenService _screenService;
		private readonly SlotsGameService _slotsGameService;

		private readonly CompositeDisposable _compositeDisposable = new();
		
		public SlotsGameStartService (
			SlotsGameService slotsGameService,
			SlotsGameScreenService screenService) {
			_slotsGameService = slotsGameService;
			_screenService = screenService;
		}

		public void StartGame () {
			_screenService.ShowSlotsControlsScreen(_ => _slotsGameService.MakeSpin(), _compositeDisposable);
		}

		public void Dispose() {
			_compositeDisposable.Dispose();
		}
	}
}
