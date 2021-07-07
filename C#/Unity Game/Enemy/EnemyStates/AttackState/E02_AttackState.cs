using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E02_AttackState : AttackState
    //projectile attack with cooldown (after an attack)
{
    GameObject enemy;

    public bool cooldown;
    public float cooldownTimer;
    private bool hitboxActive;

    public ProjectileScript Projectile { get; private set; }

    public E02_AttackState(Entity entity, FiniteStateMachine stateMachine, EntityData entityData, string animBoolName, GameObject enemy) : base(entity, stateMachine, entityData, animBoolName)
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

        cooldown = true;
        entity.Anim.SetBool("cooldown", true);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        enemy.SendMessage("ShootProjectile");
    }

    public override void Enter()
    {
        base.Enter();

        cooldownTimer = entityData.cooldownTimer;
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

        if (cooldown)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if(cooldownTimer <= 0)
        {
            entity.Anim.SetBool("cooldown", false);
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
    }
}
