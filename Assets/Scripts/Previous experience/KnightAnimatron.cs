/*using UnityEngine;
using System.Collections;

public class KnightAnimatron : MonoBehaviour {

	private KnightController knightControl;
	private KnightAction knightAct;
	private Animator anim;
	private float vSpeed;
	private float speed;
	private float move;
	private float hMove;
	private float vMove;
	private float prij;
	private bool grounded;
	private	int climb;
	private bool crouching;
	private int crouchMoment;
	private bool death;
	private bool isItRight;
	private bool doubleJump;
	private int armed;

	private int d;
	private int b;
	private int a;
	private int a1;
	private int b1;
	private int d1;
	private int act;

	void Awake () {
		knightControl = GetComponent<KnightController> ();
		anim = GetComponent<Animator> ();
		knightAct = GetComponent<KnightAction> ();
	}
	

	void Update () {

		anim.SetFloat ("speed", knightControl.move);
		anim.SetFloat ("move", Mathf.Abs (knightControl.move));
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
		anim.SetFloat ("hMove", Mathf.Abs (rigidbody2D.velocity.x));
		anim.SetFloat ("vMove", Mathf.Abs (rigidbody2D.velocity.y));
		anim.SetBool ("grounded", knightControl.grounded);
		isItRight = knightControl.IsItRight;
		anim.SetBool ("right", isItRight);

		crouching = knightControl.crouching;
		climb = knightControl.grabing;
		grounded = knightControl.grounded;
		move = Mathf.Abs(rigidbody2D.velocity.x);
		death = knightControl.death;
		doubleJump = knightControl.doubleJump;
		prij = knightControl.wallTimer * 1f;
		crouchMoment = knightControl.crouchMoment;
		armed = knightControl.armed;
		if ((death)||(knightControl.strongInjured)) {
			if (death)
				d = 1;
			else if (knightControl.strongInjured)
				d=2;
			b = -1;
			a = -1;
		} else
			d = 0;
		if (d == 0) 
		{
			if (!grounded)
			{
				a=0;
				if ((climb>0)&&(knightControl.climb)&&(knightControl.climbUpTime==0))
					b=1;
				else if (knightControl.climbUpTime>0)
					b=4;
				else if (doubleJump)
					b=2;
				else 
					b=3;
			}
			else 
			{
				b=0;
				if (knightControl.slide)
					a=9;
				else if ((crouching)&&(move<0.1f)&&(crouchMoment<=20))
					a=1;
				else if((crouching)&&(move<0.1f))
					a=6;
				else if ((crouching)&&(move>=0.1f))
				    a=5;
				else if (knightControl.fastRun)
					a=8;
				else if (knightControl.armed>0)
					a=12;
				else if (knightControl.injured)
					a=7;
				else if (prij>1f)
					a=2;
				else if (knightControl.iFOTA==1)
					a=10;
				else if (knightControl.iFOTA==2)
					a=11;
				else if (move<0.1f)
					a=3;
				else a=4;
			}
		}

		if ((armed==0)&&(knightAct.actionState==0)) {
			a1 = a;
			b1 = b;
			d1 = d;
		}
		else {
			a1=-2;
			b1=-2;
			d1=-2;
		}
		if (d1<=0)
			act = knightAct.actionState;
		else
			act=0;

		
		anim.SetInteger ("a", a);
		anim.SetInteger ("b", b);
		anim.SetInteger ("d", d);
		anim.SetInteger ("a1", a1);
		anim.SetInteger ("b1", b1);
		anim.SetInteger ("d1", d1);
		anim.SetBool("armed", armed==0);
		anim.SetInteger ("act", act);
		anim.SetInteger ("climb", climb);
	}
}
*/