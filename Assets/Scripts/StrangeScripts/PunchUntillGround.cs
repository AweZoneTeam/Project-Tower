using UnityEngine;
using System.Collections;

public class PunchUntillGround : MonoBehaviour {

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
	public int kk;

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
		kk++;
		stage = 1;
	}

	public void Work()
	{
		int i;
		if ((bTime==0)&&(stats.stats.groundness >= 3))
			stage = 2;
		if ((stats.stats.groundness < 3)&&(eTime>0))
			stage = 3;
		//controller.animNumb=sp.AddAnimation(controller.whatToPerform, activity.howLook[stage-1].anim, controller.animNumb);
		controller.whatToPerform.Add (activity.howLook[stage-1].anim);
		controller.animNumb = controller.whatToPerform.Count;
		if (stage<3)
		{
			if (stage==2)
				hitBox.actTime++;
			else bTime--;
		}
		else
			eTime--;
		if (stage!=0)
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon)&&(eTime!=0))
					sp.ChangeTimer(sp.EmployTime(controller.whatToEmploy [i],controller.actions.activities)+1,
					               controller.whatToEmploy [i],controller.actions.activities);
		if (eTime == 0)
			stage = 0;
	}
}
