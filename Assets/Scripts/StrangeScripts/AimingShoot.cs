using UnityEngine;
using System.Collections;

public class AimingShoot : MonoBehaviour {

	private ActivityClass.activites activity;
	private WeaponClass weapon;
	private RootCharacterController controller;
	private Organism stats;
	private SpFunctions sp;
	private clavisher clav;
	
	private int numb;
	private int clavNumb1;
	private int clavNumb2;
	public int bTime;
	private int time;
	public int eTime;
	public int stage;
	public float shootForce;
	public float maxAimDistance;
	public float aimSpeed;
	public GameObject aim;
	public GameObject obj;

	private Vector3 pos;
	private Vector3 nextPos;
	

	public void SetValues(RootCharacterController c,
	                      WeaponClass w,
	                      int n, int cN1, int cN2, int b, int e,
	                      SpFunctions s,
	                      clavisher cl, Vector3 p)
	{
		bTime = b; eTime = e; controller = c; weapon = w; numb = n; clavNumb1 = cN1; clavNumb2 = cN2;
		activity = weapon.moveset [numb];
		stats = controller.gameObject.GetComponent<Organism> ();
		sp = s;
		clav = cl;
		pos = new Vector3 (p.x, p.y, p.z);
	}
	
	public void Work()
	{
		int i,j;
		Vector2 direct;
		Vector2 forward;
		float angle=0f;
		if (clav.buttons[clavNumb1].push>0)
		{
			if (bTime>0)
			{
				stage=1;
				bTime--;
			}
			else
				stage=2;
		}
		else 
			stage=3;
		if ((stage==3)&&(bTime>0))
		{
			eTime=0;
			bTime=0;
			stage=0;
		}

		switch (stage) 
		{
		case 1:
			controller.whatToPerform.Add (activity.howLook [0].anim);
			controller.animNumb = controller.whatToPerform.Count;
			if (bTime==0) 
				obj=Instantiate(aim,pos,aim.transform.rotation) as GameObject;
			break;
		case 2:
			nextPos=new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
			if (clav.buttons[clavNumb2].push>0)
				nextPos.x+=aimSpeed;
			else if (clav.buttons[clavNumb2+2].push>0)
				nextPos.x-=aimSpeed;
			if (clav.buttons[clavNumb2+1].push>0)
				nextPos.y+=aimSpeed;
			else if (clav.buttons[clavNumb2+3].push>0)
				nextPos.y-=aimSpeed;
			if ((stats.direction*(nextPos.x-pos.x)<0)||
			    (Vector2.Distance(sp.VectorConvert(pos),sp.VectorConvert(nextPos))>=maxAimDistance))
				nextPos=new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
			obj.transform.position=new Vector3(nextPos.x,nextPos.y,nextPos.z);
			direct=new Vector2(nextPos.x-pos.x, nextPos.y-pos.y).normalized;
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon))
				{
					controller.whatToEmploy[i].whatIf[0].PRM[9]=Mathf.Abs(Mathf.RoundToInt(shootForce*direct.x));
					controller.whatToEmploy[i].whatIf[0].PRM[10]=Mathf.RoundToInt(shootForce*direct.y);
				}
			forward=new Vector2(stats.direction*1f, 0f);
			angle=stats.direction*sp.RealAngle(forward,direct);
			for (j=1;j<=9;j++)
				if (angle<=activity.howLook[j].speedY)
					break;
			controller.whatToPerform.Add (activity.howLook [j].anim);
			controller.animNumb = controller.whatToPerform.Count;
			break;
		case 3:
			if (obj!=null)
			{
				for (j=10;j<=18;j++)
					if (angle<=activity.howLook[j].speedY)
						break;
				controller.whatToPerform.Add (activity.howLook [j].anim);
				controller.animNumb = controller.whatToPerform.Count;
				Destroy(obj);
				obj=null;
			}
			eTime--;
			break;
		default:
			break;
		}
		if ((stage!=0)&&(stage<3))
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon)&&(eTime!=0))
					sp.ChangeTimer(sp.EmployTime(controller.whatToEmploy [i],controller.actions.activities)+1,
					               controller.whatToEmploy [i],controller.actions.activities);
		if (eTime == 0)
			stage = 0;
	}
}
