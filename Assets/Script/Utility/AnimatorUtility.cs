using System.Collections;
using UnityEngine;

namespace RogueGods.Utility
{
    public static class AnimatorUtility
    {
        public static IEnumerator WaitAnimPlayOver(Animator animator,string animName, float timeLength = 1f,string finalAnimName ="")
        {
            AnimatorStateInfo info;

            var targetAnimHash = Animator.StringToHash(animName);
            var finalAnimHash = string.IsNullOrEmpty(finalAnimName) ? -1 : Animator.StringToHash(finalAnimName);
            do
            {
                info = animator.GetCurrentAnimatorStateInfo(0);
                if (finalAnimHash != -1 && info.shortNameHash == finalAnimHash)
                {
                    yield break;
                }

                yield return null;
            } while (animator != null && info.shortNameHash != targetAnimHash || info.normalizedTime < timeLength);
        }
    }
}