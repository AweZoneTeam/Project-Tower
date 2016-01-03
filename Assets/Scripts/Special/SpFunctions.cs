using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpFunctions : MonoBehaviour {

	public Rigidbody2D rigid;
	public bool[] k;
	public int k1;
	public int k2;
	public int k3;
	public int k4;
	public bool i1;
	public bool i2;
	public int jj1, jj2;
	public string ss;
	

	public bool FComparation(float f, float g)
	{
		return (((g < 0) && (f < -1 * g)) || ((g > 0) && (f > g))||(g==0));
	}

	public bool IntComparation(int f, int g)
	{
		return (((g < 0) && (f < -1 * g)) || ((g > 0) && (f > g))||(g==0));
	}

	public bool ComprFunctionality(int f, ComparativeClass.compr cpr)
	{
		return ((f < cpr.val) && (cpr.oper == "<") ||
						(f <= cpr.val) && (cpr.oper == "<=") ||
						(f == cpr.val) && (cpr.oper == "=") ||
						(f >= cpr.val) && (cpr.oper == ">") ||
						(f >= cpr.val) && (cpr.oper == ">=") ||
						(f != cpr.val) && (cpr.oper == "!=")||
		        		(cpr.oper=="!"));
	}

	public bool ComprFunctionality(float f, FComparativeClass.compr cpr)
	{
		return ((f < cpr.val) && (cpr.oper == "<") ||
		        (f <= cpr.val) && (cpr.oper == "<=") ||
		        (f == cpr.val) && (cpr.oper == "=") ||
		        (f >= cpr.val) && (cpr.oper == ">") ||
		        (f >= cpr.val) && (cpr.oper == ">=") ||
		        (f != cpr.val) && (cpr.oper == "!=")||
		        (cpr.oper=="!"));
	}

	public int ChooseDirection(ButtonClass.button b1, ButtonClass.button b2)
	{
		if (b1.timer>b2.timer)
			return 1;
		else if (b1.timer<b2.timer)
			return -1;
		else return 0;
	}

	public int WhatToChange(int a, int b)
	{
		if (b>-1) 
			return b; 
		else 
			return a;
	}

	public float RealAngle(Vector2 vect1, Vector2 vect2)
	{
		return 180/Mathf.PI*
			realSign(vect1.x*vect2.y-vect1.y*vect2.x)*
				(Mathf.Acos((vect1.x * vect2.x + vect1.y * vect2.y) /vect1.magnitude /vect2.magnitude));
	}

	public Vector2 VectorConvert(Vector3 vect)
	{
		return new Vector2 (vect.x, vect.y);
	}

	public int AddAction(ActionClass.act[] actions, ActionClass.act action, int numb)
	{
		actions [numb] = action;
		numb++;
		return numb;
	}

	public int DeleteAction(ActionClass.act[] actions, int del, int numb)
	{
		for (int i=del+1;i<numb;i++)
			actions[i-1]=actions[i];
		numb--;
		return numb;
	}

	public int AddAnimation(animClass.anim[] animats, animClass.anim animat, int numb)
	{
		animats [numb] = animat;
		numb++;
		return numb;
	}
	
	public int DeleteAnimation(animClass.anim[] animats, int del, int numb)
	{
		for (int i=del+1;i<numb;i++)
			animats[i-1]=animats[i];
		numb--;
		return numb;
	}

	public int SortAnimation(animClass.anim[] anims, int numb)
	{
		int i, j;
		animClass.anim x;
		for (i=0;i<numb-1; i++)
			for (j=i+1;j<numb;j++)
				if (anims[i].mod>anims[j].mod)
				{
					x=anims[i];
					anims[i]=anims[j];
					anims[j]=x;
				}
		return numb;
	}

	public void ChangeTimer(int timer, ActivityClass.activites act, ActivityClass.activites[] acts)
	{
		if (act.weapon == null)
			acts [act.numb].timer = timer;
		else 
			act.weapon.moveset [act.numb].timer = timer;

	}

	public int EmployTime(ActivityClass.activites act, ActivityClass.activites[] acts)
	{
		if (act.weapon == null)
			return acts [act.numb].timer;
		else 
			return act.weapon.moveset [act.numb].timer;
	}

	public int AddActivity(ActivityClass.activites[] activities, ActivityClass.activites activity, int numb)
	{
		activities [numb] = activity;
		numb++;
		return numb;
	}

	public int AddActivity(OrgActivityClass.activities[] activities, OrgActivityClass.activities activity, int numb)
	{
		activities [numb] = activity;
		numb++;
		return numb;
	}

	public int DeleteActivity(ActivityClass.activites[] activities, int del, int numb)
	{
		for (int i=del+1;i<numb;i++)
			activities[i-1]=activities[i];
		numb--;
		return numb;
	}

	public int DeleteActivity(OrgActivityClass.activities[] activities, int del, int numb)
	{
		for (int i=del+1;i<numb;i++)
			activities[i-1]=activities[i];
		numb--;
		return numb;
	}

	public int AddPart(PartConroller[] parts, PartConroller part, int numb)
	{
		parts [numb] = part;
		numb++;
		return numb;
	}
	
	public int DeletePart(PartConroller[] parts, int del, int numb)
	{
		for (int i=del+1;i<numb;i++)
			parts[i-1]=parts[i];
		numb--;
		return numb;
	}

	public void ChangeAction(ActionClass.act[] actions, ActionClass.act action, int ch)
	{
		actions [ch] = action;
	}

	public Vector2 Ortog(Vector2 vect1, Vector2 vect2)
	{
		Vector2 vect3;
		float scal;
		scal = vect1.x * vect2.x + vect1.y + vect2.y;
		vect3 = scal * vect2 / vect2.sqrMagnitude;
		return vect1 - vect3;
	}

	public int DoActions(ActionClass.act[] actions, int numb)
	{
		ActionTypes actTypes = GetComponent<ActionTypes> ();
		InfoGetTypes infTypes = GetComponent<InfoGetTypes> ();
		clavisher clavi = GetComponent<clavisher> ();
		int j;
		for (int i=0;i<numb;i++)
		{
			if (actions[i].actType==1)
			{
				if (actions[i].actNumb==1)
					actTypes.AchieveSpeedX(actions[i].OBJ[0].GetComponent<Rigidbody2D>(),
					                       actions[i].OBJ[0].GetComponent<Stats>(), 
					                       ChooseDirection(clavi.buttons[actions[i].PRM[2]],clavi.buttons[actions[i].PRM[3]])*actions[i].PRM[0], 
					                       actions[i].PRM[1]);
				else if (actions[i].actNumb==2)
					actTypes.AchieveSpeedY(actions[i].OBJ[0].GetComponent<Rigidbody2D>(),
					                       actions[i].OBJ[0].GetComponent<Stats>(), 
					                       ChooseDirection(clavi.buttons[actions[i].PRM[2]],clavi.buttons[actions[i].PRM[3]])*actions[i].PRM[0], 
					                       actions[i].PRM[1]);
				else if (actions[i].actNumb==3)
					actTypes.AchieveSpeed(actions[i].OBJ[0].GetComponent<Rigidbody2D>(),
					                       actions[i].OBJ[0].GetComponent<Stats>(), 
					                       ChooseDirection(clavi.buttons[actions[i].PRM[2]],clavi.buttons[actions[i].PRM[3]])*actions[i].PRM[0], 
					                       ChooseDirection(clavi.buttons[actions[i].PRM[4]],clavi.buttons[actions[i].PRM[5]])*actions[i].PRM[0], 
					                       actions[i].PRM[1]);
				else if (actions[i].actNumb==4)
					actTypes.SetVeloc(actions[i].OBJ[0].GetComponent<Rigidbody2D>(),
					                  new Vector2(actions[i].OBJ[0].GetComponent<Stats>().direction*actions[i].PRM[0]*1f,
					            				  actions[i].PRM[1]*1f));
				else if (actions[i].actNumb==5)
					actTypes.Forcing(actions[i].OBJ[0].GetComponent<Rigidbody2D>(), 
					                 new Vector2(ChooseDirection(clavi.buttons[actions[i].PRM[2]],
					                                             clavi.buttons[actions[i].PRM[3]])*actions[i].PRM[0]*1f,
					            	 			 actions[i].PRM[1]*1f),
					                 this);
				else if (actions[i].actNumb==6)
				{
					actTypes.Climb(actions[i].OBJ[0].GetComponent<Rigidbody2D>(),
					               actions[i].OBJ[0].GetComponent<Stats>(),
					               infTypes,
					               actions[i].OBJ[0].GetComponent<InfoGets>(),
					               actions[i].PRM[6],
					               ChooseDirection(clavi.buttons[actions[i].PRM[2]],
					                			   clavi.buttons[actions[i].PRM[3]])*actions[i].PRM[0],
					               ChooseDirection(clavi.buttons[actions[i].PRM[4]],
					                			   clavi.buttons[actions[i].PRM[5]])*actions[i].PRM[1],
					               this);
					k1++;
				}
				else if (actions[i].actNumb==7)
					actTypes.MoveToClimb(actions[i].OBJ[0].GetComponent<Rigidbody2D>(),
					                     actions[i].OBJ[0].GetComponent<Stats>(),
					                     actions[i].OBJ[0].GetComponent<InfoGets>(),
					                     actions[i].PRM[0],
					                     this);
			}
			else if (actions[i].actType==0)
			{
				if (actions[i].actNumb==1)
				{
					actTypes.ChangeLocation(actions[i].OBJ[0].transform,
					                        actions[i].OBJ[1].transform,
					                        new Vector3(actions[i].PRM[0]*1f,
					            						actions[i].PRM[1]*1f,
					            						actions[i].PRM[2]*1f));
				}
				else if (actions[i].actNumb==2)
				{
					actTypes.ChangeLocationSmoothly(actions[i].OBJ[0].transform,
					              			        actions[i].OBJ[1].transform,
					                        		new Vector3(actions[i].PRM[0]*1f,
					            								actions[i].PRM[1]*1f,
					            								actions[i].PRM[2]*1f),
					                                actions[i].PRM[3]);
				}
				else if (actions[i].actNumb==3)
				{
					actTypes.MakeBTrigger(actions[i].OBJ[0].GetComponent<BoxCollider2D>(),
					                      actions[i].PRM[0]);
				}
				else if (actions[i].actNumb==4)
				{
					actTypes.MakeCTrigger(actions[i].OBJ[0].GetComponent<CircleCollider2D>(),
					                      actions[i].PRM[0]);
				}
				else if (actions[i].actNumb==5)
				{
					if (actions[i].OBJ[0].GetComponent<Indicator>()!=null)
						actTypes.MakeColAble(actions[i].OBJ[0].GetComponent<Indicator>(),
					                      	 actions[i].PRM[0]);
					else 
							actTypes.MakeColAble(actions[i].OBJ[0].GetComponent<CircleCollider2D>(),
							                     actions[i].PRM[0]);
				}
				else if (actions[i].actNumb==6)
				{
					actTypes.MakeSizeOfCol(actions[i].OBJ[0].GetComponent<BoxCollider2D>(),
					                       actions[i].PRM[0]*1f/10f,
					                       actions[i].PRM[1]*1f/10f);
				}
			}
			else if (actions[i].actType==2)
			{
				/*if (actions[i].actNumb==1)
					actTypes.WeaponMove(actions[i].OBJ[0].GetComponent<Equipment>(),
					                    actions[i].OBJ[1].GetComponent<RootCharacterController>(),
					                    actions[i].OBJ[1].GetComponent<Stats>(),
					                    this, 
					                    actions[i].PRM[0]==1);
				else */if (actions[i].actNumb==2)
					actTypes.WeaponHit(actions[i].OBJ[0].GetComponent<HitController>(),
					                  	actions[i].PRM[0]*1f, actions[i].PRM[1]*1f, actions[i].PRM[2]*1f, actions[i].PRM[3]*1f,
					           					  actions[i].PRM[4],
					                              actions[i].PRM[5]==1,
					                              actions[i].PRM[6]*0.1f,
					                   actions[i].PRM[7],
					                   actions[i].PRM[8],
					                   actions[i].PRM[9],
					                   actions[i].PRM[10]);
				else if (actions[i].actNumb==3)
					actTypes.WeaponPush(actions[i].OBJ[0].GetComponent<HitController>(),actions[i].OBJ[1].GetComponent<Rigidbody2D>(),
					                    actions[i].PRM[0]*1f, actions[i].PRM[1]*1f, actions[i].PRM[2]*1f, actions[i].PRM[3]*1f,
					                    actions[i].PRM[4],
					                    actions[i].PRM[5]==1,
					                    actions[i].PRM[6]*0.1f,
					                    actions[i].PRM[7],
					                    actions[i].PRM[8],
					                    actions[i].PRM[9],
					                    actions[i].PRM[10],
					                    new Vector2(ChooseDirection(clavi.buttons[actions[i].PRM[13]],
					                            clavi.buttons[actions[i].PRM[14]])*actions[i].PRM[11]*1f,
					            				actions[i].PRM[12]*1f));
				else if (actions[i].actNumb==4)
					actTypes.Shoot(actions[i].PRM[0]*1f, actions[i].PRM[1]*1f, actions[i].PRM[2]*1f, actions[i].PRM[3]*1f,
					               actions[i].PRM[4],
					               actions[i].PRM[5]==1,
					               actions[i].PRM[6]*0.1f,
					               actions[i].PRM[7],
					               actions[i].PRM[8],
					               actions[i].OBJ[0].GetComponent<Equipment>().arrows,
					               new Vector2(actions[i].PRM[9]*1f*actions[i].OBJ[0].GetComponent<Organism>().direction,
					            			   actions[i].PRM[10]*1f),
					               actions[i].OBJ[1].transform.position);
			}
			else if (actions[i].actType==3)
			{
				if (actions[i].actNumb==0)
				{
					if (actions[i].OBJ[0].GetComponent<Equipment>().rightWeapon!=null)
						actTypes.PrepareWeapon(actions[i].OBJ[0].GetComponent<Equipment>().rightWeapon,
						                       actions[i].OBJ[0].GetComponent<SmartObjectController>(),
						                       actions[i].PRM[0],
						                       (actions[i].PRM[1]==1));
					if (actions[i].OBJ[0].GetComponent<Equipment>().leftWeapon!=null)
						actTypes.PrepareWeapon(actions[i].OBJ[0].GetComponent<Equipment>().leftWeapon,
						                       actions[i].OBJ[0].GetComponent<SmartObjectController>(),
						                       actions[i].PRM[0],
						                       (actions[i].PRM[1]==1));
				}
			}

			else if (actions[i].actType==4)
			{
				if (actions[i].actNumb==0)
					actTypes.GravityOn(actions[i].OBJ[0].GetComponent<Rigidbody2D>(),
						               actions[i].PRM[0]*1f/10f,
					                   actions[i].PRM[1]==1);
			}

			else if (actions[i].actType==5)
			{
				if (actions[i].actNumb==0)
				{
					for (j=0;j<10;j++)
						if (clavi.buttons[j+12].push==3)
							break;
					if (actions[i].OBJ[0].GetComponent<Equipment>().bag.Count>=j)
					{
						if ((actions[i].OBJ[0].GetComponent<Equipment>().bag[j-1].GetComponent<ItemClass>().objects[0].GetComponent<WeaponClass>().handEmployment==2)||
							(clavi.buttons[9].push==1))
						{
						    actTypes.ChangeWeapon(actions[i].OBJ[0].GetComponent<Equipment>(),
						                   j-1,
					                	   true,
					            	       actions[i].PRM[0],
					        	           actions[i].PRM[1],
					    	               this);
							clavi.buttons[j+12].push=0;
							clavi.buttons[j+12].timer=0;
						}
						else if (clavi.buttons[8].push==1)
						{
							actTypes.ChangeWeapon(actions[i].OBJ[0].GetComponent<Equipment>(),
						            	          j-1,
						        	              1==0,
						    	                  actions[i].PRM[0],
							                      actions[i].PRM[1],
							                      this);	
							clavi.buttons[j+12].push=0;
							clavi.buttons[j+12].timer=0;
						}
					}
				}
			}
			else if (actions[i].actType==6)
			{
				if (actions[i].actNumb==1)
					actTypes.Punchuntilground(actions[i].OBJ[0].GetComponent<PunchUntillGround>(),
					                          actions[i].OBJ[1].GetComponent<RootCharacterController>(),
					                          actions[i].OBJ[2].GetComponent<WeaponClass>(),
					                          actions[i].PRM[0],actions[i].PRM[1],actions[i].PRM[2],this, 
					                          actions[i].OBJ[3].GetComponent<HitController>());
				if (actions[i].actNumb==2)
					actTypes.Twodifferentweaponslock(actions[i].OBJ[0].GetComponent<TwoDifferentWeaponsLock>(),
					                                 actions[i].OBJ[1].GetComponent<Equipment>(),
					                          		 actions[i].OBJ[2].GetComponent<WeaponClass>(),
					                                 actions[i].OBJ[1].GetComponent<RootCharacterController>(),
					                          		 actions[i].PRM[0]);
				if (actions[i].actNumb==3)
					actTypes.Holdandunleash(actions[i].OBJ[0].GetComponent<HoldAndUnleash>(),
					                          actions[i].OBJ[1].GetComponent<RootCharacterController>(),
					                          actions[i].OBJ[2].GetComponent<WeaponClass>(),
					                          actions[i].PRM[0],actions[i].PRM[1],actions[i].PRM[2],actions[i].PRM[3],this, 
					                          actions[i].OBJ[3].GetComponent<HitController>(), clavi);
				if (actions[i].actNumb==4)
					actTypes.Aimingshoot(actions[i].OBJ[0].GetComponent<AimingShoot>(),
					                     actions[i].OBJ[1].GetComponent<RootCharacterController>(),
					                     actions[i].OBJ[2].GetComponent<WeaponClass>(),
					                     actions[i].PRM[0],actions[i].PRM[1],actions[i].PRM[2],actions[i].PRM[3],actions[i].PRM[4],
					                     this,clavi,
					                     actions[i].OBJ[3].transform.position);
				if (actions[i].actNumb==5)
					actTypes.Punchuntiltarget(actions[i].OBJ[0].GetComponent<PunchUntilTarget>(),
					                          actions[i].OBJ[1].GetComponent<RootCharacterController>(),
					                          actions[i].OBJ[2].GetComponent<WeaponClass>(),
					                          actions[i].PRM[0],actions[i].PRM[1],actions[i].PRM[2],this, 
					                          actions[i].OBJ[3].GetComponent<HitController>());
				if (actions[i].actNumb==6)
					actTypes.Shieldcontroller(actions[i].OBJ[0].GetComponent<ShieldController>(),
					                          actions[i].OBJ[1].GetComponent<RootCharacterController>(),
					                          actions[i].OBJ[2].GetComponent<WeaponClass>(),
					                          actions[i].PRM[0],actions[i].PRM[1],actions[i].PRM[2],this, 
					                          clavi);

			}
			else if (actions[i].actType==10)
			{
				actTypes.Experimental();
			}
		}
		numb = 0;
		return numb;
	}

	public void AnalyseSituation(StatsClass.stats stats1, StatsClass.stats stats2)
	{
		stats1.groundness=WhatToChange(stats1.groundness,stats2.groundness);
		stats1.obstacleness=WhatToChange(stats1.obstacleness,stats2.obstacleness);
		stats1.interaction=WhatToChange(stats1.interaction,stats2.interaction);
		stats1.maxInteraction=WhatToChange(stats1.maxInteraction,stats2.maxInteraction);
		stats1.hitted=WhatToChange(stats1.hitted,stats2.hitted);
		stats1.upness=WhatToChange(stats1.upness,stats2.upness);
		stats1.specialness=WhatToChange(stats1.specialness,stats2.specialness);
	}

	public void GetInformation(InfoGets inf)
	{
		InfoGetTypes infType=GetComponent<InfoGetTypes>();

		for (int i=0;i<inf.infoGets.Length;i++)
		{
			if (inf.infoGets[i].OBJ[0].GetComponent<Indicator>()!=null)
				if (!inf.infoGets[i].OBJ[0].GetComponent<Indicator>().isItActive())
					continue;
			if ((inf.infoGets[i].numbOfInfo==1)&&(infType.Overlaper(inf,i,this))||
			    (inf.infoGets[i].numbOfInfo==2)&&(infType.Raycaster(inf, 
			                                                    	i, 
			                                                    	new Vector2 (realSign(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.targetSpeedX)*inf.infoGets[i].VCT[0].x,
			             														 inf.infoGets[i].VCT[0].y),
			                                                    this))||
			    (inf.infoGets[i].numbOfInfo==4)&&(infType.Raycaster(inf, 
			                                                    i, 
			                                                    new Vector2 (realSign(inf.infoGets[i].OBJ[1].GetComponent<Stats>().direction)*inf.infoGets[i].VCT[0].x,
			      														     inf.infoGets[i].VCT[0].y),
			                                                    this)))

			{
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.groundness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.groundness,inf.infoGets[i].stats.groundness);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.obstacleness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.obstacleness,inf.infoGets[i].stats.obstacleness);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.interaction=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.interaction,inf.infoGets[i].stats.interaction);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.maxInteraction=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.maxInteraction,inf.infoGets[i].stats.maxInteraction);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.hitted=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.hitted,inf.infoGets[i].stats.hitted);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.upness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.upness,inf.infoGets[i].stats.upness);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.specialness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.specialness,inf.infoGets[i].stats.specialness);
			}
			else if (inf.infoGets[i].typeOfInfo==1)
			{
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.groundness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.groundness,inf.infoGets[i].elseStats.groundness);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.obstacleness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.obstacleness,inf.infoGets[i].elseStats.obstacleness);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.interaction=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.interaction,inf.infoGets[i].elseStats.interaction);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.maxInteraction=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.maxInteraction,inf.infoGets[i].elseStats.maxInteraction);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.hitted=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.hitted,inf.infoGets[i].elseStats.hitted);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.upness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.upness,inf.infoGets[i].elseStats.upness);
				inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.specialness=WhatToChange(inf.infoGets[i].OBJ[1].GetComponent<Stats>().stats.specialness,inf.infoGets[i].elseStats.specialness);
			}
			if (inf.infoGets[i].numbOfInfo==3)
			{
				if (inf.infoGets[i].OBJ[0].GetComponent<Stats>().stats.targetSpeedX>0)
					inf.infoGets[i].OBJ[0].GetComponent<Stats>().direction=1;
				if (inf.infoGets[i].OBJ[0].GetComponent<Stats>().stats.targetSpeedX<0)
					inf.infoGets[i].OBJ[0].GetComponent<Stats>().direction=-1;
			}
		}
	}

	public int ChemByZanyatsya(ActivityClass.activites[] activities, Stats stats, int e, clavisher clav)
	{
		int j,i,h,k,jj;
		bool weapon;
		bool change=false;
		h = 0;
		while ((e!=stats.stats.employment)||(change))
		{	
			change=false;
			e=stats.stats.employment;
			j=-1;
			for (i=activities.Length-1; i>=0; i--)
			{
				k=0;
				if (activities[i].hasOwnActivities)
				{
					if ((activities[i].weapon!=null))
						k=ChemByZanyatsya(activities[i].weapon.moveset, stats, 12, clav);
					if (k==1)
					{
						h=k;
						break;
					}
				}
				else if (ClaveComparation(activities[i].why.claves,clav))
					if (FComparation(Mathf.Abs(stats.gameObject.GetComponent<Rigidbody2D>().velocity.x),activities[i].why.speedX*1f)&&
					    FComparation(Mathf.Abs(stats.gameObject.GetComponent<Rigidbody2D>().velocity.y),activities[i].why.speedY*1f)&&
					    IntComparation(stats.direction+2, activities[i].why.direction)&&
					    IntComparation(stats.stats.groundness,activities[i].why.groundness)&&
					    IntComparation(stats.stats.obstacleness,activities[i].why.obstacleness)&&
					    IntComparation(stats.stats.groundness,activities[i].why.groundness)&&
					    ComprFunctionality(stats.stats.maxInteraction,activities[i].why.maxinteraction)&&
					    ComprFunctionality(stats.stats.interaction,activities[i].why.interaction)&&
					    IntComparation(stats.stats.upness,activities[i].why.upness)&&
					    IntComparation(stats.stats.specialness,activities[i].why.specialness))
				{	
					if (activities[i].weapon==null)
						weapon=true;
					else
						weapon=ComprFunctionality(activities[i].weapon.ready,activities[i].why.weaponReady);
					if (weapon)
					{
						if (i+1<activities.Length)
						{
					   		if ((activities[i+1].change)&&(activities[i].why.employment<=stats.stats.employment+activities[i+1].why.employment))
							{
								change=true;
								j=i;
								break;
							}
							else if ((activities[i].why.employment<=stats.stats.employment)&&
					   	 		(!activities[i].chosen))
							{
								j=i;
								break;
							}
						}
						if ((activities[i].why.employment<=stats.stats.employment)&&
					   	 (!activities[i].chosen))
						{
							j=i;
							break;
						}
					}
				}
			}
			if ((change)&&(j>=0))
			{
				for (jj=0;jj<stats.gameObject.GetComponent<RootCharacterController>().whatToEmploy.Count;jj++)
					if (stats.gameObject.GetComponent<RootCharacterController>().whatToEmploy[jj].name.Equals(activities[j+1].name))
								break;
				if (jj<stats.gameObject.GetComponent<RootCharacterController>().whatToEmploy.Count)
				{
					stats.gameObject.GetComponent<RootCharacterController>().whatToEmploy.Remove(stats.gameObject.GetComponent<RootCharacterController>().whatToEmploy[jj]);
					activities[j+1].chosen=false;
					activities[j+1].change=false;
					stats.stats.employment+=activities[j+1].employ;
				}
			}
			if (j>=0)
			{
				stats.gameObject.GetComponent<RootCharacterController>().whatToEmploy.Add (activities[j]);
				activities[j].timer=activities[j].timeOfAction;
				stats.stats.employment-=activities[j].employ;
				if (activities[j].soWhat.groundness>=0) stats.stats.groundness=activities[j].soWhat.groundness;
				if (activities[j].soWhat.obstacleness>=0) stats.stats.obstacleness=activities[j].soWhat.obstacleness;
				if (activities[j].soWhat.interaction>=0)stats.stats.interaction=activities[j].soWhat.interaction;
				if (activities[j].soWhat.upness>0)stats.stats.upness=activities[j].soWhat.upness-1;
				if (activities[j].soWhat.specialness>=0)stats.stats.specialness=activities[j].soWhat.specialness;
				activities[j].chosen=true;
				h=1;
			}
		}
		return h;
	}

	
	public int ChemByZanyatsya(OrgActivityClass.activities[] activities, Organism stats)
	{
		int j, i;
		j = activities.Length-1;
		while (j>=0)
		{	
			for (i=j; i>=0; i--)
			{
				j=-1;
				if ((ComprFunctionality(stats.health,activities[i].why.health))&&
					(ComprFunctionality(stats.stats.hitted,activities[i].why.hitted))&&
					(!activities[i].chosen))
				{
					j=i;
					break;
				}
			}
			if (j>=0)
			{
				stats.gameObject.GetComponent<DestroyableObjectController>().activityNumb=AddActivity(stats.gameObject.GetComponent<DestroyableObjectController>().whatToEmploy,
				                                                                                  activities[j],
				                                                                                  stats.gameObject.GetComponent<DestroyableObjectController>().activityNumb);

				activities[j].chosen=true;
			}
		}
		return 1;
	}

	public GameObject FindPart(List<PartConroller> parts, string name)
	{
		GameObject obj=null;
		int i;
		for (i=0;i<parts.Count;i++)
		{
			if ((string.Equals(name, parts[i].gameObject.name))||(string.Equals(name+"(Clone)", parts[i].gameObject.name)))
			{
				obj=parts[i].gameObject;
				break;
			}
			obj=FindPart(parts[i].parts, name);
			if (obj!=null) break;
		}
		return obj;
	}

	public GameObject FindObject(GameObject obj1, string name)
	{
		GameObject obj=null;
		if (string.Equals(name, obj1.name))
		    return obj1;
		int i;
		for (i=0;i<obj1.transform.childCount;i++)
		{
			if (string.Equals(name, obj1.transform.GetChild(i).gameObject.name))
			{
				obj=obj1.transform.GetChild(i).gameObject;
				break;
			}
			obj=FindObject(obj1.transform.GetChild(i).gameObject, name);
			if (obj!=null) break;
		}
		return obj;
	}

	public void SetMoveset (GameObject obj, WeaponClass weapon)
	{
		for (int i=0; i<weapon.moveset.Length; i++)
		{
			for (int j=0; j<weapon.moveset[i].what.Length; j++)
				for (int k=0; k<weapon.moveset[i].what[j].OBJDescription.Length; k++)
					weapon.moveset [i].what [j].OBJ [k] = FindObject (obj, weapon.moveset [i].what [j].OBJDescription[k]);
			for (int j=0; j<weapon.moveset[i].whatIf.Length; j++)
				for (int k=0; k<weapon.moveset[i].whatIf[j].OBJDescription.Length; k++)
					weapon.moveset [i].whatIf [j].OBJ [k] = FindObject (obj, weapon.moveset [i].whatIf [j].OBJDescription[k]);
		}
	}

	public void ChangePartColor (List<PartConroller> parts, Color color)
	{
		int i;
		for (i=0;i<parts.Count;i++)
		{
			if (parts[i].mov.individualMaterials[0]!=null)
				parts[i].mov.setMaterialColor(color);
			ChangePartColor(parts[i].parts, color);
		}
	}

	public void BeginAnimateIt(List<PartConroller> parts)
	{
		int i;
		for (i=0;i<parts.Count;i++)
		{
			parts[i].animationMod=-1;
			BeginAnimateIt(parts[i].parts);
		}
	}

	public void AnimateIt(List<PartConroller> parts, animClass.anim anim)
	{
		int i;
		for (i=0;i<parts.Count;i++)
		{
			if ((anim.mod<=parts[i].mod)||(parts[i].animationMod<parts[i].mod))
			{
				parts[i].type=anim.type;
				parts[i].numb=anim.numb;
				parts[i].animationMod=anim.mod;
				AnimateIt(parts[i].parts,anim);
			}
		}
	}

	public void FinallyAnimateIt(List<PartConroller> parts)
	{
		int i;
		for (i=0;i<parts.Count;i++)
		{
			parts[i].Work();
			FinallyAnimateIt(parts[i].parts);
		}
	}

	public void AnimateLedGafs(List<PartConroller> parts, int addictiveFrame)
	{
		int i;
		for (i=0; i<parts.Count; i++) {
			//if (parts[i].animationMod==0)
			//{
			parts [i].led = true;
			parts [i].addictiveFrame = addictiveFrame;
		//}
			//else 
			//	parts[i].led=false;
			AnimateLedGafs(parts[i].parts,addictiveFrame);
		}
		return;
	}
	public void FrameToFrame(GAF.Core.GAFMovieClip mov, bool loop)
	{
		int i;
		uint k1=1;
		i=(int)mov.getCurrentFrameNumber ();
		i++;
		if ((i>(int)mov.currentSequence.endFrame)&&(loop))
			i=(int)mov.currentSequence.startFrame;
		else if ((i>(int)mov.currentSequence.endFrame))
		if (i<(int)mov.currentSequence.startFrame)
			i=(int)mov.currentSequence.startFrame;
		mov.gotoAndStop(k1*(uint)i);

	}

	public bool ClaveComparation(ClaveNoteClass1.CLV kk, clavisher jj)
	{	
		int j;
		for (int i=0; i<4; i++)
						k [i] = true;
		for (int i=0; i<kk.clv.Length;i++)
		{
			j=0;
			k[i]=false;
			while ((j<kk.clv[i].but.Length)&&(kk.clv[i].but[j].push!=jj.buttons[kk.clv[i].but[j].timer].push))
				j++;

			if (j<kk.clv[i].but.Length) k[i]=true;
		}
		return k[0]&&k[1]&&k[2]&&k[3];
	}

	public int ExperimentFunction(int kk)
	{
		kk++;
		return kk;
	}

	public void Flip(Transform trans, int x)
	{
		if (Mathf.Sign (trans.localScale.x)!=x*1f)
			trans.localScale=new Vector3(trans.localScale.x*-1,
			                             trans.localScale.y,
			                             trans.localScale.z);
	}

	public float realSign(float x)
	{
		if (x==0)
			return 0f;
		else return Mathf.Sign(x);
	}

	public float Div(float x, float y)
	{
		if ((x/y-Mathf.Round (x/y))<0)
			return Mathf.Round(x/y)-1;
		else 
			return Mathf.Round (x/y);
	}

	public void ChangeRenderOrder(int order, GameObject obj)
	{
		GAF.Objects.GAFRenderProcessor rend;
		rend = new GAF.Objects.GAFRenderProcessor ();
		obj.GetComponent<GAF.Core.GAFMovieClip>().settings.spriteLayerValue = order;
		rend.init(obj.GetComponent<GAF.Core.GAFMovieClip>(),
		          obj.GetComponent<MeshFilter>(),
		          obj.GetComponent<Renderer>());
	}

	public bool IsItEven(float x)
	{
		if (Div (x,1)-2*Div (x,2)>0)
			return false;
		else 
			return true;
	}
}
