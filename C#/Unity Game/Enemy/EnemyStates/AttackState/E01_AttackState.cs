using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_AttackState : AttackState
{
    GameObject enemy;

    private bool hitboxActive;

    public E01_AttackState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();

        if (didPlayerPassCenter)
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
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        //have a alternating bool that sets the hitboxs on and off, so hitboxActive = !hitboxActive
        hitboxActive = !hitboxActive;
        enemy.SendMessage("SetHitbox", hitboxActive);
    }

    public override void Enter()
    {
        base.Enter();

        hitboxActive = false;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.SendMessage("SetHitbox", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        entity.CalculateVelocity(0f);
    }
}
