using System.Linq;
using UnityEditor;

namespace AbilityEditor
{
    public class AbilityProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string rootPath = AbilityPreference.Instance.AbilityConfigurationFolder;
            if (NoAbility(importedAssets) &&
                NoAbility(deletedAssets)  &&
                NoAbility(movedAssets)    &&
                NoAbility(movedFromAssetPaths))
            {
                return;
            }

            string[]               assets    = AssetDatabase.FindAssets($"t:{nameof(AbilityConfiguration)}");
            AbilityConfiguration[] abilities = assets
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AbilityConfiguration>)
                .ToArray();
            if (AbilityPreference.Instance.Abilities == null || 
                AbilityPreference.Instance.Abilities.All(x=>abilities.Contains(x)) == false ||
                abilities.All(x=>AbilityPreference.Instance.Abilities.Contains(x)) == false)
            {
                AbilityPreference.Instance.Abilities = abilities;
                EditorUtility.SetDirty(AbilityPreference.Instance);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            bool NoAbility(string[] assetPaths)
            {
                for (int i = 0; i < assetPaths.Length; i++)
                {
                    if (assetPaths[i].StartsWith(rootPath))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}