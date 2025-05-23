namespace Core.Context {
	public struct SymbolViewContext {
		public int order { get; }
		public int ordersLength { get; }

		public SymbolViewContext (int order, int ordersLength) {
			this.order = order;
			this.ordersLength = ordersLength;
		}
	}
}
