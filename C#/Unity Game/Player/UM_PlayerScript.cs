using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2DScript))]
public class UM_PlayerScript : MonoBehaviour
{
    [Header("Move Variable")]
    public float accelerationTimeGrounded = 0.1f;
    public float moveSpeed = 8;
    public float accelerationSpeed = 0;
    public float topSpeed = 15;
    public float rollSpeed = 25;
    public bool isRunning = false;
    public bool isRolling = false;
    private float rollTime;
    public float setRollTime = 0.75f;
    private float velocityXSmoothing = 0;

    [Header("Jump Variable")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    public float setJumpBuffer = 0.2f;
    private float jumpBuffer;
    private bool canJumpBuffer;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private int amountOfJumps;
    public int setAmountOfJumps = 1;
    public bool isJumping;
    public bool isJumpReleased;

    [Header("Wall Jump Variable")]
    public Vector2 wallLeap;
    private bool wallJump;
    private bool wallAir;

    [Header("Wall Slide Variable")]
    public float wallSlideSpeedMax = 3;
    public float setWallStickTime = 0.5f;
    public float wallUnstickTime;
    protected bool wallSlide;
    private int wallDirX;
    public Transform wallSlideChecker;

    [Header("In Air Variable")]
    public float accelerationTimeAirborne = 0.2f;
    public float setCoyoteTime = 0.2f;
    private float coyoteTime;
    public bool canCoyoteTime;
    public float setBigFallTime = 0.5f;
    private float bigFallTime;
    private bool bigFall;
    public float setRecoverBigFallTime = 1;
    public float recoverBigFallTime;

    [Header("Attack Variable")]
    public GameObject hitboxHolder;
    public GameObject[] hitboxes;
    public LayerMask whatIsDamagable;
    public Vector2 attackKnockback;
    public bool isAttacking = false;
    public bool airDownAttack = false;
    private List<Collider2D> oldDetectedObjects = new List<Collider2D>();

    [Header("Hit Variable")]
    public float setIsDamagedTime = 0.5f;
    public float setIFrameTime = 1f;
    private float isDamagedTime;
    private float iFrameTime = 0f;
    public bool isDamaged;
    public Vector2 damageKnockback = new Vector2(9, 13);
    public float hitstopDamage = 0.25f;
    public float hitstopAttack = 0.1f;

    [Header("Interactive Variable")]
    public GameObject lockOn;
    public GameObject lockOnCheck;
    public string lockOnTag;
    public bool isSwinging = false;
    public float swingXPos = 0;
    public float swingYPos = 0;
    private float smoothSwing = 0;
    public float swingTime = 0.5f;

    [Header("Scripts")]
    public CameraFollowScript cam;
    public HitStop hitStop;
    protected Controller2DScript controller;
    protected AttackDetails attackDetails;
    public FieldOfView fov;
    [SerializeField] private DialogueUI DialogueUIScript;

    [Header("Other Variables")]
    public int currentHealth;
    public int maxHealth = 3;
    public int facingDir = 1;
    public float gravity;
    private bool actionLock = false;
    public bool isDialogue;
    public bool cinematicStop;
    public Vector3 velocity;
    public Vector2 directionalInput;
    public LayerMask whatIsGround;
    public LayerMask whatIsWeakSpot;
    public Transform trackerPos;
    public Transform savePoint;
    public Transform ledgePoint;

    [Header("UI")]
    public Image[] hearts;
    public GameObject lowEgoBar;
    public GameObject lowEgoUI;
    public HealthBarScript lowEgoScript;
    public float lowEgoFillBar;
    public float lowEgoTime = 5;
    public GameObject blackScreen;

    [Header("Components")]
    public SpriteRenderer theSR;
    protected Animator anim;
    public PlayerTrigger2DChecker triggerScript;

    [Header("Summon")]
    public bool isSummoning;
    public SummonScript summonScript;
    public GameObject summonGO;

    [Header("Sound")]
    public AudioSource music1;
    public AudioSource music2;
    public AudioSource music3;
    public float musicTime = 0.15f;
    public float musicWaitTime = .5f;
    public bool restoreMusic;
    public float pitchMod;
    private float musicSmoothing = 0;
    private float musicSmoothing2 = 0;
    private float musicSmoothing3 = 0;
    private float musicSmoothing4 = 0;
    public bool musicDamp = false;
    public bool muteMusic;
    public bool muteSoundCredit;
    public bool lowerWorldVolume = false;
    public bool restoreSound;
    private float musicSmoothing5 = 0;
    private float musicSmoothing6 = 0;
    private float musicSmoothing7 = 0;

    [Header("Cinema Values")]
    public bool vsDSD = false;
    public bool vsDaddy;
    public string currentAnim;
    public DarkShippieDuesScript DSDScript;
    public DaddyScript daddyScript;
    public GameObject[] smallTextGO;
    public GameObject wallCheck;
    public FieldOfView wallCheckFOW;
    public Animator LumbyAnim;
    public DialogueData ending1;
    public Transform LumbyLoc;
    public GameObject LumbyGO;

    public GameObject whiteScreen;
    public EnemySpawner ESScript;
    public GameObject lockOnHolder;
    public Animator titleScreenAnim;

    public bool isStarting = true;
    public DialogueData introDD;
    public LumbyScript lumbyScript;

    public AudioSource lockOnSFX;
    public bool firstStart = true;
    public GameObject triggerColliderGO;
    public bool restoremusicforcredit;
    public bool startRestart;

    public bool openingChest;

    void Start()
    {
        controller = GetComponent<Controller2DScript>();
        anim = GetComponent<Animator>();
        hitStop = GetComponent<HitStop>();
        fov = GetComponentInChildren<FieldOfView>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        music2.Play();
        //INTRO
        cam.stopFollowingPlayer = true;
        cam.verticalOffset = 25;
        cinematicStop = true;
        Spawn();
        //music1.Play(); //temp, need to start when level starts, maybe a better sound control script.
    }

    public void DialogueAnimation(string nextAnim)
    {
        if (nextAnim != "")
        {
            if (nextAnim == "closeMouth")
            {
                anim.SetBool("openMouth", false);
                currentAnim = "";
            }
            else if (nextAnim != currentAnim)
            {
                if (currentAnim != "")
                    anim.SetBool(currentAnim, false);
                anim.SetBool(nextAnim, true);
                currentAnim = nextAnim;
            }
        }
        else if (currentAnim != "")
        {
            anim.SetBool(currentAnim, false);
            currentAnim = "";
        }
    }

    public void PlayMusic3()
    {
        music3.Play();
    }

    public void SmallTextAnimation(string textBubble)
    {
        if (textBubble == "what")
        {
            smallTextGO[0].transform.localPosition = new Vector2(smallTextGO[0].transform.localPosition.x * facingDir, smallTextGO[0].transform.localPosition.y);
            if (facingDir == -1)
            {
                smallTextGO[0].GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (facingDir == 1)
            {
                smallTextGO[0].GetComponent<SpriteRenderer>().flipX = false;
            }
            anim.SetTrigger("whatST");
        }
        else if (textBubble == "good")
        {
            smallTextGO[1].transform.localPosition = new Vector2(smallTextGO[1].transform.localPosition.x * facingDir, smallTextGO[1].transform.localPosition.y);
            if (facingDir == -1)
            {
                smallTextGO[1].GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (facingDir == 1)
            {
                smallTextGO[1].GetComponent<SpriteRenderer>().flipX = false;
            }
            anim.SetTrigger("goodST");
            DialogueAnimation("idle");
        }
        else if (textBubble == "name")
        {
            smallTextGO[1].transform.localPosition = new Vector2(smallTextGO[1].transform.localPosition.x * facingDir, smallTextGO[1].transform.localPosition.y);
            if (facingDir == -1)
            {
                smallTextGO[1].GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (facingDir == 1)
            {
                smallTextGO[1].GetComponent<SpriteRenderer>().flipX = false;
            }
            anim.SetTrigger("nameST");
            DialogueAnimation("idle");
        }
    }

    void Update()
    {
        CalculateVelocity();

        if(openingChest)
        {
            cinematicStop = true;
        }
        if (isSwinging)
            Swinging();

        //HandleWallSliding();
        if (isDialogue || cinematicStop)
        {
            directionalInput = new Vector2(0, 0);
            velocity.x = 0;
        }
        else if (!cinematicStop)
        {
            anim.SetBool("isDialogue", false);
            DialogueAnimation("");
        }
        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (isSwinging)
        {
            lockOn.SetActive(false);
        }

        if (controller.collisions.below)
        {
            anim.SetBool("isGrounded", true);
            wallJump = false;
            wallAir = false;
            wallUnstickTime = 0;
            coyoteTime = setCoyoteTime;
            canCoyoteTime = true;
            amountOfJumps = setAmountOfJumps;
            isJumping = false;
            isJumpReleased = false;
            cam.airDownAttack = false;

            //BigFall();

            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else if (controller.collisions.below)
            {
                velocity.y = 0;
                if (isSummoning)
                    velocity.x = 0;
            }
            /*
            if (directionalInput.y == -1 && !isRolling)
            {
                velocity.x = 0;
                //anim.SetBool("crouch", true);
            }
            else if (directionalInput.y != -1 || directionalInput.x != 0)
            {
                //anim.SetBool("crouch", false);
            }*/
        }
        else if (controller.collisions.above)
        {
            velocity.y = 0;
        }
        else if (!controller.collisions.below)
        {
            //anim.SetBool("crouch", false);
            anim.SetBool("isGrounded", false);
            if (coyoteTime > 0)
            {
                coyoteTime -= Time.deltaTime;
            }
        }

        if (coyoteTime <= 0 && canCoyoteTime && !isJumping)
        {
            canCoyoteTime = false;
            amountOfJumps--;
        }

        if (jumpBuffer > 0)
        {
            jumpBuffer -= Time.deltaTime;
            OnJumpInputDown();

        }
        else if (jumpBuffer <= 0)
        {
            canJumpBuffer = true;
        }
        //Attack-----------------------------------------------------------------------
        if (isAttacking || airDownAttack)
        {
            velocity.x = 0.1f * facingDir;
            isAttacking = false;
            airDownAttack = false;
        }

        //-----------------------------------------------------------------------------
        //Damaged----------------------------------------------------------------------
        if (isDamagedTime > 0)
        {
            isDamagedTime -= Time.deltaTime;
        }
        else if (isDamaged && isDamagedTime <= 0)
        {
            anim.SetBool("hit", false);
            iFrameTime = setIFrameTime;
            isDamaged = false;
            ActionUnlock();
        }
        if (iFrameTime > 0)
        {
            anim.SetBool("iframe", true);
            iFrameTime -= Time.deltaTime;
        }
        else
        {
            if (!isDamaged && gameObject.layer != 14)
                gameObject.layer = 9; // Player
            anim.SetBool("iframe", false);
        }
        //-----------------------------------------------------------------------------
        //Rolling ---------------------------------------------------------------------
        if (isRolling)
        {
            if (rollTime > 0)
            {
                rollTime -= Time.deltaTime;
            }
            if (rollTime <= 0)
            {
                isRolling = false;
            }
        }
        //-----------------------------------------------------------------------------

        //anim.SetInteger("yDirection", (int)directionalInput.y);
        anim.SetFloat("yVelocity", Mathf.Abs(velocity.y));

        if (!CheckInteractiveInRange() && lockOn.activeSelf)
        {
            //if lock on is active, but no longer in range, disable lock-on
            lockOn.SetActive(false);
        }

        FlipSprite();

        //UI -----------------------------------------------------------------------
        if (lowEgoUI.activeSelf)
        {
            if (lowEgoFillBar > 0)
                lowEgoFillBar -= Time.deltaTime;
            lowEgoScript.SetHealth(lowEgoFillBar);
        }
        if (lowEgoUI.activeSelf && lowEgoFillBar <= 0)
        {
            lowEgoUI.SetActive(false);
        }
        //--------------------------------------------------------------------------
        //Music ---------------------------------------------------------------------
        if(!restoremusicforcredit)
        {
            if(music1.isPlaying)
            {
                if (musicDamp && !muteSoundCredit && !restoremusicforcredit && !lowerWorldVolume)
                {
                    //music1.pitch += musicTime * Time.deltaTime;
                    //music1.volume = .05f;
                    music1.volume = Mathf.SmoothDamp(music1.volume, .14f, ref musicSmoothing, .05f);
                    //music1.pitch += .05f * Time.deltaTime;
                }
                if (music1.volume <= 0.15f && musicDamp && !muteSoundCredit)
                    musicDamp = false;
            }
            if (muteMusic)
            {
                music3.volume = Mathf.SmoothDamp(music3.volume, 0, ref musicSmoothing7, 1f);
            }
            else if (music3.isPlaying && !muteMusic)
            {
                if (musicDamp && muteSoundCredit && !restoremusicforcredit && !lowerWorldVolume)
                {
                    music3.volume = Mathf.SmoothDamp(music3.volume, .14f, ref musicSmoothing5, .05f);
                }
                if(music3.volume <= 0.15f && musicDamp && muteSoundCredit)
                    musicDamp = false;
                if (music3.volume <= 0.45 && !musicDamp && muteSoundCredit && !restoremusicforcredit && !lowerWorldVolume && !muteMusic)
                {
                    music3.volume = Mathf.SmoothDamp(music3.volume, 0.46f, ref musicSmoothing5, musicTime);
                }
            }
            if (music1.volume <= .35f && !restoremusicforcredit && !lowerWorldVolume && !muteSoundCredit && !musicDamp)
            {
                music1.volume = Mathf.SmoothDamp(music1.volume, .36f, ref musicSmoothing, musicTime);
                //restoreMusic = false;
            }
            else if (!lowerWorldVolume && !muteSoundCredit && !musicDamp)
                music1.volume = Mathf.SmoothDamp(music1.volume, 0.35f, ref musicSmoothing2, musicTime);
            else if (!lowerWorldVolume && muteSoundCredit && !musicDamp && !muteMusic)
                music3.volume = Mathf.SmoothDamp(music3.volume, 0.45f, ref musicSmoothing2, musicTime);
            if (lowerWorldVolume && !restoremusicforcredit)
            {
                    music1.volume = Mathf.SmoothDamp(music1.volume, 0, ref musicSmoothing2, musicTime);
                    music3.volume = Mathf.SmoothDamp(music3.volume, 0, ref musicSmoothing6, musicTime);
            }
            if (muteSoundCredit && !restoremusicforcredit)
            {
                music1.volume = Mathf.SmoothDamp(music1.volume, 0, ref musicSmoothing3, 1f);
            }
        }
        else if (restoremusicforcredit)
        {
            music1.volume = Mathf.SmoothDamp(music1.volume, 0.35f, ref musicSmoothing4, 1.5f);
        }
    }
    //---------------------------------------------------------------------------

    public void Spawn()
    {
        //call this function when the player is activate again when dead, or falls off a cliff
        //the two are basically the same since the sceen will go back, and the player will appear at last save point
        //the save points for falling is different than death, will check the condition and use the proper port.
        //this will modify the transform position of the player.
        if(!isStarting)
        {
            triggerColliderGO.SetActive(true);
            cinematicStop = false;
            actionLock = false;
            gameObject.layer = 9; //Player
            if (currentHealth <= 0) //account for fall death preemptively
            {
                ESScript.Respawn();
                currentHealth = maxHealth;
                transform.position = savePoint.position;
                for (int i = 0; i < hearts.Length; i++)
                {
                    if (i < currentHealth)
                    {
                        hearts[i].color = new Color(1, 0, 0);
                        hearts[i].gameObject.SetActive(false); // refresh
                        hearts[i].gameObject.SetActive(true);
                    }

                }
            }
            else
            {
                //hearts[currentHealth - 1].gameObject.GetComponent<Animator>().SetTrigger("hit");
                //currentHealth = -1;
                transform.position = ledgePoint.position;
            }
            vsDSD = false;
            vsDaddy = false;
            cam.stopFollowingPlayer = false;
            cam.DSDFocus = false;
            //muteSoundCredit = false;
            //set the collision mask back to ground(8), moving ground(10), interactive(15)
            controller.collisionMask = (1 << 8) | (1 << 10) | (1 << 15);

        }
    }

    public void OnJumpInputDown()
    {
        if (!OnAllFaceInputDown())
        {
            //anim.SetBool("move", false);
            isJumpReleased = false;
            if (wallSlide && !isDamaged && !isRolling)
            {
                //amountOfJumps--;
                wallJump = true;
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
                wallUnstickTime = 0;
            }
            else if (!isDamaged && !isRolling && (controller.collisions.below || coyoteTime > 0f || amountOfJumps > 0))
            {
                amountOfJumps--;
                isJumping = true;
                jumpBuffer = 0f;
                if (controller.collisions.slidingDownMaxSlope)
                {
                    if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                    { // not jumping against max slope
                        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y * 1.5f;
                        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    }
                }
                else
                {
                    velocity.y = maxJumpVelocity;
                    coyoteTime = 0f;
                }
            }
            else if (!controller.collisions.below && canJumpBuffer)
            {
                jumpBuffer = setJumpBuffer;
                canJumpBuffer = false;
            }
        }
    }

    public void OnJumpInputUp()
    {
        isJumpReleased = true;
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void HandleWallSliding()
    {
        anim.SetBool("wallSlide", wallSlide);
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSlide = false;
        if (controller.WallSlidableSlopeAngle() && CanWallSlide())
        {
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && (directionalInput.x == wallDirX || wallAir)) // replace wallAir with || timeToWallUnstick > 0 if you want to have analog movement determine wall cling between jump
            {
                wallSlide = true;
                wallAir = true;
                wallJump = false;
                bigFallTime = setBigFallTime;
                //BigFall();

                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }
            }
            /*
            if (wallUnstickTime > 0 && wallSlide)
            {
                //velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x == -wallDirX || directionalInput.y == -1)
                {
                    wallUnstickTime -= Time.deltaTime;
                }
                else if (directionalInput.x == 0 || directionalInput.x == wallDirX) //NOTE: parameters might not be needed
                {
                    wallUnstickTime = setWallStickTime;
                }
            }
            else if (wallUnstickTime == 0)
            {
                wallUnstickTime = setWallStickTime;
            }
            */
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        /*
        //Acceleration from running ----------------------------------------------------------------------------------------------
        if (accelerationSpeed > 0 && !isRunning)
        {
            accelerationSpeed -= 1; // need to fix pretty decrementing system
        }
        else if (accelerationSpeed < 0)
        {
            accelerationSpeed = 0;
        }
        //------------------------------------------------------------------------------------------------------------------------

        if (isRunning && targetVelocityX != 0)
        {
            
            //TODO: problem - acceleration speed will not reset on interruptions
            if (accelerationSpeed < topSpeed - Mathf.Abs(targetVelocityX))
            {
                accelerationSpeed += 10 * Time.deltaTime;
                //accelerationSpeed += Mathf.SmoothDamp(0, topSpeed, ref velocityXSmoothing, 0.5f);
                Debug.Log(accelerationSpeed);
                velocity.x = targetVelocityX + accelerationSpeed * directionalInput.x;
                 //(controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne
            }
            else
            {
                velocity.x = topSpeed * directionalInput.x;
            }
        }
        */
        if (!isDamaged && !isRolling && (!actionLock || airDownAttack))
        {
            if (velocity.y <= 0 && !isSwinging)
            {
                if (!controller.collisions.left || !controller.collisions.right)
                {
                    wallJump = false;
                }
                bigFallTime -= Time.deltaTime;
            }
            else if (velocity.y > 0)
            {
                bigFallTime = setBigFallTime;
            }
            if (bigFallTime <= 0)
            {
                bigFall = true;
                cam.bigFall = true;
            }

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        }
        if (controller.collisions.below && directionalInput.x != 0 && !cinematicStop)
        {
            anim.SetBool("move", true);
        }
        else
        {
            anim.SetBool("move", false);
        }
        velocity.y += gravity * Time.deltaTime;
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public bool CanWallSlide()
    {
        return Physics2D.Raycast(wallSlideChecker.transform.position, transform.right * controller.collisions.faceDir, 0.15f, whatIsGround);
    }

    public void SetAttackActive(int num)
    {
        bool isActive = (num == 1) ? true : false;
        hitboxHolder.SetActive(isActive);
    }

    public virtual void Damage(AttackDetails attackDetails)
    {
        if (!isDamaged && iFrameTime <= 0 && currentHealth > 0)
        {
            gameObject.layer = 18; // player damaged
            hearts[currentHealth - 1].gameObject.GetComponent<Animator>().SetTrigger("hit");
            //music1.volume = 0.35f;
            musicDamp = true;
            //music1.pitch = pitchMod; // music pitch ------------------------
            //StartCoroutine(MusicRestore());

            if (isSummoning)
            {
                SummonFinished("dogBower");
            }

            currentHealth -= attackDetails.damageAmount;
            if (currentHealth <= 0)
            {
                //death animation, and set inactive
                //anim.SetBool("dead", true);
                gameObject.layer = 14; //Dead
                                       //deadTime = setDeadTime;
                                       //isDead = true;
                anim.SetBool("hit", true);
                hitStop.Stop(hitstopDamage);
                controller.collisionMask = 0; // fall through ground
                actionLock = true;
                cinematicStop = true;
                cam.stopFollowingPlayer = true;
                triggerColliderGO.SetActive(false);
                if (vsDSD)
                {
                    summonScript.PlayerDead();
                }
                else
                {
                    StartCoroutine(Dead());
                }
            }
            else
            {
                if (!summonScript.currentSummon)
                    LowEgoSystem();

                anim.SetBool("hit", true);
                hitStop.Stop(hitstopDamage);
                isDamaged = true;
                isDamagedTime = setIsDamagedTime;
                actionLock = true;
            }

            velocity.x = damageKnockback.x * -Mathf.Sign(attackDetails.position.x - transform.position.x); //change to opposite side of enemy's attack direction
            velocity.y = damageKnockback.y;
        }
    }

    private void FlipSprite()
    {
        if (!isDamaged && !isAttacking && !isSwinging)
        {
            if (facingDir == -1)
            {
                theSR.flipX = true;
            }
            else if (facingDir == 1)
            {
                theSR.flipX = false;
            }
            if (directionalInput.x != 0)
            {
                facingDir = controller.collisions.faceDir;
            }
            wallSlideChecker.localPosition = new Vector2(Mathf.Abs(wallSlideChecker.localPosition.x) * controller.collisions.faceDir, wallSlideChecker.localPosition.y);
            trackerPos.localPosition = new Vector2(Mathf.Abs(trackerPos.localPosition.x) * -controller.collisions.faceDir, trackerPos.localPosition.y);
            lockOnCheck.transform.localRotation = Quaternion.Euler(0, 0, 155 * controller.collisions.faceDir);
        }
    }

    private void BigFall()
    {
        //TODO: FIX THIS CODE for big fall -- seems clunky
        if (!bigFall)
        {
            cam.bigFall = false;
            cam.recoveringFromBigFall = false;
        }
        else
        {
            cam.bigFall = false;
            cam.recoveringFromBigFall = true;
            recoverBigFallTime -= Time.deltaTime;
        }
        if (recoverBigFallTime <= 0)
        {
            cam.bigFall = false;
            cam.recoveringFromBigFall = false;
            bigFall = false;
            recoverBigFallTime = setRecoverBigFallTime;
        }
        bigFallTime = setBigFallTime;
        //
    }

    public void OnLeftInputDown()
    {
        //Before Tome
        //After Tome
        //lock-on

        if (!OnAllFaceInputDown())
        {
            if (CheckInteractiveInRange() && !lockOn.activeSelf)
            {
                //spawn lock on symbol at target location
                lockOnSFX.Play();
                lockOn.SetActive(true);
                lockOnHolder.transform.position = fov.TargetMaxPosition();
                lockOnTag = fov.TargetMaxTagName();
            }
            else
                lockOn.SetActive(false);
        }
    }

    public void OnRightInputDown()
    {
        //this will be use to interact with object, spawning text
        if (!OnAllFaceInputDown())
        {
            //isRunning = true;
            if (triggerScript.IsTriggerInteractive() && !cinematicStop)
            {
                //Might need to change to not be entirely isDialogue, since textbubble may appear a bit later
                isDialogue = true;
                triggerScript.PlayDialogue();
            }
        }
    }

    public void OnRightInputUp()
    {
        isRunning = false;
    }

    public void OnBottomInputDown()
    {
        //Before Tome
        //After Tome
        //Send signal for Summon to attack
        if (!OnAllFaceInputDown())
            SummonAction();
    }

    public void SummonAction()
    {
        //TODO: fix to be more flexible, where different summons can fufill different interactives
        //check if summon = true from summon script else if... continue
        if (!summonScript.currentSummon && !actionLock)
        {
            anim.SetBool("summon", true);
            isSummoning = true;
            actionLock = true;
            if (controller.collisions.below) //attacking movement
            {
                velocity.x = 0f * controller.collisions.faceDir; //change to a varible that differs between attacks?
            }
        }
        else if (lockOn.activeSelf) // interactive?
        {
            if (lockOnTag == "Swing")
            {
                summonScript.SummonerSignal("swinging");
                isSwinging = true;
            }
        }
        else
        {
            if (summonScript.currentSummon && !actionLock && summonScript.currentHealth > 0)
            {
                summonScript.SummonerSignal("attack");
                if (!isAttacking && !isDamaged && !isRolling)
                {
                    isAttacking = true;
                    anim.SetBool("attack", true);
                    actionLock = true;
                }
                if (controller.collisions.below) //attacking movement
                {
                    velocity.x = 0; //change to a varible that differs between attacks?
                }
            }
        }
    }

    public void SummonFinished(string sumName)
    {

        isSummoning = false;
        if (lowEgoUI.activeSelf || sumName == "dogBower")
            summonScript.Summon("dogBower");
        else
            summonScript.Summon("grinnock");
    }

    public void Swinging()
    {
        //Send signal to summonGO, that will in turn turn on an arms to do the movmement
        if (isSwinging)
        {
            //transform.position = summonGO.transform.position;
            anim.SetBool("swing", true);
            swingXPos = Mathf.SmoothDamp(transform.position.x, summonGO.transform.position.x, ref smoothSwing, swingTime * Time.deltaTime);
            swingYPos = Mathf.SmoothDamp(transform.position.y, summonGO.transform.position.y, ref smoothSwing, swingTime * Time.deltaTime);
            transform.position = new Vector2(swingXPos, swingYPos);
            velocity.y = 0;
        }
        else //jump off
        {
            anim.SetBool("swing", false);
            velocity.y = maxJumpVelocity;
            summonScript.actionLock = false;
            summonScript.SummonerSignal("swinging");
        }
        /*
        theRB.simulated = true;
        //player needs to set the connector anchor to the hook location
        cam.isSwinging = true;
        //need to play animation, latch, then do the effects below
        theHJ.autoConfigureConnectedAnchor = false;
        velocity.y = 0;
        //USE DAMP FOR ANCHOR POSITION
        swingXOffset = Mathf.SmoothDamp(swingXOffset, 0, ref smoothSwing, swingAnchorTime * 1.5f);
        swingYOffset = Mathf.SmoothDamp(swingYOffset, 7.2f, ref smoothSwing, swingAnchorTime * 1.5f);
        theHJ.anchor = new Vector2(swingXOffset, swingYOffset);
        theHJ.connectedAnchor = lockOn.transform.position;
        */
    }

    public bool OnAllFaceInputDown() // commands when it's needed that all top, right, bottom, or left button is pressed
    {
        if (isSwinging)
        {
            isSwinging = false;
            Swinging();
            return true;
        }
        else if (isDialogue)
        {
            DialogueUIScript.PlayerInput();
            return true;
        }
        else if (cinematicStop)
        {
            return true;
        }
        return false; //default for no special condition 
    }

    public void AnimationFinished(string currentAnimation)
    {
        anim.SetBool(currentAnimation, false);
    }

    public void ActionUnlock() => actionLock = false;

    public virtual bool CheckInteractiveInRange()
    {
        return fov.FoundTargetMax();
    }

    public void DialogueEnabler(DialogueData DD)
    {
        isDialogue = true;
        anim.SetBool("isDialogue", true);
        DialogueUIScript.OnPlayerEnable(DD);
    }

    public void LowEgoSystem()
    {
        //if no summon, and gets hit, or loses summon low ego bar appears, and disappear when bar is unfilled
        //on call, make the low system appear
        lowEgoUI.SetActive(true);
        lowEgoScript.SetHealth(lowEgoTime);
        lowEgoFillBar = lowEgoTime;
    }

    public void PlayerIsDead()
    {
        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(.5f);
        blackScreen.SetActive(true);
        if(currentHealth <= 0)
        {
            for (int i = 0; i < maxHealth; i++)
            {
                hearts[i].gameObject.GetComponent<Animator>().SetTrigger("dead");
            }
        }
        yield return new WaitForSeconds(1.5f);
        if (vsDSD) //lost to DSD
            DSDScript.Restart();
        if (vsDaddy)
            daddyScript.Restart();
        lowEgoUI.SetActive(false);

        if (summonScript.currentSummon && currentHealth <= 0)
            summonScript.PlayerDead(); //unsummon summon if still alive when dead

        Spawn();
        anim.SetBool("hit", false);
        blackScreen.GetComponent<Animator>().SetBool("fade", true);
        yield return new WaitForSeconds(1);
        blackScreen.SetActive(false);
    }

    IEnumerator MusicRestore()
    {
        yield return new WaitForSeconds(musicWaitTime);
        restoreMusic = true;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(hitboxHolder.transform.position, attackRadius);
        Gizmos.DrawLine(wallSlideChecker.transform.position, wallSlideChecker.transform.position + (Vector3)(Vector2.right * 0.15f));
        //Gizmos.DrawCube (trackerPos.centre, focusAreaSize);
    }

    ///////////////////ENDING SEQUENCE
    public void EndingScenes()
    {
        //center camera, maybe even focus a little
        cam.CurrentCamPos();
        cam.playerFocus = true;
        //unsummon summon if is summon
        if (summonScript.currentSummon)
            summonScript.Unsummon();
        StartCoroutine(EndingScene());
        //check for wall, turn if facing one
        //lumby appear
        //startnext dialogue ending1
    }

    public void ToCredit()
    {
        whiteScreen.SetActive(true);
        whiteScreen.GetComponent<Animator>().speed = 0.35f;
        //muteSoundCredit = true;
        cam.camPanCredit = true;
        StartCoroutine(ThankYou());
    }

    IEnumerator ThankYou()
    {
        yield return new WaitForSeconds(5f);
        titleScreenAnim.SetBool("credit", true);
        yield return new WaitForSeconds(0.25f);
        startRestart = true;
    }

        IEnumerator EndingScene()
    {
        wallCheck.transform.localRotation = Quaternion.Euler(0, 0, 135 * facingDir);
        yield return new WaitForSeconds(0.5f);
        if (wallCheckFOW.FoundTargetMax())
        {
            facingDir *= -1;
        }
        LumbyLoc.localScale = new Vector3(facingDir, 1, 1);
        LumbyGO.SetActive(true);
        LumbyAnim.SetTrigger("appearHat");
        yield return new WaitForSeconds(0.5f);
        DialogueEnabler(ending1);
    }

    public void Pit()
    {
        cam.stopFollowingPlayer = true;
        hearts[currentHealth - 1].gameObject.GetComponent<Animator>().SetTrigger("hit");
        currentHealth -= 1;
        musicDamp = true;
        if (currentHealth >= 1)
            StartCoroutine(Pitfall());
        else
            StartCoroutine(Dead());
    }

    IEnumerator Pitfall()
    {
        yield return new WaitForSeconds(.5f);
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        lowEgoUI.SetActive(true);
        Spawn();
        blackScreen.GetComponent<Animator>().SetBool("fade", true);
        yield return new WaitForSeconds(1);
        blackScreen.SetActive(false);
    }
    //GAMESTART
    public void OnStartInputDown()
    {
        if(firstStart)
        {
            firstStart = false;
            titleScreenAnim.SetBool("fade", true);
            cam.stopFollowingPlayer = false;
            anim.SetBool("isDialogue", true);
            lumbyScript.DialogueAnimation("neutralNoHat");
            StartCoroutine(StartGame());
        }
        if (startRestart)
            SceneManager.LoadScene("Test Scene");

    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(.5f);
        cam.CurrentCamPos();
        cam.camPanIntro = true;
        yield return new WaitForSeconds(1.5f);
        DialogueEnabler(introDD);
    }

    public void DialogueIntroFinished()
    {
        cam.camPanIntro = false;
        music2.Stop();
        lumbyScript.DialogueAnimation("disappear");
    }

    public void StartTheGame()
    {
        isStarting = false;
        music1.Play();
        currentHealth = maxHealth;
        music1.volume = 0.35f;
        cinematicStop = false;
        actionLock = false;
        gameObject.layer = 9; //Player
        controller.collisionMask = (1 << 8) | (1 << 10) | (1 << 15);
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].color = new Color(1, 0, 0);
                hearts[i].gameObject.SetActive(false); // refresh
                hearts[i].gameObject.SetActive(true);
            }

        }
    }

}