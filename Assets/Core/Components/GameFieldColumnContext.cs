using UnityEngine;

namespace Core.Components {
	public class GameFieldColumnContext : MonoBehaviour {
		[SerializeField]
		private Transform[] _joints;
		
		public Transform[] joints => _joints;
	}
}
