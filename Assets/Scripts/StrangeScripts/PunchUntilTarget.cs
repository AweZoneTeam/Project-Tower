using UnityEngine;
using System.Collections;

public class PunchUntilTarget : MonoBehaviour {

	
	private ActivityClass.activites activity;
	private WeaponClass weapon;
	private RootCharacterController controller;
	private HitController hitBox;
	private Stats stats;
	private SpFunctions sp;
	
	private int numb;
	private int bTime;
	
	public int eTime;
	public int stage;
	public float speed;
	
	public void SetValues(RootCharacterController c,
	                      WeaponClass w,
	                      int n, int b, int e,
	                      SpFunctions s,
	                      HitController h)
	{
		bTime = b; eTime = e; controller = c; weapon = w; numb = n;
		activity = weapon.moveset [numb];
		stats = controller.gameObject.GetComponent<Stats> ();
		sp = s;
		hitBox = h;
	}
	
	public void Work()
	{
		int i;
		if (bTime > 0) 
			stage = 1;
		else if (eTime > 0) 
		{
			eTime--;
			stage = 2;
			if (hitBox.onTarget)
			{
				eTime=0;
				stage=3;
			}
		}
		else
			stage = 3;
		controller.whatToPerform.Add (activity.howLook[stage-1].anim);
		if (stage<3)
		{
			if (stage==2)
			{
				hitBox.actTime++;
				controller.gameObject.GetComponent<Rigidbody2D>().velocity=new Vector2(speed*stats.direction,
				                                                                       controller.gameObject.GetComponent<Rigidbody2D>().velocity.y);
			}
			else bTime--;
		}
		if (stage==2)
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon)&&(eTime!=0))
					sp.ChangeTimer(sp.EmployTime(controller.whatToEmploy [i],controller.actions.activities)+1,
					               controller.whatToEmploy [i],controller.actions.activities);
		if (stage==3)
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon)&&
				    (sp.EmployTime(controller.whatToEmploy [i],controller.actions.activities)==1))
					stage = 0;
	}
}
