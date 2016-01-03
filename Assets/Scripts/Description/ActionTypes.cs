using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionTypes : MonoBehaviour 
{
	public GameObject obj;
	public string s, s1,s2;

	public int kk;
	public int ii;
	public List<GameObject> listOfObjects;
	//тип 1 номер 1
	public void AchieveSpeedX (Rigidbody2D rigid, Stats stats, int targetSpeed, int acceleration)
	{
		//rigid.gravityScale = 1f;
		if (Mathf.Abs(targetSpeed*1f)!=1f)
		{
			if (Mathf.Abs(rigid.velocity.x-targetSpeed*1f)>0.5f) 
				stats.stats.currentSpeed = Vector2.Lerp (stats.stats.currentSpeed, new Vector2 (targetSpeed * 1f, rigid.velocity.y), acceleration * Time.deltaTime);
			else
				stats.stats.currentSpeed=new Vector2(targetSpeed*1f,rigid.velocity.y);
			//возможно, придётся просто использовать acceleration без time.deltaTime
			rigid.velocity = new Vector2 (stats.stats.currentSpeed.x, rigid.velocity.y);
		}
		stats.stats.targetSpeedX = targetSpeed;
		/*		rigid.gravityScale = 1f;
		if (Mathf.Abs(rigid.velocity.x-targetSpeed*1f)>0.5f) 
			rigid.velocity = Vector2.Lerp (rigid.velocity, new Vector2 (targetSpeed * 1f, rigid.velocity.y), acceleration * Time.deltaTime);
		else
			rigid.velocity=new Vector2(targetSpeed*1f,rigid.velocity.y);
		//возможно, придётся просто использовать acceleration без time.deltaTime
		stats.stats.targetSpeedX = targetSpeed;*/
	}

	//тип 1 номер 2
	public void AchieveSpeedY (Rigidbody2D rigid, Stats stats, int targetSpeed, int acceleration)
	{
		//rigid.gravityScale = 1f;
		if (Mathf.Abs(rigid.velocity.y-targetSpeed*1f)>0.5f) 
			rigid.velocity = Vector2.Lerp (rigid.velocity, new Vector2 (rigid.velocity.x, targetSpeed * 1f), acceleration * Time.deltaTime);
		else
			rigid.velocity=new Vector2(rigid.velocity.x,targetSpeed*1f);
		//возможно, придётся просто использовать acceleration без time.deltaTime
		stats.stats.targetSpeedY = targetSpeed;
	}

	//тип 1 номер 3
	public void AchieveSpeed (Rigidbody2D rigid, Stats stats, int targetSpeedX, int targetSpeedY, int acceleration)
	{
		//rigid.gravityScale = 1f;
		Vector2 targetSpeed = new Vector2 (targetSpeedX * 1f, targetSpeedY * 1f);
		if (Mathf.Abs(rigid.velocity.sqrMagnitude-targetSpeed.sqrMagnitude)>0.25f) 
			rigid.velocity = Vector2.Lerp (rigid.velocity, targetSpeed, acceleration * Time.deltaTime);
		else
			rigid.velocity=targetSpeed;
		//возможно, придётся просто использовать acceleration без time.deltaTime
		stats.stats.targetSpeedX = targetSpeedX;
		stats.stats.targetSpeedY = targetSpeedY;
	}

	//тип 1 номер 4
	public void SetVeloc (Rigidbody2D rigid, Vector2 vect)
	{
		//rigid.gravityScale = 1f;
		rigid.velocity = new Vector2 (vect.x, vect.y);
	}

	//тип 1 номер 5
	public void Forcing (Rigidbody2D rigid, Vector2 fVect, SpFunctions sp)
	{
		//rigid.gravityScale = 1f;
		if ((fVect.x!=0) ||(fVect.y!=0))
		rigid.velocity = sp.Ortog(rigid.velocity, fVect);
		rigid.AddForce(fVect);
	}

	//тип 1 номер 6
	public void Climb(Rigidbody2D rigid, Stats stats, InfoGetTypes IGT, InfoGets inf, int numb, int targetSpeedX, int targetSpeedY, SpFunctions sp)
	{
		rigid.gravityScale = 0f;
		if (IGT.Raycaster(inf,numb,new Vector2(targetSpeedX*1f,targetSpeedY*1f),sp))
		    rigid.velocity=new Vector2(targetSpeedX*1f,targetSpeedY*1f);
		else
			rigid.velocity=new Vector2(0f,0f);
		stats.stats.targetSpeedX = targetSpeedX;
		stats.stats.targetSpeedY = targetSpeedY;
	}

	//тип 1 номер 7
	public void MoveToClimb(Rigidbody2D rigid, Stats stats, InfoGets inf, int numb, SpFunctions sp)
	{
		RaycastHit2D hit;
		rigid.velocity = new Vector2 (0f, 0f);
		rigid.gravityScale = 0;
		int koof2;
		if (rigid.velocity.y>=0) 
			koof2=1;
		else
			koof2=-1;
		bool k = Physics2D.Raycast (inf.infoGets [numb].OBJ [0].transform.position,
		                            new Vector2(inf.infoGets [numb].VCT [0].x*sp.realSign(stats.direction),
		            							inf.infoGets [numb].VCT [0].y*koof2).normalized,
		                      inf.infoGets [numb].PRM2 [0],
		                      inf.infoGets [numb].LYR);
		hit=Physics2D.Raycast (inf.infoGets [numb].OBJ [0].transform.position,
		                       new Vector2(inf.infoGets [numb].VCT [0].x*sp.realSign(stats.direction),
		            					   inf.infoGets [numb].VCT [0].y*koof2).normalized,
		                       inf.infoGets [numb].PRM2 [0],
		                       inf.infoGets [numb].LYR);
		float koof = -1*sp.realSign (hit.normal.x);
		if (k) 
		{
				rigid.velocity=new Vector2(0f,0f);
				float length = hit.distance;
				Vector2 vect = inf.infoGets [numb].VCT [0].normalized;
				vect = new Vector2 (vect.x *koof* length, vect.y *koof2* length); 
				rigid.gameObject.transform.position = new Vector3 (rigid.gameObject.transform.position.x + vect.x,
		                                                		   rigid.gameObject.transform.position.y + vect.y,
		                                                		   rigid.gameObject.transform.position.z);
		}
	}
	

	//тип 0 номер 1
	public void ChangeLocation(Transform parent, Transform child, Vector3 vect)
	{
		child.position = new Vector3 (parent.position.x + vect.x*vect.x*Mathf.Sign(parent.localScale.x),
		                           parent.position.y + vect.y,
		                           parent.position.z + vect.z);
	}

	//тип 0 номер 2
	public void ChangeLocationSmoothly (Transform parent, Transform child, Vector3 vect, int accel)
	{
		child.position = Vector3.Lerp (child.position, new Vector3 (parent.position.x + vect.x*Mathf.Sign(parent.localScale.x),
		                           parent.position.y + vect.y,
		                                                       parent.position.z + vect.z), accel * Time.deltaTime);
	}

	//тип 0 номер 3
	public void MakeBTrigger (BoxCollider2D col, int b)
	{
		col.isTrigger = (b==1);
	}

	//тип 0 номер 4
	public void MakeCTrigger (CircleCollider2D col, int b)
	{
		col.isTrigger = (b==1);
	}
	
	//тип 0 номер 5
	public void MakeColAble(Collider2D col, int b)
	{
		col.enabled = (b == 1);
	}

	public void MakeColAble(Indicator ind, int b)
	{
		ind.Activate (b == 1);
	}

	//тип 0 номер 6
	public void MakeSizeOfCol(BoxCollider2D col, float x, float y)
	{
		col.size = new Vector2 (x, y);
	}

	//тип 0 номер 7
	public void CreateObject(GameObject obj, Transform trans, Vector3 rot, Vector2 vect)
	{
		GameObject bullet;
		bullet=Instantiate(obj, trans.position, trans.rotation) as GameObject;
		bullet.transform.Rotate (rot);
		bullet.GetComponent<Rigidbody2D>().AddForce(vect);
	}

	/*//тип 2 номер 1
	public void WeaponMove(Equipment equip, RootCharacterController control, Stats stats, SpFunctions Sp, bool isItRight)
	{
		WeaponClass weapon;
		if (isItRight)
			weapon = equip.rightWeapon;
		else
			weapon = equip.leftWeapon;
		int i;
		int j=-1;
		Rigidbody2D rigid = control.gameObject.rigidbody2D;
		clavisher clav = Sp.gameObject.GetComponent<clavisher> ();
		for (i=weapon.moveset.Length-1; i>=0; i--)
		{
			if (Sp.ClaveComparation(weapon.moveset[i].why.claves,clav))
				if (Sp.FComparation(Mathf.Abs(rigid.velocity.x),weapon.moveset[i].why.speedX*1f)&&
					Sp.FComparation(Mathf.Abs(rigid.velocity.y),weapon.moveset[i].why.speedY*1f)&&
					(weapon.moveset[i].why.employment<=stats.stats.employment)&&
					Sp.IntComparation(stats.stats.direction+2, weapon.moveset[i].why.direction)&&
					Sp.IntComparation(stats.stats.groundness,weapon.moveset[i].why.groundness)&&
					Sp.IntComparation(stats.stats.obstacleness,weapon.moveset[i].why.obstacleness)&&
					Sp.IntComparation(stats.stats.groundness,weapon.moveset[i].why.groundness)&&
					Sp.ComprFunctionality(stats.stats.maxInteraction,weapon.moveset[i].why.maxinteraction)&&
					Sp.ComprFunctionality(stats.stats.interaction,weapon.moveset[i].why.interaction)&&
					Sp.IntComparation(stats.stats.upness,weapon.moveset[i].why.upness)&&
					Sp.IntComparation(stats.stats.specialness,weapon.moveset[i].why.specialness)&&
					(!weapon.moveset[i].chosen))
				{				
					j=i;
					break;
				}
		}
		if (j>=0)
		{
			control.activityNumb=Sp.AddActivity(control.whatToEmploy,weapon.moveset[j],control.activityNumb);
			stats.stats.employment-=weapon.moveset[j].employ;
			if (weapon.moveset[j].soWhat.groundness>=0) stats.stats.groundness=weapon.moveset[j].soWhat.groundness;
			if (weapon.moveset[j].soWhat.obstacleness>=0) stats.stats.obstacleness=weapon.moveset[j].soWhat.obstacleness;
			if (weapon.moveset[j].soWhat.interaction>=0)stats.stats.interaction=weapon.moveset[j].soWhat.interaction;
			if (weapon.moveset[j].soWhat.specialness>=0)stats.stats.specialness=weapon.moveset[j].soWhat.specialness;
			weapon.moveset[i].chosen=true;
		}
	}*/

	//тип 2 номер 2
	public void WeaponHit(HitController hitControl, 
	                      float pDamage,float fDamage,float dDamage,float aDamage,
	                      int attack,
	                      bool backStab,
	                      float backStabKoof, int time, int beginTime, int endTime, int direction)//beginTime должен быть больше endTime
	{
		hitControl.pDamage = pDamage;
		hitControl.fDamage = fDamage;
		hitControl.dDamage = dDamage;
		hitControl.aDamage = aDamage;
		hitControl.attack = attack;
		hitControl.backStab = backStab;
		hitControl.backStabKoof = backStabKoof;
		if (hitControl.actTime==0)
			hitControl.actTime= time;
		if ((hitControl.actTime > endTime)&&(hitControl.actTime<=beginTime))
			hitControl.GetComponent<BoxCollider2D> ().enabled = true;
		else if (hitControl.actTime >beginTime)
			hitControl.GetComponent<BoxCollider2D> ().enabled = false;
		hitControl.direction = direction;


	}

	//тип 2 номер 3
	public void WeaponPush(HitController hitControl, Rigidbody2D rigid, 
	                      float pDamage,float fDamage,float dDamage,float aDamage,
	                      int attack,
	                      bool backStab,
	                      float backStabKoof, int time, int beginTime, int endTime, int timeToPush,
	                      Vector2 fVect)//beginTime должен быть больше endTime
	{
		int i;
		bool b=false;
		hitControl.pDamage = pDamage;
		hitControl.fDamage = fDamage;
		hitControl.dDamage = dDamage;
		hitControl.aDamage = aDamage;
		hitControl.attack = attack;
		hitControl.backStab = backStab;
		hitControl.backStabKoof = backStabKoof;
		if (hitControl.actTime==0)
			hitControl.actTime= time;
		if ((hitControl.actTime > endTime)&&(hitControl.actTime<=beginTime))
			hitControl.GetComponent<BoxCollider2D> ().enabled = true;
		else if (hitControl.actTime >beginTime)
			hitControl.GetComponent<BoxCollider2D> ().enabled = false;
		for (i=0; i<hitControl.list.Count; i++)
			if (hitControl.list [i] != rigid.gameObject)
				b = true;
		if ((b)&&(hitControl.actTime==timeToPush))
		{
			if (rigid.gameObject.GetComponent<Stats>()!=null)
			{
				if (rigid.gameObject.GetComponent<Stats>().stats.groundness==4)
					rigid.AddForce(fVect);
			}
			else 
				rigid.AddForce(fVect);
		}
	}

	//тип 2 номер 4
	public void Shoot(float pDamage,float fDamage,float dDamage,float aDamage,
	                  int attack,
	                  bool backStab,
	                  float backStabKoof, int actTime, int direction,
	                  GameObject bullet, Vector2 veloc, Vector3 pos)
	{

		GameObject obj = Instantiate (bullet,pos, bullet.transform.rotation) as GameObject;
		obj.GetComponent<Rigidbody2D>().velocity = new Vector2 (veloc.x, veloc.y);
		HitController hitControl = obj.GetComponentInChildren<HitController> ();
		hitControl.pDamage = pDamage;
		hitControl.fDamage = fDamage;
		hitControl.dDamage = dDamage;
		hitControl.aDamage = aDamage;
		hitControl.attack = attack;
		hitControl.backStab = backStab;
		hitControl.backStabKoof = backStabKoof;
		hitControl.actTime= actTime;
		hitControl.GetComponent<BoxCollider2D> ().enabled = true;
		hitControl.direction = direction;
	}

	//тип 3 номер 0
	public void PrepareWeapon(WeaponClass weapon, SmartObjectController obj, int ready, bool b)
	{
		obj.battleStance = b;
		weapon.active = b;
		weapon.ready = ready;
	}

	//тип 4 номер 0
	public void GravityOn(Rigidbody2D rigid, float f, bool stop)
	{	
		if (stop)
			rigid.velocity = new Vector2 (rigid.velocity.x, 0f);
		rigid.gravityScale = f;
	}

	//тип 5 номер 0
	public void ChangeWeapon(Equipment equip, int numb, bool right, int lActNumb,int rActNumb, SpFunctions sp)
	{
		ItemClass item = equip.bag [numb].GetComponent<ItemClass> ();
		int wActNumb = right == true ? rActNumb : lActNumb;
		GameObject weapon;
		WeaponClass chWeapon = right==true ? item.objects[0].GetComponent<WeaponClass>() : item.objects [1].GetComponent<WeaponClass>();
		WeaponClass defWeapon = right==true ? equip.defRightWeapon.GetComponent<WeaponClass>() : equip.defLeftWeapon.GetComponent<WeaponClass>();
		if (!string.Equals(equip.gameObject.GetComponent<Actions> ().activities [wActNumb].weapon.gameObject.name,
		                   chWeapon.gameObject.name+"(Clone)")) 
		{
			s1=equip.gameObject.GetComponent<Actions> ().activities [wActNumb].weapon.gameObject.name;
			s2=chWeapon.gameObject.name;
			if (item.objects[0].GetComponent<WeaponClass>().handEmployment==2)
			{
				if (!string.Equals(equip.gameObject.GetComponent<Actions> ().activities [lActNumb].weapon.gameObject.name, 
				                   equip.defLeftWeapon.name))
				{
					DeleteWeapon(equip,equip.gameObject.GetComponent<Actions> ().activities [lActNumb].actMode, 
					             0==1, lActNumb,sp,equip.defLeftWeapon.GetComponent<WeaponClass>());
				}
				if (!string.Equals(equip.gameObject.GetComponent<Actions> ().activities [rActNumb].weapon.gameObject.name, 
				                   equip.defRightWeapon.name))
				{
					DeleteWeapon(equip,equip.gameObject.GetComponent<Actions> ().activities [rActNumb].actMode, 
					             1==1, rActNumb,sp,equip.defRightWeapon.GetComponent<WeaponClass>());
				}

			}
			else 
			{
				if (!string.Equals(equip.gameObject.GetComponent<Actions> ().activities [wActNumb].weapon.gameObject.name, 
			    	              defWeapon.gameObject.name))
				{
					DeleteWeapon(equip,equip.gameObject.GetComponent<Actions> ().activities [wActNumb].actMode ,
					             right,wActNumb,sp,defWeapon);
				}
				if (string.Equals(equip.gameObject.GetComponent<Actions> ().activities [rActNumb].weapon.gameObject.name,
			   	               item.objects[0].name+"(Clone)"))
				{
					DeleteWeapon(equip,numb, 1==1, rActNumb,sp,equip.defRightWeapon.GetComponent<WeaponClass>());
				}
				if (item.objects[0].GetComponent<WeaponClass>().handEmployment==1)
				{
					if (string.Equals(equip.gameObject.GetComponent<Actions> ().activities [lActNumb].weapon.gameObject.name,
				   	               item.objects[1].name+"(Clone)"))
						DeleteWeapon(equip,numb, 0==1, lActNumb,sp,equip.defLeftWeapon.GetComponent<WeaponClass>());
				}
			}
			AddWeapon(equip, numb, right,wActNumb,sp);
		}
		else
		{
			kk++;
			DeleteWeapon(equip, numb, right,wActNumb,sp,defWeapon);
		}
	}


	public void AddWeapon(Equipment equip, int numb, bool right, int wActNumb, SpFunctions sp)
	{
		int i,j;
		int cLen = equip.bag [numb].GetComponent<ItemClass>().coordinates.Length/3;
		CharacterAnimator anim = equip.gameObject.GetComponent<CharacterAnimator> ();
		ItemClass item = equip.bag [numb].GetComponent<ItemClass> ();
		GameObject weapon;
		weapon = Instantiate (right == true ? item.objects [0] : item.objects [1],
		                      new Vector3 (equip.gameObject.transform.position.x + item.coordinates [0]*equip.gameObject.transform.lossyScale.x,
		             equip.gameObject.transform.position.y + item.coordinates [1]*equip.gameObject.transform.lossyScale.y,
		             equip.gameObject.transform.position.z + item.coordinates [2]),
		                      equip.gameObject.transform.rotation)as GameObject;
		weapon.transform.localScale=new Vector3(weapon.transform.localScale.x*sp.realSign(equip.gameObject.transform.localScale.x),
		                                        weapon.transform.localScale.y,
		                                        weapon.transform.localScale.z);
		weapon.transform.SetParent (equip.gameObject.transform);
		sp.SetMoveset(equip.gameObject,weapon.GetComponent<WeaponClass>());
		j = 0;
		while (j<item.parametres1.Length) {
			if (item.parametres1 [j] != 0)
				break;
			anim.allParts [item.parametres2 [j]].parts.Add (weapon.GetComponent<PartConroller> ());
			j++;
		}
		for (i=2; i<equip.bag[numb].GetComponent<ItemClass>().objects.Length; i++) {
			listOfObjects.Add (GameObject.Instantiate (item.objects [i],
			                                           new Vector3 (equip.gameObject.transform.position.x + item.coordinates [i * cLen]*equip.gameObject.transform.lossyScale.x,
			             equip.gameObject.transform.position.y + item.coordinates [i * cLen + 1]*equip.gameObject.transform.lossyScale.y,
			             equip.gameObject.transform.position.z + item.coordinates [i * cLen + 2]),
			                                           equip.gameObject.transform.rotation)as GameObject);
			listOfObjects [i-2].transform.localScale=new Vector3(listOfObjects [i-2].transform.localScale.x*sp.realSign(equip.gameObject.transform.localScale.x),
			                                                     listOfObjects [i-2].transform.localScale.y,
			                                                     listOfObjects [i-2].transform.localScale.z);
			listOfObjects [i-2].transform.SetParent (equip.gameObject.transform);
			listOfObjects[i-2].GetComponent<PartConroller>().right=(right==true? 1: -1);
			while (j<item.parametres1.Length) {
				if (item.parametres1 [j] != i)
					break;
				anim.allParts [item.parametres2 [j]].parts.Add (listOfObjects [i-2].GetComponent<PartConroller> ());
				j++;
			}
		}
		equip.gameObject.GetComponent<Actions> ().activities [wActNumb].weapon = weapon.GetComponent<WeaponClass> ();
		equip.gameObject.GetComponent<Actions> ().activities [wActNumb].actMode = numb;
		if (weapon.GetComponent<WeaponClass> ().handEmployment == 2) 
		{
			equip.gameObject.GetComponent<Actions> ().activities [wActNumb-1].weapon = weapon.GetComponent<WeaponClass> ();
			equip.gameObject.GetComponent<Actions> ().activities [wActNumb-1].actMode = numb;
			equip.leftWeapon=weapon.GetComponent<WeaponClass>();
		}
		if (right)
			equip.rightWeapon=weapon.GetComponent<WeaponClass>();
		else
			equip.leftWeapon=weapon.GetComponent<WeaponClass>();
		listOfObjects.Clear ();

	}

	public void DeleteWeapon(Equipment equip, int numb, bool right, int wActNumb, SpFunctions sp, WeaponClass defWeapon)
	{
		int i,j;
		int cLen = equip.bag [numb].GetComponent<ItemClass>().coordinates.Length/3;
		CharacterAnimator anim = equip.gameObject.GetComponent<CharacterAnimator> ();
		ItemClass item = equip.bag [numb].GetComponent<ItemClass> ();
		GameObject weapon;
		weapon=null;
		j = 0;
		weapon=equip.gameObject.GetComponent<Actions> ().activities [wActNumb].weapon.gameObject;
		while (j<item.parametres1.Length) {
			if (item.parametres1 [j] != 0)
				break;
			anim.allParts [item.parametres2 [j]].parts.Remove (weapon.GetComponent<PartConroller> ());
			j++;
		}
		Destroy (weapon);
		for (i=2; i<item.objects.Length; i++) 
		{
			obj=sp.FindPart(anim.allParts, item.objects[i].name);
			s=item.objects[i].name;
			listOfObjects.Add (obj);
			while (j<item.parametres1.Length) {
				if (item.parametres1 [j] != i)
					break;
				anim.allParts [item.parametres2 [j]].parts.Remove (listOfObjects [i-2].GetComponent<PartConroller> ());
				j++;
			}
		}
		for (i=listOfObjects.Count-1;i>=0;i--)
		{
			Destroy(listOfObjects[i]);
			listOfObjects.RemoveAt(i);
		}
		equip.gameObject.GetComponent<Actions> ().activities [wActNumb].weapon = defWeapon;
		if (weapon.GetComponent<WeaponClass> ().handEmployment == 2) 
		{
			equip.gameObject.GetComponent<Actions> ().activities [right==true? wActNumb-1:wActNumb+1].weapon = (right==true? equip.defLeftWeapon: equip.defRightWeapon).GetComponent<WeaponClass>();
			equip.gameObject.GetComponent<Actions> ().activities [right==true? wActNumb-1:wActNumb+1].actMode = 0;
			equip.leftWeapon=equip.defLeftWeapon.GetComponent<WeaponClass>();
		}
		if (right)
			equip.rightWeapon=defWeapon;
		else
			equip.leftWeapon=defWeapon;
		equip.gameObject.GetComponent<Actions> ().activities [wActNumb].actMode = 0;

	}

	//тип 6 номер 1
	public void Punchuntilground(PunchUntillGround p, 
	                             RootCharacterController c,
	                             WeaponClass w,
	                             int n, int b, int e,
	                             SpFunctions s,
	                             HitController h)
	{
		if (p.stage == 0) 
			p.SetValues (c, w, n, b, e, s, h);
		p.Work ();

	}

	//тип 6 номер 2
	public void Twodifferentweaponslock (TwoDifferentWeaponsLock t,
	                                     Equipment e,
	                                     WeaponClass w,
	                                     RootCharacterController c,
	                                     int n)
	{
		t.SetEquip (e);
		t.SetValues (c, w, n);
		t.Work ();
	}

	//тип 6 номер 3
	public void Holdandunleash(HoldAndUnleash hld, 
	                             RootCharacterController c,
	                             WeaponClass w,
	                             int n, int b, int e, int cN,
	                             SpFunctions s,
	                             HitController h,
	                           clavisher cl)
	{
		if (hld.stage == 0) 
			hld.SetValues (c, w, n, b, e,cN, s, h, cl);
		hld.Work ();
		
	}

	//тип 6 номер 4
	public void Aimingshoot(AimingShoot a, 
	                        RootCharacterController c,
	                        WeaponClass w,
	                        int n, int cN1, int cN2, int b, int e,
	                        SpFunctions s,
	                        clavisher cl, Vector3 p)
	{
		if (a.stage == 0) 
			a.SetValues (c,w,n,cN1,cN2,b,e,s,cl,p);
		a.Work ();

	}

	//тип 6 номер 5
	public void Punchuntiltarget(PunchUntilTarget p, 
	                             RootCharacterController c,
	                             WeaponClass w,
	                             int n, int b, int e,
	                             SpFunctions s,
	                             HitController h)
	{
		if (p.stage == 0) 
			p.SetValues (c, w, n, b, e, s, h);
		p.Work ();
		
	}

	//тип 6 номер 6
	public void Shieldcontroller (ShieldController sh, 
	                              RootCharacterController c,
	                              WeaponClass w,
	                              int n, int b,int cN,
	                              SpFunctions s,
	                              clavisher cl)
	{
		if (sh.stage == 0) 
			sh.SetValues (c, w, n, b, cN, s, cl);
		sh.Work ();
		
	}

	//тип 10 номер 1
	public void Experimental()
	{
		kk++;
	}
}
