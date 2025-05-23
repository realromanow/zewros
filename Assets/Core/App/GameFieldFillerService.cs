using Core.Data;
using Core.Models;
using Core.ViewModels;
using UniRx;

namespace Core.App {
	public class GameFieldFillerService {
		private readonly SymbolsViewsFactory _symbolsViewsFactory;

		public GameFieldFillerService (
			SymbolsViewsFactory symbolsViewsFactory) {
			_symbolsViewsFactory = symbolsViewsFactory;
		}

		public void FillField (SymbolsPackModel[] symbolsPacks, GameFieldData field, ReactiveCommand expireCommand) {
			var order = 0;

			if (symbolsPacks.Length != field.columns.Length) throw new System.ArgumentException("symbols packs length not equals field columns length");

			for (var i = 0; i < symbolsPacks.Length; i++) {
				var viewModels = new SymbolViewModel[symbolsPacks[i].symbols.Length];

				for (var j = 0; j < symbolsPacks[i].symbols.Length; j++) {
					viewModels[j] = new SymbolViewModel(symbolsPacks[i].symbols[j], order++, symbolsPacks[i].packLength, expireCommand);
				}

				_symbolsViewsFactory.PackToView(viewModels, field.columns[i].joints);
			}
		}
	}
}
