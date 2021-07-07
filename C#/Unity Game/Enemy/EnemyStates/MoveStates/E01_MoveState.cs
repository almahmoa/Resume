using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_MoveState : MoveState
    //Standard ground movement and check
{
    private GameObject enemy;
    private float flipTimer;

    public E01_MoveState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        entity.CalculateVelocity(entityData.moveSpeed);

        if (flipTimer > 0)
            flipTimer -= Time.deltaTime;

        if (isDetectingWall || !isNotDetectingLedge)
        {
            entity.Flip();
            flipTimer = 0.2f;
        }
        else if ((isPlayerInMinAgroRange || isPlayerInMaxAgroRange) && isNotDetectingLedge && flipTimer <= 0)
        {
            enemy.SendMessage("ChangeState", "PlayerDetected");
        }
    }
}
