using UnityEngine;

namespace Core.Components {
	public class SlotsViewContext : MonoBehaviour {
		[SerializeField]
		private GameFieldColumnContext[] _columns;
		
		public GameFieldColumnContext[] columns => _columns;
	}
}
