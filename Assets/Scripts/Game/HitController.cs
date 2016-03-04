using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Скрипт, прикрепляющийся к BoxCollider и делающий из него хитбокс.
/// </summary>
public class HitController : MonoBehaviour 
{
	public BoxCollider col;//Область удара
	public List<string> enemies;//По каким тегам искать врагов?
	public float actTime;// Время активности хитбокса (как долго ещё хитбокс атакует?)
	public Organism target;//Кого атаковать?

    public HitClass hitData;

	public List<GameObject> list;//Список всех атакованных противников. (чтобы один удар не отнимал hp дважды)

	//Инициализация
	public void Awake()
	{
		col = GetComponent<BoxCollider> ();
	}

	public void FixedUpdate()
	{
        if ((actTime >= 0f) && (col.enabled == true)) {
			actTime-=Time.deltaTime; 
		}
		if (actTime <0f) {
			col.enabled = false;
            list.Clear();
            actTime = 0f;
		}
	}


    /// <summary>
    /// Настройка ХитБокса
    /// </summary>
    public void SetHitBox(float _actTime, HitClass _hitData)
    {
        actTime = _actTime;
        hitData = _hitData;
        col.enabled = true;
    }
    
	//Cмотрим, попал ли хитбокс по врагу, и, если попал, то идёт расчёт урона
	void OnTriggerEnter(Collider other)
	{
        DmgObjController dmg = other.gameObject.GetComponent<DmgObjController>();
		bool k=true;
		for (int i=0; i<list.Count; i++)
			if (other.gameObject == list [i])
		{
			k=false;
		}
		if ((k)&&(dmg!=null)) 
		{
			for (int i=0;i<enemies.Count;i++)
			if (other.gameObject.tag==enemies[i])
			{	
				list.Add (other.gameObject);
				target=(Organism)dmg.GetStats();
				if (target!=null)
				{
                    if (target.stability <= hitData.attack - 2)
                    {
                        target.hitted = (hittedEnum)(1 + hitData.direction);
                        if (target.stunTimer <= 0f)
                        {
                            target.stunTimer = target.macroStun;
                            StartCoroutine(target.Stunned(target.macroStun));
                        }
                        else
                        {
                            target.stunTimer = target.macroStun;
                        }
                        Rigidbody rigid = other.gameObject.GetComponent<Rigidbody>();
                        if (hitData.direction == 1)
                        {
                            rigid.AddForce(new Vector3(SpFunctions.realSign(gameObject.transform.lossyScale.x) * 2000f, 0f, 0f));
                        }
                        else if (hitData.direction == 2)
                        {
                            rigid.AddForce(new Vector3(0f, 2000f, 0f));
                        }
                        else if (hitData.direction == 3)
                        {
                            rigid.AddForce(new Vector3(0f, -2000f, 0f));
                        }
                    }
                    else if ((target.stability <= hitData.attack)&&((int)target.hitted<1))
                    {
                        target.stunTimer=target.microStun;
                        StartCoroutine(target.Stunned(target.microStun));
                        target.hitted = (hittedEnum)1;
                    }
                    else if ((int)target.hitted<1)
                    {
                        target.hitted=0;
                    }
                        if (SpFunctions.realSign(gameObject.transform.lossyScale.x * other.gameObject.transform.lossyScale.x) > 0f)
                        {
                            target.health -= (hitData.pDamage * (100 - target.pDefence) / 100 + hitData.fDamage * (100 - target.fDefence) / 100 +
                                                 hitData.aDamage * (100 - target.aDefence) / 100 + hitData.dDamage * (100 - target.dDefence) / 100) * hitData.backStabKoof;
                        }
                        else
                        {
                            target.health -= hitData.pDamage * (100 - target.pDefence) / 100 + hitData.fDamage * (100 - target.fDefence) / 100 +
                                                hitData.aDamage * (100 - target.aDefence) / 100 + hitData.dDamage * (100 - target.dDefence) / 100;
                        }
				}
                break;
			}
		}
	}
}
