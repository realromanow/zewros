using UnityEngine;

namespace Core.Utils {
	public enum OffsetCalculationType
	{
		LocalCenter,    // Смещение относительно локального центра спрайта
		GlobalCenter    // Смещение относительно глобального центра сцены (0,0,0)
	}
	
	public class SpriteOffsetUtils {

		/// <summary>
		/// Вычисляет смещение центра SpriteRenderer при изменении масштаба
		/// </summary>
		/// <param name="spriteRenderer">Компонент SpriteRenderer</param>
		/// <param name="newScale">Новый масштаб</param>
		/// <param name="calculationType">Тип расчета смещения</param>
		/// <param name="originalScale">Исходный масштаб (по умолчанию текущий)</param>
		/// <returns>Вектор смещения центра по осям X и Y</returns>
		public static Vector2 CalculateOffset (SpriteRenderer spriteRenderer, Vector2 newScale, OffsetCalculationType calculationType = OffsetCalculationType.LocalCenter, Vector2? originalScale = null) {
			if (spriteRenderer == null || spriteRenderer.sprite == null)
				return Vector2.zero;

			var currentScale = originalScale ?? spriteRenderer.transform.localScale;
			var scaleChange = new Vector2(newScale.x / currentScale.x, newScale.y / currentScale.y);

			var offset = Vector2.zero;

			switch (calculationType) {
				case OffsetCalculationType.LocalCenter:
					offset = CalculateLocalCenterOffset(spriteRenderer, scaleChange);
					break;

				case OffsetCalculationType.GlobalCenter:
					offset = CalculateGlobalCenterOffset(spriteRenderer, scaleChange);
					break;
			}

			return offset;
		}

		/// <summary>
		/// Рассчитывает смещение относительно локального центра спрайта
		/// </summary>
		private static Vector2 CalculateLocalCenterOffset (SpriteRenderer spriteRenderer, Vector2 scaleChange) {
			// Получаем размер спрайта в world units
			var spriteBounds = spriteRenderer.bounds;
			var spriteSize = new Vector2(spriteBounds.size.x, spriteBounds.size.y);

			// Получаем pivot спрайта (нормализованные координаты от 0 до 1)
			var pivot = spriteRenderer.sprite.pivot;
			var spritePixelSize = new Vector2(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height);
			var normalizedPivot = new Vector2(pivot.x / spritePixelSize.x, pivot.y / spritePixelSize.y);

			// Вычисляем расстояние от pivot до центра спрайта
			var pivotToCenter = new Vector2(
				spriteSize.x * (0.5f - normalizedPivot.x),
				spriteSize.y * (0.5f - normalizedPivot.y)
			);

			// Вычисляем смещение при масштабировании
			var offset = new Vector2(
				pivotToCenter.x * (scaleChange.x - 1f),
				pivotToCenter.y * (scaleChange.y - 1f)
			);

			return offset;
		}

		/// <summary>
		/// Рассчитывает смещение относительно глобального центра сцены (0,0,0)
		/// </summary>
		private static Vector2 CalculateGlobalCenterOffset (SpriteRenderer spriteRenderer, Vector2 scaleChange) {
			// Текущая позиция объекта в мировых координатах
			var currentWorldPosition = spriteRenderer.transform.position;

			// Рассчитываем новую позицию после масштабирования относительно глобального центра
			var newPosition = new Vector2(
				currentWorldPosition.x * scaleChange.x,
				currentWorldPosition.y * scaleChange.y
			);

			// Смещение = новая позиция - текущая позиция
			var offset = new Vector2(
				newPosition.x - currentWorldPosition.x,
				newPosition.y - currentWorldPosition.y
			);

			return offset;
		}

		/// <summary>
		/// Перегрузка для uniform масштабирования
		/// </summary>
		public static Vector2 CalculateOffset (SpriteRenderer spriteRenderer, float newScale, OffsetCalculationType calculationType = OffsetCalculationType.LocalCenter, float originalScale = 1f) {
			var newScaleVec = Vector2.one * newScale;
			var originalScaleVec = Vector2.one * originalScale;
			return CalculateOffset(spriteRenderer, newScaleVec, calculationType, originalScaleVec);
		}

		/// <summary>
		/// Применяет масштабирование с компенсацией смещения
		/// </summary>
		public static void ScaleWithoutOffset (SpriteRenderer spriteRenderer, Vector2 newScale, OffsetCalculationType calculationType = OffsetCalculationType.LocalCenter) {
			var offset = CalculateOffset(spriteRenderer, newScale, calculationType);

			spriteRenderer.transform.localScale = newScale;
			spriteRenderer.transform.position -= (Vector3)offset;
		}

		/// <summary>
		/// Применяет uniform масштабирование с компенсацией смещения
		/// </summary>
		public static void ScaleWithoutOffset (SpriteRenderer spriteRenderer, float newScale, OffsetCalculationType calculationType = OffsetCalculationType.LocalCenter) {
			ScaleWithoutOffset(spriteRenderer, Vector2.one * newScale, calculationType);
		}

	}
}
