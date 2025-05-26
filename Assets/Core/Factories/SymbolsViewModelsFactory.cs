using Core.Context;
using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Factories {
	public class SymbolsViewModelsFactory {
		public SymbolViewModel[] CreateViewModelsFromPack (SymbolsPackModel symbolPack, int columnOrder, int fieldLength, Transform[] joints, ICollection<IDisposable> disposables) {
			var viewModels = new SymbolViewModel[symbolPack.packLength];

			for (var i = 0; i < viewModels.Length; i++) {
				viewModels[i] = new SymbolViewModel(symbolPack.symbols[i], symbolPack.symbols[i].isWinner, new SymbolViewContext((columnOrder + i) + (symbolPack.packLength * columnOrder), symbolPack.packLength, fieldLength, columnOrder, joints[i]));
				viewModels[i].AddTo(disposables);
			}

			return viewModels;
		}
	}
}
