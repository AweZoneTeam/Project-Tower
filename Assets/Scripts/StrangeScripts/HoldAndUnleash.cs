using UnityEngine;
using System.Collections;

public class HoldAndUnleash : MonoBehaviour {
	
	private ActivityClass.activites activity;
	private WeaponClass weapon;
	private RootCharacterController controller;
	private HitController hitBox;
	private Stats stats;
	private SpFunctions sp;
	private clavisher clav;
	
	private int numb;
	private int bTime;
	private int time;
	
	public int eTime;
	private int claveNumb;
	public int stage;
	public int kk;
	
	public void SetValues(RootCharacterController c,
	                      WeaponClass w,
	                      int n, int b, int e, int cN,
	                      SpFunctions s,
	                      HitController h, 
	                      clavisher cl)
	{
		bTime = b; eTime = e; controller = c; weapon = w; numb = n; claveNumb = cN;
		activity = weapon.moveset [numb];
		stats = controller.gameObject.GetComponent<Stats> ();
		sp = s;
		hitBox = h;
		clav = cl;
		time = 0;
	}
	
	public void Work()
	{

		int i;
		if (clav.buttons[claveNumb].push > 0) 
			stage = 1;
		else
			stage = 2;
		controller.whatToPerform.Add (activity.howLook[stage-1].anim);
		if (stage == 1) {
			hitBox.actTime++;
			time++;
		} 
		else 
		{
			if (time<bTime)
				eTime=0;

		}
		if ((stage!=0)&&(eTime!=0))
		{
			if (stage==2)
				eTime--;
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon)&&(eTime!=0))
					sp.ChangeTimer(sp.EmployTime(controller.whatToEmploy [i],controller.actions.activities)+1,
					               controller.whatToEmploy [i],controller.actions.activities);
		}
		if (eTime == 0)
			stage = 0;
	}
}
