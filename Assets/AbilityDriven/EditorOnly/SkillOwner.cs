#if UNITY_EDITOR
using UnityEditor;

namespace RogueGods.Gameplay.AbilityDriven
{
    public static class SkillOwner
    {
        private const string KEY = "SkillTimelineOwner_";

        public static Actor Get(string key)
        {
            string actorPath = EditorPrefs.GetString(GetKey(key));
            Actor  actor     = AssetDatabase.LoadAssetAtPath<Actor>(actorPath);
            return actor;
        }

        public static void Set(string key, Actor actor)
        {
            if (actor == null) return;
            string path = AssetDatabase.GetAssetPath(actor.gameObject);
            EditorPrefs.SetString(GetKey(key), path);
        }

        private static string GetKey(string key)
        {
            return $"{KEY}_{key}";
        }
    }
}
#endif