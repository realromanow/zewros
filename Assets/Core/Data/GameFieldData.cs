using UnityEngine;

namespace Core.Data {
	public class GameFieldData : MonoBehaviour {
		[SerializeField]
		private ColumnData[] _columns;
		
		public ColumnData[] columns => _columns;
	}
}
