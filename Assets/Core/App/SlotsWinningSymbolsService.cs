using Core.Config;
using Core.Data;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.App {
	public class SlotsWinningSymbolsService {
		private readonly PaylineSettings _paylineSettings;
		private readonly PayoutSettings _payoutSettings;

		public bool testMode { get; set; } = false;
		public SymbolId? forceTestSymbol { get; set; } = null;

		public SlotsWinningSymbolsService (PaylineSettings paylineSettings, PayoutSettings payoutSettings) {
			_paylineSettings = paylineSettings;
			_payoutSettings = payoutSettings;
		}

		public SymbolsWinsResultModel CheckWins (SymbolsGridModel grid, decimal betPerLine) {
			var result = new SymbolsWinsResultModel();

			if (testMode) {
				Debug.Log("=== CHECKING WINS ===");
				LogGrid(grid);
			}

			for (var i = 0; i < _paylineSettings.paylines.Count; i++) {
				var payline = _paylineSettings.paylines[i];
				if (!payline.isActive) continue;

				var lineWin = CheckPayline(i, payline, grid, betPerLine);
				if (lineWin != null) {
					result.AddWinningLine(lineWin);

					if (testMode) Debug.Log($"WIN on Line {i + 1}: {lineWin.symbolCount}x {lineWin.winningSymbol} = ${lineWin.winAmount}");
				}
			}

			if (testMode && !result.hasWins) Debug.Log("No wins this spin");

			return result;
		}

		private SymbolsWinsResultModel.WinLineModel CheckPayline (int lineIndex, PaylineSettings.Payline payline, SymbolsGridModel grid, decimal betPerLine) {
			if (payline.positions == null || payline.positions.Count < 3) return null;

			var symbolsOnLine = new List<SymbolId>();
			var maxPositions = Mathf.Min(payline.positions.Count, 5);

			for (var i = 0; i < maxPositions; i++) {
				symbolsOnLine.Add(grid.GetSymbolAt(payline.positions[i]));
			}

			if (testMode) {
				var symbolsStr = string.Join(", ", symbolsOnLine);
				Debug.Log($"Line {lineIndex + 1} symbols: {symbolsStr}");
			}

			var firstSymbol = symbolsOnLine[0];
			var matchingPositions = new List<Vector2Int> { payline.positions[0] };
			var consecutiveCount = 1;
			var winningSymbol = firstSymbol;

			if (_payoutSettings.IsWildSymbol(firstSymbol)) {
				for (var i = 1; i < symbolsOnLine.Count; i++) {
					if (!_payoutSettings.IsWildSymbol(symbolsOnLine[i])) {
						winningSymbol = symbolsOnLine[i];
						for (var j = 0; j < i; j++) {
							if (_payoutSettings.IsWildSymbol(symbolsOnLine[j]))
								if (j == 0 || j == consecutiveCount) {
									consecutiveCount++;
									if (j > 0) matchingPositions.Add(payline.positions[j]);
								}
						}
						break;
					}
				}

				if (winningSymbol == firstSymbol)
					for (var i = 1; i < symbolsOnLine.Count; i++) {
						if (_payoutSettings.IsWildSymbol(symbolsOnLine[i])) {
							consecutiveCount++;
							matchingPositions.Add(payline.positions[i]);
						}
						else {
							break;
						}
					}
			}

			for (var i = consecutiveCount; i < symbolsOnLine.Count; i++) {
				var currentSymbol = symbolsOnLine[i];

				if (currentSymbol == winningSymbol || _payoutSettings.IsWildSymbol(currentSymbol)) {
					consecutiveCount++;
					matchingPositions.Add(payline.positions[i]);
				}
				else {
					break;
				}
			}

			if (consecutiveCount >= 3) {
				var winAmount = _payoutSettings.GetWinAmount(winningSymbol, consecutiveCount, betPerLine);
				if (winAmount > 0)
					return new SymbolsWinsResultModel.WinLineModel(
						lineIndex,
						matchingPositions.Take(consecutiveCount).ToList(),
						winningSymbol,
						consecutiveCount,
						winAmount
					);
			}

			return null;
		}

		private void LogGrid (SymbolsGridModel grid) {
			Debug.Log($"Grid {grid.width}x{grid.height}:");
			for (var y = 0; y < grid.height; y++) {
				var row = "";
				for (var x = 0; x < grid.width; x++) {
					row += grid.GetSymbol(x, y).ToString().PadRight(12) + " ";
				}
				Debug.Log($"Row {y}: {row}");
			}
		}
	}
}
