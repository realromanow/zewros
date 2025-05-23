using Core.Data;
using Core.Models;
using UniRx;

namespace Core.ViewModels {
	public class SymbolViewModel {
		public ReactiveCommand expire { get; }
		public SymbolId id => _symbolModel.id;
		public int order { get; }
		public int selfPackLength { get; }

		private readonly SymbolModel _symbolModel;

		public SymbolViewModel (SymbolModel symbolModel, int order, int selfPackLength, ReactiveCommand expire) {
			_symbolModel = symbolModel;
			this.order = order;
			this.selfPackLength = selfPackLength;
			this.expire = expire;
		}
	}
}
