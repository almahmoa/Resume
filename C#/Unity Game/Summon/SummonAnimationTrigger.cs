using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonAnimationTrigger : MonoBehaviour
{
    public SummonScript summonScript;

    private void SetAttackActive(int num) => summonScript.SetAttackActive(num);
    private void SetHitBoxOne(float hitboxX) => summonScript.SetHitBoxOne(hitboxX);
    private void AnimationFinished(string currentAnimation) => summonScript.AnimationFinished(currentAnimation);
}
