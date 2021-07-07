using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Controller2DScript))]
public class DaddyScript : MonoBehaviour
{
    public float teleportDistance;
    public float idleTimer;
    public float setIdleTimer = 1.5f;
    public int facingDir = -1;
    public float jumpVelocity = 12;
    private float Smoothing = 0;
    private float Smoothing2 = 0;
    public bool isJumping;
    public bool jumpSlowDown;
    public float jumpSlowDownTime = 0.15f;
    public Vector3 velocity;
    public Vector2 playerLoc;
    public float ranNum = 0;
    public float baseY;
    public bool isCharging = false;
    public float fallSpeed = 9;
    public GameObject aura;
    public Animator auraAnim;
    public SpriteRenderer auraSR;
    public float attackSpeed = 10;
    public bool hitFloor;
    public float floorTime = 0.25f;
    public bool isIdle;
    public float setChargeTimer = 1f;
    public float chargeTimer;
    public bool isChargeAttack;
    public Vector2 startPos;

    [Header("Attack Variable")]
    public GameObject hitboxHolder;
    public GameObject[] hitboxes;
    private List<Collider2D> oldDetectedObjects = new List<Collider2D>();
    public LayerMask whatIsDamagable;
    public bool isBouncing = false;
    private float bounceXSmooth;
    private float bounceYSmooth;
    public float bounceTime = 3f;
    public float bounceMultiplier = 1;

    [Header("Hit Variable")]
    public bool isIFrame;
    public bool isDamaged;
    public float setIsDamagedTime = 0.5f;
    private float isDamagedTime;
    public float damagedX = 5;
    public float damagedTime = 0.15f;
    public Vector2 damageKnockback = new Vector2(9, 13);
    public float hitstopDamage = 0.25f;
    public float hitstopAttack = 0.1f;
    public int lastDamageDirection;

    [Header("Current Values")]
    public int currentHealth;
    public int maxHealth = 5;

    [Header("Components")]
    public SpriteRenderer theSR;
    //public GameObject wallCheck;
    //public GameObject playerCheck;
    public Animator anim;
    private Controller2DScript controller;
    //public GameObjectPool ProjectilePool;
    //public GameObject PPGO;
    public Transform playerPos;
    public AttackDetails attackDetails;
    public HealthBarScript hpScript;
    //public FieldOfView fov;
    //public FieldOfView fovPlayer;

    [Header("Cinema Values")]
    public CameraFollowScript cam;
    public DialogueData DDIntro;
    public DialogueData DDIntro2;
    public DialogueData DDIntro3;
    public DialogueData DDAlt1;
    public DialogueData DDAlt2;
    public DialogueData DDEnding1;
    public DialogueData DDEnding2;
    //public DialogueData DDIntroAlt;
    //public DialogueData DDEnding;
    public bool firstMeeting = true;
    public UM_PlayerScript playerScript;
    public bool waitAction = true;
    public GameObject hpBar;
    public TMP_Text nameBar;
    public float fillBar;
    public bool fillHealth = false;
    public float introTimer;
    public float setIntroTimer = 1;
    public string currentAnim;
    public GameObject dialogueTrigger;
    public bool wasDead;
    public bool defeated;
    public GameObject smoke;
    public Animator hpAnim;

    [Header("Sounds")]
    public SoundEffectsScript SFXScript;
    public AudioSource poofSFX;
    public AudioSource chargingSFX;
    public AudioSource hurtSFX;
    public AudioSource slamSFX;
    public AudioSource worldmusic3;
    public AudioSource blastOff;
    public GameObject wallOne;
    public bool muteMusic;
    private float musicSmoothing = 0.0f;


    //Cinematic --------------------------------------------------------------------
    //player procs the trigger
    //check if it is the first meeing
    public void IntroCinematic()
    {
        slamSFX.Play();
        playerScript.muteSoundCredit = true;
        wallOne.layer = 8;
        wallOne.transform.position = new Vector3(wallOne.transform.position.x, wallOne.transform.position.y, 0);
        playerScript.vsDaddy = true;
        cam.horizontalPan = true;
        anim.SetBool("isDialogue", true);
        playerScript.cinematicStop = true;
        if (firstMeeting)
        {
            playerScript.DialogueEnabler(DDIntro);
        }
        else if (wasDead)
        {
            playerScript.DialogueEnabler(DDAlt1);
            wasDead = false;
        }
        else
            playerScript.DialogueEnabler(DDAlt2);
        //after the dialogue finishes, hp bar appears and fills

        //after hp is filled, dsd disappears and teleports
    }

    public void HealthBarAppear()
    {
        StartCoroutine(IntroDialogueFinished());
    }

    IEnumerator IntroDialogueFinished()
    {
        yield return new WaitForSeconds(1);
        hpBar.SetActive(true);
        nameBar.text = "Daddy";
        hpScript.SetMaxHealth(maxHealth);
        fillHealth = true;
    }
    //if currenthealth == 0 proc the ending cinematic
    public void ZoomIn()
    {
        if(!defeated)
            StartCoroutine(ZoomInWaitIntro());
        else
            StartCoroutine(ZoomInWaitEnd());
    }

    public void ZoomOut()
    {
        StartCoroutine(ZoomOutWait());
    }

    IEnumerator ZoomInWaitIntro()
    {
        DialogueAnimation("frontIdle");
        cam.CurrentCamPos();
        cam.stopFollowingPlayer = true;
        cam.daddyFocus = true;
        yield return new WaitForSeconds(1.5f);
        playerScript.PlayMusic3();
        //worldmusic3.Play();
        cam.daddyFocus = false;
        playerScript.DialogueEnabler(DDIntro2);
    }

    IEnumerator ZoomInWaitEnd()
    {
        hpAnim.SetTrigger("fade");
        DialogueAnimation("smileShocked");
        yield return new WaitForSeconds(1f);
        playerScript.muteMusic = true;
        smoke.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        playerScript.music2.Play();
        hpBar.SetActive(false);
        cam.daddyFocus = false;
        cam.ddefeated = false;
        playerScript.DialogueEnabler(DDEnding2);
    }

    public void SmokeTrigger()
    {
        //play online in cinematic after first ending to disappear
        poofSFX.Play();
        anim.SetBool("smoke", true);
    }

    IEnumerator ZoomOutWait()
    {
        cam.daddyFocusOut = true;
        yield return new WaitForSeconds(0.5f);
        cam.daddyFocusOut = true;
        playerScript.DialogueEnabler(DDIntro3);
    }

    public void DialogueAnimation(string nextAnim)
    {
        if (nextAnim != "")
        {
            if (nextAnim != currentAnim)
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
    //------------------------------------------------------------------------------

    private void Start()
    {
        controller = GetComponent<Controller2DScript>();
        currentHealth = maxHealth;
        //hpScript.SetMaxHealth(maxHealth);
        startPos = transform.position;
    }
    //NOTE: during boss battles, the camera should also zoom out to show the boss if they are our of range!!!
    private void Update()
    {
        /*
        if (muteMusic)
        {
            worldmusic3.volume = Mathf.SmoothDamp(worldmusic3.volume, 0, ref musicSmoothing, 1f);
        }*/
        //intro cutscene-------------------------------------------
        if (fillBar < maxHealth && fillHealth)
        {
            fillBar += Time.deltaTime * 2;
            hpScript.SetHealth(fillBar);
        }
        if (fillBar >= maxHealth && fillHealth)
        {
            fillHealth = false;
            fillBar = 0;
            hpScript.SetHealth(maxHealth);
            playerScript.cinematicStop = false;
            cam.stopFollowingPlayer = false;
            if (cam.horizontalPan)
                cam.xPan = 0;
            //cam.DSDFight = true;
            anim.SetBool("isDialogue", false);
            anim.SetBool(currentAnim, false);
            currentAnim = "";
            waitAction = false;
            AttackAction();
        }
        //---------------------------------------------------------

        controller.Move(velocity * Time.deltaTime, false);

        if(idleTimer > 0 && isIdle && !waitAction && !isBouncing)
        {
            idleTimer -= Time.deltaTime;
            if(idleTimer <= 0 && !isCharging && isIdle)
            {
                idleTimer = setIdleTimer;
                isIdle = false;
                AttackAction();
            }
        }

        if(chargeTimer > 0 && isCharging && !isBouncing)
        {
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0 && isCharging)
               StopAction();
        }

        if (isJumping)
            velocity.y = jumpVelocity;

        if (jumpSlowDown)
            velocity.y = Mathf.SmoothDamp(velocity.y, 0, ref Smoothing, jumpSlowDownTime);

        if(hitFloor && !isBouncing)
            velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref Smoothing2, floorTime);

        /*
        if (isDamagedTime > 0 && isDamaged)
        {
            isDamagedTime -= Time.deltaTime;
            if (isDamagedTime <= 0)
            {
                velocity.x = 0;
                isDamaged = false;
            }
        }
        if (isDamaged)
            velocity.x = Mathf.SmoothDamp(velocity.x, damagedX * -lastDamageDirection, ref Smoothing, damagedTime);
            */
        if(isBouncing)
        {
            if (Mathf.Abs(velocity.x) > 0)
                if (velocity.x > 0)
                    velocity.x -= Time.deltaTime * bounceTime;
                else
                    velocity.x += Time.deltaTime * bounceTime;
            if (Mathf.Abs(velocity.y) > 0)
                if (velocity.y > 0)
                    velocity.y -= Time.deltaTime * bounceTime;
                else
                    velocity.y += Time.deltaTime * bounceTime;
            //velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref bounceXSmooth, bounceTime);
            //velocity.y = Mathf.SmoothDamp(velocity.y, 0, ref bounceYSmooth, bounceTime);
            if (Mathf.Abs(velocity.x) <= .1f && isBouncing)
            {
                isBouncing = false;
                anim.SetBool("isBouncing", false);
                velocity.x = 0;
            }
        }

        if(!isBouncing && currentHealth <= 0 && playerScript.currentHealth > 0 && !defeated)
        {
            //start ending cutscene
            defeated = true;
            waitAction = true;
            isIdle = false;
            SetAllAnimationFalse();
            cam.CurrentCamPos();
            cam.stopFollowingPlayer = true;
            cam.daddyFocus = true;
            cam.ddefeated = true;
            anim.SetBool("isDialogue", true);
            playerScript.cinematicStop = true;
            playerScript.DialogueEnabler(DDEnding1);
        }

        if (controller.collisions.below && !isBouncing)
        {
            anim.SetBool("inAir", false); // temp
            velocity.y = 0;
            if (!isCharging)
            {
                isIdle = true;
            }
            //velocity.x = 0;
        }
        else if ((controller.collisions.below || controller.collisions.above) && isBouncing)
            velocity.y *= -1;

        if(!controller.collisions.below && !isCharging && !isBouncing)
        {
            //play
            anim.SetBool("inAir", true); // temp
        }

        if ((controller.collisions.right || controller.collisions.left) && isChargeAttack && !isBouncing)
        {
            isChargeAttack = false;
            isCharging = false;
            aura.SetActive(false);
            velocity.y = 0;
            velocity.x = 0;
            anim.SetBool("wall", true);
            anim.SetBool("charge", false);
            anim.SetBool("super", false);
            chargeTimer = 0;
            //play charge wall animation
        }
        else if((controller.collisions.right || controller.collisions.left) && isBouncing)
        {
            velocity.x *= -1;
        }

        //for testing, proc teleport after every few seconds

        //Attack----------------------------------------------------------------------
        if (hitboxHolder.activeSelf == true && currentHealth > 0 || isBouncing)
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
                        //hitStop.Stop(hitstopAttack);

                        collider.transform.SendMessage("Damage", attackDetails); //can send knockback and such
                        oldDetectedObjects.Add(collider);
                        //need to add self knockback when hitting an enemy, just once recoil
                        SFXScript.OhYeahSFX();
                    }
                }
            }
        }
        if (hitboxes[1].activeSelf == false)
        {
            oldDetectedObjects.Clear();
        }
        //---------------------------------------------------------------------------------------
        if(!isCharging)
        {
            velocity.y -= 0.25f; //keeps the controller.collisions.bottom active
            isIFrame = false;
        }
    }

    public void HitSprite()
    {
        SetAllAnimationFalse();
    }

    public void AttackAction()
    {
        isIFrame = true;
        isIdle = false;
        isDamaged = false;
        isCharging = true;
        anim.SetBool("charge", true);
        aura.SetActive(true);
        StartCoroutine(Charging());
        //currently only change attack, maybe jump later
        /*
        ranNum = Random.Range(0, 2);
        if (ranNum == 1)
            anim.SetBool("jump", true); // temp
        else
            anim.SetBool("pAttack", true); //temp*/
    }

    IEnumerator Charging()
    {
        chargingSFX.Play();
        if (playerScript.currentHealth > 0 && !waitAction) //try to stop if player dies at beginning of charge
        {
            SFXScript.OhYeahSFX();
            yield return new WaitForSeconds(1.375f);
            if (playerScript.currentHealth > 0 && !waitAction) //try to stop if player dies at beginning of charge
            {
                yield return new WaitForSeconds(1.375f);
                anim.SetBool("super", true);
                if (playerScript.currentHealth > 0 && !waitAction)
                {
                    yield return new WaitForSeconds(0.75f);
                    LocatePlayerPosition();
                    FlipSprite();
                    isChargeAttack = true;
                    anim.SetBool("attack", true);
                    chargingSFX.Stop();
                    blastOff.Play();
                    auraAnim.SetTrigger("dash");
                    chargeTimer = setChargeTimer;
                }
            }
        }
    }

    public void StopAction()
    {
        isCharging = false;
        aura.SetActive(false);
        if (velocity.y == 0)
            anim.SetBool("fall", true);
        anim.SetBool("charge", false);
        anim.SetBool("super", false);
        if (velocity.y <= 0)
            isIdle = true;
    }

        private void FlipSprite()
    {
        //if (!isDamaged && !isAttacking)
        if (transform.position.x < playerPos.position.x)
        {
            theSR.flipX = true;
            auraSR.flipX = true;
            facingDir = 1;
            //PPGO.transform.localPosition = new Vector2(Mathf.Abs(ProjectilePool.transform.localPosition.x), ProjectilePool.transform.localPosition.y);
        }
        else if (transform.position.x >= playerPos.position.x)
        {
            theSR.flipX = false;
            auraSR.flipX = false;
            facingDir = -1;
            //PPGO.transform.localPosition = new Vector2(Mathf.Abs(ProjectilePool.transform.localPosition.x) * -1, ProjectilePool.transform.localPosition.y);
        }

        //wallCheck.transform.localRotation = Quaternion.Euler(0, 0, 270 * facingDir);
        //playerCheck.transform.localRotation = Quaternion.Euler(0, 0, 180 * facingDir);
        //ProjectilePool.facingDir = -facingDir;
    }

    public void BounceOffWall()
    {
        velocity.x = 1 * -facingDir;
    }

    public void ResetSpeed()
    {
        velocity.x = 0;
        hitFloor = false;
        FlipSprite();
        anim.SetBool("attack", false);
        anim.SetBool("wall", false);
        anim.SetBool("fall", false);
    }

    public void HitFloor() => hitFloor = true;

    public void Damage(AttackDetails attackDetails)
    {
        if (!isIFrame && currentHealth > 0)
        {
            hurtSFX.pitch = Random.Range(.75f, .9f);
            hurtSFX.Play();
            isIdle = false;
            lastDamageDirection = (attackDetails.position.x > transform.position.x) ? 1 : -1;
            velocity.x = 11f * -lastDamageDirection * bounceMultiplier;
            velocity.y = 11f * bounceMultiplier;
            bounceMultiplier *= 1.2f;
            isBouncing = true;
            currentHealth -= attackDetails.damageAmount;
            if (currentHealth <= 0)
                wasDead = true;
            hpScript.SetHealth(currentHealth);
            anim.SetBool("hit", true);
            anim.SetBool("isBouncing", true);
            //isDamagedTime = setIsDamagedTime;
            isDamaged = true;
            isIFrame = true;
        }
    }

    public void LocatePlayerPosition()
    {
        playerLoc = playerPos.position;
        velocity = (playerLoc - (Vector2)transform.position).normalized * attackSpeed;
        if (playerPos.position.y - transform.position.y < 3)
            velocity.y = 0;
    }
    /*
    public void ProjectileAttack()
    {
        var projectile = ProjectilePool.Get();
        projectile.gameObject.SetActive(true);
    }
    */

    public void SetAllAnimationFalse()
    {
        anim.SetBool("jump", false);
        anim.SetBool("charge", false);
        anim.SetBool("super", false);
        anim.SetBool("hit", false);
    }

    public void Restart()
    {
        //muteMusic = true;
        chargingSFX.Stop();
        hpBar.SetActive(false);
        bounceMultiplier = 1.75f;
        //restart starting position
        //restart HP
        currentHealth = maxHealth;
        //set first false to false
        firstMeeting = false;
        isIdle = true;
        idleTimer = setIdleTimer;
        waitAction = true;
        SetAllAnimationFalse();
        anim.SetBool("charge", false);
        anim.SetTrigger("reset");
        isCharging = false;
        isChargeAttack = false;
        //set trigger active again
        aura.SetActive(false);
        velocity.y = 0;
        velocity.x = 0;
        wallOne.layer = 0;
        wallOne.transform.position = new Vector3(wallOne.transform.position.x, wallOne.transform.position.y, 11.69f);
        transform.position = startPos;
        dialogueTrigger.SetActive(true);
        FlipSprite();
    }
}
