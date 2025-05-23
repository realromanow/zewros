using Core.UI.ViewModels;
using Plugins.Modern.UI.Api;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI {
	public class CoreUIService {
		private readonly IModernUIService _uiService;
		private readonly IDictionary<string, GameObject> _screens;
		
		private readonly Canvas _canvas;

		public CoreUIService (IModernUIService uiService, IDictionary<string, GameObject> screens) {
			_uiService = uiService;
			_screens = screens;
			
			_canvas = Object.Instantiate(screens["canvas"]).GetComponent<Canvas>();
		}

		public void ShowControlsScreen (SlotsControlsScreenViewModel viewModel) {
			_uiService.SetupForm(_screens["controls_screen"], _canvas, viewModel);
		}
	}
}
