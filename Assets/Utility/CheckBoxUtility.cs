using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace RogueGods
{
    public static class CheckBoxUtility
    {
        public struct Circle
        {
            //法线为Vector3.up的地面坐标
            public float2 Position;
            public float  Radius;
        }

        public struct Square
        {
            //法线为Vector3.up的地面坐标
            public float2 Position;

            //以Vector3.forward为参考的旋转
            public float Roation;

            //float1为vector3.right为正，float2为vector3.forward为正
            public float2 Scale;
        }

        public struct Sector
        {
            //法线为Vector3.up的地面坐标
            public float2 Position;

            //忽略Y轴的方向
            //public float2 ForwardAxies;
            public float  minAxiesAngle;
            public float  maxAxiesAngele;
            public float  MinDistance;
            public float  MaxDistance;
        }

        public static bool Check(Circle circle1, Circle circle2)
        {
            float distance           = math.distancesq(circle1.Position, circle2.Position);
            float radiusAddition     = circle1.Radius + circle2.Radius;
            float radiusAdditionPow2 = math.pow(radiusAddition, 2);
            return distance < radiusAdditionPow2;
        }

        public static bool Check(Square square, Circle circle)
        {
            quaternion forwardQuaternion = quaternion.RotateY(square.Roation*Mathf.Deg2Rad);
            float3     boxForward3d      = math.forward(forwardQuaternion);
            float2     boxForward        = new float2(boxForward3d.x, boxForward3d.z);

            Quaternion rightQuaternion = quaternion.RotateY((square.Roation+90) *Mathf.Deg2Rad);
            Vector3    boxRight3d      = rightQuaternion *Vector3.forward;
            Vector2    boxRight        = Utility.Utilities.ThirdPersonToTopCoordinate(boxRight3d);
            

            float2 tempBox1Scale = new float2(square.Scale.x + circle.Radius * 2, square.Scale.y);
            float2 tempBox2Scale = new float2(square.Scale.x,                     square.Scale.y + circle.Radius * 2);

            float2 circle2Dir = circle.Position - square.Position;

            float forwardLength = math.dot(circle2Dir, boxForward);
            float rightLength   = math.dot(circle2Dir, boxRight);
            if (math.abs(forwardLength) < (tempBox1Scale.y / 2) && math.abs(rightLength) < (tempBox1Scale.x / 2))
            {
                return true;
            }

            if (math.abs(forwardLength) < (tempBox2Scale.y / 2) && math.abs(rightLength) < (tempBox2Scale.x / 2))
            {
                return true;
            }

            float2 leftUpPoint    = square.Position + boxForward * square.Scale.y / 2 - (float2)boxRight * square.Scale.x / 2;
            float2 leftDownPoint  = square.Position                                   - boxForward       * square.Scale.y / 2 - (float2)boxRight * square.Scale.x / 2;
            float2 rightUpPoint   = square.Position                                   + boxForward       * square.Scale.y / 2 + (float2)boxRight         * square.Scale.x / 2;
            float2 rightDownPoint = square.Position - boxForward * square.Scale.y                        / 2                  + (float2)boxRight         * square.Scale.x / 2;

            if (math.distancesq(circle.Position, leftUpPoint) <math.pow(circle.Radius,2))
            {
                return true;
            }

            if (math.distancesq(circle.Position, leftDownPoint) < math.pow(circle.Radius, 2))
            {
                return true;
            }

            if (math.distancesq(circle.Position, rightUpPoint) < math.pow(circle.Radius, 2))
            {
                return true;
            }

            if (math.distancesq(circle.Position, rightDownPoint) < math.pow(circle.Radius, 2))
            {
                return true;
            }

            return false;
        }

        public static bool Check(Sector sector, Circle circle2)
        {
            float distance         = math.distance(sector.Position, circle2.Position);

            float radiusAddition     = sector.MaxDistance + circle2.Radius;
            if (distance > radiusAddition)
            {
                return false;
            }

            if (distance + circle2.Radius < sector.MinDistance)
            {
                return false;
            }

            
            Vector3 minSectorDir    =  Quaternion.Euler(0, sector.minAxiesAngle,  0) * Vector3.forward;
            Vector3 maxSectorDir    = Quaternion.Euler(0, sector.maxAxiesAngele, 0) * Vector3.forward;

            Vector3 sectorLeftPoint  = new Vector3(sector.Position.x, 0, sector.Position.y) + minSectorDir * sector.MinDistance;
            Vector3 sectorRightPoint = new Vector3(sector.Position.x, 0, sector.Position.y) + maxSectorDir * sector.MinDistance;
            Vector3 circlePosition   = new float3(circle2.Position.x, 0, circle2.Position.y);
            float   circleRadius     = math.pow(circle2.Radius, 2);
            if (math.distancesq(sectorLeftPoint, circlePosition) <= circleRadius || math.distancesq(sectorRightPoint, circlePosition) <= circleRadius)
            {
                return true;
            }
            

            Vector2 v2Dir           = circle2.Position - sector.Position;
            Vector3 dir             = new Vector3(v2Dir.x, 0, v2Dir.y);
            float   targetHalfAngle = math.atan(circle2.Radius / distance)   *Mathf.Rad2Deg;
            Vector3 minCircleDir    = Quaternion.Euler(0, -targetHalfAngle, 0) *dir ;
            Vector3 maxCircleDir    = Quaternion.Euler(0, targetHalfAngle,  0) *dir ;

            float aa =Vector3.Dot(minCircleDir, minSectorDir);
            float bb =Vector3.Dot(minCircleDir, maxSectorDir);
            
            Vector3 a  = Vector3.Cross(minSectorDir, minCircleDir);
            Vector3 b  = Vector3.Cross(minCircleDir, maxSectorDir);
         

            //扇形是否是锐角
            bool  sectorAcuteAngle =Vector3.Cross(minSectorDir, maxSectorDir).y >0;
            //扇形的角度
            float sectorAngle = Vector3.Angle(minSectorDir, maxSectorDir);
            sectorAngle = sectorAcuteAngle ? sectorAngle : 360 - sectorAngle;
            
            
            bool  circleAcuteAngle =Vector3.Cross(minSectorDir, minCircleDir).y >0;
            float circleAngle      = Vector3.Angle(minSectorDir, minCircleDir);
            circleAngle = circleAcuteAngle ? circleAngle : 360 - circleAngle;

            if (circleAngle <= sectorAngle)
            {
                return true;
            }
            
            
            circleAcuteAngle = Vector3.Cross(minSectorDir, maxCircleDir).y >0;
            circleAngle      = Vector3.Angle(minSectorDir, maxCircleDir);
            circleAngle      = circleAcuteAngle ? circleAngle : 360 - circleAngle;
            
            if (circleAngle <= sectorAngle)
            {
                return true;
            }
            
            

            sectorAcuteAngle = Vector3.Cross(minCircleDir, maxCircleDir).y >0;
            sectorAngle      = Vector3.Angle(minCircleDir, maxCircleDir);
            sectorAngle      = sectorAcuteAngle ? sectorAngle : 360 - sectorAngle;
            
            
            circleAcuteAngle = Vector3.Cross(minCircleDir, minSectorDir).y >0;
            circleAngle      = Vector3.Angle(minCircleDir, minSectorDir);
            circleAngle      = circleAcuteAngle ? circleAngle : 360 - circleAngle;

            if (circleAngle <= sectorAngle)
            {
                return true;
            }
            
            
            circleAcuteAngle = Vector3.Cross(minCircleDir, maxSectorDir).y >0;
            circleAngle      = Vector3.Angle(minCircleDir, maxSectorDir);
            circleAngle      = circleAcuteAngle ? circleAngle : 360 - circleAngle;

            if (circleAngle <= sectorAngle)
            {
                return true;
            }
            
            return false;
        }
    }
}
