using AYellowpaper.SerializedCollections;
using Core.App;
using Core.Components;
using Core.Data;
using Core.UI;
using Core.Views;
using Plugins.Modern.DI.App;
using Plugins.Modern.DI.Units;
using UnityEngine;

namespace Core.Units {
	[CreateAssetMenu(menuName = "Core/Units/Create")]
	public class CoreUnit : ModernAppUnit {
		[SerializeField]
		private SerializedDictionary<string, GameObject> _screens;
		
		[SerializeField]
		private GameFieldComponent _gameFieldComponent;

		[SerializeField]
		private SerializedDictionary<SymbolId, Sprite> _symbolsCards;
		
		[SerializeField]
		private SymbolView _symbolViewPrefab;
		
		public override void SetupUnit (ModernComponentsRegistry componentsRegistry) {
			base.SetupUnit(componentsRegistry);

			componentsRegistry.Instantiate<CoreUIService>(_screens);
			
			componentsRegistry.Instantiate<SymbolsPacksFactory>();
			componentsRegistry.Instantiate<SymbolsViewsFactory>(_symbolsCards, _symbolViewPrefab);

			componentsRegistry.Instantiate<GameFieldProvider>(Instantiate(_gameFieldComponent));
			componentsRegistry.Instantiate<GameFieldFillerService>();

			componentsRegistry.Instantiate<GameController>().StartGame();
		}
	}
}
