using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_BlockState : BlockState
{
    GameObject enemy;

    public E01_BlockState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
    {
    }

}
