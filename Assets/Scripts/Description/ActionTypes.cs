using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///Ранее из функций этого скрипта составлялись действия - то, что происходит в результате деятельностей.
/// </summary>
public static class ActionTypes
{

	//тип 1 номер 1
	/// <summary>
	/// Функция, обеспечивающая достижение персонажем заданной скорости по оси X, используя ускорение.
	/// </summary>
	public static void AchieveSpeedX (Rigidbody2D rigid, Stats stats, int targetSpeed, int acceleration)
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
	/// <summary>
	/// Достичь заданной скорости по оси Y, используя ускорение.
	/// </summary>
	public static void AchieveSpeedY (Rigidbody2D rigid, Stats stats, int targetSpeed, int acceleration)
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
	/// <summary>
	/// Достичь заданной скорости (здесь скорость - двумерный вектор), используя ускорение
	/// </summary>
	public static void AchieveSpeed (Rigidbody2D rigid, Stats stats, int targetSpeedX, int targetSpeedY, int acceleration)
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
	/// <summary>
	/// Сразу задать некоторую скорость, не ускоряясь при этом (скорость приобретается мнговенно)
	/// </summary>
	public static void SetVelocity (Rigidbody2D rigid, Vector2 vect)
	{
		//rigid.gravityScale = 1f;
		rigid.velocity = new Vector2 (vect.x, vect.y);
	}

	//тип 1 номер 5
	/// <summary>
	/// Функция, производящая толчок над телом с заданной силой
	/// </summary>
	public static void ForceMove (Rigidbody2D rigid, Vector2 fVect)
	{
		//rigid.gravityScale = 1f;
		if ((fVect.x!=0) ||(fVect.y!=0))
		rigid.velocity = SpFunctions.Ortog(rigid.velocity, fVect);
		rigid.AddForce(fVect);
	}
		
	//тип 1 номер 6
	/// <summary>
	/// Функция, обеспечивающая передвижение по таким поверхностям, как заросли, верёвка лестница и тп.)
	/// </summary>
	public static void Climb(Rigidbody2D rigid, Stats stats, InfoGetTypes IGT, InfoGets inf, int numb, int targetSpeedX, int targetSpeedY)
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
	/// <summary>
	/// Функция, обеспечивающая попадание на верёвку, лестницу.
	/// (Можно находится неподалёку от верёвки, нажать Е и сразу же оказаться на верёвке)
	/// </summary>
	public static void MoveToClimb(Rigidbody2D rigid, Stats stats, InfoGets inf, int numb)
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
	/// <summary>
	/// Меняет местоположение данного объекта (child) относительно другого объекта (parent)
	/// </summary>
	public static void ChangeLocation(Transform parent, Transform child, Vector3 vect)
	{
		child.position = new Vector3 (parent.position.x + vect.x*vect.x*Mathf.Sign(parent.localScale.x),
		                           parent.position.y + vect.y,
		                           parent.position.z + vect.z);
	}

	//тип 0 номер 2
	/// <summary>
	/// Медленно меняет расположение данного объекта child относительно родителя
	/// </summary>
	/// <param name="accel">Accel.</param>
	public static void ChangeLocationSmoothly (Transform parent, Transform child, Vector3 vect, int accel)
	{
		child.position = Vector3.Lerp (child.position, new Vector3 (parent.position.x + vect.x*Mathf.Sign(parent.localScale.x),
		                           parent.position.y + vect.y,
		                                                       parent.position.z + vect.z), accel * Time.deltaTime);
	}

	//тип 0 номер 3
	/// <summary>
	///делает 2D коллайдер твёрдым, или способным пропускать в себя объекты.
	/// </summary>
	public static void MakeBTrigger (Collider2D col, int b)
	{
		col.isTrigger = (b==1);
	}
	
	//тип 0 номер 5
	/// <summary>
	/// Включает/выключает (если b=1, то включает, если нет, то выключает) заданный 2D коллайдер, 
	/// и он начинает/заканичвает учитывать столкновения
	/// </summary>
	public static void MakeColAble(Collider2D col, int b)
	{
		col.enabled = (b == 1);
	}

	/// <summary>
	/// Включает/выключает (если b=1, то включает, если нет, то выключает) заданный индикатор, 
	/// и он начинает/заканичвает собирать информацию, касающуюся окружающей персонажа среды
	/// </summary>
	public static void MakeColAble(Indicator ind, int b)
	{
		ind.Activate (b == 1);
	}

	//тип 0 номер 6
	/// <summary>
	/// Ставит коллайдеру заданные размеры. Эта функция хороша для работы с хитбоксами
	/// </summary>
	public static void MakeSizeOfCol(BoxCollider2D col, float x, float y)
	{
		col.size = new Vector2 (x, y);
	}

	/// <summary>
	/// Создаёт указанный объект в указанном в trans месте, 
	/// поворачивает его так, как указано в rot (в углах Эйлера), и задаёт ему скорость, указанную в vect
	/// </summary>
	//тип 0 номер 7
	public static void CreateObject(GameObject obj, Transform trans, Vector3 rot, Vector2 vect)
	{
		GameObject bullet = GameObject.Instantiate(obj, trans.position, trans.rotation) as GameObject;
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
	/// <summary>
	/// Настраивает указанный хитбокс, задаёт ему параметры удара, а также время исполнения удара
	/// (описывает длительность 3-ёх стадий: подготовку, удар и восстановление)
	/// По сути - здесь совершается удар
	/// </summary>
	public static void WeaponHit(HitController hitControl, 
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
	/// <summary>
	/// Настраивает указанный хитбокс, задаёт ему параметры удара, а также время исполнения удара
	/// (описывает длительность 3-ёх стадий: подготовку, удар и восстановление)
	/// + отталкивает совершающего удар персонажа в указанном направлении с указанной силой 
	/// </summary>
	public static void WeaponPush(HitController hitControl, Rigidbody2D rigid, 
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
	/// <summary>
	/// Производит выстрел, посредством создания предмета, которому задаются скорость, 
	/// местоположение, а также параметры хитбокса
	/// </summary>
	public static void Shoot(float pDamage,float fDamage,float dDamage,float aDamage,
	                  int attack,
	                  bool backStab,
	                  float backStabKoof, int actTime, int direction,
	                  GameObject bullet, Vector2 veloc, Vector3 pos)
	{

		GameObject obj = GameObject.Instantiate (bullet,pos, bullet.transform.rotation) as GameObject;
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
	/// <summary>
	/// Достать оружие и встать в боевую стойку
	/// </summary>
	public static void PrepareWeapon(WeaponClass weapon, SmartObjectController obj, int ready, bool b)
	{
		obj.battleStance = b;
		weapon.active = b;
		weapon.ready = ready;
	}

	//тип 4 номер 0
	/// <summary>
	/// Останавливает передвижение персонажа по оси Y, и ставит такую гравитацию, которая указно в f
	/// </summary>
	public static void GravityOn(Rigidbody2D rigid, float f, bool stop)
	{	
		if (stop)
			rigid.velocity = new Vector2 (rigid.velocity.x, 0f);
		rigid.gravityScale = f;
	}

}
