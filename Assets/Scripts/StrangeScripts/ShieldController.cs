using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour {


	public bool active;
	public int pDefence, fDefence, aDefence, dDefence;
	public float speed;
	public float acceleration;
	private float targetSpeed;

	private ActivityClass.activites activity;
	private WeaponClass weapon;
	private RootCharacterController controller;
	private Organism stats;
	private SpFunctions sp;
	private clavisher clav;
	private Rigidbody2D rigid;

	private int numb;
	private int bTime;
	private int time;
	private int animNumb;

	private int claveNumb;
	public int stage;
	
	public void SetValues(RootCharacterController c,
	                      WeaponClass w,
	                      int n, int b, int cN,
	                      SpFunctions s,
	                      clavisher cl)
	{
		bTime = b; controller = c; weapon = w; numb = n; claveNumb = cN;
		activity = weapon.moveset [numb];
		stats = controller.gameObject.GetComponent<Organism> ();
		stats.shield = this;
		sp = s;
		clav = cl;
		time = 0;
		rigid = controller.gameObject.GetComponent<Rigidbody2D>();
		animNumb = 0;
	}
	
	public void Work()
	{
		
		int i;
		if (clav.buttons[claveNumb].push > 0) 
		{
			if (time<bTime+5)
				time++;
		}
		else 
			time=0;
		if ((time > 0) && (time < bTime)) 
			stage = 1; 
		else if (time>0)
			stage=2;
		else 
		{
			stage=0;
			active=false;
			stats.shield=null;
		}
		if (stage==2)
		{
			active=true;
			if (clav.buttons[2].timer>clav.buttons[3].timer)
				targetSpeed=speed;
			else if (clav.buttons[2].timer<clav.buttons[3].timer)
				targetSpeed=-1*speed;
			else 
				targetSpeed=0f;
			if (Mathf.Abs(targetSpeed*1f)!=1f)
			{
				if (Mathf.Abs(rigid.velocity.x-targetSpeed*1f)>0.5f) 
					stats.stats.currentSpeed = Vector2.Lerp (stats.stats.currentSpeed, new Vector2 (targetSpeed * 1f, rigid.velocity.y), acceleration * Time.deltaTime);
				else
					stats.stats.currentSpeed=new Vector2(targetSpeed*1f,rigid.velocity.y);
				rigid.velocity = new Vector2 (stats.stats.currentSpeed.x, rigid.velocity.y);
			}
		}

		if (stage == 1) animNumb = 0;
		else if (stage==2)
		{
			if (targetSpeed*stats.direction>0) animNumb=2; 
			else if (targetSpeed*stats.direction<0) animNumb=3;
			else animNumb=1;
		}

		//controller.animNumb=sp.AddAnimation(controller.whatToPerform, activity.howLook[animNumb].anim, controller.animNumb);
		controller.whatToPerform.Add (activity.howLook[animNumb].anim);
		controller.animNumb = controller.whatToPerform.Count;
		if (stage>0)
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon))
					sp.ChangeTimer(sp.EmployTime(controller.whatToEmploy [i],controller.actions.activities)+1,
					               controller.whatToEmploy [i],controller.actions.activities);

	}

}
