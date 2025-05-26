using UnityEngine;

namespace Core.Utils {
	[RequireComponent(typeof(SpriteRenderer))]
	public class ShatterEffect : MonoBehaviour {
		[SerializeField]
		private SpriteRenderer _spriteRenderer;
		
		[SerializeField]
		private Material _shatterMaterial;

		public void InitShader () {
			_spriteRenderer.material = _shatterMaterial;
		}
	}
}
