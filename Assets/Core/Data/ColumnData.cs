using UnityEngine;

namespace Core.Data {
	public class ColumnData : MonoBehaviour {
		[SerializeField]
		private Transform[] _joints;
		
		public Transform[] joints => _joints;
	}
}
