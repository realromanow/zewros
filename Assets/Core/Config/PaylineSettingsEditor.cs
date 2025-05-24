// PaylineSettingsEditor.cs - Кастомный редактор для PaylineSettings
// Поместите этот файл в папку Editor

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Config {
	// Класс для экспорта данных
	[System.Serializable]
	public class PaylineExportData {
		public int gridWidth;
		public int gridHeight;
		public int[][] patterns;
	}

	[CustomEditor(typeof(PaylineSettings))]
	public class PaylineSettingsEditor : UnityEditor.Editor {
		private PaylineSettings settings;
		private int selectedPaylineIndex = -1;
		private Vector2 scrollPosition;

		private void OnEnable () {
			settings = (PaylineSettings)target;

			// Инициализация сеток для всех линий
			foreach (var payline in settings.paylines) {
				if (payline.gridSelection == null ||
					payline.gridSelection.GetLength(0) != settings.gridWidth ||
					payline.gridSelection.GetLength(1) != settings.gridHeight) {
					payline.gridSelection = new bool[settings.gridWidth, settings.gridHeight];

					// Восстановление состояния из positions
					foreach (var pos in payline.positions) {
						if (pos.x >= 0 && pos.x < settings.gridWidth &&
							pos.y >= 0 && pos.y < settings.gridHeight)
							payline.gridSelection[pos.x, pos.y] = true;
					}
				}
			}
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			// Заголовок
			EditorGUILayout.LabelField("Slot Machine Payline Settings", EditorStyles.boldLabel);

			// Быстрая статистика
			if (settings.paylines.Count > 0) {
				var activeLines = settings.paylines.Count(p => p.isActive);
				EditorGUILayout.HelpBox($"Total Lines: {settings.paylines.Count} | Active: {activeLines} | Grid: {settings.gridWidth}x{settings.gridHeight}", MessageType.Info);
			}

			EditorGUILayout.Space();

			// Настройки сетки
			EditorGUILayout.LabelField("Grid Configuration", EditorStyles.boldLabel);
			EditorGUI.BeginChangeCheck();
			settings.gridWidth = EditorGUILayout.IntSlider("Reels (Width)", settings.gridWidth, 3, 7);
			settings.gridHeight = EditorGUILayout.IntSlider("Symbols per Reel (Height)", settings.gridHeight, 3, 5);

			if (EditorGUI.EndChangeCheck())
				// Пересоздаем сетки при изменении размера
				foreach (var payline in settings.paylines) {
					payline.gridSelection = new bool[settings.gridWidth, settings.gridHeight];
					payline.positions.Clear();
				}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Paylines", EditorStyles.boldLabel);

			// Информация о линиях
			var activeCount = settings.paylines.Count(p => p.isActive);
			EditorGUILayout.LabelField($"Total Lines: {settings.paylines.Count} | Active: {activeCount}", EditorStyles.miniLabel);

			// Кнопки управления
			EditorGUILayout.BeginHorizontal();

			// Кнопка добавления новой линии
			if (GUILayout.Button("Add New Payline", GUILayout.Height(30))) {
				Undo.RecordObject(settings, "Add Payline");
				settings.AddPayline();
				selectedPaylineIndex = settings.paylines.Count - 1;
				EditorUtility.SetDirty(settings);
			}

			// Кнопка генерации стандартных линий
			if (GUILayout.Button("Generate Standard Paylines", GUILayout.Height(30)))
				if (EditorUtility.DisplayDialog("Generate Standard Paylines",
						"This will replace all existing paylines with 50 standard patterns. Continue?",
						"Yes", "No")) {
					Undo.RecordObject(settings, "Generate Standard Paylines");
					settings.GenerateStandardPaylines();
					EditorUtility.SetDirty(settings);
				}

			EditorGUILayout.EndHorizontal();

			// Кнопки быстрого управления
			if (settings.paylines.Count > 0) {
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Enable All")) {
					Undo.RecordObject(settings, "Enable All Paylines");
					settings.paylines.ForEach(p => p.isActive = true);
					EditorUtility.SetDirty(settings);
				}
				if (GUILayout.Button("Disable All")) {
					Undo.RecordObject(settings, "Disable All Paylines");
					settings.paylines.ForEach(p => p.isActive = false);
					EditorUtility.SetDirty(settings);
				}
				if (GUILayout.Button("Clear All") && EditorUtility.DisplayDialog("Clear All Paylines",
						"Are you sure you want to remove all paylines?", "Yes", "No")) {
					Undo.RecordObject(settings, "Clear All Paylines");
					settings.paylines.Clear();
					selectedPaylineIndex = -1;
					EditorUtility.SetDirty(settings);
				}

				// Экспорт/Импорт
				if (GUILayout.Button("Export JSON")) {
					var patterns = new System.Collections.Generic.List<int[]>();
					foreach (var payline in settings.paylines) {
						var pattern = new int[settings.gridWidth];
						for (var x = 0; x < settings.gridWidth; x++) {
							pattern[x] = -1; // По умолчанию -1 если ничего не выбрано
							for (var y = 0; y < settings.gridHeight; y++) {
								if (payline.gridSelection[x, y]) {
									pattern[x] = y;
									break;
								}
							}
						}
						patterns.Add(pattern);
					}

					var json = JsonUtility.ToJson(new PaylineExportData {
						gridWidth = settings.gridWidth,
						gridHeight = settings.gridHeight,
						patterns = patterns.ToArray(),
					}, true);

					var path = EditorUtility.SaveFilePanel("Export Paylines", "", "paylines", "json");
					if (!string.IsNullOrEmpty(path)) {
						System.IO.File.WriteAllText(path, json);
						Debug.Log($"Paylines exported to: {path}");
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();

			// Список выигрышных линий
			if (settings.paylines.Count == 0) {
				EditorGUILayout.HelpBox("No paylines defined. Click 'Add New Payline' or 'Generate Standard Paylines' to start.", MessageType.Info);
			}
			else {
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

				for (var i = 0; i < settings.paylines.Count; i++) {
					var payline = settings.paylines[i];

					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.BeginHorizontal();

					// Раскрывающаяся панель для каждой линии
					var isExpanded = selectedPaylineIndex == i;
					if (GUILayout.Button(isExpanded ? "▼" : "►", GUILayout.Width(20))) selectedPaylineIndex = isExpanded ? -1 : i;

					// Номер линии
					EditorGUILayout.LabelField($"{i + 1}", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter }, GUILayout.Width(30));

					// Активность линии
					payline.isActive = EditorGUILayout.Toggle(payline.isActive, GUILayout.Width(20));

					// Имя линии
					payline.name = EditorGUILayout.TextField(payline.name);

					// Цвет линии
					payline.lineColor = EditorGUILayout.ColorField(payline.lineColor, GUILayout.Width(50));

					// Количество выбранных позиций
					EditorGUILayout.LabelField($"[{payline.positions.Count} cells]", GUILayout.Width(60));

					// Кнопка удаления
					if (GUILayout.Button("X", GUILayout.Width(25))) {
						Undo.RecordObject(settings, "Remove Payline");
						settings.paylines.RemoveAt(i);
						if (selectedPaylineIndex >= i) selectedPaylineIndex--;
						EditorUtility.SetDirty(settings);
						break;
					}

					EditorGUILayout.EndHorizontal();

					// Развернутое содержимое
					if (isExpanded) {
						EditorGUILayout.Space();
						DrawPaylineGrid(payline);

						EditorGUILayout.BeginHorizontal();

						// Кнопка очистки
						if (GUILayout.Button("Clear")) {
							Undo.RecordObject(settings, "Clear Payline");
							payline.gridSelection = new bool[settings.gridWidth, settings.gridHeight];
							settings.UpdatePaylinePositions(payline);
							EditorUtility.SetDirty(settings);
						}

						// Валидация (проверка, что в каждом барабане выбран один символ)
						var isValid = settings.ValidatePayline(payline);
						var hasDuplicate = settings.HasDuplicatePattern(payline);

						if (hasDuplicate) {
							GUI.color = Color.red;
							GUILayout.Label("⚠ DUPLICATE", EditorStyles.miniLabel, GUILayout.Width(80));
						}

						GUI.color = isValid ? Color.green : Color.yellow;
						if (GUILayout.Button(isValid ? "✓ Valid" : "⚠ Invalid", GUILayout.Width(80)))
							if (!isValid)
								EditorUtility.DisplayDialog("Invalid Payline",
									"Each reel must have exactly one symbol selected for a valid payline.",
									"OK");
						GUI.color = Color.white;

						EditorGUILayout.EndHorizontal();

						// Отображение позиций
						if (payline.positions.Count > 0) {
							EditorGUILayout.Space();
							EditorGUILayout.LabelField("Pattern Preview:", EditorStyles.miniLabel);

							// Визуализация паттерна в текстовом виде
							var pattern = $"Line {i + 1}: ";
							for (var x = 0; x < settings.gridWidth; x++) {
								var found = false;
								for (var y = 0; y < settings.gridHeight; y++) {
									if (payline.gridSelection[x, y]) {
										pattern += y.ToString();
										found = true;
										break;
									}
								}
								if (!found) pattern += "-";
								if (x < settings.gridWidth - 1) pattern += " → ";
							}
							EditorGUILayout.LabelField(pattern, EditorStyles.boldLabel);
						}
					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
				}

				EditorGUILayout.EndScrollView();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawPaylineGrid (PaylineSettings.Payline payline) {
			// Размер ячейки
			var cellSize = 30f;
			var spacing = 2f;

			// Вычисляем общий размер сетки
			var totalWidth = (settings.gridWidth * (cellSize + spacing)) - spacing;
			var totalHeight = (settings.gridHeight * (cellSize + spacing)) - spacing;

			// Центрирование сетки
			EditorGUILayout.BeginVertical();
			GUILayout.Space(10);

			// Получаем прямоугольник для всей области
			var containerRect = GUILayoutUtility.GetRect(
				GUIContent.none,
				GUIStyle.none,
				GUILayout.Height(totalHeight + 40) // Дополнительное место для меток
			);

			// Вычисляем начальную позицию для центрирования
			var startX = containerRect.x + ((containerRect.width - totalWidth) * 0.5f);
			var startY = containerRect.y;

			// Рисование сетки
			for (var x = 0; x < settings.gridWidth; x++) {
				for (var y = 0; y < settings.gridHeight; y++) {
					var cellRect = new Rect(
						startX + (x * (cellSize + spacing)),
						startY + (y * (cellSize + spacing)),
						cellSize,
						cellSize
					);

					// Фон ячейки
					EditorGUI.DrawRect(cellRect, payline.gridSelection[x, y] ? payline.lineColor : new Color(0.2f, 0.2f, 0.2f));

					// Рамка ячейки
					GUI.Box(cellRect, "");

					// Чекбокс
					EditorGUI.BeginChangeCheck();
					var newValue = EditorGUI.Toggle(cellRect, payline.gridSelection[x, y]);

					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(settings, "Toggle Payline Cell");
						payline.gridSelection[x, y] = newValue;
						settings.UpdatePaylinePositions(payline);
						EditorUtility.SetDirty(settings);
					}

					// Метка позиции (маленький текст в углу)
					var labelStyle = new GUIStyle(EditorStyles.miniLabel) {
						alignment = TextAnchor.LowerRight,
						normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 0.7f) },
						fontSize = 9,
					};
					GUI.Label(cellRect, $"{x},{y}", labelStyle);
				}
			}

			// Метки для барабанов (снизу)
			for (var x = 0; x < settings.gridWidth; x++) {
				var labelRect = new Rect(
					startX + (x * (cellSize + spacing)),
					startY + (settings.gridHeight * (cellSize + spacing)) + 5,
					cellSize,
					20
				);
				GUI.Label(labelRect, $"R{x + 1}", new GUIStyle(EditorStyles.miniLabel) {
					alignment = TextAnchor.UpperCenter,
				});
			}

			EditorGUILayout.EndVertical();
			GUILayout.Space(25); // Пространство после сетки
		}
	}
#endif
}
