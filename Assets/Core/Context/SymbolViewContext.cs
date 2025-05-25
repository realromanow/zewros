namespace Core.Context {
	public struct SymbolViewContext {
		public int order { get; }
		public int rowOrdersLength { get; }
		public int totalOrdersLength { get; }

		public SymbolViewContext (int order, int rowOrdersLength, int totalOrdersLength) {
			this.order = order;
			this.rowOrdersLength = rowOrdersLength;
			this.totalOrdersLength = totalOrdersLength;
		}
	}
}
