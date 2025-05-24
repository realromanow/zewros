#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Core.Data;

namespace Core.Config.Editor {
    [CustomEditor(typeof(PayoutSettings))]
    public class PayoutSettingsEditor : UnityEditor.Editor {
        private SerializedProperty symbolPayoutsProperty;
        
        private void OnEnable() {
            symbolPayoutsProperty = serializedObject.FindProperty("symbolPayouts");
        }
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Slot Machine Payout Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Кнопка для автозаполнения всех символов
            if (GUILayout.Button("Initialize All Symbols", GUILayout.Height(30))) {
                InitializeAllSymbols();
            }
            
            EditorGUILayout.Space();
            
            // Показываем список символов
            EditorGUILayout.PropertyField(symbolPayoutsProperty, new GUIContent("Symbol Payouts"), true);
            
            serializedObject.ApplyModifiedProperties();
            
            // Дополнительная информация
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Configure payouts for each symbol:\n" +
                "• Wild symbols can substitute for other symbols\n" +
                "• Scatter symbols pay regardless of paylines\n" +
                "• Payouts are multiplied by bet per line",
                MessageType.Info
            );
        }
        
        private void InitializeAllSymbols() {
            var settings = (PayoutSettings)target;
            var symbolPayouts = serializedObject.FindProperty("symbolPayouts");
            
            symbolPayouts.ClearArray();
            
            var allSymbols = System.Enum.GetValues(typeof(SymbolId));
            foreach (SymbolId symbol in allSymbols) {
                symbolPayouts.InsertArrayElementAtIndex(symbolPayouts.arraySize);
                var element = symbolPayouts.GetArrayElementAtIndex(symbolPayouts.arraySize - 1);
                
                element.FindPropertyRelative("symbolId").enumValueIndex = (int)symbol;
                element.FindPropertyRelative("isWild").boolValue = false;
                element.FindPropertyRelative("isScatter").boolValue = false;
                
                // Устанавливаем стандартные выплаты в зависимости от типа символа
                float payout3 = 0, payout4 = 0, payout5 = 0;
                
                switch (symbol) {
                    case SymbolId.ZEWS:
                        payout3 = 50; payout4 = 200; payout5 = 1000;
                        break;
                    case SymbolId.AID:
                        payout3 = 40; payout4 = 150; payout5 = 750;
                        break;
                    case SymbolId.PERSEPHORA:
                        payout3 = 30; payout4 = 100; payout5 = 500;
                        break;
                    case SymbolId.SYMBOL_DIAMOND:
                    case SymbolId.SYMBOL_AMETIST:
                    case SymbolId.SYMBOL_RUBY:
                        payout3 = 20; payout4 = 50; payout5 = 200;
                        break;
                    case SymbolId.SYMBOL_ROUGH:
                        payout3 = 100; payout4 = 500; payout5 = 2000;
                        element.FindPropertyRelative("isWild").boolValue = true;
                        break;
                    default:
                        payout3 = 10; payout4 = 25; payout5 = 100;
                        break;
                }
                
                element.FindPropertyRelative("payout3").floatValue = payout3;
                element.FindPropertyRelative("payout4").floatValue = payout4;
                element.FindPropertyRelative("payout5").floatValue = payout5;
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
        }
    }
}
#endif
