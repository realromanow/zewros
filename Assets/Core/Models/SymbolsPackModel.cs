namespace Core.Models {
	public class SymbolsPackModel {
		public int packLength => symbols.Length;
		public int packGeneration { get; private set; }
		public string seed { get; }
		public SymbolModel[] symbols { get; }

		public SymbolsPackModel (SymbolModel[] symbols, string seed, int packGeneration) {
			this.symbols = symbols;
			this.seed = seed;
			this.packGeneration = packGeneration;
		}

		public void UpdateGeneration (int generation) {
			packGeneration = generation;
		}
	}
}
