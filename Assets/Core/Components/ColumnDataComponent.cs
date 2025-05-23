using UnityEngine;

namespace Core.Components {
	public class ColumnDataComponent : MonoBehaviour {
		[SerializeField]
		private Transform[] _joints;
		
		public Transform[] joints => _joints;
	}
}
