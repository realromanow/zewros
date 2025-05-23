using Core.Data;

namespace Core.App {
	public class GameFieldProvider {
		public GameFieldData activeField { get; }
		
		public GameFieldProvider (GameFieldData activeField) {
			this.activeField = activeField;
		}
	}
}
