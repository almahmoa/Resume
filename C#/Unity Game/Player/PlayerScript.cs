using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Controller2DScript))]
public class PlayerScript : MonoBehaviour
{
    [Header("Move Variable")]
    public float accelerationTimeGrounded = 0.1f;
    public float moveSpeed = 10;
    public float rollSpeed = 25;
    public bool isRolling = false;
    private float rollTime;
    public float setRollTime = 0.75f;
    private float velocityXSmoothing;

    [Header("Jump Variable")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float setJumpBuffer = 0.2f;
    private float jumpBuffer;
    private bool canJumpBuffer;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private int amountOfJumps;
    public int setAmountOfJumps = 1;
    private bool isJumping;

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
    public float accelerationTimeAirborne = .2f;
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
    public float setAttackTime = 0.5f;
    protected float attackTime;
    public bool isAttacking = false;
    public bool airDownAttack = false;
    private List<Collider2D> oldDetectedObjects = new List<Collider2D>();

    [Header("Hit Variable")]
    public float setIsDamagedTime = 0.5f;
    public float setIFrameTime = 1f;
    private float isDamagedTime;
    private float iFrameTime = 0f;
    public bool isDamaged;
    public Vector2 damageKnockback;
    public float hitstopDamage = 0.25f;
    public float hitstopAttack = 0.1f;

    [Header("Scripts")]
    public CameraFollowScript cam;
    public HitStop hitStop;
    protected Controller2DScript controller;
    protected AttackDetails attackDetails;

    [Header("Other Variables")]
    public float gravity;
    public int facingDir = 1;
	public Vector3 velocity;
    protected Vector2 directionalInput;
    public LayerMask whatIsGround;
    public LayerMask whatIsWeakSpot;

    [Header("Components")]
    public SpriteRenderer theSR;
    protected Animator anim;

    void Start()
    {
        controller = GetComponent<Controller2DScript> ();
        anim = GetComponent<Animator>();
        hitStop = GetComponent<HitStop>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
    }

	void Update()
    {
        CalculateVelocity();
        HandleWallSliding ();
        
        //Attack-------------------------------------------------------------------------------------------------------------
        if (hitboxHolder.activeSelf == true)
        {
            //Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(hitbox.transform.position, attackRadius, whatIsDamagable);
            foreach(GameObject hitbox in hitboxes)
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

                    velocity.x = attackKnockback.x * -controller.collisions.faceDir;
                    velocity.y = attackKnockback.y;
                    hitStop.Stop(hitstopAttack);

                    isAttacking = true;
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
        if (hitboxHolder.activeSelf == false)
        {
            oldDetectedObjects.Clear();
        }
        if(attackTime > 0)
        {
            attackTime -= Time.deltaTime;
        }
        else if(attackTime <= 0 && (isAttacking || airDownAttack))
        {
            velocity.x = 0.1f * facingDir;
            isAttacking = false;
            airDownAttack = false;
        }
        //------------------------------------------------------------------------------------------------------------------------

        controller.Move (velocity * Time.deltaTime, directionalInput);

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
            cam.airDownAttack = false;

            BigFall();

            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else if(controller.collisions.below)
            {
				velocity.y = 0;
			}

            if(directionalInput.y == -1 && !isRolling)
            {
                velocityXSmoothing = 0f;
                velocity.x = 0;
                anim.SetBool("crouch", true);
            }
            else if(directionalInput.y != -1 || directionalInput.x != 0)
            {
                anim.SetBool("crouch", false);
            }
        }
        else if (controller.collisions.above)
        {
            velocity.y = 0;
        }
        else if(!controller.collisions.below)
        {
            anim.SetBool("crouch", false);
            anim.SetBool("isGrounded", false);
            if(coyoteTime > 0)
            {
                coyoteTime -= Time.deltaTime;
            }
        }

        if (coyoteTime <= 0 && canCoyoteTime &&!isJumping)
        {
            canCoyoteTime = false;
            amountOfJumps--;
        }

        if (jumpBuffer > 0)
        {
            jumpBuffer -= Time.deltaTime;
            OnJumpInputDown();

        }
        else if(jumpBuffer <= 0)
        {
            canJumpBuffer = true;
        }
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
        }
        if(iFrameTime > 0)
        {
            iFrameTime -= Time.deltaTime;
        }
        //-----------------------------------------------------------------------------
        //Rolling
        if(isRolling)
        {
            if(rollTime > 0)
            {
                rollTime -= Time.deltaTime;
            }
            if(rollTime <= 0)
            {
                isRolling = false;
            }
        }
        //-----------------------------------------------------------------------------

        anim.SetInteger("yDirection", (int)directionalInput.y);
        anim.SetFloat("yVelocity", Mathf.Abs(velocity.y));

        FlipSprite();
    }

	public void OnJumpInputDown()
    {
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
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x))
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
        else if(!controller.collisions.below && canJumpBuffer)
        {
            jumpBuffer = setJumpBuffer;
            canJumpBuffer = false;
        }
	}

	public void OnJumpInputUp()
    {
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
        if(controller.WallSlidableSlopeAngle() && CanWallSlide())
        {
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && (directionalInput.x == wallDirX || wallAir)) // replace wallAir with || timeToWallUnstick > 0 if you want to have analog movement determine wall cling between jump
            {
                wallSlide = true;
                wallAir = true;
                wallJump = false;
                bigFallTime = setBigFallTime;
                BigFall();

                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }
            }

            if (wallUnstickTime > 0 && wallSlide)
            {
                velocityXSmoothing = 0;
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
        }
    }

	private void CalculateVelocity()
    {
		float targetVelocityX = directionalInput.x * moveSpeed;
        if(!isDamaged && !isRolling && (attackTime <= 0 || airDownAttack))
        {
            if (velocity.y <= 0)
            {
                if (!controller.collisions.left || !controller.collisions.right)
                {
                    wallJump = false;
                }
                bigFallTime -= Time.deltaTime;
            }
            else if(velocity.y > 0)
            {
                bigFallTime = setBigFallTime;
            }
            if (bigFallTime <= 0)
            {
                bigFall = true;
                cam.bigFall = true;
            }
            if (!wallJump && (directionalInput.y != -1 || !controller.collisions.below))
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            }
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
        if(!isDamaged && iFrameTime <= 0)
        {
            anim.SetBool("hit", true);
            hitStop.Stop(hitstopDamage);
            isDamaged = true;

            velocity.x = damageKnockback.x * -Mathf.Sign(attackDetails.position.x - transform.position.x); //change to opposite side of enemy's attack direction
            velocity.y = damageKnockback.y;

            isDamagedTime = setIsDamagedTime;
        }
    }

    private void FlipSprite()
    {
        if(!isDamaged && !isAttacking)
        {
            if (facingDir == -1)
            {
                theSR.flipX = true;
            }
            else if (facingDir == 1)
            {
                theSR.flipX = false;
            }
            if(directionalInput.x != 0)
            {
                facingDir = controller.collisions.faceDir;
            }
            wallSlideChecker.localPosition = new Vector2(Mathf.Abs(wallSlideChecker.localPosition.x) * controller.collisions.faceDir, wallSlideChecker.localPosition.y);
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

    public void OnRollInputDown()
    {
        //shoot forward, i-frame
        //after recovering, there is a small buffer where you cannot roll again
        //crouch roll animation slightly different than standing roll? use blend tree if starting with yDirection -1 and finishing ydirection -1
        if (controller.collisions.below && !isRolling && !isDamaged)
        {
            anim.SetBool("roll", true);
            velocity.x = rollSpeed * facingDir;
            isRolling = true;
            rollTime = setRollTime;
            iFrameTime = setIFrameTime;
        }
    }

    public void OnAttackInputDown()
    {
        if (attackTime <= 0 && !isAttacking && !isDamaged && !isRolling)
        {
            anim.SetBool("attack", true);
            attackTime = setAttackTime; //same for testing purpose
            #region Sword
            if (!wallSlide)
            {
                //use setAttackTime to customize animation buffer length
                if (directionalInput.y == 0 && controller.collisions.below)
                {
                    //Debug.Log("Neutral Attack");
                    attackKnockback = new Vector2(3f, velocity.y);
                    hitboxes[0].SetActive(true);
                    hitboxes[0].transform.localPosition = new Vector2(1.75f * controller.collisions.faceDir, 0.25f);
                    hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(2.95f, 1.4f);
                    hitboxes[1].SetActive(false);
                    hitboxes[2].SetActive(false);
                }
                else if (directionalInput.y == 1 && controller.collisions.below)
                {
                    //Debug.Log("Up Attack");
                    attackKnockback = new Vector2(1f, velocity.y);
                    hitboxes[0].SetActive(true);
                    hitboxes[0].transform.localPosition = new Vector2(0, 2.6f);
                    hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(3.45f, 2.25f);
                    hitboxes[1].SetActive(false);
                    hitboxes[2].SetActive(false);
                }
                else if (directionalInput.y == -1 && controller.collisions.below)
                {
                    //Debug.Log("Crouch Attack");
                    attackKnockback = new Vector2(velocity.x, velocity.y);
                    hitboxes[0].SetActive(true);
                    hitboxes[0].transform.localPosition = new Vector2(2f * controller.collisions.faceDir, -0.65f);
                    hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(3.25f, 1f);
                    hitboxes[1].SetActive(false);
                    hitboxes[2].SetActive(false);
                }
                else if (directionalInput.y == 0 && !controller.collisions.below)
                {
                    //Debug.Log("Neutral Air Attack");
                    attackKnockback = new Vector2(-velocity.x, velocity.y);
                    hitboxes[0].SetActive(true);
                    hitboxes[0].transform.localPosition = new Vector2(1.75f * controller.collisions.faceDir, 0.25f);
                    hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(2.95f, 1.4f);
                    hitboxes[1].SetActive(false);
                    hitboxes[2].SetActive(false);
                }
                else if (directionalInput.y == 1 && !controller.collisions.below)
                {
                    //Debug.Log("Up Air Attack");
                    attackKnockback = new Vector2(-velocity.x, velocity.y);
                    hitboxes[0].SetActive(true);
                    hitboxes[0].transform.localPosition = new Vector2(0, 2.6f);
                    hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(3.45f, 2.25f);
                    hitboxes[1].SetActive(false);
                    hitboxes[2].SetActive(false);
                }
                else if (directionalInput.y == -1 && !controller.collisions.below)
                {
                    //Debug.Log("Down Air Attack");
                    airDownAttack = true;
                    attackKnockback = new Vector2(-velocity.x, 20f);
                    hitboxes[0].SetActive(true);
                    hitboxes[0].transform.localPosition = new Vector2(0, -0.65f);
                    hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(6, 1);
                    hitboxes[1].SetActive(true);
                    hitboxes[1].transform.localPosition = new Vector2(0, -1.4f);
                    hitboxes[1].GetComponent<BoxCollider2D>().size = new Vector2(4.75f, 1);
                    hitboxes[2].SetActive(false);
                }
            }
            else if (wallSlide)
            {
                Debug.Log("Wall Attack");
                hitboxes[0].SetActive(true);
                hitboxes[0].transform.localPosition = new Vector2(-1.5f * controller.collisions.faceDir, 0);
                hitboxes[0].GetComponent<BoxCollider2D>().size = new Vector2(2.25f, 2.25f);
                hitboxes[1].SetActive(false);
                hitboxes[2].SetActive(false);
            }
            if (controller.collisions.below) //attacking movement
            {
                velocity.x = 3f * controller.collisions.faceDir; //change to a varible that differs between attacks?
            }
            #endregion
        }
    }

    public void AnimationFinished(string currentAnimation)
    {
        anim.SetBool(currentAnimation, false);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(hitboxHolder.transform.position, attackRadius);
        Gizmos.DrawLine(wallSlideChecker.transform.position, wallSlideChecker.transform.position + (Vector3)(Vector2.right * 0.15f));
    }
}
