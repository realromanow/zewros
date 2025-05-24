using Core.Config;
using Core.Factories;
using Core.Models;
using Core.ViewModels;
using System;
using UniRx;

namespace Core.App {
    public class SlotsRoundService : IDisposable {
        public IReadOnlyReactiveProperty<bool> canSpin => _canSpin;
        public IReadOnlyReactiveProperty<WinCheckResult> lastWinResult => _lastWinResult;
        
        private readonly ReactiveCommand _newRound = new();
        private readonly ReactiveProperty<bool> _canSpin = new(true);
        private readonly ReactiveProperty<WinCheckResult> _lastWinResult = new();
        
        private readonly SymbolsPacksFactory _symbolsPacksFactory;
        private readonly SlotsGameViewBuilderService _viewBuilderService;
        private readonly SlotsGameFieldProvider _fieldProvider;
        private readonly WinningSymbolsService _winningSymbolsService;
        private readonly PaylineSettings _paylineSettings;
        
        private readonly CompositeDisposable _compositeDisposable = new();
        private SymbolViewModel[,] _currentViewModels;
        private decimal _currentBetPerLine = 1m; // Базовая ставка на линию

        public SlotsRoundService (
            SymbolsPacksFactory symbolsPacksFactory,
            SlotsGameViewBuilderService viewBuilderService,
            SlotsGameFieldProvider fieldProvider,
            WinningSymbolsService winningSymbolsService,
            PaylineSettings paylineSettings) {
            _symbolsPacksFactory = symbolsPacksFactory;
            _viewBuilderService = viewBuilderService;
            _fieldProvider = fieldProvider;
            _winningSymbolsService = winningSymbolsService;
            _paylineSettings = paylineSettings;
        }

        public void MakeSpin () {
            if (!_canSpin.Value) return;
            
            _canSpin.Value = false;
            _newRound.Execute();
            
            // Очищаем предыдущие выигрыши
            ClearWinningSymbols();
            
            var packsCreated = 0;
            var context = _fieldProvider.activeField.Value;
            
            var symbolsPacks = new SymbolsPackModel[context.columns.Length];

            for (var i = 0; i < symbolsPacks.Length; i++) {
                var packLength = context.columns[i].joints.Length;

                symbolsPacks[i] =
                    _symbolsPacksFactory
                        .GetPack(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") + packsCreated++,
                            packLength);
            }
            
            _currentViewModels = _viewBuilderService.BuildViews(symbolsPacks, context, _newRound, _compositeDisposable);
            
            // Запускаем проверку выигрышей после завершения анимации
            Observable.Timer(TimeSpan.FromSeconds(2f)) // Подождать пока символы встанут на места
                .Subscribe(_ => CheckForWins())
                .AddTo(_compositeDisposable);
        }
        
        private void CheckForWins() {
            if (_currentViewModels == null) return;
            
            // Создаем модель сетки
            var grid = new SymbolsGridModel(_currentViewModels.GetLength(0), _currentViewModels.GetLength(1));
            
            // Заполняем сетку текущими символами
            for (int x = 0; x < grid.width; x++) {
                for (int y = 0; y < grid.height; y++) {
                    grid.SetSymbol(x, y, _currentViewModels[x, y].id);
                }
            }
            
            // Проверяем выигрыши
            var winResult = _winningSymbolsService.CheckWins(grid, _currentBetPerLine);
            _lastWinResult.Value = winResult;
            
            // Подсвечиваем выигрышные символы
            if (winResult.HasWins) {
                ShowWinningSymbols(winResult);
                
                // Показываем выигрыши 3 секунды, затем разрешаем новый спин
                Observable.Timer(TimeSpan.FromSeconds(3f))
                    .Subscribe(_ => {
                        ClearWinningSymbols();
                        _canSpin.Value = true;
                    })
                    .AddTo(_compositeDisposable);
            } else {
                _canSpin.Value = true;
            }
        }
        
        private void ShowWinningSymbols(WinCheckResult winResult) {
            foreach (var winningPos in winResult.allWinningPositions) {
                if (winningPos.x >= 0 && winningPos.x < _currentViewModels.GetLength(0) &&
                    winningPos.y >= 0 && winningPos.y < _currentViewModels.GetLength(1)) {
                    _currentViewModels[winningPos.x, winningPos.y]?.SetWinning(true);
                }
            }
        }
        
        private void ClearWinningSymbols() {
            if (_currentViewModels == null) return;
            
            for (int x = 0; x < _currentViewModels.GetLength(0); x++) {
                for (int y = 0; y < _currentViewModels.GetLength(1); y++) {
                    _currentViewModels[x, y]?.SetWinning(false);
                }
            }
        }

        public void Dispose () {
            _compositeDisposable.Dispose();
            _newRound.Dispose();
            _canSpin.Dispose();
            _lastWinResult.Dispose();
        }
    }
}
