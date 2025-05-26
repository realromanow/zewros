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
	public class SlotsSymbolsViewModelMapperService {
		private readonly SymbolsViewsFactory _symbolsViewsFactory;
		private readonly SymbolsViewModelsFactory _symbolsViewModelsFactory;

		private SymbolViewModel[,] _lastViewModels = {};

		public SlotsSymbolsViewModelMapperService (
			SymbolsViewsFactory symbolsViewsFactory,
			SymbolsViewModelsFactory symbolsViewModelsFactory) {
			_symbolsViewsFactory = symbolsViewsFactory;
			_symbolsViewModelsFactory = symbolsViewModelsFactory;
		}

		public void InitializeViews (SymbolsPackModel[] symbolsPacks, SlotsFieldViewContextComponent contextComponent, ReactiveCommand expireViews, ICollection<IDisposable> disposables) {
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
			if (_lastViewModels.Length <= 0) throw new Exception($"Nothing to rebuild");
			
			var newViewModels = new SymbolViewModel[symbolsPacks.Length, symbolsPacks[0].packLength];
			var updatedSymbols = new List<SymbolViewModel>();
			
			var oldViewModelsByID = new Dictionary<string, SymbolViewModel>();
			for (var row = 0; row < _lastViewModels.GetLength(0); row++) {
				for (var col = 0; col < _lastViewModels.GetLength(1); col++) {
					if (_lastViewModels[row, col] != null) {
						oldViewModelsByID[_lastViewModels[row, col].id] = _lastViewModels[row, col];
					}
				}
			}

			for (var i = 0; i < symbolsPacks.Length; i++) {
				for (var j = 0; j < symbolsPacks[i].packLength; j++) {
					var currentSymbol = symbolsPacks[i].symbols[j];
					var fieldOrder = (i * symbolsPacks[i].packLength) + j;

					if (symbolsPacks[i].packGeneration < currentSymbol.generation) {
						var newViewModel = _symbolsViewModelsFactory.CreateViewModel(
							currentSymbol, 
							i, 
							fieldOrder, 
							symbolsPacks[i].packLength, 
							symbolsPacks.Length * symbolsPacks[i].packLength, 
							contextComponent.columns[i].joints[j]);
						
						newViewModel.AddTo(disposables);
						expireViews.Subscribe(_ => newViewModel.Expire())
							.AddTo(disposables);

						newViewModels[i, j] = newViewModel;
						updatedSymbols.Add(newViewModel);
					}
					else {
						if (oldViewModelsByID.TryGetValue(currentSymbol.id, out var existingViewModel)) {
							existingViewModel.UpdateContext(
								new SymbolViewContext(
									fieldOrder,
									symbolsPacks[i].packLength,
									symbolsPacks.Length * symbolsPacks[i].packLength, 
									i,
									contextComponent.columns[i].joints[j]));
							
							newViewModels[i, j] = existingViewModel;
						}
						else {
							Debug.LogWarning($"Could not find existing viewModel for symbol {currentSymbol.id}, creating new one");
							var newViewModel = _symbolsViewModelsFactory.CreateViewModel(
								currentSymbol, 
								i, 
								fieldOrder, 
								symbolsPacks[i].packLength, 
								symbolsPacks.Length * symbolsPacks[i].packLength, 
								contextComponent.columns[i].joints[j]);
							
							newViewModel.AddTo(disposables);
							expireViews.Subscribe(_ => newViewModel.Expire())
								.AddTo(disposables);

							newViewModels[i, j] = newViewModel;
							updatedSymbols.Add(newViewModel);
						}
					}
				}
			}
			
			_lastViewModels = newViewModels;
			
			_symbolsViewsFactory.PackToView(updatedSymbols);
		}
	}
}
