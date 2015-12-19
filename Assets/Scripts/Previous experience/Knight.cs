using UnityEngine;
using System.Collections;

public class Knight : MonoBehaviour {

	public float maxSpeed = 10f;
	bool facingRight = true;
    bool grounded = false;
	public Transform groundCheck;
	float groundRadius=0.1f;
	public LayerMask whatIsGround;
    public bool grabing=false;
	bool grabing2=false;
	public Transform grabCheck;
	float grabRadius = 0.1f;
	float grabRadius2 = 0.3f;
	public LayerMask whatIsGrab;
	float jumpForce=550f;
	public int climb=0;
	float move;
	float movex;
	float movey;
	public int grabTiming=0;
	public float grabSpeed=5f;
	void Start () {
		
	}
	
	void FixedUpdate(){
		grabTiming += 1;
		if (grabTiming > 60) 
			grabTiming = 30;
		grabing = Physics2D.OverlapCircle (grabCheck.position, grabRadius, whatIsGrab);
		grabing2 = Physics2D.OverlapCircle (grabCheck.position, grabRadius2, whatIsGrab);
		if (climb == 0) {
			rigidbody2D.WakeUp();
			grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
			move = Input.GetAxis ("Horizontal");
			rigidbody2D.velocity = new Vector2 (move * maxSpeed, rigidbody2D.velocity.y);
			if ((move < 0) && (facingRight)) 
				Flip ();
			if ((move > 0) && (!facingRight))
				Flip ();
		}
		if (climb == 1) {
			rigidbody2D.Sleep();
			grounded=false;
			movey=Input.GetAxis ("Vertical");
			movex=Input.GetAxis ("Horizontal");
			if (grabing2) rigidbody2D.velocity=new Vector2(movex*grabSpeed,movey*grabSpeed);
		}
	}
	
	
	void Update () {
		if (grounded && Input.GetKeyDown (KeyCode.Space)) {
			rigidbody2D.AddForce (new Vector2 (0, jumpForce));
		}
		if ((grabTiming>=30) && grabing && Input.GetKeyDown (KeyCode.E) && (climb == 0)) {
			climb=1;
			grabTiming=0;
		}
		if ((grabTiming>=30)&& Input.GetKeyDown (KeyCode.E) && (climb == 1)) {
			climb=0;
			grabTiming=0;
		} 
	}
	
	void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	} 
}
