using Core.Factories;
using Core.Models;
using System;
using UniRx;

namespace Core.App {
	public class SlotsRoundService : IDisposable {
		public IReadOnlyReactiveProperty<bool> canSpin => _canSpin;
		
		private readonly ReactiveCommand _newRound = new();
		private readonly ReactiveProperty<bool> _canSpin =  new(true);
		
		private readonly SymbolsPacksFactory _symbolsPacksFactory;
		private readonly SlotsGameViewBuilderService _viewBuilderService;
		private readonly SlotsGameFieldProvider _fieldProvider;
		
		private readonly CompositeDisposable _compositeDisposable = new();


		public SlotsRoundService (
			SymbolsPacksFactory symbolsPacksFactory,
			SlotsGameViewBuilderService viewBuilderService,
			SlotsGameFieldProvider fieldProvider) {
			_symbolsPacksFactory = symbolsPacksFactory;
			_viewBuilderService = viewBuilderService;
			_fieldProvider = fieldProvider;
		}

		public void MakeSpin () {
			_newRound.Execute();
			
			var packsCreated = 0;
			var context = _fieldProvider.activeField.Value;
			
			var symbolsPacks = new SymbolsPackModel[context.columns.Length];

			for (var i = 0; i < symbolsPacks.Length; i++) {
				var packLength = context.columns[i].joints.Length;

				symbolsPacks[i] =
					_symbolsPacksFactory
						.GetPack(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") + packsCreated++,
							packLength);
			}
			
			_viewBuilderService.BuildViews(symbolsPacks, context, _newRound, _compositeDisposable);
		}

		public void Dispose () {
			_compositeDisposable.Dispose();
		}
	}
}
