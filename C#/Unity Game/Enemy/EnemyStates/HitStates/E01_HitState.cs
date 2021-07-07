using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_HitState : HitState
{
    GameObject enemy;

    public E01_HitState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();

        /*
        if (entity.lastDamageDirection != entity.FacingDirection)
        {
            entity.Flip();
        }
        if (isPlayerInMinAgroRange)
        {
            enemy.SendMessage("ChangeState", "Attack");
        }
        else if (isPlayerInMaxAgroRange)
        {
            enemy.SendMessage("ChangeState", "Charge");
        }
        else
        {
            enemy.SendMessage("ChangeState", "Idle");
        }
        */
    }

    public override void Enter()
    {
        base.Enter();
        enemy.GetComponent<Controller2DScript>().collisionMask = 0;
        enemy.SendMessage("SetHitboxHolder", false);
    }

    public override void Exit()
    {
        base.Exit();
        //enemy.SendMessage("SetHitboxHolder", true);
    }
}
