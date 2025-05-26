using Core.Data;

namespace Core.Models {
	public class SymbolModel {
		public bool isWinner { get;  private set; }
		public SymbolId id { get; }

		public SymbolModel (SymbolId id) {
			this.id = id;
		}

		public void MarkAsWinner () {
			isWinner = true;
		}
	}
}
