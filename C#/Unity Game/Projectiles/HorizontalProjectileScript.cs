using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2DScript))]
public class HorizontalProjectileScript : MonoBehaviour
{
    public float maxSpeed = 6;
    public float acceleration = 2;
    //public float travelDistance;
    public float checkRadius;
    public int facingDir;

    public bool startMove = false;
    public float lifeTime;

    public Vector3 velocity;
    private float velocityXSmoothing;

    public LayerMask whatIsDamagable;
    public LayerMask whatIsGround;

    public GameObject hitboxHolder;
    public GameObject[] hitboxes;
    private List<Collider2D> oldDetectedObjects = new List<Collider2D>();

    #region Components
    public SpriteRenderer theSR;
    public GameObjectPool pooler;
    private Controller2DScript controller;
    private AttackDetails attackDetails;
    public SoundEffectsScript SFXScript;
    public DarkShippieDuesScript DSDScript;
    #endregion

    private void Start()
    {
        controller = GetComponent<Controller2DScript>();
        //pooler = GetComponentInParent<GameObjectPool>();
    }

    private void OnEnable()
    {
        hitboxHolder.SetActive(true);
        //On enable send start pos
        transform.position = pooler.transform.position;
        hitboxes[0].transform.localPosition = new Vector2(Mathf.Abs(hitboxes[0].transform.localPosition.x) * -pooler.facingDir, hitboxes[0].transform.localPosition.y);
        if (pooler.facingDir == -1)
        {
            theSR.flipX = true;
        }
        else if (pooler.facingDir == 1)
        {
            theSR.flipX = false;
        }
        facingDir = -pooler.facingDir;
    }

    private void OnDisable()
    {
        velocity.x = 0;
        //transform.localScale = new Vector3(1, 1, 1); // temp to return to normal size
        //driftTime = 1f;
    }

    private void Update()
    {
        if (startMove)
        {
            controller.Move(velocity * Time.deltaTime, false);
            //set 
            velocity.x = Mathf.SmoothDamp(velocity.x, maxSpeed * facingDir, ref velocityXSmoothing, acceleration);
        }

        if (controller.collisions.right || controller.collisions.left)
        {
            hitboxHolder.SetActive(false);
            pooler.ReturnToPool(gameObject);
        }
        //Attack----------------------------------------------------------------------
        if (hitboxHolder.activeSelf == true && DSDScript.currentHealth > 0)
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
                        hitboxHolder.SetActive(false);
                        pooler.ReturnToPool(gameObject);

                        SFXScript.ProjectileSFX();
                        //need to add self knockback when hitting an enemy, just once recoil
                    }
                }
            }
        }
        if (hitboxHolder.activeSelf == false)
        {
            oldDetectedObjects.Clear();
        }
        //---------------------------------------------------------------------------------------
    }

    public void SetLifeTime(float lifeTime) => this.lifeTime = lifeTime;
    public void StartMove(int startMove) => this.startMove = (startMove == 1) ? true : false;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
