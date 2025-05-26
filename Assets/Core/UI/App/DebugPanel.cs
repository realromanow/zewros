using Core.App;
using Core.Data;
using Core.Factories;
using UnityEngine;

namespace Core.UI.App {
	public class DebugPanel : MonoBehaviour {
		private TestSymbolsPacksFactory _testFactory;
		private SlotsWinningSymbolsService _slotsWinningService;
		private bool _showDebug = false;
        
		public void Initialize (TestSymbolsPacksFactory testFactory, SlotsWinningSymbolsService slotsWinningService) {
			_testFactory = testFactory;
			_slotsWinningService = slotsWinningService;
		}
        
		void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				_showDebug = !_showDebug;
			}
		}
        
		void OnGUI() {
			if (!_showDebug) return;
            
			GUILayout.BeginArea(new Rect(10, 10, 300, 400));
			GUILayout.Box("Debug Panel");
            
			GUILayout.Label("Test Mode Controls:");
            
			if (GUILayout.Button("Toggle Test Mode")) {
				_slotsWinningService.testMode = !_slotsWinningService.testMode;
			}
            
			GUILayout.Label($"Test Mode: {_slotsWinningService.testMode}");
            
			if (GUILayout.Button("Force ZEUS wins")) {
				_testFactory.SetTestMode(true, SymbolId.ZEWS);
			}
            
			if (GUILayout.Button("Force AID wins")) {
				_testFactory.SetTestMode(true, SymbolId.AID);
			}
            
			if (GUILayout.Button("Force Random wins")) {
				var symbols = System.Enum.GetValues(typeof(SymbolId));
				var randomSymbol = (SymbolId)symbols.GetValue(Random.Range(0, symbols.Length));
				_testFactory.SetTestMode(true, randomSymbol);
			}
            
			if (GUILayout.Button("Disable forced wins")) {
				_testFactory.SetTestMode(false);
			}
            
			GUILayout.EndArea();
		}
	}
}
