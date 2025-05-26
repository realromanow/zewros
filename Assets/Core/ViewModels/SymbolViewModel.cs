using Core.Context;
using Core.Data;
using Core.Models;
using System;
using UniRx;

namespace Core.ViewModels {
	public class SymbolViewModel : IDisposable {
		public ReactiveCommand expire { get; } = new();
		public bool isWinner { get; }
		public int generation => _symbolModel.generation;
		public IReadOnlyReactiveProperty<SymbolViewContext> context => _symbolContext;
		public SymbolId symbolId => _symbolModel.symbolId;
		public string id => _symbolModel.id;

		private readonly ReactiveProperty<SymbolViewContext> _symbolContext;
		private readonly SymbolModel _symbolModel;

		public SymbolViewModel (SymbolModel symbolModel, bool isWinner, SymbolViewContext context) {
			_symbolModel = symbolModel;
			this.isWinner = isWinner;
			
			_symbolContext = new ReactiveProperty<SymbolViewContext>(context);
		}

		public void Expire () {
			expire.Execute();
		}

		public void UpdateContext (SymbolViewContext updateContext) {
			_symbolContext.Value = updateContext;
		}
		
		public void Dispose () {
			_symbolContext.Dispose();
			expire.Dispose();
		}
	}
}
