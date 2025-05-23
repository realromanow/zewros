using Core.Data;

namespace Core.Models {
	public class SymbolModel {
		public SymbolId id { get; }

		public SymbolModel (SymbolId id) {
			this.id = id;
		}
	}
}
