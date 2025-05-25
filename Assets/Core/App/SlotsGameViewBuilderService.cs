using Core.Components;
using Core.Factories;
using Core.Models;
using Core.ViewModels;
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

		public SymbolViewModel[,] BuildViews (SymbolsPackModel[] symbolsPacks, SlotsViewContext context, ReactiveCommand expireViews, ICollection<IDisposable> disposables) {
			var order = 0;
			var viewModelGrid = new SymbolViewModel[symbolsPacks.Length, context.columns[0].joints.Length];
            
			for (var i = 0; i < symbolsPacks.Length; i++) {
				var viewModels = _symbolsViewModelsFactory.CreateViewModel(symbolsPacks[i], ref order, viewModelGrid.Length, context.columns[i].joints, disposables);

				for (var j = 0; j < viewModels.Length; j++) {
					var symbolViewModel = viewModels[j];
					
					symbolViewModel.SetGridPosition(i, j);
					viewModelGrid[i, j] = symbolViewModel;
                    
					expireViews.Subscribe(_ => symbolViewModel.Expire())
						.AddTo(disposables);
				}
                
				_symbolsViewsFactory.PackToView(viewModels);
			}
            
			return viewModelGrid;
		}
	}
}
