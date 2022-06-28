using UnityEngine;

namespace RogueGods.Utility
{
    public static class PositionConvertUtility
    {
        public static Vector2 WorldToScreenPoint(Camera camera, Vector3 worldPos)
        {
            var cameraTransform = camera.transform;
            var camNormal = cameraTransform.forward;
            var vectorFromCam = worldPos - cameraTransform.position;
            var camNormDot = Vector3.Dot(camNormal, vectorFromCam);
            if (camNormDot <= 0)
            {
                // we are behind the camera forward facing plane, project the position in front of the plane
                var proj = (camNormal * (camNormDot * 1.01f));
                worldPos = camera.transform.position + (vectorFromCam - proj);
            }

            return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
        }

        public static Vector2 WorldToScreenPoint(Camera camera, Rect screenRect, Vector3 worldPos)
        {
            var cameraTransform = camera.transform;
            var camNormal = cameraTransform.forward;
            var vectorFromCam = worldPos - cameraTransform.position;
            var camNormDot = Vector3.Dot(camNormal, vectorFromCam);
            if (camNormDot <= 0)
            {
                // we are behind the camera forward facing plane, project the position in front of the plane
                var proj = (camNormal * (camNormDot * 1.01f));
                worldPos = camera.transform.position + (vectorFromCam - proj);
            }

            var viewportPos = camera.WorldToViewportPoint(worldPos);
            var result = new Vector2(viewportPos.x * screenRect.width, viewportPos.y * screenRect.height);
            return result;
        }

        public static bool RectangleContainsScreenPoint(Rect rect, Vector2 screenPoint)
        {
            if (screenPoint.x <= 0 || screenPoint.x >= rect.width)
            {
                return false;
            }

            if (screenPoint.y <= 0 || screenPoint.y >= rect.height)
            {
                return false;
            }

            return true;
        }
    }
}