using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_PlayerDetectedState : PlayerDetectedState
{
    GameObject enemy;

    public E01_PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SendMessage("NoticeSound");

    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();

        enemy.SendMessage("ChangeState", "Charge");

        /*
        else if (isPlayerInMinAgroRange)
        {
            enemy.SendMessage("ChangeState", "Attack");
        }*/
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
}
