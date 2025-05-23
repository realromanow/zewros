using Core.Components;
using System;
using UniRx;

namespace Core.App {
	public class SlotsGameFieldProvider : IDisposable {
		public IReadOnlyReactiveProperty<SlotsViewContext> activeField => _activeField;
		
		private readonly ReactiveProperty<SlotsViewContext> _activeField = new();

		public SlotsGameFieldProvider (SlotsViewContext context) {
			_activeField.Value = context;
		}

		public void Dispose () {
			_activeField.Dispose();
		}
	}
}
