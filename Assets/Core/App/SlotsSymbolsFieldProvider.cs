using Core.Components;
using System;
using UniRx;

namespace Core.App {
	public class SlotsSymbolsFieldProvider : IDisposable {
		public IReadOnlyReactiveProperty<SlotsFieldViewContextComponent> activeField => _activeField;
		
		private readonly ReactiveProperty<SlotsFieldViewContextComponent> _activeField = new();

		public SlotsSymbolsFieldProvider (SlotsFieldViewContextComponent contextComponent) {
			_activeField.Value = contextComponent;
		}

		public void Dispose () {
			_activeField.Dispose();
		}
	}
}
