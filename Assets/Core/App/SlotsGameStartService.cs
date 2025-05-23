using System;
using UniRx;

namespace Core.App {
	public class SlotsGameStartService : IDisposable {
		private readonly SlotsGameScreenService _screenService;
		private readonly SlotsRoundService _slotsRoundService;

		private readonly CompositeDisposable _compositeDisposable = new();
		
		public SlotsGameStartService (
			SlotsRoundService slotsRoundService,
			SlotsGameScreenService screenService) {
			_slotsRoundService = slotsRoundService;
			_screenService = screenService;
		}

		public void StartGame () {
			_screenService.ShowSlotsControlsScreen(_ => _slotsRoundService.MakeSpin(), _slotsRoundService.canSpin, _compositeDisposable);
		}

		public void Dispose() {
			_compositeDisposable.Dispose();
		}
	}
}
