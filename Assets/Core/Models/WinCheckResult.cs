using System.Collections.Generic;
using UnityEngine;

namespace Core.Models {
	public class WinCheckResult {
		public class WinningLine {
			public int lineIndex { get; }
			public List<Vector2Int> positions { get; }
			public Core.Data.SymbolId winningSymbol { get; }
			public int symbolCount { get; }
			public decimal winAmount { get; }
            
			public WinningLine(int lineIndex, List<Vector2Int> positions, Core.Data.SymbolId winningSymbol, int symbolCount, decimal winAmount) {
				this.lineIndex = lineIndex;
				this.positions = new List<Vector2Int>(positions);
				this.winningSymbol = winningSymbol;
				this.symbolCount = symbolCount;
				this.winAmount = winAmount;
			}
		}
        
		public List<WinningLine> winningLines { get; }
		public HashSet<Vector2Int> allWinningPositions { get; }
		public decimal totalWin { get; private set; }
        
		public WinCheckResult() {
			winningLines = new List<WinningLine>();
			allWinningPositions = new HashSet<Vector2Int>();
		}
        
		public void AddWinningLine(WinningLine line) {
			winningLines.Add(line);
			foreach (var pos in line.positions) {
				allWinningPositions.Add(pos);
			}
			totalWin += line.winAmount;
		}
        
		public bool HasWins => winningLines.Count > 0;
	}
}
