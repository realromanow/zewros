using UnityEngine;

namespace Core.Components {
	public class GameFieldComponent : MonoBehaviour {
		[SerializeField]
		private ColumnDataComponent[] _columns;
		
		public ColumnDataComponent[] columns => _columns;
	}
}
