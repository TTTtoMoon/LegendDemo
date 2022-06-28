using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods
{
    public class CameraSystem : GameSystem
    {
        public enum CameraOffsetType
        {
            //todo：之后要把这几个（除了拉镜和旋转）都删了
            [LabelText("前后,北到南")]     NorthToSouth,         //前后位移
            [LabelText("右左,东到西")]     EastToWest,           //右左位移
            [LabelText("左前右后,西北到东南")] NorthwestToSoutheast, //左前右后位移,西北到东南
            [LabelText("右前左后,东北到西南")] NortheastToSouthwest, //右前左后位移,东北到西南
            [LabelText("上下,拉镜")]      Distance,             //拉镜
            [LabelText("旋转")]         Rotation,             //旋转
        }

        private Camera                   PlayerCamera;
        private Camera                   _UICamera;
        public  Camera                   UICamera => _UICamera;
        private CinemachineBrain         _CinemachineBrain;
        private CinemachineVirtualCamera PlayerVirtualCamera;
        private CinemachineCameraOffset  CinemachineCameraOffset;
        private CinemachineConfiner      _CinemachineConfiner;
        private Coroutine                _ShakeTask;
        private Transform                _FollowTrans;

        public override void Start()
        {
            GameObject cameraParent = GameObject.Find("CameraParent");
            Transform  cameraTrans  = cameraParent.transform.Find("PlayerCamera");
            PlayerCamera = cameraTrans.GetComponent<Camera>();
            _UICamera    = cameraParent.transform.Find("UICamera").GetComponent<Camera>();

            _CinemachineBrain = cameraTrans.GetComponent<CinemachineBrain>();
            Transform mainVirtualCameraTrans = cameraParent.transform.Find("MainVirtualCamera");
            PlayerVirtualCamera     = mainVirtualCameraTrans.GetComponent<CinemachineVirtualCamera>();
            CinemachineCameraOffset = mainVirtualCameraTrans.GetComponent<CinemachineCameraOffset>();
            _CinemachineConfiner    = mainVirtualCameraTrans.GetComponent<CinemachineConfiner>();

            Object.DontDestroyOnLoad(cameraParent);
            base.Start();
        }

        public void SetConfiner(Collider cameraReachableArea)
        {
            _CinemachineConfiner.m_BoundingVolume = cameraReachableArea;
        }

        public void SetTargetAndFollow(Transform trans)
        {
            _FollowTrans               = trans;
            PlayerVirtualCamera.Follow = trans;
            //PlayerVirtualCamera.LookAt      = trans;
            //PlayerDeathVirtualCamera.LookAt = trans;
        }

        public void SetFollowActive(bool active)
        {
            PlayerVirtualCamera.Follow = active ? _FollowTrans : null;
        }

        //拉镜
        public void SetCameraDistance(AnimationCurve curve, CameraOffsetType cameraOffsetType = CameraOffsetType.Distance)
        {
            if (_ShakeTask != null)
            {
                return;
            }

            _ShakeTask = GameManager.Instance.StartCoroutine(SetCameraDistanceCoroutine(cameraOffsetType, curve));
        }

        public void SetCameraDistance(float distance)
        {
            CinemachineCameraOffset.m_Offset = Vector3.down * distance;
        }

        public void ResetCameraDistance()
        {
            CinemachineCameraOffset.m_Offset = Vector3.zero;
        }

        private IEnumerator SetCameraDistanceCoroutine(CameraOffsetType cameraOffsetType, AnimationCurve curve)
        {
            float startTime = curve.keys[0].time;
            float endTime   = curve.keys[curve.length - 1].time;

            //Vector3 addition = Vector3.zero;
            float nowTime = 0;
            while (nowTime < endTime)
            {
                nowTime += Time.deltaTime;

                if (nowTime >= startTime)
                {
                    // CinemachineCameraOffset.m_Offset -= addition;
                    // addition                         =  dir * curve.Evaluate(nowTime);
                    // CinemachineCameraOffset.m_Offset += addition;

                    if (cameraOffsetType == CameraOffsetType.Distance)
                    {
                        CinemachineCameraOffset.m_Offset = Vector3.down * curve.Evaluate(nowTime);
                    }
                    else
                    {
                        PlayerVirtualCamera.m_Lens.Dutch = curve.Evaluate(nowTime);
                    }
                }

                yield return null;
            }

            if (cameraOffsetType == CameraOffsetType.Distance)
            {
                CinemachineCameraOffset.m_Offset = Vector3.zero;
            }
            else
            {
                PlayerVirtualCamera.m_Lens.Dutch = 0;
            }

            //CinemachineCameraOffset.m_Offset -= addition;
            _ShakeTask = null;
        }

        public Vector2 TransWorldSpaceToScreen(Vector3 position)
        {
            return PlayerCamera.WorldToScreenPoint(position);
        }

        public Camera GetPlayerCamera()
        {
            return PlayerCamera;
        }

        public CinemachineBrain GetCinemachineBrain()
        {
            return _CinemachineBrain;
        }

        public CinemachineVirtualCamera GetPlayerVirtualCamera()
        {
            return PlayerVirtualCamera;
        }
    }
}