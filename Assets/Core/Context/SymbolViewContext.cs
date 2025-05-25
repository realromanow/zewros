using UnityEngine;

namespace Core.Context {
	public struct SymbolViewContext {
		public int order { get; }
		public int rowOrdersLength { get; }
		public int totalOrdersLength { get; }
		public Transform joint { get; }

		public SymbolViewContext (int order, int rowOrdersLength, int totalOrdersLength, Transform joint) {
			this.order = order;
			this.rowOrdersLength = rowOrdersLength;
			this.totalOrdersLength = totalOrdersLength;
			this.joint = joint;
		}
	}
}
