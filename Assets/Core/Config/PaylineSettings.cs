// PaylineSettings.cs - ScriptableObject для хранения настроек выигрышных линий

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Config {
	[CreateAssetMenu(fileName = "PaylineSettings", menuName = "Slot Machine/Payline Settings")]
	public class PaylineSettings : ScriptableObject {
		[Serializable]
		public class Payline {
			public string name = "New Payline";
			public bool isActive = true;
			public Color lineColor = Color.white;
			public List<Vector2Int> positions = new();

			// Вспомогательное поле для редактора - сетка чекбоксов
			[HideInInspector]
			public bool[,] gridSelection;
		}

		[Header("Grid Settings")]
		public int gridWidth = 5; // Количество барабанов

		public int gridHeight = 4; // Количество символов на барабане

		[Header("Paylines")]
		public List<Payline> paylines = new();

		// Метод для добавления новой линии
		public void AddPayline () {
			var newPayline = new Payline {
				name = $"Payline {paylines.Count + 1}",
				lineColor = GetRandomColor(),
				gridSelection = new bool[gridWidth, gridHeight],
			};
			paylines.Add(newPayline);
		}

		// Генерация случайного цвета для линии
		private Color GetRandomColor () {
			return new Color(
				UnityEngine.Random.Range(0.3f, 1f),
				UnityEngine.Random.Range(0.3f, 1f),
				UnityEngine.Random.Range(0.3f, 1f),
				1f
			);
		}

		// Конвертация сетки чекбоксов в список позиций
		public void UpdatePaylinePositions (Payline payline) {
			payline.positions.Clear();
			if (payline.gridSelection == null) return;

			for (var x = 0; x < gridWidth; x++) {
				for (var y = 0; y < gridHeight; y++) {
					if (payline.gridSelection[x, y]) payline.positions.Add(new Vector2Int(x, y));
				}
			}
		}

		// Генерация стандартных выигрышных линий
		public void GenerateStandardPaylines () {
			paylines.Clear();

			// Паттерны для 5x4 сетки (как в большинстве современных слотов)
			var standardPatterns = new int[,] {
				// Горизонтальные линии
				{ 0, 0, 0, 0, 0 }, // Линия 1 - верхняя
				{ 1, 1, 1, 1, 1 }, // Линия 2 - средне-верхняя
				{ 2, 2, 2, 2, 2 }, // Линия 3 - средне-нижняя
				{ 3, 3, 3, 3, 3 }, // Линия 4 - нижняя

				// V-образные
				{ 0, 1, 2, 1, 0 }, // Линия 5 - V
				{ 3, 2, 1, 2, 3 }, // Линия 6 - перевернутая V
				{ 1, 0, 1, 2, 1 }, // Линия 7 - маленькая V сверху
				{ 2, 3, 2, 1, 2 }, // Линия 8 - маленькая перевернутая V снизу

				// Диагонали и зигзаги
				{ 0, 1, 2, 3, 2 }, // Линия 9
				{ 3, 2, 1, 0, 1 }, // Линия 10
				{ 1, 2, 3, 2, 1 }, // Линия 11
				{ 2, 1, 0, 1, 2 }, // Линия 12

				// W-образные
				{ 0, 2, 0, 2, 0 }, // Линия 13
				{ 3, 1, 3, 1, 3 }, // Линия 14
				{ 1, 3, 1, 3, 1 }, // Линия 15
				{ 2, 0, 2, 0, 2 }, // Линия 16

				// Сложные зигзаги
				{ 0, 1, 0, 1, 0 }, // Линия 17
				{ 3, 2, 3, 2, 3 }, // Линия 18
				{ 1, 0, 2, 0, 1 }, // Линия 19
				{ 2, 3, 1, 3, 2 }, // Линия 20

				// Дополнительные паттерны
				{ 0, 2, 1, 2, 0 }, // Линия 21
				{ 3, 1, 2, 1, 3 }, // Линия 22
				{ 1, 2, 2, 2, 1 }, // Линия 23
				{ 2, 1, 1, 1, 2 }, // Линия 24

				// Более сложные паттерны
				{ 0, 0, 1, 2, 3 }, // Линия 25
				{ 3, 3, 2, 1, 0 }, // Линия 26
				{ 0, 1, 1, 1, 0 }, // Линия 27
				{ 3, 2, 2, 2, 3 }, // Линия 28

				// Экстремальные зигзаги
				{ 0, 3, 0, 3, 0 }, // Линия 29
				{ 3, 0, 3, 0, 3 }, // Линия 30
				{ 1, 0, 3, 0, 1 }, // Линия 31
				{ 2, 3, 0, 3, 2 }, // Линия 32

				// Дополнительные уникальные паттерны
				{ 0, 2, 3, 2, 0 }, // Линия 33
				{ 3, 1, 0, 1, 3 }, // Линия 34
				{ 1, 3, 2, 3, 1 }, // Линия 35
				{ 2, 0, 1, 0, 2 }, // Линия 36

				// Финальные паттерны
				{ 0, 1, 3, 1, 0 }, // Линия 37
				{ 3, 2, 0, 2, 3 }, // Линия 38
				{ 1, 0, 0, 0, 1 }, // Линия 39
				{ 2, 3, 3, 3, 2 }, // Линия 40

				// Дополнительные паттерны для 50 линий
				{ 0, 0, 2, 0, 0 }, // Линия 41
				{ 3, 3, 1, 3, 3 }, // Линия 42
				{ 1, 1, 0, 1, 1 }, // Линия 43
				{ 2, 2, 3, 2, 2 }, // Линия 44
				{ 0, 2, 2, 2, 0 }, // Линия 45
				{ 3, 1, 1, 1, 3 }, // Линия 46
				{ 1, 0, 2, 3, 2 }, // Линия 47
				{ 2, 3, 1, 0, 1 }, // Линия 48
				{ 0, 3, 3, 3, 0 }, // Линия 49
				{ 1, 2, 0, 2, 1 }, // Линия 50
			};

			// Создаем линии на основе паттернов
			for (var i = 0; i < standardPatterns.GetLength(0); i++) {
				var payline = new Payline {
					name = $"Line {i + 1}",
					lineColor = GetColorForLineIndex(i),
					gridSelection = new bool[gridWidth, gridHeight],
				};

				// Заполняем сетку согласно паттерну
				for (var x = 0; x < gridWidth; x++) {
					var y = standardPatterns[i, x];
					if (y >= 0 && y < gridHeight) payline.gridSelection[x, y] = true;
				}

				UpdatePaylinePositions(payline);
				paylines.Add(payline);
			}
		}

		// Получение цвета для линии по индексу
		private Color GetColorForLineIndex (int index) {
			// Создаем различные цвета для линий
			var colors = new Color[] {
				new(1f, 0.2f, 0.2f), // Красный
				new(0.2f, 0.8f, 0.2f), // Зеленый
				new(0.2f, 0.4f, 1f), // Синий
				new(1f, 0.8f, 0.2f), // Желтый
				new(1f, 0.2f, 1f), // Пурпурный
				new(0.2f, 1f, 1f), // Циан
				new(1f, 0.6f, 0.2f), // Оранжевый
				new(0.6f, 0.2f, 1f), // Фиолетовый
			};

			return colors[index % colors.Length];
		}

		// Валидация выигрышной линии (опционально)
		public bool ValidatePayline (Payline payline) {
			// Проверка, что в каждом барабане выбран только один символ
			for (var x = 0; x < gridWidth; x++) {
				var selectedCount = 0;
				for (var y = 0; y < gridHeight; y++) {
					if (payline.gridSelection[x, y])
						selectedCount++;
				}

				if (selectedCount != 1)
					return false;
			}
			return true;
		}

		// Проверка на дубликаты
		public bool HasDuplicatePattern (Payline payline) {
			foreach (var other in paylines) {
				if (other == payline) continue;

				var isDuplicate = true;
				for (var x = 0; x < gridWidth && isDuplicate; x++) {
					for (var y = 0; y < gridHeight && isDuplicate; y++) {
						if (payline.gridSelection[x, y] != other.gridSelection[x, y])
							isDuplicate = false;
					}
				}

				if (isDuplicate) return true;
			}
			return false;
		}
	}
}
