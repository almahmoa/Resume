using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Controller2DScript))]
public class DarkShippieDuesScript : MonoBehaviour
{
    public float teleportDistance;
    public float reappearTimer;
    public float setReappearTimer = 2f;
    public float checkPlayerTimer;
    public float setCheckPlayerTimer = 0.3f;
    public int facingDir = -1;
    public float jumpVelocity = 12;
    private float Smoothing = 0;
    public bool isJumping;
    public bool isJumpKicking;
    public bool jumpSlowDown;
    public float jumpSlowDownTime = 0.15f;
    public float jumpKickSpeed = 8;
    public Vector3 velocity;
    public Vector2 playerLoc;
    public float ranNum = 0;
    public float baseY;
    public Vector2 startPos;

    [Header("Attack Variable")]
    public GameObject hitboxHolder;
    public GameObject[] hitboxes;
    private List<Collider2D> oldDetectedObjects = new List<Collider2D>();
    public LayerMask whatIsDamagable;

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

    [Header("Sound")]
    public SoundEffectsScript SFXScript;
    public AudioSource poofSFX;

    [Header("Components")]
    public SpriteRenderer theSR;
    public GameObject wallCheck;
    public GameObject playerCheck;
    public Animator anim;
    private Controller2DScript controller;
    public GameObjectPool ProjectilePool;
    public GameObject PPGO;
    public Transform playerPos;
    public AttackDetails attackDetails;
    public HealthBarScript hpScript;
    public FieldOfView fov;
    public FieldOfView fovPlayer;

    public GameObject smoke;
    public bool isSmoke;

    [Header("Cinema Values")]
    public GameObject dialogueTrigger;
    public CameraFollowScript cam;
    public DialogueData DDIntro;
    public DialogueData DDIntroAlt;
    public DialogueData DDWon1;
    public DialogueData DDWon2;
    public DialogueData DDWon3;
    public DialogueData DDWon4;
    public DialogueData DDEnd;
    public DialogueData DDFinalWords;
    //public DialogueData DDEnding;
    public bool firstMeeting;
    public UM_PlayerScript playerScript;
    public bool waitAction = true;
    public GameObject hpBar;
    public Animator hpAnim;
    public TMP_Text nameBar;
    public float fillBar;
    public bool fillHealth = false;
    public float introTimer;
    public float setIntroTimer = 1;
    public string currentAnim;
    public bool vsPlayer;
    public bool DSDDead;

    public GameObject wallOne;
    public GameObject wallTwo;


    //Cinematic --------------------------------------------------------------------
    //player procs the trigger
    //check if it is the first meeing
    public void IntroCinematic()
    {
        wallOne.layer = 8;
        vsPlayer = true;
        anim.SetBool("isDialogue", true);
        playerScript.cinematicStop = true;
        playerScript.vsDSD = true;
        if (firstMeeting)
        {
            playerScript.DialogueEnabler(DDIntro);
        }
        else
        {
            playerScript.DialogueEnabler(DDIntroAlt);
        }
        //after the dialogue finishes, hp bar appears and fills

        //after hp is filled, dsd disappears and teleports
    }

    public void HealthBarAppear()
    {
        StartCoroutine(IntroDialogueFinished());
    }

    IEnumerator IntroDialogueFinished()
    {
        yield return new WaitForSeconds(1.5f);
        hpBar.SetActive(true);
        nameBar.text = "Dark Schippie Dues";
        hpScript.SetMaxHealth(maxHealth);
        fillHealth = true;
    }

    public void DialogueAnimation(string nextAnim)
    {
        if(nextAnim != "")
        {
            if(nextAnim != currentAnim)
            {
                if (currentAnim != "")
                    anim.SetBool(currentAnim, false);
                anim.SetBool(nextAnim, true);
                currentAnim = nextAnim;
            }
        }
        else if(currentAnim != "")
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
        baseY = transform.position.y;
        startPos = transform.position;
    }
    //NOTE: during boss battles, the camera should also zoom out to show the boss if they are our of range!!!
    private void Update()
    {
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
            if (cam.horizontalPan)
                cam.xPan = 0;
            //cam.DSDFight = true;
            anim.SetBool("isDialogue", false);
            anim.SetBool(currentAnim, false);
            currentAnim = "";
            waitAction = false;
            StartSmoke();
        }
        //---------------------------------------------------------

        if (checkPlayerTimer > 0 && !waitAction)
            checkPlayerTimer -= Time.deltaTime;

        if(reappearTimer > 0 && isSmoke && !waitAction)
        {
            if(CheckPlayerInMinRange())
                Teleport();
            reappearTimer -= Time.deltaTime;
            if (reappearTimer <= 0)
                StartSmoke();
        }
        controller.Move(velocity * Time.deltaTime, false);

        //Jumping ----------------------------------------------
        if(!isDamaged)
        {
            if (isJumping)
                velocity.y = jumpVelocity;

            if(jumpSlowDown)
                velocity.y = Mathf.SmoothDamp(velocity.y, 0, ref Smoothing, jumpSlowDownTime);
        }
        //-------------------------------------------------------
        if (isDamagedTime > 0 && isDamaged)
        {
            isDamagedTime -= Time.deltaTime;
        if (isDamagedTime <= 0)
            {
                velocity.x = 0;
                isDamaged = false;
            }
        }
        if(isDamaged)
        {
            isJumping = false;
            velocity.x = Mathf.SmoothDamp(velocity.x, damagedX * -lastDamageDirection, ref Smoothing, damagedTime);
            velocity.y = 0;
        }

        if (controller.collisions.below)
        {
            //smoke.SetActive(true);
            anim.SetBool("jump", false); // temp
            isJumpKicking = false;
            velocity.y = 0;
            velocity.x = 0;
        }

        if(!isJumpKicking)
            hitboxes[1].SetActive(false);

        if ((controller.collisions.right || controller.collisions.left) && isJumpKicking)
        {
            //if the jump kick collides with the walls
            //play teleport animation, which makes DSD invisable
            velocity.y = 0;
            velocity.x = 0;
            //anim.SetBool("jump", false);
            isJumpKicking = false;
            StartSmoke();
        }

        //for testing, proc teleport after every few seconds

        //Attack----------------------------------------------------------------------
        if (hitboxHolder.activeSelf == true && currentHealth > 0)
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
                        SFXScript.KickedSFX();
                    }
                }
            }
        }
        if (hitboxes[1].activeSelf == false)
        {
            oldDetectedObjects.Clear();
        }
        //---------------------------------------------------------------------------------------
        // DSD won fight -----------------------------------------------------------------------
        if(vsPlayer)
        {
            if (playerScript.currentHealth <= 0 && isSmoke == false && isJumpKicking == false)
            {
                vsPlayer = false;
                waitAction = true;
                StartCoroutine(DSDWonDialogue());
            }
            else if(currentHealth <= 0 && isSmoke == false && isJumpKicking == false && transform.position.y == baseY)
            {
                anim.SetBool("hit", false);
                vsPlayer = false;
                waitAction = true;
                StartCoroutine(DSDDefeatedDialogue());
            }
        }
        //-------------------------------------------------------------------------------------
    }
    
    public void Teleport()
    {
        isJumpKicking = false;
        transform.position = new Vector2(transform.position.x, baseY);
        anim.SetBool("smoke", true);
        if (currentHealth <= 0)
            teleportDistance = Random.Range(8f, 10f);
        else
            teleportDistance = Random.Range(12f, 18f);
        fov.viewRadiusMax = teleportDistance;

        if (fov.TargetMaxTagName() == "Wall")
            transform.position = new Vector2(transform.position.x + teleportDistance / 2 * facingDir, transform.position.y);
        else
            transform.position = new Vector2(transform.position.x - teleportDistance / 2 * facingDir, transform.position.y);
        FlipSprite();
    }

    IEnumerator TeleportBuffer()
    {
        yield return new WaitForSeconds(0.1f);
        Teleport();
    }

        public bool CheckPlayerInMinRange()
    {
        if (fovPlayer.TargetMinTagName() == "Player" && checkPlayerTimer <= 0)
        {
            checkPlayerTimer = setCheckPlayerTimer;
            return true;
        }
        return false;
    }

    public void AttackAction()
    {
        isIFrame = false;
        isDamaged = false;

        ranNum = Random.Range(0, 2);
        if(!waitAction && playerScript.currentHealth > 0 && currentHealth > 0)
        {
            if (ranNum == 1)
                anim.SetBool("jump", true); // temp
            else
                anim.SetBool("pAttack", true); //temp
        }
    }

    private void FlipSprite()
    {
        //if (!isDamaged && !isAttacking)
        if (transform.position.x < playerPos.position.x)
        {
            theSR.flipX = true;
            facingDir = 1;
            PPGO.transform.localPosition = new Vector2(Mathf.Abs(ProjectilePool.transform.localPosition.x), ProjectilePool.transform.localPosition.y);
        }
        else if (transform.position.x >= playerPos.position.x)
        {
            theSR.flipX = false;
            facingDir = -1;
            PPGO.transform.localPosition = new Vector2(Mathf.Abs(ProjectilePool.transform.localPosition.x) * -1, ProjectilePool.transform.localPosition.y);
        }

        wallCheck.transform.localRotation = Quaternion.Euler(0, 0, 270 * facingDir);
        playerCheck.transform.localRotation = Quaternion.Euler(0, 0, 180 * facingDir);
        ProjectilePool.facingDir = -facingDir;
    }

    public void Damage(AttackDetails attackDetails)
    {
        if (!isIFrame)
        {
            SFXScript.PunchedSFX();
            velocity.y = 0;
            lastDamageDirection = (attackDetails.position.x > transform.position.x) ? 1 : -1;
            currentHealth -= attackDetails.damageAmount;

            hpScript.SetHealth(currentHealth);
            anim.SetBool("hit", true);
            isDamagedTime = setIsDamagedTime;
            anim.SetBool("jump", false);
            anim.SetBool("pAttack", false);
            isDamaged = true;
            jumpSlowDown = false;
            isJumping = false;
            //isJumpKicking = false;
            isIFrame = true;
        }
    }

    public void Jump(int isJumping)
    {
        this.isJumping = (isJumping == 1) ? true : false;
        if (isJumping == 0)
            jumpSlowDown = true;
    }

    public void JumpKick(int isJumpKicking)
    {
        hitboxes[1].SetActive(true);
        hitboxes[1].transform.localPosition = new Vector2(Mathf.Abs(hitboxes[1].transform.localPosition.x) * facingDir, hitboxes[1].transform.localPosition.y);
        jumpSlowDown = false;
        isJumping = false;
        this.isJumpKicking = (isJumpKicking == 1) ? true : false;
    }

    public void LocatePlayerPosition()
    {
        playerLoc = playerPos.position;
        velocity = (playerLoc - (Vector2)transform.position).normalized * jumpKickSpeed;
        if (Mathf.Abs(velocity.x) < 10) //Minimum jump kick x velocity
            velocity.x = 10 * facingDir;
    }

    public void ProjectileAttack()
    {
        var projectile = ProjectilePool.Get();
        projectile.gameObject.SetActive(true);
    }

    public void SmokeTrigger()
    {
        poofSFX.Play();
        if(DSDDead)
        {
            poofSFX.pitch = Random.Range(0.75f, 0.85f);
            poofSFX.volume = 0.4f;
            anim.SetBool("smoke", true);
        }
        else
        {
            if (!isSmoke)
            {
                poofSFX.pitch = Random.Range(0.75f, 0.85f);
                poofSFX.volume = 0.25f;
                SetAllAnimationFalse();
                hitboxHolder.SetActive(false);
                if (currentHealth > 0)
                    reappearTimer = setReappearTimer;
                else
                    reappearTimer = 1;
                isSmoke = true;
                StartCoroutine(TeleportBuffer());
            }
            else
            {
                poofSFX.pitch = Random.Range(.75f, 1f);
                poofSFX.volume = 0.4f;
                anim.SetBool("smoke", false);
                hitboxHolder.SetActive(true);
                if(!waitAction && playerScript.currentHealth > 0)
                    AttackAction();
                isSmoke = false;
            }
        }
    }

    public void DeadChecker()
    {
        if (currentHealth > 0 || transform.position.y > baseY || isJumpKicking)
            StartSmoke();
    }

    public void StartSmoke()
    {
        if(!waitAction)
        {
            isIFrame = true;
            smoke.SetActive(true);
        }
    }


    public void SetAllAnimationFalse()
    {
        anim.SetBool("jump", false);
        anim.SetBool("pAttack", false);
        anim.SetBool("hit", false);
    }

    public void Respawn()
    {
        transform.position = startPos;
        firstMeeting = false;
        currentHealth = maxHealth;
    }

    IEnumerator DSDWonDialogue()
    {
        cam.CurrentCamPos();
        cam.DSDFocus = true;
        anim.SetBool("pAttack", false);
        //fade hpbar
        hpAnim.SetTrigger("fade");
        yield return new WaitForSeconds(2f);
        hpBar.SetActive(false);
        anim.SetBool("isDialogue", true);
        ranNum = Random.Range(0, 5);
        if(ranNum == 1)
            playerScript.DialogueEnabler(DDWon1);
        else if (ranNum == 2)
            playerScript.DialogueEnabler(DDWon2);
        else if (ranNum == 3)
            playerScript.DialogueEnabler(DDWon3);
        else 
            playerScript.DialogueEnabler(DDWon4);
    }

    IEnumerator DSDDefeatedDialogue()
    {
        anim.SetBool("pAttack", false);
        FlipSprite();
        cam.CurrentCamPos();
        cam.DSDFocus = true;
        anim.SetBool("isDialogue", true);
        playerScript.cinematicStop = true;

        yield return new WaitForSeconds(.5f);
        playerScript.DialogueEnabler(DDEnd);

        yield return new WaitForSeconds(1f);
        hpBar.SetActive(false);
    }

    public void DSDEndingDialogueFinished()
    {
            DSDDead = true;
            smoke.SetActive(true);
            StartCoroutine(FinalWords());
    }

    IEnumerator FinalWords()
    {
        yield return new WaitForSeconds(1f);
        cam.DSDFocus = false;
        yield return new WaitForSeconds(1f);
        playerScript.vsDSD = false;
        playerScript.DialogueEnabler(DDFinalWords);
        wallOne.layer = 0;
        wallTwo.layer = 0;
        gameObject.SetActive(false);
    }

    public void Restart()
    {
        //restart starting position
        wallOne.layer = 0;
        transform.position = startPos;
        //restart HP
        currentHealth = maxHealth;
        //set first false to false
        firstMeeting = false;
        //set trigger active again
        dialogueTrigger.SetActive(true);
        FlipSprite();
    }

    //When teleporting there is an animation for it, after that animation is finish (on reappear) move on to attack function
    //(currently) selects between two attack 50/50 random change to decide on reappear for teleport
    //jump attack, the rise upwards will be in the animation
    //after the animation switches to the kick, it will trigger a function to get the
    //player current pos, and move towards it on update (the player current pos is not updated)
    //this will continue until DSD is either hit or grounded
    //If grounded, play rising animation, then teleport
    //if hit, stop movement (play hit animation) then teleport after

}
