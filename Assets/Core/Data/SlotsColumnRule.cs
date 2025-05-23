using System;
using UnityEngine;

namespace Core.Data {
	[Serializable]
	public struct SlotsColumnRule {
		[SerializeField]
		private int _joints;
		
		public int joints => _joints;
	}
}
