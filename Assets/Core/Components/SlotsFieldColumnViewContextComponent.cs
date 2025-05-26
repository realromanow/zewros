using UnityEngine;

namespace Core.Components {
	public class SlotsFieldColumnViewContextComponent : MonoBehaviour {
		[SerializeField]
		private Transform[] _joints;
		
		public Transform[] joints => _joints;
	}
}
