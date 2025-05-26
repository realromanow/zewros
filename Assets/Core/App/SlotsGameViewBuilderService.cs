using Core.Components;
using Core.Context;
using Core.Factories;
using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.App {
	public class SlotsGameViewBuilderService {
		private readonly SymbolsViewsFactory _symbolsViewsFactory;
		private readonly SymbolsViewModelsFactory _symbolsViewModelsFactory;

		private SymbolViewModel[,] _lastViewModels = {};

		public SlotsGameViewBuilderService (
			SymbolsViewsFactory symbolsViewsFactory,
			SymbolsViewModelsFactory symbolsViewModelsFactory) {
			_symbolsViewsFactory = symbolsViewsFactory;
			_symbolsViewModelsFactory = symbolsViewModelsFactory;
		}

		public void BuildViews (SymbolsPackModel[] symbolsPacks, SlotsFieldViewContextComponent contextComponent, ReactiveCommand expireViews, ICollection<IDisposable> disposables) {
			_lastViewModels = new SymbolViewModel[symbolsPacks.Length, symbolsPacks[0].packLength];

			for (var i = 0; i < symbolsPacks.Length; i++) {
				var viewModels = _symbolsViewModelsFactory.CreateViewModelsFromPack(symbolsPacks[i], i, symbolsPacks.Length * symbolsPacks[i].packLength, contextComponent.columns[i].joints);

				for (var j = 0; j < viewModels.Length; j++) {
					_lastViewModels[i, j] = viewModels[j];
				}

				foreach (var symbolViewModel in viewModels) {
					symbolViewModel.AddTo(disposables);
					expireViews.Subscribe(_ => symbolViewModel.Expire())
						.AddTo(disposables);
				}

				_symbolsViewsFactory.PackToView(viewModels);
			}
		}

		public void RebuildViews (SymbolsPackModel[] symbolsPacks, SlotsFieldViewContextComponent contextComponent, ReactiveCommand expireViews, ICollection<IDisposable> disposables) {
			var updatedSymbols = new List<SymbolViewModel>();

			for (var i = 0; i < symbolsPacks.Length; i++) {
				for (var j = 0; j < symbolsPacks[i].packLength; j++) {
					if (symbolsPacks[i].packGeneration < symbolsPacks[i].symbols[j].generation) {
						var viewModel = _symbolsViewModelsFactory.CreateViewModel(symbolsPacks[i].symbols[j], i, i * j, symbolsPacks[i].packLength, symbolsPacks.Length * symbolsPacks[i].packLength, contextComponent.columns[i].joints[j]);
						viewModel.AddTo(disposables);
						expireViews.Subscribe(_ => viewModel.Expire())
							.AddTo(disposables);

						updatedSymbols.Add(viewModel);
					}
					else {
						var symbolPackId = symbolsPacks[i].symbols[j].id;

						var foundOldViewModel = false;
             
						for (var row = 0; row < _lastViewModels.GetLength(0); row++) {
							for (var col = 0; col < _lastViewModels.GetLength(1); col++) {
								if (_lastViewModels[row, col].id != symbolPackId) continue;

								var existingViewModel = _lastViewModels[row, col];
								existingViewModel.UpdateContext(
									new SymbolViewContext(
										i * j,
										symbolsPacks[i].packLength,
										symbolsPacks.Length * symbolsPacks[i].packLength, 
										i,
										contextComponent.columns[i].joints[j]));
								
								foundOldViewModel = true;
								break;
							}
							if (foundOldViewModel) break;
						}
					}
				}
			}
    
			_symbolsViewsFactory.PackToView(updatedSymbols);
		}
	}
}
