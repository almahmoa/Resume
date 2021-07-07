using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_IdleState : IdleState
{
    GameObject enemy;

    public E01_IdleState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
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

        entity.CalculateVelocity(0f);

        if (isPlayerInMinAgroRange || isPlayerInMaxAgroRange)
        {
            enemy.SendMessage("ChangeState", "PlayerDetected");
        }
        else if (isIdleTimeOver)
        {
            isIdleTimeOver = false;
            if(isDetectingWall || !isNotDetectingLedge)
                entity.Flip();
        }
        else if(isTurnTimeOver)
        {
            enemy.SendMessage("ChangeState", "Move");
        }
    }
}
