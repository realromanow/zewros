using Core.Components;

namespace Core.App {
	public class GameFieldProvider {
		public GameFieldComponent activeField { get; }
		
		public GameFieldProvider (GameFieldComponent activeField) {
			this.activeField = activeField;
		}
	}
}
