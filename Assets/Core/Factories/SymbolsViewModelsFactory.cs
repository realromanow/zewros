using Core.Context;
using Core.Models;
using Core.ViewModels;
using UnityEngine;

namespace Core.Factories {
	public class SymbolsViewModelsFactory {
		public SymbolViewModel[] CreateViewModelsFromPack (SymbolsPackModel symbolPack, int columnOrder, int fieldLength, Transform[] joints) {
			var viewModels = new SymbolViewModel[symbolPack.packLength];

			for (var i = 0; i < viewModels.Length; i++) {
				var fieldOrder = (columnOrder * symbolPack.packLength) + i;
				
				viewModels[i] = CreateViewModel(
					symbolPack.symbols[i],
					columnOrder, 
					fieldOrder, 
					symbolPack.packLength, 
					fieldLength, 
					joints[i]);
			}

			return viewModels;
		}

		public SymbolViewModel CreateViewModel (SymbolModel symbolModel, int columnOrder, int fieldOrder, int packLength, int fieldLength, Transform joint) {
			return new SymbolViewModel(symbolModel, new SymbolViewContext(fieldOrder, packLength, fieldLength, columnOrder, joint));
		}
	}
}
