using Core.Data;
using Core.Models;
using System;
using System.Linq;

namespace Core.Factories {
	public class SymbolsPacksBuilder {
		public virtual SymbolsPackModel GetPack (string seed, int packLength) {
			var numericSeed = seed.GetHashCode();

			var random = new Random(numericSeed);

			var symbolValues = Enum.GetValues(typeof(SymbolId)).Cast<SymbolId>().ToArray();

			var symbols = new SymbolModel[packLength];

			for (var i = 0; i < packLength; i++) {
				var randomIndex = random.Next(0, symbolValues.Length);

				symbols[i] = new SymbolModel(symbolValues[randomIndex], 0, seed + $"{i}");
			}

			return new SymbolsPackModel(symbols, seed, 0);
		}

		public void RebuildPack (SymbolsPackModel pack) {
			var symbolValues = Enum.GetValues(typeof(SymbolId)).Cast<SymbolId>().ToArray();
			
			for (var i = pack.packLength - 1; i >= 0; i--) {
				if (pack.symbols[i].isWinner) {
					for (var j = i; j < pack.packLength - 1; j++) {
						pack.symbols[j] = pack.symbols[j + 1];
					}

					pack.symbols[pack.packLength - 1] = null;
				}
			}

			for (var i = 0; i < pack.symbols.Length; i++) {
				if (pack.symbols[i] == null) {
					var numericSeed = pack.seed.GetHashCode() + i;
					var random = new Random(numericSeed);

					pack.symbols[i] = new SymbolModel(
						symbolValues[random.Next(0, symbolValues.Length)],
						pack.packGeneration + 1,
						pack.seed + $"{i}"
					);
				}
			}
		}
	}
}
