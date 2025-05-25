using Core.Context;
using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using UniRx;

namespace Core.Factories {
	public class SymbolsViewModelsFactory {
		public SymbolViewModel[] CreateViewModel (SymbolsPackModel symbolPack, ref int order, int totalOrders, ICollection<IDisposable> disposables) {
			var viewModels = new SymbolViewModel[symbolPack.packLength];

			for (var i = 0; i < viewModels.Length; i++) {
				viewModels[i] = new SymbolViewModel(symbolPack.symbols[i], new SymbolViewContext(order++, symbolPack.packLength, totalOrders));
				viewModels[i].AddTo(disposables);
			}

			return viewModels;
		}
	}
}
