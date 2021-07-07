using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2DScript))]
public class ProjectileScript : MonoBehaviour
{
    public ProjectileData pD;

    #region Components
    public GameObjectPool pooler;
    private SpriteRenderer theSR;
    private Controller2DScript controller;
    private AttackDetails attackDetails;
    #endregion

    public float startSpeed;
    public float travelDistance;
    //public Transform targetTransform;
    public Vector2 targetPos;
    public Vector3 velocity;
    private float velocityXSmoothing;
    private float velocityYSmoothing;

    public bool waiting;
    public float driftTime;
    public float lifeTime;

    public float driftSpeed = 0.5f;
    public Vector2 oldTargetPos;

    public void SetProjectileData(ProjectileData pD)
    {
        this.pD = pD;
    }

    private void Start()
    {
        controller = GetComponent<Controller2DScript>();
        pooler = GetComponentInParent<GameObjectPool>();
        
    }

    private void OnEnable()
    {
        //StartCoroutine("FindTargetsWithDelay", 0);//if using the angular movement, first shot
        //theSR = GetComponent<SpriteRenderer>();
        //theSR.sprite = pD.sprite;
        //theSR.color = pD.color;
        transform.localScale = new Vector3(pD.size, pD.size, pD.size);
        oldTargetPos = TargetPosition();

        if (!pD.isHoming)
        {
            velocity = (TargetPosition() - (Vector2)transform.position).normalized * pD.endSpeed;
        }
    }

    private void OnDisable()
    {
        startSpeed = 1;
        //transform.localScale = new Vector3(1, 1, 1); // temp to return to normal size
        //driftTime = 1f;
    }

    private void Update()
    {
        controller.Move(velocity * Time.deltaTime, false);

        //RecalibrateTargetPosition();

        if (pD.isHoming)
        {
            if (CheckInMinAgroRange())
            {
                //driftTime = 1;
            }
            else
            {
                
                velocity = (TargetPosition() - (Vector2)transform.position).normalized * startSpeed;
                //velocity.y *= 1f;
                //velocity.x *= 1.5f;
            }
        }
        startSpeed = Mathf.SmoothDamp(startSpeed, pD.endSpeed, ref velocityXSmoothing, pD.acceleration);
        //driftSpeed = Mathf.SmoothDamp(driftSpeed, 1, ref velocityXSmoothing, pD.acceleration);
    }

    private void FixedUpdate()
    {
        if (CheckGround())//change these collision to a radius check
        {
            pooler.ReturnToPool(gameObject);
        }
        //Collider2D damageHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsPlayer);
        //Collider2D groundHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsGround);
        /*
        if (damageHit)
        {
            damageHit.transform.SendMessage("Damage", attackDetails);
            Destroy(gameObject);
        }

        if (groundHit)
        {
            hasHitGround = true;
            rb.gravityScale = 0.0f;
            rb.velocity = Vector2.zero;
        }

        if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
        {
            isGravityOn = true;
            rb.gravityScale = gravity;
        }*/
    }

    private void RecalibrateTargetPosition()
    {
        if (waiting)
            return;
        StartCoroutine("FindTargetsWithDelay", driftTime);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        waiting = true;
        yield return new WaitForSeconds(delay);
        velocity = (TargetPosition() - (Vector2)transform.position).normalized * pD.endSpeed;
        waiting = false;
    }

    private Vector2 TargetPosition()
    {
        return GameObject.FindWithTag(pD.target).transform.position;
    }

    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(transform.position, pD.checkRadius, pD.whatIsGround);
    }

    private bool CheckPlayer()
    {
        return Physics2D.OverlapCircle(transform.position, pD.checkRadius, pD.whatIsDamagable);
    }

    //maybe add array later to functionas a multishot for player to use against enemies?
    public bool CheckInMinAgroRange()
    {
        return Physics2D.OverlapCircle(transform.position, pD.minRangeRadius, pD.whatIsDamagable);
    }

    public bool CheckInMaxAgroRange()
    {
        return Physics2D.OverlapCircle(transform.position, pD.maxRangeRadius, pD.whatIsDamagable);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, pD.checkRadius);
        Gizmos.DrawWireSphere(transform.position, pD.minRangeRadius);
        Gizmos.DrawWireSphere(transform.position, pD.maxRangeRadius);
    }
}
