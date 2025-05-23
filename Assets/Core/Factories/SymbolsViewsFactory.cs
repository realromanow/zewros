using Core.Data;
using Core.ViewModels;
using Core.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Factories {
	public class SymbolsViewsFactory {
		private readonly IDictionary<SymbolId, Sprite> _symbolsCards;
		private readonly SymbolView _symbolViewPrefab;

		public SymbolsViewsFactory (IDictionary<SymbolId, Sprite> symbolsCards, SymbolView symbolViewPrefab) {
			_symbolsCards = symbolsCards;
			_symbolViewPrefab = symbolViewPrefab;
		}

		private void CreateView (SymbolViewModel viewModel, Transform parent) {
			var sprite = _symbolsCards[viewModel.id];
			
			var instance = Object.Instantiate(_symbolViewPrefab, parent, false);
			
			instance.SetItem(viewModel, viewModel.GetHashCode().ToString());
			instance.SetSprite(sprite);
		}

		public void PackToView (SymbolViewModel[] viewModels, Transform[] joints) {
			if (viewModels.Length != joints.Length) throw new System.Exception("viewModels length not equals joints length");

			for (var i = 0; i < viewModels.Length; i++) {
				CreateView(viewModels[i], joints[i]);
			}
		}
	}
}
