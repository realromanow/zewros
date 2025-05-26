using UnityEngine;

namespace Core.Components {
	public class SlotsFieldViewContextComponent : MonoBehaviour {
		[SerializeField]
		private SlotsFieldColumnViewContextComponent[] _columns;
		
		public SlotsFieldColumnViewContextComponent[] columns => _columns;
	}
}
