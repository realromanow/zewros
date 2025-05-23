namespace Core.Models {
	public class SymbolsPackModel {
		public int packLength => symbols.Length;
		public SymbolModel[] symbols { get; }

		public SymbolsPackModel (SymbolModel[] symbols) {
			this.symbols = symbols;
		}
	}
}
