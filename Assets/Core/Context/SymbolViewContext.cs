using UnityEngine;

namespace Core.Context {
	public struct SymbolViewContext {
		public int fieldOrder { get; }
		public int columnOrder { get; }
		public int columnLength { get; }
		public int fieldLength { get; }
		public Transform joint { get; }

		public SymbolViewContext (int fieldOrder, int columnLength, int fieldLength, int columnOrder, Transform joint) {
			this.fieldOrder = fieldOrder;
			this.columnLength = columnLength;
			this.fieldLength = fieldLength;
			this.joint = joint;
			this.columnOrder = columnOrder;
		}
	}
}
