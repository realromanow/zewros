using Core.Config;
using Core.Factories;
using Core.Models;
using System;
using UniRx;
using UnityEngine;

namespace Core.App {
	public class SlotsRoundService : IDisposable {
		public IReadOnlyReactiveProperty<bool> canSpin => _canSpin;
		public IReadOnlyReactiveProperty<SymbolsWinsResultModel> lastWinResult => _lastWinResult;

		private readonly ReactiveCommand _newRound = new();
		private readonly ReactiveProperty<bool> _canSpin = new(true);
		private readonly ReactiveProperty<SymbolsWinsResultModel> _lastWinResult = new();

		private readonly SymbolsPacksFactory _symbolsPacksFactory;
		private readonly SlotsGameViewBuilderService _viewBuilderService;
		private readonly SlotsGameFieldProvider _fieldProvider;
		private readonly SlotsWinningSymbolsService _slotsWinningSymbolsService;
		private readonly PaylineSettings _paylineSettings;

		private readonly CompositeDisposable _compositeDisposable = new();

		private decimal _currentBetPerLine = 1m;

		public SlotsRoundService (
			SymbolsPacksFactory symbolsPacksFactory,
			SlotsGameViewBuilderService viewBuilderService,
			SlotsGameFieldProvider fieldProvider,
			SlotsWinningSymbolsService slotsWinningSymbolsService,
			PaylineSettings paylineSettings) {
			_symbolsPacksFactory = symbolsPacksFactory;
			_viewBuilderService = viewBuilderService;
			_fieldProvider = fieldProvider;
			_slotsWinningSymbolsService = slotsWinningSymbolsService;
			_paylineSettings = paylineSettings;
		}

		public void MakeSpin () {
			_newRound.Execute();

			var packsCreated = 0;
			var context = _fieldProvider.activeField.Value;

			var symbolsPacks = new SymbolsPackModel[context.columns.Length];

			for (var i = 0; i < symbolsPacks.Length; i++) {
				var packLength = context.columns[i].joints.Length;

				symbolsPacks[i] =
					_symbolsPacksFactory
						.GetPack(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") + packsCreated++,
							packLength);
			}

			CheckForWins(symbolsPacks);

			_viewBuilderService.BuildViews(symbolsPacks, context, _newRound, _compositeDisposable);

			// var winSymbols = _currentViewModels.Cast<SymbolViewModel>().Where(x => x.isWinning.Value);
			//
			// foreach (var winSymbol in winSymbols) {
			// 	winSymbol.Expire();
			// }
		}

		private void CheckForWins (SymbolsPackModel[] symbols) {
			var symbolsArray = SymbolsPackToGridArray(symbols);
			var grid = GetSymbolsGridModel(symbolsArray);

			var winResult = _slotsWinningSymbolsService.CheckWins(grid, _currentBetPerLine);
			_lastWinResult.Value = winResult;

			if (winResult.hasWins) MarkWinningSymbols(symbolsArray, winResult);
		}

		private static SymbolsGridModel GetSymbolsGridModel (SymbolModel[,] symbols) {
			var grid = new SymbolsGridModel(symbols.GetLength(0), symbols.GetLength(1));

			for (var x = 0; x < grid.width; x++) {
				for (var y = 0; y < grid.height; y++) {
					grid.SetSymbol(x, y, symbols[x, y].id);
				}
			}

			return grid;
		}

		private static void MarkWinningSymbols (SymbolModel[,] symbols, SymbolsWinsResultModel symbolsWinResultModel) {
			foreach (var winningPos in symbolsWinResultModel.allWinningPositions) {
				if (winningPos.x >= 0 && winningPos.x < symbols.GetLength(0) &&
					winningPos.y >= 0 && winningPos.y < symbols.GetLength(1))
					symbols[winningPos.x, winningPos.y].MarkAsWinner();
			}
		}

		private static SymbolModel[,] SymbolsPackToGridArray (SymbolsPackModel[] symbolsPacks) {
			var array = new SymbolModel[symbolsPacks.Length, symbolsPacks[0].packLength];

			for (var i = 0; i < symbolsPacks.Length; i++) {
				for (var j = 0; j < symbolsPacks[i].packLength; j++) {
					array[i, j] = symbolsPacks[i].symbols[j];
				}
			}
			
			return array;
		}

		public void Dispose () {
			_compositeDisposable.Dispose();
			_newRound.Dispose();
			_canSpin.Dispose();
			_lastWinResult.Dispose();
		}
	}
}
