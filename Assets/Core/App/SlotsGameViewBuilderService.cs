using Core.Components;
using Core.Factories;
using Core.Models;
using System;
using System.Collections.Generic;
using UniRx;

namespace Core.App {
	public class SlotsGameViewBuilderService {
		private readonly SymbolsViewsFactory _symbolsViewsFactory;
		private readonly SymbolsViewModelsFactory _symbolsViewModelsFactory;

		public SlotsGameViewBuilderService (
			SymbolsViewsFactory symbolsViewsFactory, 
			SymbolsViewModelsFactory symbolsViewModelsFactory) {
			_symbolsViewsFactory = symbolsViewsFactory;
			_symbolsViewModelsFactory = symbolsViewModelsFactory;
		}

		public void BuildViews (SymbolsPackModel[] symbolsPacks, SlotsViewContext context, ReactiveCommand expireViews, ICollection<IDisposable> disposables) {
			var order = 0;
			
			for (var i = 0; i < symbolsPacks.Length; i++) {
				var viewModels = _symbolsViewModelsFactory.CreateViewModel(symbolsPacks[i], ref order, disposables);

				foreach (var symbolViewModel in viewModels) {
					expireViews.Subscribe(_ => symbolViewModel.Expire())
						.AddTo(disposables);
				}
				
				_symbolsViewsFactory.PackToView(viewModels, context.columns[i].joints);
			}
		}
	}
}
