using Core.Components;
using Core.Context;
using Core.Factories;
using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
			var newViewModels = new SymbolViewModel[symbolsPacks.Length, symbolsPacks[0].packLength];
			var updatedSymbols = new List<SymbolViewModel>();

			// Создаем Dictionary для быстрого поиска старых viewModel по ID
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
					var fieldOrder = (i * symbolsPacks[i].packLength) + j; // Исправленный расчёт
					
					// Проверяем, является ли символ новым (по generation)
					if (symbolsPacks[i].packGeneration < currentSymbol.generation) {
						// Создаём новую viewModel для нового символа
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
						// Ищем существующую viewModel по ID символа
						if (oldViewModelsByID.TryGetValue(currentSymbol.id, out var existingViewModel)) {
							// Обновляем контекст для существующей viewModel на новую позицию
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
							// Если по какой-то причине не нашли старую viewModel, создаём новую
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

			// Обновляем _lastViewModels на новое состояние
			_lastViewModels = newViewModels;

			// Создаём только новые views
			_symbolsViewsFactory.PackToView(updatedSymbols);
		}
	}
}
