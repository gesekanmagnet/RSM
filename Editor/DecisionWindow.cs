using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace RSM
{
    public class DecisionWindow : EditorWindow
    {
        private Type[] decisionTypes;

        private string[] decisionNames;
        private int selectedIndex = 0;

        [MenuItem("GameObject/RSM/Create Decision...", false, 10)]
        public static void OpenWindow(MenuCommand menuCommand)
        {
            var window = GetWindow<DecisionWindow>("AI Decision");
            window.Show();
        }

        private void OnEnable()
        {
            decisionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(ITransition)) && !t.IsAbstract)
                .ToArray();

            decisionNames = decisionTypes.Select(t => t.Name).ToArray();
        }

        private void OnGUI()
        {
            if (decisionTypes == null || decisionTypes.Length == 0)
            {
                EditorGUILayout.HelpBox("No AIDecision found!", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Choose Decision", EditorStyles.boldLabel);
            selectedIndex = EditorGUILayout.Popup("Decision Type", selectedIndex, decisionNames);
            GUILayout.Space(10);

            if (GUILayout.Button("Create Decision"))
            {
                CreateDecision();
            }
        }

        private void CreateDecision()
        {
            if (decisionTypes == null || decisionTypes.Length == 0) return;

            Type chosenType = decisionTypes[selectedIndex];

            GameObject go = new GameObject(chosenType.Name);
            go.AddComponent(chosenType);

            if (Selection.activeGameObject != null)
            {
                GameObjectUtility.SetParentAndAlign(go, Selection.activeGameObject);
            }

            Undo.RegisterCreatedObjectUndo(go, $"Create {chosenType.Name}");
            Selection.activeObject = go;

            Close();
        }
    }
}