using Core.Data;
using Core.Models;
using System;
using System.Linq;

namespace Core.Factories {
	public class SymbolsPacksFactory {
		public SymbolsPackModel GetPack (string seed, int packLength) {
			var numericSeed = seed.GetHashCode();
			
			var random = new Random(numericSeed);
			
			var symbolValues = Enum.GetValues(typeof(SymbolId)).Cast<SymbolId>().ToArray();
			
			var symbols = new SymbolModel[packLength];
            
			for (var i = 0; i < packLength; i++) {
				var randomIndex = random.Next(0, symbolValues.Length);
				
				symbols[i] = new SymbolModel(symbolValues[randomIndex]);
			}
			
			return new SymbolsPackModel(symbols);
		}
	}
}
