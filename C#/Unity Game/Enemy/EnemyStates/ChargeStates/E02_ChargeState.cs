using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E02_ChargeState : ChargeState
{
    private GameObject enemy;

    protected bool didPlayerPassCenter;

    public E02_ChargeState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (didPlayerPassCenter)
        {
            entity.Flip();
            didPlayerPassCenter = false;
        }

        entity.CalculateVelocity(entityData.chargeSpeed);

        if (isPlayerInMinAgroRange)
        {
            entity.CalculateVelocity(0f);
            enemy.SendMessage("ChangeState", "Attack");
        }
        else if (isPlayerInMaxAgroRange)
        {
            chargeTime = entityData.chargeTime;
        }

        if (!isNotDetectingLedge || isDetectingWall || isChargeTimeOver)
        {
            enemy.SendMessage("ChangeState", "Idle");
        }
    }
}
