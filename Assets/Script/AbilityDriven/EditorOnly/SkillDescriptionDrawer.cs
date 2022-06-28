#if UNITY_EDITOR
using System.IO;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    partial class SkillDescription
    {
        private Actor m_Actor;
        
        private AnimationProperty DrawState(GUIContent label, InspectorProperty property)
        {
            InspectorProperty nameProperty = property.FindChild(x => x.Name == "m_Name", false);
            string            key          = AssetDatabase.GetAssetPath(property.Tree.UnitySerializedObject.targetObject);
            key = AssetDatabase.AssetPathToGUID(key);
            if (m_Actor == null)
            {
                m_Actor = SkillOwner.Get(key);
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            nameProperty.Draw(null);
            if (EditorGUILayout.DropdownButton(new GUIContent(m_Actor == null ? "Null" : m_Actor.name), FocusType.Passive))
            {
                ShowActorDropdown();
            }

            if (GUILayout.Button(GUIHelper.TempContent(EditorIcons.TriangleDown.Raw), GUILayout.Width(18f), GUILayout.Height(18f)))
            {
                if (m_Actor != null)
                {
                    ShowStateDropdown();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "当前指向角色为空，请先绑定角色。", "ok");
                }
            }

            EditorGUILayout.EndHorizontal();

            return State;

            void ShowActorDropdown()
            {
                GenericMenu menu   = new GenericMenu();
                string[]    assets = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/RogueGods/Gameplay/DynamicLoad/Roles" });
                for (int i = 0; i < assets.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(assets[i]);
                    menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(path)), false, () =>
                    {
                        m_Actor = AssetDatabase.LoadAssetAtPath<Actor>(path);
                        SkillOwner.Set(key, m_Actor);
                    });
                }

                menu.ShowAsContext();
            }

            void ShowStateDropdown()
            {
                AnimatorController animatorController = (AnimatorController)m_Actor.GetComponent<Animator>().runtimeAnimatorController;

                if (animatorController == null)
                {
                    return;
                }

                GenericMenu menu = new GenericMenu();
                foreach (AnimatorControllerLayer layer in animatorController.layers)
                {
                    AddStateMachine(layer.stateMachine);
                }

                menu.ShowAsContext();
                
                void AddStateMachine(AnimatorStateMachine machine)
                {
                    foreach (ChildAnimatorState state in machine.states)
                    {
                        string stateName = state.state.name;
                        if (state.state.motion is AnimationClip animationClip)
                        {
                            menu.AddItem(new GUIContent(stateName), false, OnSelect);

                            void OnSelect()
                            {
                                nameProperty.ValueEntry.WeakSmartValue = stateName;
                            }
                        }
                    }

                    foreach (var childMachine in machine.stateMachines)
                    {
                        AddStateMachine(childMachine.stateMachine);
                    }
                }
            }
        }

        private float GetStateClipLength()
        {
            if (m_Actor == null) return 0f;
            AnimatorController animatorController = (AnimatorController)m_Actor.GetComponent<Animator>().runtimeAnimatorController;
            if (animatorController == null) return 0f;
            AnimationClip      clip = null;
            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                if (FindState(layer.stateMachine))
                {
                    break;
                }
            }

            return clip == null ? 0f : clip.length;

            bool FindState(AnimatorStateMachine machine)
            {
                foreach (ChildAnimatorState state in machine.states)
                {
                    string stateName = state.state.name;
                    if (State.IsName(stateName) == false)
                    {
                        continue;
                    }

                    if (state.state.motion is AnimationClip temp)
                    {
                        clip = temp;
                    }

                    return true;
                }

                foreach (ChildAnimatorStateMachine childMachine in machine.stateMachines)
                {
                    if (FindState(childMachine.stateMachine))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
#endif