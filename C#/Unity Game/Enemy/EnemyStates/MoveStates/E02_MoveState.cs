using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E02_MoveState : MoveState
    //Unique Flying movement and standard checks
{
    private GameObject enemy;

    public E02_MoveState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        entity.CalculateVelocity(entityData.moveSpeed, entityData.moveSpeed, true);

        if (isPlayerInMinAgroRange || isPlayerInMaxAgroRange)
        {
            enemy.SendMessage("ChangeState", "PlayerDetected");
        }
        else if (isDetectingWall || isMoveTimeOver)
        {
            enemy.SendMessage("ChangeState", "Idle");
        }
    }
}
