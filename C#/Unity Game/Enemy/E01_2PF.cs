using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E01_2PF : Entity
/*Basic enemy (idle -> move -> player detect -> charge -> attack) (hit -> dead)
* BODY HITBOX -- TWO ATTACKS (PROJECTILE) -- NO BLOCK -- FLYING
* COUNTER SYSTEM FOR ATTACKS
* MOVEMENT IN IDLE: shaped like this "~"
*/
{
    #region State Variables
    public E01_IdleState IdleState { get; private set; }
    public E02_MoveState MoveState { get; private set; } //Standard flying movement 
    public E01_PlayerDetectedState PlayerDetectedState { get; private set; }
    public E02_ChargeState ChargeState { get; private set; } //Flip if playerPassedCenter
    public E02_AttackState AttackState { get; private set; } //Standard projectile attack at minAgroRange
    public E01_HitState HitState { get; private set; }
    public E01_DeadState DeadState { get; private set; }
    #endregion

    public GameObject weakSpotPosition;
    public float weakSpotRadius = 1f;

    public GameObject hitboxHolder;
    public GameObject[] hitboxes;

    private Dictionary<string, State> stateDictionary = new Dictionary<string, State>();

    private int attackCounter;

    private GameObject projectile;
    public GameObjectPool ProjectilePool;
    public List<ProjectileData> projectileData;

    public override void Start()
    {
        base.Start();

        StateMachine = new FiniteStateMachine();

        IdleState = new E01_IdleState(this, StateMachine, entityData, "idle", gameObject);
        MoveState = new E02_MoveState(this, StateMachine, entityData, "move", gameObject);
        PlayerDetectedState = new E01_PlayerDetectedState(this, StateMachine, entityData, "playerDetected", gameObject);
        ChargeState = new E02_ChargeState(this, StateMachine, entityData, "charge", gameObject);
        AttackState = new E02_AttackState(this, StateMachine, entityData, "attack", gameObject);
        HitState = new E01_HitState(this, StateMachine, entityData, "hit", gameObject);
        DeadState = new E01_DeadState(this, StateMachine, entityData, "dead", gameObject);

        stateDictionary.Add("Idle", IdleState);
        stateDictionary.Add("Move", MoveState);
        stateDictionary.Add("PlayerDetected", PlayerDetectedState);
        stateDictionary.Add("Charge", ChargeState);
        stateDictionary.Add("Attack", AttackState);
        stateDictionary.Add("Hit", HitState);
        stateDictionary.Add("Dead", DeadState);

        SetValues();
        StateMachine.Initialize(IdleState);
    }

    private void SetValues()
    {
        gravity = 0f;
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;
        enemyPos = new Vector2(transform.position.x, transform.position.y);
        currentSpeed = entityData.moveSpeed;
        oldRotationZ = 90;

        ledgeCheck.transform.localPosition = new Vector2(0.5f, 0);
        wallCheck.transform.localPosition = new Vector2(1, 0);
        playerCheck.transform.localPosition = new Vector2(0, 0);
        playerCheck.transform.localRotation = Quaternion.Euler(0, 0, 90);
        firstRotationZ = playerCheck.transform.eulerAngles.z;
        playerPassedCenterCheck.transform.localPosition = new Vector2(-0.9f, 0);

        gameObject.layer = 11; //Damagable
        FacingDirection = 1; //Right

        hitboxHolder.SetActive(true);
        hitboxes[0].SetActive(true);
        hitboxes[0].transform.localPosition = new Vector2(0, 0);
        hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(2f, 2.5f);
        SetHitbox(false);

        attackCounter = 0;
        projectile = ProjectilePool.Get();
    }

    private void OnEnable()
    {
        SetValues();
    }

    private void OnDisable()
    {
        StateMachine.ChangeState(IdleState);
        transform.position = new Vector2(enemyPos.x, enemyPos.y);
        hitboxHolder.SetActive(false);
    }

    public override void Flip()
    {
        base.Flip();

        weakSpotPosition.transform.localPosition = new Vector2(weakSpotPosition.transform.localPosition.x * -1, weakSpotPosition.transform.localPosition.y);
        if (FacingDirection == Mathf.Sign(playerPassedCenterCheck.transform.localPosition.x))
        {
            playerPassedCenterCheck.transform.localPosition = new Vector2(playerPassedCenterCheck.transform.localPosition.x * -1, weakSpotPosition.transform.localPosition.y);
        }
    }

    public void ChangeState(string state)
    {
        StateMachine.ChangeState(stateDictionary[state]);
        /*TODO:
         * to make weak spot work on back attack, the default value of weakspot active = true (and on idle and move)
         * when playerdetected (change and attack) weakspot active = false
         * when player pass center when weakspot active = false, SET to true
         * when state = dead, set hitboxHolder.SetActive(false)
         */
    }

    public void Damage(AttackDetails attackDetails)
    {
        lastDamageDirection = (attackDetails.position.x > gameObject.transform.position.x) ? 1 : -1;
        currentHealth -= attackDetails.damageAmount;
        //currentStunResistance -= attackDetails.stunDamageAmount;
        StateMachine.ChangeState(HitState);

        velocity.x = 5f * -lastDamageDirection; //temp knockback

        if (weakSpotPosition.activeSelf == true)
        {
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(weakSpotPosition.transform.position, weakSpotRadius, whatIsPlayerAttack);

            foreach (Collider2D collider in detectedObjects)
            {
                //WeakSpotCounter();
                //oldDetectedObjects.Add(collider);
            }
        }

        if (currentStunResistance <= 0)
        {
            isStunned = true;
        }

        if (currentHealth <= 0f)
        {
            StateMachine.ChangeState(DeadState);
        }
    }

    public override void CalculateVelocity(float moveSpeedX, float moveSpeedY, bool uniqueMove) // isFlying
    {
        moveSpeedY = moveSpeedY / 2;
        if (moveSpeedX == 0f && moveSpeedY == 0f)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref velocityXSmoothing, smoothTimeX);
            velocity.y = 0f;
        }
        else
        {
            if (!isRising)
            {
                if (velocity.y >= (targetVelocityY - 0.05f) && !canSetNewAirDir)
                {
                    targetVelocityY = -moveSpeedY;
                    canSetNewAirDir = true;
                }
                else if (velocity.y <= (targetVelocityY + 0.05) && canSetNewAirDir)
                {
                    targetVelocityY = moveSpeedY;
                    canSetNewAirDir = false;
                }
            }
            else if (isRising)
            {
                targetVelocityY = moveSpeedY;
            }

            targetVelocityX = moveSpeedX * FacingDirection;

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTimeX);
            velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, smoothTimeY);
        }
    }
    
    public void ShootProjectile()
    {
        if(attackCounter < entityData.attackCounterLimit)
        {
            Knockback(5f, 5f); // values must be changable
            projectile.GetComponent<ProjectileScript>().SetProjectileData(projectileData[0]); // have bullet be passed
        }
        else
        {
            Knockback(8f, 8f); // values must be changable
            projectile.GetComponent<ProjectileScript>().SetProjectileData(projectileData[1]); // have bullet be passed
            attackCounter = 0;
        }

        projectile.transform.parent = ProjectilePool.transform;
        projectile.transform.rotation = transform.rotation;
        projectile.transform.position = transform.position; //subject to change for more accuracy
        projectile.gameObject.SetActive(true);
        attackCounter += 1;
    }

    public void SetHitbox(bool isActive)
    {
        for (int i = 1; i < hitboxes.Length; i++)
        {
            hitboxes[i].SetActive(isActive);
            hitboxes[i].transform.localPosition = new Vector2(entityData.hitboxDistance[i - 1].x * FacingDirection, entityData.hitboxDistance[i - 1].y);
            hitboxes[i].GetComponent<BoxCollider2D>().size = new Vector2(entityData.hitboxSize[i - 1].x, entityData.hitboxSize[i - 1].y);
        }
    }

    public void SetHitboxHolder(bool isActive) => hitboxHolder.SetActive(isActive);
}
