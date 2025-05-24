using Core.Data;
using UnityEngine;

namespace Core.Models {
	public class SymbolsGridModel {
		public SymbolId[,] grid { get; }
		public int width { get; }
		public int height { get; }
        
		public SymbolsGridModel(int width, int height) {
			this.width = width;
			this.height = height;
			grid = new SymbolId[width, height];
		}
        
		public void SetSymbol(int x, int y, SymbolId symbolId) {
			if (x >= 0 && x < width && y >= 0 && y < height) {
				grid[x, y] = symbolId;
			}
		}
        
		public SymbolId GetSymbol(int x, int y) {
			if (x >= 0 && x < width && y >= 0 && y < height) {
				return grid[x, y];
			}
			return SymbolId.ZEWS; // default
		}
        
		public SymbolId GetSymbolAt(Vector2Int position) {
			return GetSymbol(position.x, position.y);
		}
	}
}
