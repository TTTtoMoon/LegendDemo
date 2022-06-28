using System;
using System.Collections;
using System.Collections.Generic;
using RogueGods.Gameplay;
using UnityEngine;

namespace RogueGods
{
    public class FlashChain : MonoBehaviour
    {
        private LineRenderer _LineRenderer;

        private void Awake()
        {
            _LineRenderer = transform.Find("Line").GetComponent<LineRenderer>();
        }

        public void SetInfo(Vector3 startPosition, float range, float interval, float duration, int maxSpreadCount, Action<Actor> callBack)
        {
            StartCoroutine(SpreadCoroutine(startPosition, range, interval, duration, maxSpreadCount, callBack));
        }

        private IEnumerator SpreadCoroutine(Vector3 findPosition, float range, float interval, float duration, int maxSpreadCount, Action<Actor> callBack)
        {
            List<Actor> alreadyAttackMonsters = new List<Actor>();

            Collider[] colliders = RaycastHitPool.LargeOverlay();
            while (maxSpreadCount > 0)
            {
                int monsterActors = Physics.OverlapSphereNonAlloc(findPosition, range, colliders);

                Actor finalMonsterActor = null;
                for (int i = 0; i < monsterActors; i++)
                {
                    Actor actor = colliders[i].GetComponent<Actor>();
                    if(actor == null) continue;
                    
                    if (alreadyAttackMonsters.Contains(actor))
                    {
                        continue;
                    }

                    if (finalMonsterActor == null)
                    {
                        finalMonsterActor = actor;
                        continue;
                    }

                    if ((actor.Position - findPosition).sqrMagnitude < (finalMonsterActor.Position - findPosition).sqrMagnitude)
                    {
                        finalMonsterActor = actor;
                    }
                }

                if (finalMonsterActor == null)
                {
                    break;
                }

                maxSpreadCount--;
                alreadyAttackMonsters.Add(finalMonsterActor);
                Vector3 finalPosition = finalMonsterActor.Position + Vector3.up * 0.4f /*finalMonsterActor.CC.height / 2*/;

                Vector3[] positions;
                if (_LineRenderer.positionCount == 0)
                {
                    _LineRenderer.positionCount = 2;
                    positions                   = new Vector3[2];
                    positions[0]                = findPosition;
                    positions[1]                = finalPosition;
                }
                else
                {
                    int count = _LineRenderer.positionCount;
                    _LineRenderer.positionCount = count + 1;
                    positions                   = new Vector3[count + 1];
                    _LineRenderer.GetPositions(positions);
                    positions[count] = finalPosition;
                }

                _LineRenderer.SetPositions(positions);
                findPosition = finalPosition;
                StartCoroutine(FlashPersistantTask(duration));
                callBack?.Invoke(finalMonsterActor);

                yield return new WaitForSeconds(interval);
            }
            
            RaycastHitPool.Release(colliders);
        }

        private IEnumerator FlashPersistantTask(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (_LineRenderer.positionCount > 0)
            {
                Vector3[] oldPositions = new Vector3[_LineRenderer.positionCount];
                _LineRenderer.GetPositions(oldPositions);
                Vector3[] newPositions = new Vector3[oldPositions.Length - 1];
                for (int i = 1; i < oldPositions.Length; i++)
                {
                    newPositions[i - 1] = oldPositions[i];
                }

                _LineRenderer.positionCount -= 1;
                _LineRenderer.SetPositions(newPositions);
            }
            else
            {
                Exit();
            }
        }

        public void Exit()
        {
            GameManager.FlashChainSystem.RemoveFlashChain(this);
        }
    }
}