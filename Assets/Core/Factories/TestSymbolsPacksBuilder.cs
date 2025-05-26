using Core.Data;
using Core.Models;
using System;
using System.Linq;
using UnityEngine;

namespace Core.Factories {
    public class TestSymbolsPacksBuilder : SymbolsPacksBuilder {
        private bool _forceWin = false;
        private SymbolId _forceSymbol = SymbolId.ZEWS;
        private int _testSpinCount = 0;
        
        public void SetTestMode(bool forceWin, SymbolId symbolToForce = SymbolId.ZEWS) {
            _forceWin = forceWin;
            _forceSymbol = symbolToForce;
            Debug.Log($"Test mode: {forceWin}, Force symbol: {symbolToForce}");
        }
        
        public override SymbolsPackModel GetPack(string seed, int packLength) {
            _testSpinCount++;
            
            if (!_forceWin) {
                return base.GetPack(seed, packLength);
            }
            
            // В тестовом режиме создаем гарантированные выигрыши
            var symbols = new SymbolModel[packLength];
            
            // Каждый 3-й спин - большой выигрыш (5 символов)
            if (_testSpinCount % 3 == 0) {
                Debug.Log($"Test spin {_testSpinCount}: Forcing 5x {_forceSymbol}");
                for (int i = 0; i < packLength; i++) {
                    symbols[i] = new SymbolModel(_forceSymbol, 0, "");
                }
            }
            // Каждый 2-й спин - средний выигрыш (4 символа)
            else if (_testSpinCount % 2 == 0) {
                Debug.Log($"Test spin {_testSpinCount}: Forcing 4x {_forceSymbol}");
                for (int i = 0; i < packLength; i++) {
                    if (i < packLength - 1) {
                        symbols[i] = new SymbolModel(_forceSymbol, 0, "");
                    } else {
                        // Последний символ случайный
                        symbols[i] = new SymbolModel(GetRandomSymbolExcept(_forceSymbol), 0, "");
                    }
                }
            }
            // Остальные спины - малый выигрыш (3 символа)
            else {
                Debug.Log($"Test spin {_testSpinCount}: Forcing 3x {_forceSymbol}");
                for (int i = 0; i < packLength; i++) {
                    if (i < 3) {
                        symbols[i] = new SymbolModel(_forceSymbol, 0, "");
                    } else {
                        // Остальные символы случайные
                        symbols[i] = new SymbolModel(GetRandomSymbolExcept(_forceSymbol), 0, "");
                    }
                }
            }
            
            return new SymbolsPackModel(symbols, seed, 0);
        }
        
        private SymbolId GetRandomSymbolExcept(SymbolId except) {
            var symbolValues = Enum.GetValues(typeof(SymbolId)).Cast<SymbolId>()
                .Where(s => s != except).ToArray();
            
            var random = new System.Random();
            return symbolValues[random.Next(0, symbolValues.Length)];
        }
    }
}
