using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Скрипт, прикрепляющийся к BoxCollider и делающий из него хитбокс.
/// </summary>
public class HitController : MonoBehaviour 
{
	public BoxCollider2D col;//Область удара
	public List<string> enemies;//По каким тегам искать врагов?
	public float actTime;// Время активности хитбокса (как долго ещё хитбокс атакует?)
	public Organism target;//Кого атаковать?

	public float pDamage, fDamage, dDamage, aDamage; //физический, огненный, тёмный, ядовитый уроны соответственно
	public int attack;//насколько данная атака пробивает стабилити
	public bool backStab; //учитывает ли данная атака, что цель находится спиной к атаке?
	public float backStabKoof;//во сколько увеличивается урон, если удар нанесён со спины? 
	public int direction;//1-атака толкает прямо, 2-вверх, 3-сбивает с ног, 4-просто оглушает

	public List<GameObject> list;//Список всез атакованных противников. (чтобы один удар не отнимал hp дважды)
	public bool onTarget;

	//Инициализация
	public void Awake()
	{
		col = GetComponent<BoxCollider2D> ();
	}

	public void FixedUpdate()
	{
		if (actTime > 0) {
			actTime--;
		}
		if (actTime == 0) {
			col.enabled = false;
		}
		if (!col.enabled) {
			list.Clear();
		}
	}

	//Cмотрим, попал ли хитбокс по врагу, и, если попал, то идёт расчёт урона
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
			for (j2=0;j2<enemies.Count;j2++)
			if (other.gameObject.tag==enemies[j2])
			{	
				list.Add (other.gameObject);
				target=other.gameObject.GetComponent<Organism>();
				if (target!=null)
				{
						//Здесь нужно написать как просчитывается урон.
				}
			}
		}
	}
}
