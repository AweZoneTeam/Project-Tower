/*using UnityEngine;
using System.Collections;

public class KnightController : MonoBehaviour {


	public float maxSpeed;
	bool facingRight = true;
	public bool grounded = false;
	public bool enemied = false;
	public bool doubleJump;
	public Transform groundCheck;
	float groundRadius;
	public LayerMask whatIsGround;
	public LayerMask whatIsRealGround;
	public LayerMask whatIsEnemy;
	public int grabing=0;
	private int grabber;
	public Transform grabCheck;
	float grabRadius;
	public LayerMask[] whatIsGrab;
	public float jumpForce;
	public bool climb;
	public int climbUpTime;
	public float move;
	float movex;
	float movey;
	public float speed;
	public bool groundForward;
	public Transform platformCheck;
	public Transform enemyCheck;
	public BoxCollider2D saveCol;
	public SaveMeNow saveMe;
	public bool crouching;
	public bool fastRun;
	public bool slide;
	public int slideTime;
	public int actionTime;
	public Transform room;
	
	private BoxCollider2D col;
	private CircleCollider2D col1;
	private Stats stats;

	private int climbTime;
	private int climbTimer;
	public int wallTimer;
	private int wallTime;
	private Vector3 forward;
	private bool forced;
	public bool pop;
	public int crouchMoment;
	private bool climbDirection;
	private float jumpDownTime;
	private Transform spot2;
	private Transform spot3;
	private Transform spot4;
	private Transform spot5;
	private KnightAction knightAct;
	public GameObject hitBox;
	private HitController hitControl;
	public bool IsItRight;
	public float prij;
	public int iFOTA;
	public bool death;
	public bool injured;
	public bool strongInjured;
	public int armed;
	public bool active;
	//public bool kk;
	public int jj;
	public int actionTimer;
	public bool prepare;
	public int preparation;
	public int itemPreparation;
	public bool itemPrepare;
	public bool armor;
	public float grav;

	void Awake()
	{
		climb = false;
		wallTimer = 0;
		wallTime = 20;
		grabRadius = 0.2f;
		groundRadius = 0.1f;
		maxSpeed = 15f;
		climbTimer = 20;
		climbTime = 0;
		jumpDownTime = 0.3f;
		doubleJump = false;
		crouching = false;
		death = false;
		crouchMoment = 0;
		armed = -1;
		actionTime=0;
		preparation = 0;
		itemPreparation = 0;
		jj = 0;
		iFOTA = 0;
		slideTime = 0;

		col = GetComponent<BoxCollider2D> ();
		col1 = GetComponent<CircleCollider2D> ();
		stats = GetComponent<Stats> ();
		knightAct = GetComponent<KnightAction> ();
		hitControl = hitBox.GetComponent<HitController> ();
		spot2 = GameObject.FindGameObjectWithTag (Tags.spot2).transform;
		spot3 = GameObject.FindGameObjectWithTag (Tags.spot3).transform;
		spot4 = GameObject.FindGameObjectWithTag (Tags.spot4).transform;
		spot5 = GameObject.FindGameObjectWithTag (Tags.spot5).transform;
	}

	void FixedUpdate(){
		grabing = 0;
		for (int i=0; i<whatIsGrab.Length; i++)
		{	
			if (Physics2D.OverlapCircle (grabCheck.position, grabRadius, whatIsGrab[i]))
				grabing=i+1;
		}
		if (Physics2D.OverlapCircle (groundCheck.position, grabRadius, whatIsGrab[3]))
			iFOTA=1;
		else if (Physics2D.OverlapCircle (groundCheck.position, grabRadius, whatIsGrab[4]))
			iFOTA=2;
		else 
			iFOTA=0;
		death = (stats.health <= 0);
		injured = (stats.injureTime > 0);
		strongInjured = (stats.strongInjureTime > 0);
		forced = ((strongInjured)||(armed>0));
		grav = rigidbody2D.gravityScale;
		stats.isItRight = IsItRight;
		stats.pos = transform.position;
		crouching= (knightAct.crouching||(grounded) && (!climb))&&((Input.GetKey (KeyCode.S)) || (Physics2D.Raycast (spot2.position, new Vector2 (0f, 1f), 5f, whatIsGround))||(Physics2D.Raycast (spot3.position, new Vector2 (0f, 1f), 5f, whatIsGround)));
		if ((rigidbody2D.velocity.x > 0)&&(!forced))
		{
			IsItRight = true;
			if (!slide)
				forward=new Vector3(1f,0f,0f);
		}
		if ((rigidbody2D.velocity.x < 0)&&(!forced))
		{
			IsItRight = false;
			if (!slide)
				forward=new Vector3(-1f,0f,0f);
		}
		if (crouching) 
		{
			if (crouchMoment<=30)
				crouchMoment++;
			col.enabled = false;
			if (!slide) maxSpeed = 7f;
			spot4.position=new Vector3(spot4.position.x,spot2.position.y+1.7f,spot4.position.z);

		} 
		else 
		{
			col.enabled=true;
			if (fastRun)
				maxSpeed=23f;
			else 
				maxSpeed=15f;
			spot4.position=new Vector3(spot4.position.x,spot2.position.y+3.8f,spot4.position.z);
			crouchMoment=0;
		}
		if (stats.strongInjureTime>26)
		{
			rigidbody2D.AddForce(-800*forward);
			climb=false;
		}
		if (death){
			rigidbody2D.gravityScale=1;
			rigidbody2D.velocity=new Vector2 (rigidbody2D.velocity.x,rigidbody2D.velocity.y);
		}
		else if (!climb) {
			rigidbody2D.gravityScale=1;
			grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsRealGround);
			enemied = ((Physics2D.OverlapCircle (new Vector2(enemyCheck.position.x,enemyCheck.position.y), 0.45f, whatIsEnemy))&&(!grounded));
			move = Input.GetAxis ("Horizontal");
			if (slide)
			{	
				if (slideTime<100)
					slideTime++;
				if (Mathf.Abs (move)<0.01*Mathf.Abs(forward.x))
					move=0f;
				else 
					move-=0.01f*slideTime*forward.x;
			}
			else
				slideTime=0;
			groundForward=((Physics2D.Raycast(spot4.position, new Vector2(move, 0f), 1.8f, whatIsGround))||(Physics2D.Raycast(spot5.position, new Vector2(move, 0f), 1.8f, whatIsGround)));
			if (groundForward)
				wallTimer++;
			else
				wallTimer=0;
			pop=(wallTime<wallTimer);
			if (!groundForward)
			{
				if (!forced)
					rigidbody2D.velocity = new Vector2 (move * maxSpeed*knightAct.mSpeed, rigidbody2D.velocity.y*knightAct.mSpeed);
			}
			else 
				rigidbody2D.velocity = new Vector2 (move * 0f*knightAct.mSpeed, rigidbody2D.velocity.y*knightAct.mSpeed);

		}
		else if (climb) {
			rigidbody2D.gravityScale = 0;
			grounded=false;
			enemied=false;
			doubleJump=true;
			movex=0f;
			movey=0f;
			if ((grabing==1)||(grabing==2))
				movey=Input.GetAxis ("Vertical");
			if ((grabing==1)||(grabing>=3))
				movex=Input.GetAxis ("Horizontal");
			climbDirection=false;
			for (int j=0; j<6;j++)
				if (Physics2D.Raycast (grabCheck.position, new Vector2(movex,movey),2f, whatIsGrab[j]))
					climbDirection=true;
			if (((Input.GetKeyDown(KeyCode.W))&&(grabing>3))||(climbUpTime>0))
			{
				if (climbUpTime==0)
				{
					grabber=grabing;
					climbUpTime+=42;
				}
				col1.enabled=false;
				col.enabled=false;
				movex=0f;
				movey=0.6f;
				if (grabber<6)
					movex=0.3f*(grabber-4.5f)*(-2);
				climbUpTime--;
				if (climbUpTime==0)
				{
					climb=false;
					climbTime=0;
					col1.enabled=true;
					col.enabled=true;
				}
				climbDirection=true;
			}
			if (climbDirection)
				rigidbody2D.velocity=new Vector2(movex*maxSpeed*knightAct.mSpeed*0.5f,movey*maxSpeed*knightAct.mSpeed*0.5f);
			else
				rigidbody2D.velocity=new Vector2(0f,0f);
		}
		if ((knightAct.actionState>0)&&(actionTime<knightAct.timer))
		{
			crouching=knightAct.crouching;
			rigidbody2D.gravityScale=knightAct.grav;
			hitControl.size=knightAct.hitSize;
			if (knightAct.backStab)
				hitControl.backStab=knightAct.backStabKoof;
			else
				hitControl.backStab=1;
			if (knightAct.rangeAttack)
			{
				if (actionTime==knightAct.hitTimeB)
				{	
					Transform gun;
					if (IsItRight)
						gun=knightAct.gunR;
					else 
						gun=knightAct.gunL;
					GameObject bullet;
					bullet=Instantiate(knightAct.bullet, gun.position, gun.rotation) as GameObject;
					Vector2 vect;
					if (IsItRight)
						vect=new Vector2(transform.right.x,transform.right.y);
					else
						vect=new Vector2(-1*transform.right.x, transform.right.y);
					bullet.rigidbody2D.AddForce(itemPreparation*80*vect);
					itemPreparation=0;
					stats.bomb--;

				}
			}
			if ((knightAct.actionState==18)&&(actionTime==knightAct.hitTimeB))
			{
				stats.health+=2;
				stats.potion--;
			}
			if (IsItRight)
				hitControl.pos=knightAct.hitCenterR;
			else
				hitControl.pos=knightAct.hitCenterL;
			hitControl.damage=knightAct.damage;
			if (((actionTime>=knightAct.hitTimeB)&&(actionTime<knightAct.hitTimeEnd))||((knightAct.fallen)&&(rigidbody2D.velocity.y<knightAct.vSpeed)))
				hitControl.activated=true;
			else
				hitControl.activated=false;
		}
		actionTimer = knightAct.timer;
		if (actionTime>=knightAct.perehodTimer)
		    knightAct.prevActionState=knightAct.actionState;
		if ((actionTime < actionTimer+1) && (knightAct.actionState > 0))
						actionTime++;
		else if (knightAct.actionState > 0) 
		{
			ChangeState (0);
			actionTime=0;
		}
		if ((prepare)&&(grounded)&&(!itemPrepare)&&(preparation<45)&&(!crouching)&&(!fastRun)) 
		{
			preparation++;
		}
		if ((!prepare)&&(itemPrepare)&&(itemPreparation<15)&&(!crouching)&&(!fastRun)) 
		{
			itemPreparation++;
		}
		if ((crouching)||(fastRun))
		{
			prepare=false;
			preparation=0;
			itemPrepare=false;
		}
		if ((prepare)&&(grounded)&&(!crouching)&&(preparation>4))
			ChangeState(8);
		if ((!prepare)&&(grounded))
		{
			if (preparation>40)
			{
				ChangeState(9);	
				preparation=0;
			}
			else if (preparation>0)
			{
				if (knightAct.prevActionState==2)
					ChangeState(3);
				else 
					ChangeState(2);
				preparation=0;
			}
		}
		if ((armor)&&(!itemPrepare)&&(!prepare)&&(!crouching)&&(grounded)&&(!fastRun)&&(stats.armorForce>3))
		{
			ChangeState(13);
			stats.armor=true;
		}
		else if ((armor)&&(!itemPrepare)&&(!prepare)&&(grounded)&&(!fastRun)&&(!crouching))
		{
			stats.armor=false;
			if ((knightAct.prevActionState==13)&&(knightAct.actionState==0))
				ChangeState (16);
		}
		else if (stats.armor)
		{
			stats.armor=false;
			ChangeState(14);
		}
			 
		if ((stats.itemNumber==0)&&(itemPrepare) && (!crouching)&&(!fastRun)&&(itemPreparation>1))
			ChangeState (11);
		if ((!itemPrepare)&&(stats.itemNumber==0))
		{
			if (itemPreparation>2)
				ChangeState (12);
			else
				itemPreparation=0;
		}
		if ((knightAct.actionState == 10) && (grounded))
		{
			ChangeState (0);
			hitControl.activated=false;
		}
		speed = rigidbody2D.velocity.x;
		if (climbTime<climbTimer)
			climbTime++;
		actionTimer = knightAct.timer;
	}

	
	void Update () {
		if (saveMe.what)
			if (Input.GetKeyDown (KeyCode.E))
				saveMe.active = true;
		if (grounded)
			doubleJump = false;
		slide = ((fastRun) && (grounded) && (!climb))&&((Input.GetKey (KeyCode.S)));
		fastRun = ((Input.GetKey (KeyCode.LeftShift)&&(Mathf.Abs(rigidbody2D.velocity.x)>0.01f)&&(grounded))
		           ||(knightAct.actionState==17)
		           ||(slide));
		if (Input.GetKey (KeyCode.S))
		{
			if (grounded && Input.GetKeyDown (KeyCode.Space))
				StartCoroutine(JumpDown());
		}
		else if ((grounded||!doubleJump) && Input.GetKeyDown (KeyCode.Space)) {
			if (!grounded) 
			{
				doubleJump=true;
				rigidbody2D.velocity=new Vector2(rigidbody2D.velocity.x,0);
			}
			rigidbody2D.AddForce (new Vector2 (0, jumpForce));
		}
		if (Input.GetKeyDown(KeyCode.Tab))
			StartCoroutine(ChangeItem());
		if ((Input.GetKeyDown (KeyCode.Q))&&(grounded)&&(stats.itemNumber==1)&&(knightAct.actionState==0))
			ChangeState(18);
		if (((grabing>0) && Input.GetKeyDown (KeyCode.E) && (!climb)&& climbTime>=6)||(grabing>=3)&&(climbTime>15)&&(climbTime<21)&&(rigidbody2D.velocity.y<-7)) {
			climb=true;
			climbTime=0;
			if (grabing>=3)
				climbTime=21;
		}
		if ((grabing>0) && Input.GetKeyDown (KeyCode.E) && (climbUpTime==0)&& (climb)&& climbTime>=6) {
			climb=false;
			climbTime=0;
		} 
		if ((grabing==0)&&(climbUpTime==0)) {
			climb=false;
		}
		if (enemied) 
			transform.position=Vector3.Lerp(transform.position,new Vector3(transform.position.x+20*forward.x,transform.position.y,transform.position.z),Time.deltaTime);
		if ((knightAct.actionState==0)&&(armed==0) && (Input.GetKeyDown (KeyCode.F))) 
		{
			prepare=true;
		}
		if ((armed==0) && (Input.GetKeyUp (KeyCode.F))) 
		{
			prepare=false;
		}
		if ((stats.bomb>0)&&(knightAct.actionState==0)&&(stats.itemNumber==0)&&(armed==0) && (Input.GetKeyDown (KeyCode.Q))) 
		{
			itemPrepare=true;
		}
		if ((armed==0) &&(stats.itemNumber==0)&& ((Input.GetKeyUp (KeyCode.Q))||(itemPreparation==15))) 
		{
			itemPrepare=false;
		}
		armor = Input.GetKey (KeyCode.R);
		if ((Input.GetKeyDown (KeyCode.F)) && (armed!=0))
			armed +=30;
		if (armed>0)
			armed--;
		if ((!death)&&(armed==0)&&(!slide)&& (Input.GetKeyDown (KeyCode.F))&&((knightAct.actionState==0)||(knightAct.prevActionState!=0))) 
		{	
			if (knightAct.prevActionState!=0)
			{
				knightAct.actionState=0;
				actionTime=0;
			}
			if (crouching)
				ChangeState (1);
			else if (fastRun)
				ChangeState(17);
			else if ((grounded)&&(!prepare)&&(preparation<=4))
			{
				if (knightAct.prevActionState==2)
					ChangeState(3);
				else 
					ChangeState(2);
				preparation=0;
			}
			else if (climb)
			{
				ChangeState(15);
			}
			else if (!climb)
			{
				if (Input.GetKey(KeyCode.S))
					ChangeState(10);
				else if (knightAct.prevActionState==4)
				    ChangeState(5);
				else if ((knightAct.prevActionState==5)||(knightAct.prevActionState==6)||(knightAct.prevActionState==7))
				{
					if ((knightAct.prevActionState==5)||(knightAct.prevActionState==7))
					    ChangeState(6);
					else ChangeState(7);
				}
				else if((!climb)&&(!grounded)) ChangeState(4);
			}
		}
		if ((grounded) &&((knightAct.prevActionState>=4)&&(knightAct.prevActionState<=7)))
		{
			knightAct.prevActionState=0;
		}
		/*if (knightAct.prevActionState == 12)
			ChangeState (13);*/
		/*if ((itemPrepare) && (!prepare) && (grounded))
			ChangeState (11);
		if ((!itemPrepare) && (grounded) && (itemPreparation > 0))
			ChangeState (12);
	}
	

	void OnTriggerEnter2D(Collider2D other)
	{
		if ((other.gameObject.tag == Tags.room) && (room != other.gameObject.transform))
			room = other.gameObject.transform;
	}
	
	IEnumerator JumpDown()
	{
		platformCheck.position = new Vector3 (platformCheck.position.x, platformCheck.position.y - 1f, platformCheck.position.z);
		yield return new WaitForSeconds (jumpDownTime);
		platformCheck.position = new Vector3 (platformCheck.position.x, platformCheck.position.y + 1f, platformCheck.position.z);

	}
	void ChangeState (int x)
	{
		int y = knightAct.actionState;
		knightAct.actionState = x;
		knightAct.prevActionState = y;
	}
	IEnumerator ChangeItem()
	{
		yield return new WaitForSeconds (1);
		if ((stats.itemNumber==0)&&(stats.potion>0)) 
			stats.itemNumber=1;
		else if ((stats.itemNumber==1)&&(stats.bomb>0))
			stats.itemNumber=0;
	}
}*/