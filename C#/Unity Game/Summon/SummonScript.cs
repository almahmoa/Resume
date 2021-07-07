using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2DScript))]
public class SummonScript : MonoBehaviour
{
    [Header("Move Variable")]
    public float moveSpeed = 8;
    public float baseSpeed = 8;
    public float maxSpeed = 15;
    private float velocityXSmoothing = 0;
    public float accelerationTimeGrounded = 0.1f;

    [Header("Jump Variable")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public bool playerJumpReleased;
    public bool isJumpReleased;
    public bool jumpBuffer = false;
    public bool isJumping;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = 0.2f;

    [Header("Other Variables")]
    public int currentHealth;
    public int maxHealth = 0;
    private float delayTimeWalk;
    public float setDelayTimeWalk = 0.35f;
    private float delayTimeJump;
    public float setDelayTimeJump = 0.15f;
    public int facingDir = 1;
    public float gravity;
    public bool isActive;
    public bool actionLock = false;
    private float deadTime;
    public float setDeadTime = 1.5f;
    public bool isDead;
    public float teleportDistance = 15;
    public Vector3 velocity;
    public string summonName;
    public int sumIndex;
    public bool canUnsummon;
    public float unsummonTimer;
    public float setUnsummonTimer = 2;
    public bool currentSummon;
    public GameObject smoke;

    [Header("Hit Variable")]
    public float setIsDamagedTime = 0.5f;
    public float setIFrameTime = 1f;
    private float isDamagedTime;
    private float iFrameTime = 0f;
    public bool isDamaged;
    public Vector2 damageKnockback = new Vector2(9, 13);
    public float hitstopDamage = 0.25f;
    public float hitstopAttack = 0.1f;

    [Header("Attack Variable")]
    public GameObject hitboxHolder;
    public GameObject[] hitboxes;
    private float hitboxYOne = 0.1f;
    public LayerMask whatIsDamagable;
    public Vector2 attackKnockback;
    public float setAttackBuffer = 0.2f;
    private float attackBuffer;
    public bool isAttacking = false;
    public bool airDownAttack = false;
    private List<Collider2D> oldDetectedObjects = new List<Collider2D>();
    public bool canAttack;

    [Header("Components")]
    protected SpriteRenderer theSR;
    protected Animator anim;
    public GameObject[] summonGO;
    public SummonData[] SDList;
    public SummonData SD;
    public BoxCollider2D BC;
    public GameObject wallCheck;

    [Header("UI")]
    public Image[] hearts;

    [Header("Player Variable")]
    public Transform trackingPos;
    public Transform playerPos;
    public UM_PlayerScript playerScript;
    public int playerFacingDir = 1;
    public bool playerIsJumping;
    public bool playerIsAttacking;

    [Header("Interactive Variable")]
    public bool canSwing = true; //for this case, make it specific for each summon as a condition
    public bool isSwinging = false;
    public GameObject swingArm;
    public bool canFollow;

    [Header("Scripts")]
    protected Controller2DScript controller;
    protected AttackDetails attackDetails;
    public CameraFollowScript cam;
    public SwingArmSummonScript swingArmScript;
    public FieldOfView fov;

    [Header("Sounds")]
    public AudioSource punch0SFX;
    public AudioSource poofSFX;

    void Start()
    {
        moveSpeed = baseSpeed;
        controller = GetComponent<Controller2DScript>();
        anim = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update()
    {
        controller.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;

        //Player Variables
        playerFacingDir = playerScript.facingDir;
        playerIsJumping = playerScript.isJumping;
        playerJumpReleased = playerScript.isJumpReleased;
        playerIsAttacking = playerScript.isAttacking;
        //if (!playerIsAttacking) //wait for player to finish their attack animation at the very least, update to include condition if Summon's attack is longer
        //isAttacking = false;

        if (isSwinging)
        {
            velocity.y = 0;
            anim.SetBool("swing", true);
            transform.position = swingArm.transform.position;
        }

        if (controller.collisions.slidingDownMaxSlope)
        {
            velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
        }
        if (controller.collisions.below)
        {
            velocity.y = 0;
            anim.SetBool("isGrounded", true);
            anim.SetBool("land", true);
            isJumpReleased = false;
            isJumping = false;
            if (isDead)
                velocity.x = 0;
        }
        else if (controller.collisions.above)
        {
            velocity.y = 0;
        }
        else if(!controller.collisions.below)
        {
            anim.SetBool("isGrounded", false);
            anim.SetBool("land", false);
        }
        /* short hop does not work, is not desync with player causing the most minimal jump every time if any
        if (!controller.collisions.below && isJumpReleased && delayTimeJump <= 0)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        */

        //Attack-------------------------------------------------------------------------------------------------------------
        if (attackBuffer > 0)
        {
            FlipSprite();
            isAttacking = true;
            attackBuffer -= Time.deltaTime;
            anim.SetBool("attack", true);
            punch0SFX.pitch = Random.Range(.8f, 1.1f);
            punch0SFX.Play();
            velocity.y = 0;
        }
        else if (attackBuffer <= 0)
            anim.SetBool("attack", false);

        if (hitboxHolder.activeSelf == true)
        {
            foreach (GameObject hitbox in hitboxes)
            {
                if (hitbox.activeSelf == true)
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

                        if (airDownAttack)
                        {
                            cam.airDownAttack = true;
                        }

                        collider.transform.SendMessage("Damage", attackDetails); //can send knockback and such
                        oldDetectedObjects.Add(collider);
                        //need to add self knockback when hitting an enemy, just once recoil
                    }
                }
            }
        }
        if (hitboxHolder.activeSelf == false)
        {
            oldDetectedObjects.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------
        //Damaged-----------------------------------------------------------------------------------------------------------------
        if (isDamagedTime > 0)
        {
            isDamagedTime -= Time.deltaTime;
        }
        else if (isDamaged && isDamagedTime <= 0)
        {
            velocity.x = 0;
            anim.SetBool("hit", false);
            iFrameTime = setIFrameTime;
            isDamaged = false;
        }
        if (iFrameTime > 0)
        {
            anim.SetBool("iframe", true);
            iFrameTime -= Time.deltaTime;
            gameObject.layer = 0; // default
        }
        else
        {
            anim.SetBool("iframe", false);
            if(gameObject.layer != 14)
                gameObject.layer = 13; // Summon
        }
        if(!summonGO[sumIndex].activeSelf)
        {
            gameObject.layer = 0; // default
        }
        //-----------------------------------------------------------------------------------------------------------
        //Delay System ----------------------------------------------------------------------------------------------
        if (canFollow)
        {
            if (delayTimeWalk > 0)
            {
                delayTimeWalk -= Time.deltaTime;
                if (delayTimeWalk <= 0)
                {
                    isActive = true;
                    FlipSprite();
                }
            }
            if (delayTimeJump > 0)
            {
                delayTimeJump -= Time.deltaTime;
                if (delayTimeJump <= 0)
                {
                    FollowY();
                }
            }


            if (!actionLock)
            {
                if (delayTimeWalk <= 0 && playerScript.directionalInput.x != 0)
                {
                    delayTimeWalk = setDelayTimeWalk;
                }
                if (delayTimeJump <= 0 && playerIsJumping)
                {
                    delayTimeJump = setDelayTimeJump;
                }
                if (playerJumpReleased)
                {
                    isJumpReleased = true;
                }
            }

            if (isActive && !actionLock)
            {
                FollowX();
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        //Teleport to Player ------------------------------------------------------------------------------------------
        //TODO: add smoke cloud on teleport
        if (!actionLock)
        {
            if ((Mathf.Abs(transform.position.x - trackingPos.position.x) > teleportDistance / 1.5f) || (Mathf.Abs(transform.position.y - trackingPos.position.y) > teleportDistance))
            {
                anim.SetTrigger("teleport");
                //PROBLEM: when teleporting, need to make sure ceiling cannot jam Summon
                transform.position = new Vector2(playerPos.position.x, playerPos.position.y + 2);
                velocity.y = playerScript.velocity.y;
            }
            if (isAttacking)
            {
                wallCheck.transform.localRotation = Quaternion.Euler(0, 0, 135 * playerFacingDir);
                if(CheckInWall())
                    transform.position = new Vector2(playerPos.position.x - (1 * playerFacingDir), playerPos.position.y - 0.55f);
                else
                    transform.position = new Vector2(playerPos.position.x + (1 * playerFacingDir), playerPos.position.y - 0.55f); //number for height difference in Y
                isAttacking = false;
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        //Dead --------------------------------------------------------------------------------------------------------
        /*
        if (deadTime > 0)
        {
            deadTime -= Time.deltaTime;
        }
        if (deadTime <= 0 && isDead)
        {
            isDead = false;
            actionLock = false;
            anim.SetBool("dead", false);
            gameObject.SetActive(false);
        }*/
        //-------------------------------------------------------------------------------------------------------------
        //Unsummon Timer --------------------------------------------------------------------------------------------------------
        if(canUnsummon)
        {
            if (unsummonTimer > 0)
                unsummonTimer -= Time.deltaTime;
            if (unsummonTimer <= 0 && !isDead)
                Unsummon();
        }
        //-------------------------------------------------------------------------------------------------------------
        if (!isJumping)
        velocity.y -= 0.1f; //keeps the controller.collisions.bottom active

        if (playerScript.currentHealth <= 0)
            actionLock = true;
    }

    private void FlipSprite()
    {
        //if (!isDamaged && !isAttacking)
        if (playerFacingDir == -1)
        {
            theSR.flipX = true;
        }
        else if (playerFacingDir == 1)
        {
            theSR.flipX = false;
        }
    }

    void FollowX()
    {
        //Walking ---------------------------------------------------------------------------------------------------
        if (Mathf.Abs(transform.position.x - trackingPos.position.x) > maxSpeed)
            moveSpeed = maxSpeed;
        else
            moveSpeed = Mathf.Abs(transform.position.x - trackingPos.position.x) + baseSpeed;
        float targetVelocityX = moveSpeed * playerFacingDir; //need to check for player facingdir
        //TODO: factor in facing dir (w/ turning around), delay
        if ((playerFacingDir == 1 && transform.position.x < trackingPos.position.x) || (playerFacingDir == -1 && transform.position.x > trackingPos.position.x))
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }
        else
        {
            velocity.x = 0;
            isActive = false;
        }
        //-------------------------------------------------------------------------------------------------------------

        if ((controller.collisions.left || controller.collisions.right) && controller.collisions.below) //Summon will jump if pressed against wall
        {
            velocity.y = maxJumpVelocity;
            isJumping = true;
        }
    }

    void FollowY()
    {
        //Jumping -----------------------------------------------------------------------------------------------------
        //TODO: short hop with player, follow their jump height
        if (playerIsJumping && controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
            isJumping = true;
        }
        //------------------------------------------------------------------------------------------------------------
    }

    public virtual void Damage(AttackDetails attackDetails)
    {
        if(summonGO[sumIndex].activeSelf)
        {
            if (!isDamaged && iFrameTime <= 0 && !isDead)
            {
                hearts[currentHealth-1].gameObject.GetComponent<Animator>().SetTrigger("hit");
                currentHealth -= attackDetails.damageAmount;
                if (currentHealth <= 0f)
                {
                    actionLock = true;
                    //death animation, and set inactive
                    anim.SetBool("dead", true);
                    gameObject.layer = 14; //Dead
                    //deadTime = setDeadTime;
                    isDead = true;
                    StartCoroutine(Dead());
                }
                else
                {
                    anim.SetBool("hit", true);
                    //hitStop.Stop(hitstopDamage);
                    isDamaged = true;
                    isDamagedTime = setIsDamagedTime;
                }

                velocity.x = damageKnockback.x * -Mathf.Sign(attackDetails.position.x - transform.position.x); //change to opposite side of enemy's attack direction
                velocity.y = damageKnockback.y;
            }
        }
    }

    public void SetAttackActive(int num)
    {
        bool isActive = (num == 1) ? true : false;
        hitboxHolder.SetActive(isActive);
        if (!isActive)
        {
            foreach (GameObject hitbox in hitboxes) //turn off all individual hitboxes
                hitbox.SetActive(false);
        }
    }

    #region HitBoxes Position
    public void SetHitBoxOne(float hitboxX)
    {
        attackKnockback = new Vector2(0f, velocity.y);
        hitboxes[0].SetActive(true);
        hitboxes[0].transform.localPosition = new Vector2(hitboxX * playerFacingDir, hitboxYOne);
        hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(1.72f, 1f); //make more malleable for future proofing
    }

    public void SetHitBoxTwo(float hitboxX)
    {
        attackKnockback = new Vector2(3f, velocity.y);
        hitboxes[1].SetActive(true);
        hitboxes[1].transform.localPosition = new Vector2(hitboxX * playerFacingDir, hitboxYOne);
        hitboxes[1].GetComponent<BoxCollider2D>().size = new Vector2(1.72f, 1f); //make more malleable for future proofing
    }

    public void SetHitBoxThree(float hitboxX)
    {
        attackKnockback = new Vector2(3f, velocity.y);
        hitboxes[2].SetActive(true);
        hitboxes[2].transform.localPosition = new Vector2(hitboxX * playerFacingDir, hitboxYOne);
        hitboxes[2].GetComponent<BoxCollider2D>().size = new Vector2(1.72f, 1f); //make more malleable for future proofing
    }
    #endregion

    public void AnimationFinished(string currentAnimation)
    {
        anim.SetBool(currentAnimation, false);
    }

    public void SummonerSignal(string action)
    {
        if (!actionLock)
        {
            if (action == "attack" && canAttack)
                attackBuffer = setAttackBuffer;
            else if (action == "swinging")
            {
                if (canSwing && !isSwinging)
                {
                    //theSR.color = new Color(0, 0, 0, 0);
                    //turn on arm to do swinging
                    transform.position = playerPos.position;
                    anim.SetBool("swingStart", true);
                    swingArm.SetActive(true);
                    //swingArmScript.isSwinging = true;
                    isSwinging = true;
                    actionLock = true;
                }
                else if (canSwing && isSwinging)
                {
                    theSR.color = new Color(1, 1, 1, 1);
                    anim.SetBool("swing", false);
                    swingArm.SetActive(false);
                    isSwinging = false;
                    velocity.y = maxJumpVelocity;
                }
            }
        }
    }

    public void Summon(string sumName)
    {
        if (sumName == "dogBower")
            sumIndex = 0;
        else if (sumName == "grinnock")
            sumIndex = 1;

        SD = SDList[sumIndex];
        currentSummon = true;
        SummonData();
    }

    public void SummonData()
    {
        anim = summonGO[sumIndex].GetComponent<Animator>();
        theSR = summonGO[sumIndex].GetComponent<SpriteRenderer>();

        moveSpeed = SD.moveSpeed;
        baseSpeed = SD.baseSpeed;
        maxSpeed = SD.maxSpeed;

        maxJumpHeight = SD.maxJumpHeight;
        minJumpHeight = SD.minJumpHeight;
        timeToJumpApex = SD.timeToJumpApex;
        accelerationTimeAirborne = SD.accelerationTimeAirborne;

        BC.size = new Vector2(SD.sizeX, SD.sizeY);
        BC.offset = new Vector2(SD.offsetX, SD.offsetY);

        maxHealth = SD.maxHealth;
        currentHealth = maxHealth;
        summonName = SD.summonName;
        setDelayTimeWalk = SD.setDelayTimeWalk;
        setDelayTimeJump = SD.setDelayTimeJump;
        teleportDistance = SD.teleportDistance;
        unsummonTimer = SD.unsummonTime;
        if (unsummonTimer > 0)
            canUnsummon = true;

        canAttack = SD.canAttack;
        canSwing = SD.canSwing;
        canFollow = SD.canFollow;

        fov.viewRadiusMax = SD.sizeX;

        //Summon
        currentHealth = maxHealth;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].color = new Color(0, 0, 1);
                hearts[i].gameObject.SetActive(true);
            }

        }
        actionLock = false;
        playerFacingDir = playerScript.facingDir;
        FlipSprite();

        transform.position = new Vector2(playerPos.position.x + (((fov.viewRadiusMax / 2) + 1) * playerFacingDir), playerPos.position.y);
        wallCheck.transform.localRotation = Quaternion.Euler(0, 0, 135 * playerFacingDir);
        StartCoroutine(WallCheckBuffer());
    }

    public bool CheckInWall()
    {
        return fov.FoundTargetMax();
    }

    IEnumerator WallCheckBuffer()
    {
        yield return new WaitForSeconds(0.1f);
        if (CheckInWall())
            transform.position = new Vector2(playerPos.position.x + (((fov.viewRadiusMax / 2) + 1) * -playerFacingDir), playerPos.position.y);
        smoke.SetActive(true);
    }

    public void SmokeTrigger()
    {
        poofSFX.pitch = Random.Range(.95f, 1.15f);
        poofSFX.Play();
        if (!isDead)
        {
            summonGO[sumIndex].SetActive(true);
            gameObject.layer = 13; // Summon
        }
        else
        {
            summonGO[sumIndex].SetActive(false);
            for (int i = 0; i < maxHealth; i++)
            {
                hearts[i].gameObject.SetActive(false);
            }
            ResetBoolean();
            StartCoroutine(SummonBuffer());
        }
    }

    public void Unsummon()
    {
        isDead = true;
        smoke.SetActive(true);
        for (int i = 0; i < maxHealth; i++)
        {
            hearts[i].gameObject.GetComponent<Animator>().SetTrigger("dead");
        }
        //summonGO[sumIndex].SetActive(false);
        //animated unsummon
        //deactive the current GO
    }

    public void PlayerDead()
    {
        if(summonGO[sumIndex].activeSelf)
        {
            currentHealth = 0;
            actionLock = true;
            //death animation, and set inactive
            anim.SetBool("dead", true);
            gameObject.layer = 14; //Dead
                                   //deadTime = setDeadTime;
            isDead = true;
            StartCoroutine(Dead());
        }
        /*
        summonGO[sumIndex].SetActive(false);
        for (int i = 0; i < maxHealth; i++)
        {
            hearts[i].gameObject.SetActive(false);
        }
        ResetBoolean();
        StartCoroutine(SummonBuffer());
        */
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(1);
        smoke.SetActive(true);
        for (int i = 0; i < maxHealth; i++)
        {
            hearts[i].gameObject.GetComponent<Animator>().SetTrigger("dead");
        }
        //summonGO[sumIndex].SetActive(false);
    }

    IEnumerator SummonBuffer()
    {
        yield return new WaitForSeconds(.5f);
        currentSummon = false;
    }

    public void ResetBoolean()
    {
        isDamaged = false;
        isDead = false;
        canAttack = false;
        canFollow = false;
        canUnsummon = false;
        attackBuffer = 0;
        iFrameTime = 0f;
        //start low ego bar for player
        if(sumIndex != 0 && playerScript.currentHealth > 0)
            playerScript.LowEgoSystem();
    }
}