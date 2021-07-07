using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class E01_1MG : Entity
/*Basic enemy (idle -> move -> player detect -> charge -> attack) (hit -> dead)
* BODY HITBOX -- ONE ATTACK (MELEE) -- NO BLOCK -- GROUNDED
*/
{
    #region State Variables
    //public E01_IdleState IdleState { get; private set; }
    public E01_MoveState MoveState { get; private set; } //Standard ground movement
    public E01_PlayerDetectedState PlayerDetectedState { get; private set; }
    public E01_ChargeState ChargeState { get; private set; } //Does not flip when playerPassedCenter
    //public E01_AttackState AttackState { get; private set; } //Standard melee attack at minAgroRange
    public E01_HitState HitState { get; private set; }
    public E01_DeadState DeadState { get; private set; }
    #endregion

    public GameObject weakSpotPosition;
    public float weakSpotRadius = 1f;

    public GameObject hitboxHolder;
    public GameObject[] hitboxes;
    private List<Collider2D> oldDetectedObjects = new List<Collider2D>();

    //use dictionary for future enemyscripts to facilitate different options for similar states (ex. attack1, attack2)
    private Dictionary<string, State> stateDictionary = new Dictionary<string, State>();
    public AudioSource deadSFX;
    public SoundEffectsScript SFXScript;

    public override void Start()
    {
        base.Start();

        StateMachine = new FiniteStateMachine();

        //IdleState = new E01_IdleState(this, StateMachine, entityData, "idle", gameObject);
        MoveState = new E01_MoveState(this, StateMachine, entityData, "move", gameObject);
        PlayerDetectedState = new E01_PlayerDetectedState(this, StateMachine, entityData, "playerDetected", gameObject);
        ChargeState = new E01_ChargeState(this, StateMachine, entityData, "charge", gameObject);
        //AttackState = new E01_AttackState(this, StateMachine, entityData, "attack", gameObject);
        HitState = new E01_HitState(this, StateMachine, entityData, "hit", gameObject);
        DeadState = new E01_DeadState(this, StateMachine, entityData, "dead", gameObject);

        //stateDictionary.Add("Idle", IdleState);
        stateDictionary.Add("Move", MoveState);
        stateDictionary.Add("PlayerDetected", PlayerDetectedState);
        stateDictionary.Add("Charge", ChargeState);
        //stateDictionary.Add("Attack", AttackState);
        stateDictionary.Add("Hit", HitState);
        stateDictionary.Add("Dead", DeadState);

        SetValues();
        StateMachine.Initialize(MoveState);
    }

    public override void Update()
    {
        base.Update();

        //Attack-------------------------------------------------------------------------------------------------------------
        if (hitboxHolder.activeSelf == true)
        {
            //Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(hitbox.transform.position, attackRadius, whatIsDamagable);
            foreach (GameObject hitbox in hitboxes)
            {
                if(hitbox.activeSelf == true)
                {
                    Collider2D[] detectedObjects = Physics2D.OverlapBoxAll(hitbox.transform.position, hitbox.GetComponent<BoxCollider2D>().size, 0, whatIsDamagable);
                    foreach (Collider2D collider in detectedObjects)
                    {
                        if (oldDetectedObjects.Contains(collider))
                        {
                            break;
                        }
                        attackDetails.damageAmount = 1;//change damage amount outside of here for different attack/weapon so make that into a variable (instead of 1f)
                        attackDetails.position = transform.position;

                        //velocity.x = attackKnockback.x * -controller.collisions.faceDir;
                        //velocity.y = attackKnockback.y;
                        //hitStop.Stop(hitstopAttack);

                        //isAttacking = true;

                        collider.transform.SendMessage("Damage", attackDetails); //can send knockback and such
                        oldDetectedObjects.Add(collider);
                        //need to add self knockback when hitting an enemy, just once recoil

                        SFXScript.KickedSFX();
                    }
                }
            }
        }
        if (hitboxes[2].activeSelf == false)
        {
            oldDetectedObjects.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------

        velocity.y -= 0.45f;
    }

    private void SetValues()
    {
        //set the collision mask back to ground(8), moving ground(10), interactive(15)
        controller.collisionMask = (1 << 8) | (1 << 10) | (1 << 15);

        gravity = -62.5f;
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;
        enemyPos = new Vector2(transform.position.x, transform.position.y);
        currentSpeed = entityData.moveSpeed;
        oldRotationZ = 90;

        ledgeCheck.transform.localPosition = new Vector2(1.50f, 0);
        wallCheck.transform.localPosition = new Vector2(2.25f, 0);
        playerCheck.transform.localPosition = new Vector2(0, 0);
        playerCheck.transform.localRotation = Quaternion.Euler(0, 0, 90);
        firstRotationZ = playerCheck.transform.eulerAngles.z;
        playerPassedCenterCheck.transform.localPosition = new Vector2(-0.9f, 0);

        gameObject.layer = 11; //Damagable
        FacingDirection = 1; //Right

        hitboxHolder.SetActive(true);

        //hitboxes[0].SetActive(true);
        //hitboxes[0].transform.localPosition = new Vector2(0, 0);
        //hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 3f);
        //SetHitbox(false);
        //Flip();
    }

    private void OnEnable()
    {
        SetValues();
    }

    private void OnDisable()
    {
        StateMachine.ChangeState(MoveState);
        transform.position = new Vector2(enemyPos.x, enemyPos.y);
        hitboxHolder.SetActive(false);
    }

    public override void Flip()
    {
        base.Flip();

        //weakSpotPosition.transform.localPosition = new Vector2(weakSpotPosition.transform.localPosition.x * -1, weakSpotPosition.transform.localPosition.y);
        if(FacingDirection == Mathf.Sign(playerPassedCenterCheck.transform.localPosition.x))
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

        //temp knockback
        velocity.x = 5f * -lastDamageDirection;
        velocity.y = 15f;
        SFXScript.PunchedSFX();
        deadSFX.pitch = Random.Range(.65f, .75f);
        deadSFX.Play();
        /*
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

        */
        if (currentHealth <= 0f)
        {
            StartCoroutine(DeadTimer());
        }
    }

    public void NoticeSound()
    {
        deadSFX.pitch = Random.Range(.9f, 1f);
        deadSFX.Play();
    }

    IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(2);
        StateMachine.ChangeState(DeadState);
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
