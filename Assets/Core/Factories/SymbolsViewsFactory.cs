using Core.Data;
using Core.ViewModels;
using Core.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Factories {
	public class SymbolsViewsFactory {
		private readonly IDictionary<SymbolId, Sprite> _symbolsCards;
		private readonly SymbolView _symbolViewPrefab;
		private readonly AnimationSpeedService _animationSpeedService;

		public SymbolsViewsFactory (
			IDictionary<SymbolId, Sprite> symbolsCards, 
			SymbolView symbolViewPrefab,
			AnimationSpeedService animationSpeedService) {
			_symbolsCards = symbolsCards;
			_symbolViewPrefab = symbolViewPrefab;
			_animationSpeedService = animationSpeedService;
		}

		private void CreateView (SymbolViewModel viewModel) {
			var sprite = _symbolsCards[viewModel.id];
            
			var instance = Object.Instantiate(_symbolViewPrefab);

			instance.SetItem(viewModel, viewModel.GetHashCode().ToString());
			instance.SetSprite(sprite);
		}

		public void PackToView (IEnumerable<SymbolViewModel> viewModels) {
			foreach (var symbolViewModel in viewModels) {
				CreateView(symbolViewModel);
			}
		}
	}
}
