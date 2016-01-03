using UnityEngine;
using System.Collections.Generic;

public class HitController : MonoBehaviour 
{
	public BoxCollider2D col;
	public GameObject attacker;
	public string[] enemies;
	public int actTime;
	public Organism target;

	public float pDamage, fDamage, dDamage, aDamage; //физический, огненный, тёмный, ядовитый уроны соответственно
	public int attack;//насколько данная атака пробивает стабилити
	public bool backStab; //учитывает ли данная атака, что цель находится спиной к атаке?
	public float backStabKoof;//во сколько увеличивается урон, если удар нанесён со спины? 
	public int direction;//1-атака толкает прямо, 2-вверх, 3-сбивает с ног, 4-просто оглушает

	public List<GameObject> list;
	public bool onTarget;

	public void Awake()
	{
		onTarget = false;
		col = GetComponent<BoxCollider2D> ();
	}

	public void FixedUpdate()
	{
		if (actTime > 0)
			actTime--;
		if (actTime==0)
			col.enabled=false;
		if (!col.enabled)
		{
			list.Clear();
			if (attacker!=null)
				list.Add (attacker);
			onTarget=false;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		int j1,j2;
		bool k=true;
		for (j1=0; j1<list.Count; j1++)
			if (other.gameObject == list [j1])
		{
			k=false;
		}
		if (k) 
		{
			for (j2=0;j2<enemies.Length;j2++)
			if (other.gameObject.tag==enemies[j2])
			{	
				list.Add (other.gameObject);
				target=other.gameObject.GetComponent<Organism>();
				if (target!=null)
				{
					onTarget=true;
					target.prevHealth=target.health;
					if (target.direction*(target.gameObject.transform.position.x-transform.position.x)<=0)
					{
						target.health-=(pDamage*(1f-(target.pDefence+target.addPDefence)*1f/100f)+
						                fDamage*(1f-(target.fDefence+target.addFDefence)*1f/100f)+
						                dDamage*(1f-(target.dDefence+target.addDDefence)*1f/100f)+
						                aDamage*(1f-(target.aDefence+target.addADefence)*1f/100f));
					}
					else
					{
						target.health-=(pDamage*(1f-target.pDefence*1f/100f)+
						                fDamage*(1f-target.fDefence*1f/100f)+
						                dDamage*(1f-target.dDefence*1f/100f)+
						                aDamage*(1f-target.aDefence*1f/100f));
						target.health-=pDamage*(1f-target.pDefence*1f/100f)*(backStabKoof)+
										fDamage*(1f-target.fDefence*1f/100f)*(backStabKoof)+
										 dDamage*(1f-target.dDefence*1f/100f)*(backStabKoof);
					}
					//if (attack>=3*target.stability) - здесь я хочу реализовать захват
					if (attack>=2*target.stability)
					{
						Rigidbody2D rigid=target.gameObject.GetComponent<Rigidbody2D>();
						if (rigid!=null)
						{
							if (direction==1)
								rigid.AddForce(10*pDamage*new Vector2(target.gameObject.transform.position.x-transform.position.x,0f).normalized);
							else if (direction==2)
								rigid.AddForce (10*pDamage*new Vector2(0f,1f));
						}
						target.stats.hitted=direction+1;
						rigid=null;

					}
					else if (attack>=target.stability)
					{
						target.stats.hitted=1;
					}
					target=null;
				}
			}
		}
	}
}
