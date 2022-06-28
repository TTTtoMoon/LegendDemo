using System.Collections.Generic;
using Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor
{
    sealed class AbilityEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem(AbilityConst.AbilityEditorWindow)]
        private static void DoShow()
        {
            var     window  = GetWindow<AbilityEditorWindow>();
            Vector2 minSize = new Vector2(1000f, 800f);
            float   width   = Mathf.Max(window.position.width,  minSize.x);
            float   height  = Mathf.Max(window.position.height, minSize.y);
            window.minSize  = minSize;
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(width, height);
        }

        internal static AbilityEditorWindow Instance { get; private set; }

        private OdinMenuTreeDrawingConfig m_Config;
        private AbilityConfiguration      m_NewAbility;
        private AbilityTag                m_FilterTag;

        private AbilityConfiguration EditingAbility => m_NewAbility != null ? m_NewAbility : MenuTree?.Selection?.SelectedValue as AbilityConfiguration;

        private void Awake()
        {
            Instance = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = new GUIContent("Ability Editor");
            m_Config = new OdinMenuTreeDrawingConfig
            {
                DrawSearchToolbar             = true,
                ConfirmSelectionOnDoubleClick = false,
            };
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree   = new OdinMenuTree(false, m_Config);
            string[]     assets = AssetDatabase.FindAssets("t:AbilityConfiguration");
            for (int i = 0; i < assets.Length; i++)
            {
                string               path          = AssetDatabase.GUIDToAssetPath(assets[i]);
                AbilityConfiguration configuration = AssetDatabase.LoadAssetAtPath<AbilityConfiguration>(path);
                string               menuName      = $"[{configuration.Ability.ConfigurationID}] {configuration.CustomName}";
                menuName = string.IsNullOrWhiteSpace(configuration.Group) ? menuName : $"{configuration.Group}/{menuName}";
                tree.Add(menuName, configuration);
            }

            tree.SortMenuItemsByName();
            tree.Selection.SelectionChanged += (x) =>
            {
                if (m_NewAbility != null && x == SelectionChangedType.ItemAdded)
                {
                    DestroyImmediate(m_NewAbility);
                }
            } ;

            tree.Selection.SelectionConfirmed += delegate(OdinMenuTreeSelection selection)
            {
                GUIHelper.RemoveFocusControl();
            };
            
            return tree;
        }

        protected override IEnumerable<object> GetTargets()
        {
            yield return EditingAbility;
        }

        protected override void OnGUI()
        {
            if (MakeSureCatalogue() == false)
            {
                return;
            }

            DrawToolbar();
            base.OnGUI();
        }

        private void DrawToolbar()
        {
            SirenixEditorGUI.BeginHorizontalToolbar();

            NewAbilityButton();
            SaveButton();
            SaveAllButton();
            GUIHelper.PushGUIEnabled(EditingAbility != null && EditingAbility != m_NewAbility);
            DeleteButton();
            GUIHelper.PopGUIEnabled();
            
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void NewAbilityButton()
        {
            if (SirenixEditorGUI.ToolbarButton("新建"))
            {
                if (m_NewAbility != null)
                {
                    DestroyImmediate(m_NewAbility);
                }

                m_NewAbility = CreateInstance<AbilityConfiguration>();
                MenuTree.Selection.Clear();
            }
        }

        private void SaveButton()
        {
            if (SirenixEditorGUI.ToolbarButton("保存"))
            {
                if (EditingAbility != null)
                {
                    EditingAbility.Save();
                    m_NewAbility = null;
                }

                ForceMenuTreeRebuild();
            }
        }
        
        private void SaveAllButton()
        {
            if (SirenixEditorGUI.ToolbarButton("全部保存"))
            {
                if (m_NewAbility != null)
                {
                    m_NewAbility.Save();
                    m_NewAbility = null;
                }

                for (int i = 0; i < AbilityPreference.Instance.Abilities.Length; i++)
                {
                    AbilityPreference.Instance.Abilities[i].Save(false);
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                ForceMenuTreeRebuild();
            }
        }

        private void DeleteButton()
        {
            if (SirenixEditorGUI.ToolbarButton("删除") && EditorUtility.DisplayDialog("提示", $"确认删除{EditingAbility.name}?", "yes", "no"))
            {
                EditingAbility.Destroy();
                ForceMenuTreeRebuild();
            }
        }

        private bool MakeSureCatalogue()
        {
            return AbilityPreference.Instance != null;
        }
    }
}