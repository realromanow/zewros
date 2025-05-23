using System;
using UnityEngine;

namespace Core.Data {
	[Serializable]
	public struct SlotsRules {
		[SerializeField]
		private string _rulesId;
		
		[SerializeField]
		private SlotsColumnRule[] _columns;
		
		public string rulesId => _rulesId;
		public SlotsColumnRule[] columns => _columns;
	}
}
