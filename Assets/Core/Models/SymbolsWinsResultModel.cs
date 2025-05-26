using Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Models {
	public class SymbolsWinsResultModel {
		public List<WinLineModel> winningLines { get; } = new();
		public HashSet<Vector2Int> allWinningPositions { get; } = new();
		public decimal totalWin { get; private set; }
		public bool hasWins => winningLines.Count > 0;

		public void AddWinningLine (WinLineModel lineModel) {
			winningLines.Add(lineModel);

			foreach (var pos in lineModel.positions) {
				allWinningPositions.Add(pos);
			}

			totalWin += lineModel.winAmount;
		}

		public class WinLineModel {
			public int lineIndex { get; }
			public List<Vector2Int> positions { get; }
			public SymbolId winningSymbol { get; }
			public int symbolCount { get; }
			public decimal winAmount { get; }

			public WinLineModel (int lineIndex, List<Vector2Int> positions, SymbolId winningSymbol, int symbolCount, decimal winAmount) {
				this.lineIndex = lineIndex;
				this.positions = new List<Vector2Int>(positions);
				this.winningSymbol = winningSymbol;
				this.symbolCount = symbolCount;
				this.winAmount = winAmount;
			}
		}
	}
}
