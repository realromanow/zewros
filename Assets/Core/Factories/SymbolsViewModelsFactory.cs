using Core.Context;
using Core.Models;
using Core.ViewModels;
using UnityEngine;

namespace Core.Factories {
	public class SymbolsViewModelsFactory {
		public SymbolViewModel[] CreateViewModelsFromPack (SymbolsPackModel symbolPack, int columnOrder, int fieldLength, Transform[] joints) {
			var viewModels = new SymbolViewModel[symbolPack.packLength];

			for (var i = 0; i < viewModels.Length; i++) {
				viewModels[i] = CreateViewModel(symbolPack.symbols[i], columnOrder, (columnOrder + i) + (symbolPack.packLength * columnOrder), symbolPack.packLength, fieldLength, joints[i]);
			}

			return viewModels;
		}

		public SymbolViewModel CreateViewModel (SymbolModel symbolModel, int columnOrder, int fieldOrder, int packLength, int fieldLength, Transform joint) {
			return new SymbolViewModel(symbolModel, symbolModel.isWinner, new SymbolViewContext(fieldOrder, packLength, fieldLength, columnOrder, joint));
		}
	}
}
