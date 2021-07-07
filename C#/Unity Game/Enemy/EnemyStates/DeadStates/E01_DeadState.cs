using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_DeadState : DeadState
{
    GameObject enemy;

    public E01_DeadState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        entity.enemySpawner.AddEnemy(entity.gameObject);
        entity.gameObject.SetActive(false);
    }
}
