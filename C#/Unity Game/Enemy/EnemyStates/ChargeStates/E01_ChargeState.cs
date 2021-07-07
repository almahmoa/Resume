using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_ChargeState : ChargeState
{
    private GameObject enemy;

    public E01_ChargeState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        entity.CalculateVelocity(entityData.chargeSpeed);
        if (isPlayerInMaxAgroRange)
        {
            chargeTime = entityData.chargeTime;
        }
        chargeTime -= Time.deltaTime;
        if (chargeTime <= 0)
            isChargeTimeOver = true;
        //Debug.Log(chargeTime);
        /*
        if (isPlayerInMinAgroRange)
        {
            entity.CalculateVelocity(0f);
            //check if enemy canAttack, a bool to see if it should change state : or "Attack" is read as something else...? (if enemy does not have normal attack)
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
        */
        if (!isNotDetectingLedge || isDetectingWall || isChargeTimeOver)
        {
            enemy.SendMessage("ChangeState", "Move");
        }
    }
}
