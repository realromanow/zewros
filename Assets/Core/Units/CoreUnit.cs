using AYellowpaper.SerializedCollections;
using Core.App;
using Core.Components;
using Core.Data;
using Core.Factories;
using Core.UI;
using Core.Views;
using Plugins.Modern.DI.App;
using Plugins.Modern.DI.Units;
using UnityEngine;

namespace Core.Units {
	[CreateAssetMenu(menuName = "Core/Units/Create")]
	public class CoreUnit : ModernAppUnit {
		[SerializeField]
		private SlotsViewContext _viewContextPrefab;
		
		[SerializeField]
		private SerializedDictionary<string, GameObject> _screens;

		[SerializeField]
		private SerializedDictionary<SymbolId, Sprite> _symbolsCards;
		
		[SerializeField]
		private SymbolView _symbolViewPrefab;
		
		public override void SetupUnit (ModernComponentsRegistry componentsRegistry) {
			base.SetupUnit(componentsRegistry);

			componentsRegistry.Instantiate<CoreUIService>(_screens);
			
			componentsRegistry.Instantiate<SymbolsPacksFactory>();
			componentsRegistry.Instantiate<SymbolsViewModelsFactory>();
			componentsRegistry.Instantiate<SymbolsViewsFactory>(_symbolsCards, _symbolViewPrefab);

			componentsRegistry.Instantiate<SlotsGameFieldProvider>(Instantiate(_viewContextPrefab));
			componentsRegistry.Instantiate<SlotsGameViewBuilderService>();

			componentsRegistry.Instantiate<SlotsRoundService>();
			componentsRegistry.Instantiate<SlotsGameScreenService>();
			componentsRegistry.Instantiate<SlotsGameStartService>().StartGame();
		}
	}
}
