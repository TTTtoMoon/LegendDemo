using System.Reflection;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor
{
    public class EditorIconWindow : EditorWindow
    {
        [MenuItem("Tools/Editor Icons")]
        private static void ShowWindow()
        {
            var window = GetWindow<EditorIconWindow>();
            window.titleContent = new GUIContent("Editor Icons");
            window.Show();
        }

        private Vector2 scroll;

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            PropertyInfo[] properties = typeof(EditorIcons).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
            for (int i = 0; i < properties.Length; i++)
            {
                Texture tex;
                switch (properties[i].GetValue(null))
                {
                    case EditorIcon icon:
                        tex = icon.Raw;
                        break;
                    case Texture texture:
                        tex = texture;
                        break;
                    default:
                        continue;
                }

                EditorGUILayout.LabelField(GUIHelper.TempContent(properties[i].Name, tex));
            }

            EditorGUILayout.EndScrollView();
        }
    }
}