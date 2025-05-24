using System;
using System.Collections.Generic;
using Core.Data;
using UnityEngine;

namespace Core.Config {
    [CreateAssetMenu(fileName = "PayoutSettings", menuName = "Slot Machine/Payout Settings")]
    public class PayoutSettings : ScriptableObject {
        [Serializable]
        public class SymbolPayout {
            public SymbolId symbolId;
            public bool isWild; // Wild символ может заменять другие
            public bool isScatter; // Scatter символ работает вне линий
            
            [Header("Payouts for symbol count")]
            [SerializeField] public float payout3 = 0; // Выплата за 3 символа
            [SerializeField] public float payout4 = 0; // Выплата за 4 символа
            [SerializeField] public float payout5 = 0; // Выплата за 5 символов
            
            public decimal GetPayout(int count) {
                return count switch {
                    3 => (decimal)payout3,
                    4 => (decimal)payout4,
                    5 => (decimal)payout5,
                    _ => 0m
                };
            }
        }
        
        [SerializeField]
        private List<SymbolPayout> symbolPayouts = new();
        
        private Dictionary<SymbolId, SymbolPayout> _payoutLookup;
        
        private void OnEnable() {
            BuildLookup();
        }
        
        private void OnValidate() {
            BuildLookup();
        }
        
        private void BuildLookup() {
            _payoutLookup = new Dictionary<SymbolId, SymbolPayout>();
            if (symbolPayouts != null) {
                foreach (var payout in symbolPayouts) {
                    if (payout != null) {
                        _payoutLookup[payout.symbolId] = payout;
                    }
                }
            }
        }
        
        public SymbolPayout GetPayoutForSymbol(SymbolId symbolId) {
            if (_payoutLookup == null || _payoutLookup.Count == 0) BuildLookup();
            return _payoutLookup.TryGetValue(symbolId, out var payout) ? payout : null;
        }
        
        public bool IsWildSymbol(SymbolId symbolId) {
            var payout = GetPayoutForSymbol(symbolId);
            return payout != null && payout.isWild;
        }
        
        public decimal GetWinAmount(SymbolId symbolId, int count, decimal betPerLine) {
            var payout = GetPayoutForSymbol(symbolId);
            if (payout == null) return 0m;
            
            return payout.GetPayout(count) * betPerLine;
        }
    }
}
