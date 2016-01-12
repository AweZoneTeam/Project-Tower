using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RootCharacterController : NAAObjectController {

	public List<ActivityClass.activites> whatToEmploy;
	public List<ActionClass.act> whatToDo; 
	public List<animClass.anim> whatToPerform;
	//public int animNumb;
	public float kk;
	public int jj1;
	public int jj2;
	public PartController kkPart;
	
	private Rigidbody2D rigid;
	private Stats stats;
	private Equipment equip;
	private CharacterAnimator animator;
	public Actions actions;
	private InfoGets infoGets;
	private clavisher clav;
	private SpFunctions Sp;

	int i, e, j;

	void Awake () 
	{
		whatToDo.Clear ();
		rigid = gameObject.GetComponent<Rigidbody2D> ();
		stats = gameObject.GetComponent<Stats> ();
		animator = GetComponent<CharacterAnimator> ();
		actions = gameObject.GetComponent<Actions> ();
		infoGets = gameObject.GetComponent<InfoGets> ();
		clav = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<clavisher> ();
		Sp=GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
		equip = gameObject.GetComponent<Equipment> ();

	}

	public void LearnActivities()// Метод, в котором персонаж узнаёт, какой деятельностью заняться. Деятельность - то, что однозначно характеризует, что совершает персонаж
	{
				j = Sp.ChemByZanyatsya (actions.activities, stats, 12, clav);
		while (stats.stats.employment<0) 
		{
			stats.stats.employment+=whatToEmploy[0].employ;
			if (whatToEmploy[0].weapon==null) 
				actions.activities[whatToEmploy[0].numb].chosen=false; 
			else 
				whatToEmploy[0].weapon.moveset[whatToEmploy[0].numb].chosen=false;
			whatToEmploy.Remove(whatToEmploy[0]);
		}

	}

	public void PrepareEquipment()//Метод, в котором персонаж входит в боевую стадию, или выходит из неё
	{
		if (equip.rightWeapon!=null)
		{
			if ((!equip.rightWeapon.active)&&(!battleStance))
				equip.rightWeapon.ready=-2;
			else if ((battleStance)&&(!equip.rightWeapon.active))
				equip.rightWeapon.ready=0;
			else if ((equip.rightWeapon.active)&&(!battleStance)) 
				equip.rightWeapon.ready=-1;
			else if (equip.rightWeapon.ready<2) 
				equip.rightWeapon.ready=1;
		}
		if (equip.leftWeapon!=null)
		{
			if ((!equip.leftWeapon.active)&&(!battleStance))
				equip.leftWeapon.ready=-2;
			else if ((battleStance)&&(!equip.leftWeapon.active))
				equip.leftWeapon.ready=0;
			else if ((equip.leftWeapon.active)&&(!battleStance))
				equip.leftWeapon.ready=-1;
			else if (equip.leftWeapon.ready<2) 
				equip.leftWeapon.ready=1;
		}
		if ((equip.leftWeapon == null) && (equip.rightWeapon == null))
			battleStance = false;
	}

	public void LearnActions()//Персонаж узнаёт, какие действия совершать. Деятельность содержит множество действий - элементарных функций, которые совершает персонаж
	{
		ActivityClass.activites act;
		for (i=0; i<whatToEmploy.Count; i++)
		{
			if (whatToEmploy[i].weapon==null) 
				act=actions.activities[whatToEmploy[i].numb];
			else 
				act=whatToEmploy[i].weapon.moveset[whatToEmploy[i].numb];
			if (((act.timeToReverse>0)&&
			     (act.timeOfAction-act.timer==act.timeToReverse))||
			    (!(((act.actMode==1)||(act.actMode==5))&&(act.timer<act.timeOfAction))))
			{
				if ((act.actMode!=3)&&
				    (act.timeToReverse>0)&&(act.timeOfAction-act.timer==act.timeToReverse))
				{
					for (j=0; j<act.whatIf.Length;j++)
					{
						if (act.whatIf [j].actType == 0) {
							whatToDo.Add (act.whatIf [j]);
							//actNumb=Sp.AddAction(whatToDo,act.whatIf[j],actNumb);
						
						} else 
						{
							e=0;
							while ((whatToDo[e].actType!=act.whatIf[j].actType)&&(e<whatToDo.Count))
								e++;
							if (e!=whatToDo.Count)
								//Sp.ChangeAction(whatToDo,act.whatIf[j],e);
								whatToDo[e] = act.whatIf[j];
							else
							{
								whatToDo.Add (act.whatIf [j]);
								//actNumb=Sp.AddAction(whatToDo,act.whatIf[j],actNumb);
							}
						}
					}
				}
				else if (!((act.actMode==3)&&(act.timer<=act.timeToReverse)))
					for (j=0; j<act.what.Length;j++)
					{
						if (act.what [j].actType == 0) {
							//actNumb = Sp.AddAction (whatToDo, act.what [j], actNumb);
							whatToDo.Add(act.what[j]);
						
						} else 
						{
							e=0;
							while ((e<whatToDo.Count)&&(whatToDo[e].actType!=act.what[j].actType))
							e++;
							if (e != whatToDo.Count)
								whatToDo [e] = act.what [j];	
							//Sp.ChangeAction(whatToDo,act.what[j],e);
							else {
								//actNumb = Sp.AddAction (whatToDo, act.what [j], actNumb);
								whatToDo.Add (act.what [j]);
							}
					}
				}
				
			}
		}
	}

	public void LearnAnimations()//Персонаж анализирует, какие анимации ему надо проигрывать
	{
		ActivityClass.activites act;
		for (i=0;i<whatToEmploy.Count;i++)
		{
			if (whatToEmploy[i].weapon==null)
				act=actions.activities[whatToEmploy[i].numb];
			else
				act=whatToEmploy[i].weapon.moveset[whatToEmploy[i].numb];
			if ((!((act.actMode==3)&&(act.timer<=act.timeToReverse)))&&
			    (act.actMode<5))
				for (j=0;j<act.howLook.Length; j++)
						if (/*Sp.IntComparation(stats.stats.direction+2, act.howLook[j].stats.direction)&&
				    	*/Sp.IntComparation(stats.stats.groundness,act.howLook[j].stats.groundness)&&/*
				    	Sp.IntComparation(stats.stats.obstacleness,act.howLook[j].stats.obstacleness)&&
				    	Sp.IntComparation(stats.stats.groundness,act.howLook[j].stats.groundness)&&
				    	Sp.IntComparation(stats.stats.maxInteraction,act.howLook[j].stats.interaction)&&
				    	Sp.IntComparation(stats.stats.interaction,act.howLook[j].stats.interaction)&&
				    	Sp.IntComparation(stats.stats.upness,act.howLook[j].stats.upness)&&
				    	Sp.IntComparation(stats.stats.specialness,act.howLook[j].stats.specialness)&&*/
				    	Sp.FComparation(Mathf.Abs(rigid.velocity.x),act.howLook[j].speedX)&&
				    	Sp.FComparation(-1 * stats.direction * rigid.velocity.x,act.howLook[j].nSpeedX)&&
				    	Sp.FComparation(Mathf.Abs(rigid.velocity.y),act.howLook[j].speedY)&&
				    	Sp.FComparation(-1*rigid.velocity.y,act.howLook[j].nSpeedY)&&
				    	Sp.FComparation(Mathf.Abs (Sp.realSign(stats.stats.targetSpeedX)-Sp.realSign(rigid.velocity.x)), act.howLook[j].direction*1f)  )
						{	
							//if ((!battleStance)||
				    		//	((whatToEmploy[i].howLook[j].weaponInRightHand)&&(gameObject.GetComponent<Equipment>().rightWeapon.type==whatToEmploy[i].howLook[j].weaponType))||
				    		//	((!whatToEmploy[i].howLook[j].weaponInRightHand)&&(gameObject.GetComponent<Equipment>().leftWeapon.type==whatToEmploy[i].howLook[j].weaponType))||
				    		//	(whatToEmploy[i].howLook[j].weaponType==0)){
						whatToPerform.Add(act.howLook[j].anim);
						//animNumb = whatToPerform.Count;
									//animNumb=Sp.AddAnimation(whatToPerform, act.howLook[j].anim, animNumb);
									break;
							//	}
						}
		}
		Sp.SortAnimation (whatToPerform);
	}

	public void AnimateIt()
	{
		Sp.BeginAnimateIt (animator.parts);
		for (i=0;i<whatToPerform.Count;i++)
			Sp.AnimateIt(animator.parts, whatToPerform[i]);
		Sp.FinallyAnimateIt (animator.parts);
	}

	public void CoordinateActivities()
	{
		ActivityClass.activites act;
		for(i=0;i<whatToEmploy.Count;i++)
		{	
			if (whatToEmploy[i].weapon==null)
				act=actions.activities[whatToEmploy[i].numb];
			else
				act=whatToEmploy[i].weapon.moveset[whatToEmploy[i].numb];
			if (act.timer>0)
				Sp.ChangeTimer(act.timer-1,whatToEmploy[i],actions.activities);
			if (act.weapon==null) 
				actions.activities[act.numb].change=((act.actMode==2)&&(act.timer<=act.timeToReverse)); 
			else 
				act.weapon.moveset[act.numb].change=((act.actMode==2)&&(act.timer<=act.timeToReverse)); 
			if((act.timer==act.timeToReverse)&&(act.actMode==3))
				stats.stats.employment+=act.employ;
			if (act.timer==0)
			{
				if (act.actMode!=3)
					stats.stats.employment+=act.employ;
				if (act.weapon==null) 
				{
					actions.activities[act.numb].chosen=false; 
					actions.activities[act.numb].change=false; 
				}
				else 
				{
					act.weapon.moveset[act.numb].chosen=false;
					act.weapon.moveset[act.numb].change=false;
				}
				whatToEmploy.Remove(whatToEmploy[i]);
				i--;
				if (stats.stats.employment>11) 
					stats.stats.employment=11;				
			}
			
		}
	}

	public void OrientateIt()
	{
		if ((stats.stats.interaction<=2)||(stats.stats.interaction>6))
			Sp.Flip (transform, stats.direction);
		else if (stats.direction<0)
			Sp.Flip(transform,1);
	}

	public void FixedUpdate () 
	{
		LearnActivities ();
		PrepareEquipment ();
		LearnActions ();
		Sp.DoActions (whatToDo);
		LearnAnimations ();
		AnimateIt ();
		CoordinateActivities ();
		Sp.GetInformation (infoGets);
		whatToPerform.Clear ();
		OrientateIt ();
	}

}
