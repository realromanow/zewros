using Core.Context;
using Core.Data;
using Core.Models;
using System;
using UniRx;

namespace Core.ViewModels {
	public class SymbolViewModel : IDisposable {
		public ReactiveCommand expire { get; } = new();
		public bool isWinner { get; }
		public SymbolViewContext context { get; }
		public SymbolId id => _symbolModel.id;

		private readonly SymbolModel _symbolModel;

		public SymbolViewModel (SymbolModel symbolModel, bool isWinner, SymbolViewContext context) {
			_symbolModel = symbolModel;
			this.isWinner = isWinner;
			this.context = context;
		}

		public void Expire () {
			expire.Execute();
		}
        
		public void Dispose () {
			expire.Dispose();
		}
	}
}
