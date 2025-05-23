using Core.UI;
using Core.UI.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using UniRx;

namespace Core.App {
	public class GameController : IDisposable {
		private readonly ReactiveCommand _expireCommand = new();

		private readonly CoreUIService _uiService;
		private readonly SymbolsPacksFactory _symbolsPacksFactory;
		private readonly GameFieldFillerService _fieldFillerService;
		private readonly GameFieldProvider _fieldProvider;
		private readonly CompositeDisposable _compositeDisposable = new();

		public GameController (
			CoreUIService uiService,
			GameFieldFillerService fieldFillerService,
			GameFieldProvider fieldProvider,
			SymbolsPacksFactory symbolsPacksFactory) {
			_uiService = uiService;
			_fieldFillerService = fieldFillerService;
			_fieldProvider = fieldProvider;
			_symbolsPacksFactory = symbolsPacksFactory;
		}

		public void StartGame () {
			var packsLength = 0;

			var spinCommand = new ReactiveCommand().AddTo(_compositeDisposable);
			spinCommand.Subscribe(_ => {
					_expireCommand.Execute();

					var symbolsPacks = _fieldProvider.activeField.columns
						.Select(x =>
							_symbolsPacksFactory.GetPack(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) + packsLength++, x.joints.Length))
						.ToArray();

					_fieldFillerService.FillField(symbolsPacks, _fieldProvider.activeField, _expireCommand);
				})
				.AddTo(_compositeDisposable);

			var controllersScreenModel = new SlotsControlsScreenViewModel(spinCommand);
			_uiService.ShowControlsScreen(controllersScreenModel);
		}

		public void Dispose () {
			_expireCommand.Dispose();
			_compositeDisposable.Dispose();
		}
	}
}
