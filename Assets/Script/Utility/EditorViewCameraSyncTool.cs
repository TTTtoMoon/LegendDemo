#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueGods.Utility
{
    // 场景视图和游戏视图的摄像机transfomr同步工具
    
    [ExecuteInEditMode]
    public class EditorViewCameraSyncTool : MonoBehaviour
    {
        public enum SyncMode
        {
            E_Scene2Game = 1,
            E_Game2Scene = 2
        }

        public SyncMode CurSyncMode = SyncMode.E_Scene2Game;
        
        // Update is called once per frame
        void Update()
        {
            switch (CurSyncMode)
            {
                case SyncMode.E_Game2Scene: 
                    UpdateFromGame2Scene();
                    break;
                case SyncMode.E_Scene2Game:
                    UpdateFromScene2Game();
                    break;
            }
        }
        
        void UpdateFromGame2Scene()
        {
            Camera cameraMain = Camera.main;
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
                return;
            sceneView.cameraSettings.nearClip = cameraMain.nearClipPlane;
            sceneView.cameraSettings.fieldOfView = cameraMain.fieldOfView;
            sceneView.pivot = cameraMain.transform.position +
                              cameraMain.transform.forward * sceneView.cameraDistance;
            sceneView.rotation = cameraMain.transform.rotation;
        }

        void UpdateFromScene2Game()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
                return;
            Camera cameraMain = Camera.main;
            cameraMain.nearClipPlane = sceneView.cameraSettings.nearClip;
            cameraMain.fieldOfView = sceneView.cameraSettings.fieldOfView;
            cameraMain.transform.rotation = sceneView.rotation;
            cameraMain.transform.position = sceneView.pivot - cameraMain.transform.forward * sceneView.cameraDistance;
        }
    }
}
#endif