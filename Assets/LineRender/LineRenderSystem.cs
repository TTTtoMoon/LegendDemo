using System.Collections.Generic;
using UnityEngine;

namespace RogueGods.Gameplay.VFX
{
    public class LineRenderSystem : GameSystem
    {
        private struct RayInfo
        {
            public LineRenderer LineRenderer;
            public bool         CrossWall;
            public int          ReflectCount;
        }

        private Dictionary<LineRenderer, float>   _LineRenderers = new Dictionary<LineRenderer, float>();
        private Dictionary<LineRenderer, RayInfo> _Rays          = new Dictionary<LineRenderer, RayInfo>();

        public override void Update()
        {
            base.Update();
            int count = _LineRenderers.Count;
            if (count > 0)
            {
                List<LineRenderer> removeLineRender = new List<LineRenderer>(count);

                foreach (var lineRenderer in _LineRenderers)
                {
                    if (Time.time > lineRenderer.Value)
                    {
                        removeLineRender.Add(lineRenderer.Key);
                    }
                }

                for (int i = 0; i < removeLineRender.Count; i++)
                {
                    RemoveLineRender(removeLineRender[i]);
                }
            }
        }

        /// <summary>
        /// 创建连线
        /// </summary>
        public LineRenderer CreatLine(GameObject line, Vector3 startPosition, Vector3 endPosition, float width, float time)
        {
            LineRenderer lineRenderer = Object.Instantiate(line).GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth   = width;
            _LineRenderers.Add(lineRenderer, Time.time + time);
            return lineRenderer;
        }

        public LineRenderer CreatRay(GameObject line, Vector3 startPosition, Vector3 dir, int reflectCount, bool crossWall, float width)
        {
            LineRenderer lineRenderer = Object.Instantiate(line).GetComponent<LineRenderer>();

            lineRenderer.startWidth = width;
            lineRenderer.endWidth   = width;
            _Rays.Add(lineRenderer, new RayInfo()
            {
                LineRenderer  = lineRenderer,
                CrossWall     = crossWall,
                ReflectCount = reflectCount
            });
            UpdataRay(lineRenderer, startPosition, dir);
            //_LineRenderers.Add(lineRenderer, Time.time + time);
            return lineRenderer;
        }

        public void UpdataRay(LineRenderer ray, Vector3 startPosition, Vector3 dir)
        {
            RayInfo rayInfo = _Rays[ray];

            rayInfo.LineRenderer.positionCount = 1;
            rayInfo.LineRenderer.SetPosition(0, startPosition);

            float lineY = startPosition.y;
            startPosition.y = 0;

            int checkWallCount = rayInfo.ReflectCount + 1;
            while (checkWallCount > 0)
            {
                Ray rayLine = new Ray(startPosition, dir);
                if (Physics.Raycast(rayLine, out RaycastHit hit, 1000, rayInfo.CrossWall ? LayerDefine.Wall.Mask : (LayerDefine.Wall.Mask | LayerDefine.Obstacle.Mask)))
                {
                    int nowCount = rayInfo.LineRenderer.positionCount;
                    rayInfo.LineRenderer.positionCount += 1;
                    rayInfo.LineRenderer.SetPosition(nowCount, hit.point + Vector3.up * lineY);

                    startPosition = hit.point;
                    dir           = Vector3.Reflect(dir, hit.normal);
                }

                checkWallCount--;
            }
        }

        public void ReleaseRay(LineRenderer ray)
        {
            _Rays.Remove(ray);
            Object.DestroyImmediate(ray.gameObject);
        }

        public void RemoveLineRender(LineRenderer lineRenderer)
        {
            _LineRenderers.Remove(lineRenderer);
            Object.DestroyImmediate(lineRenderer.gameObject);
        }
    }
}