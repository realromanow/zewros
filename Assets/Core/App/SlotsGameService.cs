using Core.Factories;
using Core.Models;
using System;
using System.Globalization;
using UniRx;

namespace Core.App {
	public class SlotsGameService : IDisposable {
		private readonly ReactiveCommand _expireCommand = new();
		
		private readonly SymbolsPacksFactory _symbolsPacksFactory;
		private readonly SlotsGameViewBuilderService _viewBuilderService;
		private readonly SlotsGameFieldProvider _fieldProvider;
		
		private readonly CompositeDisposable _compositeDisposable = new();


		public SlotsGameService (
			SymbolsPacksFactory symbolsPacksFactory,
			SlotsGameViewBuilderService viewBuilderService,
			SlotsGameFieldProvider fieldProvider) {
			_symbolsPacksFactory = symbolsPacksFactory;
			_viewBuilderService = viewBuilderService;
			_fieldProvider = fieldProvider;
		}

		public void MakeSpin () {
			_expireCommand.Execute();
			
			var packsCreated = 0;
			var context = _fieldProvider.activeField.Value;
			
			var symbolsPacks = new SymbolsPackModel[context.columns.Length];

			for (var i = 0; i < symbolsPacks.Length; i++) {
				var packLength = context.columns[i].joints.Length;

				symbolsPacks[i] =
					_symbolsPacksFactory
						.GetPack(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) + packsCreated++,
							packLength);
			}
			
			_viewBuilderService.BuildViews(symbolsPacks, context, _expireCommand, _compositeDisposable);
		}

		public void Dispose () {
			_compositeDisposable.Dispose();
		}
	}
}
