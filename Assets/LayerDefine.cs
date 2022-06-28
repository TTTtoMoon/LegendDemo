using UnityEngine;

namespace RogueGods.Gameplay
{
    public readonly struct LayerDefine
    {
        public static readonly LayerDefine Default             = new LayerDefine(nameof(Default));
        public static readonly LayerDefine UI                  = new LayerDefine(nameof(UI));
        
        public static readonly LayerDefine DefaultPlayer       = new LayerDefine(nameof(DefaultPlayer));
        public static readonly LayerDefine CrossSeaPlayer      = new LayerDefine(nameof(CrossSeaPlayer));
        public static readonly LayerDefine CrossObstaclePlayer = new LayerDefine(nameof(CrossObstaclePlayer));
        public static readonly LayerDefine FlyPlayer           = new LayerDefine(nameof(FlyPlayer));
        
        
        public static readonly LayerDefine DefaultMonster       = new LayerDefine(nameof(DefaultMonster));
        public static readonly LayerDefine CrossSeaMonster      = new LayerDefine(nameof(CrossSeaMonster));
        public static readonly LayerDefine CrossObstacleMonster = new LayerDefine(nameof(CrossObstacleMonster));
        public static readonly LayerDefine FlyMonster           = new LayerDefine(nameof(FlyMonster));

        
        
        public static readonly LayerDefine Ground   = new LayerDefine(nameof(Ground));
        public static readonly LayerDefine Sea      = new LayerDefine(nameof(Sea));
        public static readonly LayerDefine Obstacle = new LayerDefine(nameof(Obstacle));
        public static readonly LayerDefine Wall     = new LayerDefine(nameof(Wall));

        public static readonly LayerDefine Interactor = new LayerDefine(nameof(Interactor));


        public static readonly LayerDefine IgnoreRender    = new LayerDefine(nameof(IgnoreRender));

        public static readonly LayerDefine Gethit = new LayerDefine(nameof(Gethit));

        public readonly int Index;
        public readonly int Mask;

        private LayerDefine(string layerName)
        {
            Index = LayerMask.NameToLayer(layerName);
            Mask  = 1 << Index;
        }
    }

    public static class LayerExtension
    {
        public static void SetLayer(this GameObject gameObject, LayerDefine layer)
        {
            if (gameObject == null) return;
            InternalSetPlayer(gameObject.transform, layer);
        }

        public static void SetLayer(this Transform transform, LayerDefine layer)
        {
            if (transform == null) return;
            InternalSetPlayer(transform, layer);
        }

        private static void InternalSetPlayer(Transform transform, LayerDefine layer)
        {
            transform.gameObject.layer = layer.Index;
            for (int i = 0, childCount = transform.childCount; i < childCount; i++)
            {
                InternalSetPlayer(transform.GetChild(i), layer);
            }
        }
    }
}