using AYellowpaper.SerializedCollections;
using Core.App;
using Core.Components;
using Core.Config;
using Core.Data;
using Core.Factories;
using Core.UI;
using Core.UI.App;
using Core.Views;
using Plugins.Modern.DI.App;
using Plugins.Modern.DI.Units;
using UnityEngine;

namespace Core.Units {
	[CreateAssetMenu(menuName = "Core/Units/Create")]
	public class CoreUnit : ModernAppUnit {
		[SerializeField]
		private SlotsFieldViewContextComponent _fieldViewContextComponentPrefab;
        
		[SerializeField]
		private SerializedDictionary<string, GameObject> _screens;

		[SerializeField]
		private SerializedDictionary<SymbolId, Sprite> _symbolsCards;
        
		[SerializeField]
		private SymbolView _symbolViewPrefab;
        
		[SerializeField]
		private PaylineSettings _paylineSettings;
        
		[SerializeField]
		private PayoutSettings _payoutSettings;
        
		public override void SetupUnit (ModernComponentsRegistry componentsRegistry) {
			base.SetupUnit(componentsRegistry);

			componentsRegistry.Instantiate<CoreUIService>(_screens);
			componentsRegistry.Instantiate<AnimationSpeedService>();
			
			componentsRegistry.Instantiate<SymbolsPacksFactory>();
			componentsRegistry.Instantiate<SymbolsViewModelsFactory>();
			componentsRegistry.Instantiate<SymbolsViewsFactory>(_symbolsCards, _symbolViewPrefab);

			componentsRegistry.Instantiate<SlotsGameFieldProvider>(Instantiate(_fieldViewContextComponentPrefab));
			componentsRegistry.Instantiate<SlotsGameViewBuilderService>();
            
			componentsRegistry.Instantiate<SlotsWinningSymbolsService>(_paylineSettings, _payoutSettings);
			componentsRegistry.Instantiate<SlotsRoundService>(_paylineSettings);
			componentsRegistry.Instantiate<SlotsGameScreenService>();
			componentsRegistry.Instantiate<SlotsGameStartService>().StartGame();
			
			FindObjectOfType<DebugPanel>()
				.Initialize(componentsRegistry.Return<TestSymbolsPacksFactory>(),
					componentsRegistry.Return<SlotsWinningSymbolsService>());
			
			// FindObjectOfType<SpeedToggle>()
			// 	.Initialize(componentsRegistry.Return<AnimationSpeedService>());
		}
	}
}
