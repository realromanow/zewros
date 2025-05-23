using Core.Context;
using Core.Data;
using Core.Models;
using System;
using UniRx;

namespace Core.ViewModels {
	public class SymbolViewModel : IDisposable {
		public ReactiveCommand expire { get; } = new();
		public SymbolViewContext context { get; }
		public SymbolId id => _symbolModel.id;

		private readonly SymbolModel _symbolModel;

		public SymbolViewModel (SymbolModel symbolModel, SymbolViewContext context) {
			_symbolModel = symbolModel;
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
