using Core.Context;
using Core.Data;
using Core.Models;
using System;
using UniRx;
using UnityEngine;

namespace Core.ViewModels {
	public class SymbolViewModel : IDisposable {
		public ReactiveCommand expire { get; } = new();
		public ReactiveCommand<bool> setWinning { get; } = new();
		public IReadOnlyReactiveProperty<bool> isWinning => _isWinning;
		public IReadOnlyReactiveProperty<Vector2Int> gridPosition => _gridPosition;
        
		public SymbolViewContext context { get; }
		public SymbolId id => _symbolModel.id;

		private readonly SymbolModel _symbolModel;
		private readonly ReactiveProperty<bool> _isWinning = new(false);
		private readonly ReactiveProperty<Vector2Int> _gridPosition = new();

		public SymbolViewModel (SymbolModel symbolModel, SymbolViewContext context) {
			_symbolModel = symbolModel;
			this.context = context;
            
			setWinning.Subscribe(value => _isWinning.Value = value);
		}
        
		public void SetGridPosition(int x, int y) {
			_gridPosition.Value = new Vector2Int(x, y);
		}

		public void Expire () {
			expire.Execute();
		}
        
		public void SetWinning(bool winning) {
			setWinning.Execute(winning);
		}
        
		public void Dispose () {
			expire.Dispose();
			setWinning.Dispose();
			_isWinning.Dispose();
			_gridPosition.Dispose();
		}
	}
}
