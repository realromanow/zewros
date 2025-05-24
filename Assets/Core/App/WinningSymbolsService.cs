using Core.Config;
using Core.Data;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.App {
    public class WinningSymbolsService {
        private readonly PaylineSettings _paylineSettings;
        private readonly PayoutSettings _payoutSettings;
        
        // Режим тестирования
        public bool testMode { get; set; } = false;
        public SymbolId? forceTestSymbol { get; set; } = null;
        
        public WinningSymbolsService(PaylineSettings paylineSettings, PayoutSettings payoutSettings) {
            _paylineSettings = paylineSettings;
            _payoutSettings = payoutSettings;
        }
        
        public WinCheckResult CheckWins(SymbolsGridModel grid, decimal betPerLine) {
            var result = new WinCheckResult();
            
            // В тестовом режиме логируем сетку
            if (testMode) {
                Debug.Log("=== CHECKING WINS ===");
                LogGrid(grid);
            }
            
            // Проверяем каждую активную линию
            for (int i = 0; i < _paylineSettings.paylines.Count; i++) {
                var payline = _paylineSettings.paylines[i];
                if (!payline.isActive) continue;
                
                var lineWin = CheckPayline(i, payline, grid, betPerLine);
                if (lineWin != null) {
                    result.AddWinningLine(lineWin);
                    
                    if (testMode) {
                        Debug.Log($"WIN on Line {i + 1}: {lineWin.symbolCount}x {lineWin.winningSymbol} = ${lineWin.winAmount}");
                    }
                }
            }
            
            if (testMode && !result.HasWins) {
                Debug.Log("No wins this spin");
            }
            
            return result;
        }
        
        private WinCheckResult.WinningLine CheckPayline(int lineIndex, PaylineSettings.Payline payline, SymbolsGridModel grid, decimal betPerLine) {
            if (payline.positions == null || payline.positions.Count < 3) return null;
            
            // Получаем символы вдоль линии (только первые 5 позиций для 5-барабанного слота)
            var symbolsOnLine = new List<SymbolId>();
            var maxPositions = Mathf.Min(payline.positions.Count, 5); // Обычно 5 барабанов
            
            for (int i = 0; i < maxPositions; i++) {
                symbolsOnLine.Add(grid.GetSymbolAt(payline.positions[i]));
            }
            
            if (testMode) {
                var symbolsStr = string.Join(", ", symbolsOnLine);
                Debug.Log($"Line {lineIndex + 1} symbols: {symbolsStr}");
            }
            
            // Ищем комбинации слева направо
            var firstSymbol = symbolsOnLine[0];
            var matchingPositions = new List<Vector2Int> { payline.positions[0] };
            var consecutiveCount = 1;
            SymbolId winningSymbol = firstSymbol;
            
            // Обработка Wild символов
            if (_payoutSettings.IsWildSymbol(firstSymbol)) {
                // Если первый символ Wild, ищем первый не-Wild
                for (int i = 1; i < symbolsOnLine.Count; i++) {
                    if (!_payoutSettings.IsWildSymbol(symbolsOnLine[i])) {
                        winningSymbol = symbolsOnLine[i];
                        // Считаем все Wild'ы до этого символа
                        for (int j = 0; j < i; j++) {
                            if (_payoutSettings.IsWildSymbol(symbolsOnLine[j])) {
                                if (j == 0 || j == consecutiveCount) {
                                    consecutiveCount++;
                                    if (j > 0) matchingPositions.Add(payline.positions[j]);
                                }
                            }
                        }
                        break;
                    }
                }
                
                // Если все символы Wild, считаем их как выигрышную комбинацию
                if (winningSymbol == firstSymbol) {
                    for (int i = 1; i < symbolsOnLine.Count; i++) {
                        if (_payoutSettings.IsWildSymbol(symbolsOnLine[i])) {
                            consecutiveCount++;
                            matchingPositions.Add(payline.positions[i]);
                        } else {
                            break;
                        }
                    }
                }
            }
            
            // Проверяем последовательные совпадения
            for (int i = consecutiveCount; i < symbolsOnLine.Count; i++) {
                var currentSymbol = symbolsOnLine[i];
                
                // Символ совпадает или это Wild
                if (currentSymbol == winningSymbol || _payoutSettings.IsWildSymbol(currentSymbol)) {
                    consecutiveCount++;
                    matchingPositions.Add(payline.positions[i]);
                } else {
                    break; // Последовательность прервана
                }
            }
            
            // Проверяем, достаточно ли символов для выигрыша (минимум 3)
            if (consecutiveCount >= 3) {
                var winAmount = _payoutSettings.GetWinAmount(winningSymbol, consecutiveCount, betPerLine);
                if (winAmount > 0) {
                    return new WinCheckResult.WinningLine(
                        lineIndex,
                        matchingPositions.Take(consecutiveCount).ToList(),
                        winningSymbol,
                        consecutiveCount,
                        winAmount
                    );
                }
            }
            
            return null;
        }
        
        private void LogGrid(SymbolsGridModel grid) {
            Debug.Log($"Grid {grid.width}x{grid.height}:");
            for (int y = 0; y < grid.height; y++) {
                string row = "";
                for (int x = 0; x < grid.width; x++) {
                    row += grid.GetSymbol(x, y).ToString().PadRight(12) + " ";
                }
                Debug.Log($"Row {y}: {row}");
            }
        }
    }
}
