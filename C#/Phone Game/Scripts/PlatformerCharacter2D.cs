using UnityEngine;
using System.Collections;

public class PlatformerCharacter2D : MonoBehaviour {
	
	[SerializeField]float jumpForce = 900f;
	[SerializeField]float dashForce = 0f;
	[SerializeField]float hopForce = 0f;
	[SerializeField]float speed = 15f;
	public float springForce;

	[SerializeField]LayerMask whatIsGround;
	[SerializeField]LayerMask whatIsSnowGround;
	Transform groundCheck;
	float groundedRadius = 0.0425f;
	bool grounded = false;

	public Animator anim;

	private bool jump;
	private bool doubleJump;
	private bool dash = false;
	private bool airDash = false;
	private bool spring = false;
	public bool hit = false;
	public bool reloadDone = true;
	public bool hitDone = false;
	float hitCounter = 0;

	float timer = 0;
	float reloadTimer = 0;
	public float counter = 0;

	public Transform objectTransfom;
	private Vector2 noMovementThreshold = new Vector2(0.001f, 0);
	private const int noMovementFrames = 3;
	Vector2[] previousLocations = new Vector2[noMovementFrames];

	ParticleSystem groundParticle;
	ParticleSystem landParticle;
	ParticleSystem springLandParticle;
	ParticleSystem dashParticle;
	bool partTrue = false;
	float partTimer = 1;
	public bool airTime = false;
	public bool springPart = false;

	BoxCollider2D[] box;
	CircleCollider2D circle;

	bool snowGround = false;
	bool command = false;
	bool startTimer = false;

	public GameObject audioS;
	//public AudioClip clip;
	//public AudioClip springSound;
	//public AudioClip dashSound;
	//public AudioClip breakSound;

	void Start() {
		box = GetComponents<BoxCollider2D>();
		circle = GetComponent<CircleCollider2D>();

		for(int i = 0; i < previousLocations.Length; i++)
		{
			previousLocations[i].x = Vector2.zero.x;
		}

		groundCheck = transform.Find("GroundCheck");
		anim = GetComponent<Animator>();

		groundParticle = GameObject.Find("PlayerGroundParticle").GetComponentInChildren<ParticleSystem>();
		landParticle = GameObject.Find("PlayerLandParticle").GetComponentInChildren<ParticleSystem>();
		springLandParticle = GameObject.Find("PlayerSpringLandParticle").GetComponentInChildren<ParticleSystem>();
		dashParticle = GameObject.Find("PlayerDashParticle").GetComponentInChildren<ParticleSystem>();
	}


	void Update(){
		if(Input.GetButtonDown("Jump") && !hit && command){
			jump = true;
		}

		if(spring){
			dash = false;
			airDash = false;
			dashForce = 0f;
			hopForce = 0f;
		}

		if(Input.GetButtonDown("Dash") && !dash && !airDash && reloadDone && !hit && command && !spring){
			audioS.SendMessage("Dash");
			if(grounded)
				dash = true;
			if(!grounded)
				airDash = true;
		}


		if((dash || airDash) && !spring && !hit && reloadDone && command){
			gameObject.tag = "Break";
			dashParticle.Play();
			dashForce = 55f;
			startTimer = true;
			//timer += Time.deltaTime;
		}
		if(startTimer)
			timer += Time.deltaTime;
		if(timer > 0.075f){
			dashParticle.Stop();
			dashForce = 0f;
			if(timer > 0.080f){
				startTimer = false;
				reloadDone = false;
				counter = 1;
				dash = false;
				airDash = false;
				gameObject.tag = "Player";
				timer = 0;
			}
		}
		if(partTrue){
			partTimer += Time.deltaTime;
			if(partTimer > 0.5f){
				groundParticle.Play();
				partTimer = 0;
			}
		}
		if(spring){
			springPart = true;
		}
		if(grounded && springPart && airTime){
			audioS.SendMessage("SpringLand");
			springLandParticle.Play();
			airTime = false;
		}

		if(grounded && airTime && !springPart){
			landParticle.Play();
			airTime = false;
		}

		if(hit){
			gameObject.tag = "Hit";
			dashForce = 0;
			box[1].enabled = false;
			box[0].enabled = false;
			circle.enabled = false;
			if(hitCounter == 0){
				hitDone = true;
			}
			Hit();
		}
		if(snowGround){
			audioS.SendMessage("Grounded");
		}
		//Debug.Log(rigidbody2D.velocity.y);
		//Death
	}

	void FixedUpdate() {
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
		snowGround = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsSnowGround);

		anim.SetBool("Ground", grounded);
		anim.SetBool("Dash", dash || airDash);
		anim.SetBool("Spring", spring);

		Move(dashForce, jump, doubleJump, dash);
		jump = false;
		doubleJump = false;

		if(!hit){
			for(int i = 0; i < previousLocations.Length - 1; i++)
			{
				previousLocations[i].x = previousLocations[i+1].x;
			}
			previousLocations[previousLocations.Length - 1].x = objectTransfom.position.x;
			
			for(int i = 0; i < previousLocations.Length - 1; i++)
			{
				//Debug.Log(previousLocations[i + 1].x - previousLocations[i].x);
				if((previousLocations[i + 1].x - previousLocations[i].x) >= noMovementThreshold.x)
				{
					hit = false;
				}
				else
				{
					hit = true;
				}
			}
		}
		if(!airTime){
			landParticle.Stop();
			springLandParticle.Stop();
			springPart = false;
		}

		if(!reloadDone){
			reloadTimer += Time.deltaTime;
		}
		if(reloadTimer > .25f){
			reloadDone = true;
			reloadTimer = 0;
		}
	}

	void OnTriggerEnter2D (Collider2D other){
		if(other.tag == "Spring"){
			jump = false;
			dash = false;
			airDash = false;
			springForce = 2200f;
		}
		if(other.gameObject.layer == LayerMask.NameToLayer("Glass")){
			if(gameObject.tag != "Break")
				hit = true;
		}
		if(other.tag == "Hit" || other.gameObject.layer == LayerMask.NameToLayer("Ground")){
			hit = true;
		}
		if(other.tag == "BreakGround"){
			other.tag = "Breaking";
		}
		if(other.tag == "CameraNormal" && !hit)
			gameObject.tag = "Default";
	}

	public void Move(float dashForce, bool jump, bool doubleJump, bool dash){

		if(dashForce > 1f){
			hopForce = 1f;
		}
		else{
			hopForce = 0f;
		}

			GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y + hopForce);

		if(grounded){
			spring = false;
			counter = 0;
		}
		if(grounded && GetComponent<Rigidbody2D>().velocity.x > 0){
			partTrue = true;
		}else{
			partTimer += 1;
			partTrue = false;
			groundParticle.Stop();
		}
		//if(rigidbody2D.velocity.y < 0){
		//	spring = false;
		//}

		if(springForce > 1){
			spring = true;
		}

		if((dash || airDash) && counter == 0){
			GetComponent<Rigidbody2D>().velocity = new Vector2(speed + dashForce, GetComponent<Rigidbody2D>().velocity.y + hopForce);
		}

	
		if((grounded) && jump){
			if(grounded){
				doubleJump = false;
			}
			audioS.SendMessage("Jump");
			//AudioSource.PlayClipAtPoint(clip, this.transform.position);
			GetComponent<Rigidbody2D>().velocity = new Vector2(speed,0);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
		}
		if(doubleJump){
			GetComponent<Rigidbody2D>().velocity = new Vector2(speed,0);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
		}
		if(spring && springForce > 1){
			audioS.SendMessage("Spring");
			//AudioSource.PlayClipAtPoint(springSound, this.transform.position);
			GetComponent<Rigidbody2D>().velocity = new Vector2(speed,0);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, springForce));
			springForce = 0f;
		}

		if(!grounded){
			audioS.SendMessage("Ungrounded");
			airTime = true;
		}
	}

	void Hit(){
		dashParticle.Stop();
		dash = false;
		airDash = false;
		if(hitDone){
			audioS.SendMessage("Dead");
			anim.SetBool("Hit", hit);
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1000));
			hitCounter = 1;
			hitDone = false;
		}
		if(this.transform.position.y <= -777){
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			GetComponent<Rigidbody2D>().transform.position = new Vector2(0, -500);
			GetComponent<Rigidbody2D>().gravityScale = 0;
		}
	}

	void Jump(){
		if(!hit){
			jump = true;
		}
	}

	void Swipe(){
		if(!hit && !dash && !airDash && reloadDone && !spring){
			audioS.SendMessage("Dash");
			//AudioSource.PlayClipAtPoint(dashSound, this.transform.position);
			if(grounded)
				dash = true;
			if(!grounded)
				airDash = true;
		}
	}

	void Break(){
		audioS.SendMessage("Break");
		//AudioSource.PlayClipAtPoint(breakSound, this.transform.position);
	}

	void Command(){
		command = true;
	}
}