using Core.Data;

namespace Core.Models {
	public class SymbolModel {
		public bool isWinner { get;  private set; }
		public SymbolId symbolId { get; }
		public string id { get; }
		public int generation { get; }

		public SymbolModel (SymbolId symbolId, int generation, string id) {
			this.symbolId = symbolId;
			this.generation = generation;
			this.id = id;
		}

		public void MarkAsWinner () {
			isWinner = true;
		}
	}
}
