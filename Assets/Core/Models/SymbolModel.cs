using Core.Data;
using System;
using UniRx;

namespace Core.Models {
	public class SymbolModel : IDisposable {
		public IReadOnlyReactiveProperty<bool> isWinner => _isWinner;
		public SymbolId symbolId { get; }
		public string id { get; }
		public int generation { get; }

		private readonly ReactiveProperty<bool> _isWinner = new();

		public SymbolModel (SymbolId symbolId, int generation, string id) {
			this.symbolId = symbolId;
			this.generation = generation;
			this.id = id;
		}

		public void MarkAsWinner () {
			_isWinner.Value = true;
		}

		public void Dispose () {
			_isWinner.Dispose();
		}
	}
}
