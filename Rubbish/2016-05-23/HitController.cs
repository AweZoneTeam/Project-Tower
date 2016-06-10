using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Скрипт, прикрепляющийся к BoxCollider и делающий из него хитбокс.
/// </summary>
public class HitController : MonoBehaviour
{
    public BoxCollider col;//Область удара
    private List<string> enemies;//По каким тегам искать врагов?
    public float actTime;// Время активности хитбокса (как долго ещё хитбокс атакует?)
    public OrganismStats target;//Кого атаковать?

    public HitClass hitData;

    public List<GameObject> list;//Список всех атакованных противников. (чтобы один удар не отнимал hp дважды)

    //Инициализация
    public void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    public void FixedUpdate()
    {
        if ((actTime >= 0f) && (col.enabled == true)) {
            actTime -= Time.deltaTime;
        }
        if (actTime < 0f) {
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
        if (enemies != null ? (enemies.Count == 0? false : enemies.Contains(other.gameObject.tag)):true)
        {
            DmgObjController target = other.gameObject.GetComponent<DmgObjController>();
            if (target != null)
            {
                if (!list.Contains(target.gameObject))
                {
                    list.Add(other.gameObject);
                    Hit(target);
					target.Hitted();
                }
            }
        }
    }

    void Hit(DmgObjController dmg)
    {
        OrganismStats target = dmg.GetOrgStats();
        DefenceClass defence = CalculateDefence(target.defence+target.addDefence);
        GameObject obj = dmg.gameObject;
        if (target != null)
        {

			Rigidbody rigid = obj.GetComponent<Rigidbody>();

			Vector3 _dir = new Vector3(
				SpFunctions.RealSign(gameObject.transform.lossyScale.x) * hitData.direction.x,
				hitData.direction.y, 
				0f);
			if (target.addDefence.pDefence <= 0) 
			{
                if (dmg.HitParticle)
				Instantiate (dmg.HitParticle, dmg.transform.position, Quaternion.Euler (new Vector3 (0f, SpFunctions.RealSign (gameObject.transform.lossyScale.x) * 90f, 0f)));
			}
			else 
			{
                if (dmg.DefParticles)
				Instantiate (dmg.DefParticles, dmg.transform.position, Quaternion.Euler (new Vector3 (0f, SpFunctions.RealSign (gameObject.transform.lossyScale.x) * -90f, 0f)));
			}
			if (defence.stability <= hitData.attack - 2)
            {
				if(hitData.direction==Vector3.zero)
				{
					if (target.stunTimer <= 0f)
					{
						target.stunTimer = target.macroStun;
						StartCoroutine(target.Stunned(target.macroStun));
					}
					else
					{
						target.stunTimer = target.macroStun;
					}
				}
				else
				{
					rigid.AddForce(_dir);
				}
            }
            else if (defence.stability <= hitData.attack)
            {
                target.stunTimer = target.microStun;
                StartCoroutine(target.Stunned(target.microStun));
            }
            bool crit= (Random.Range(0, 100) > target.critResistance);
            if ((SpFunctions.RealSign(gameObject.transform.lossyScale.x * obj.transform.lossyScale.x) > 0f)||(crit))
            {
                target.health -= (hitData.pDamage * (100 - defence.pDefence) / 100 + hitData.fDamage * (100 - defence.fDefence) / 100 +
                                  hitData.aDamage * (100 - defence.aDefence) / 100 + hitData.dDamage * (100 - defence.dDefence) / 100) * hitData.backStabKoof;
                
            }
            else
            {
                target.health -= hitData.pDamage * (100 - defence.pDefence) / 100 + hitData.fDamage * (100 - defence.fDefence) / 100 +
                                 hitData.aDamage * (100 - defence.aDefence) / 100 + hitData.dDamage * (100 - defence.dDefence) / 100;
            }
            if (dmg is PersonController)
            {
                PersonController person = (PersonController)dmg;
                BuffsList buffs = person.buffList;
                foreach (BuffClass buff in hitData.effects)
                {
                    buffs.AddBuff(buff);
                }
            }
            target.OnHealthChanged(new OrganismEventArgs(0f));
            if (dmg is AIController)
            {
                AIController ai = (AIController)dmg;
                ai.WhoAttacksMe = new TargetWithCondition(transform.parent.gameObject, "enemy");
            }
        }
    }

    /// <summary>
    /// Установить, по каким целям будет попадать хитбокс
    /// </summary>
    public void SetEnemies(List<string> _enemies)
    {
        enemies = _enemies;
    }

    DefenceClass CalculateDefence(DefenceClass _defence)
    {
        DefenceClass newDefence=new DefenceClass(Mathf.RoundToInt(Mathf.Clamp(_defence.pDefence,0f,100f)),
                                                 Mathf.RoundToInt(Mathf.Clamp(_defence.fDefence, 0f, 100f)),
                                                 Mathf.RoundToInt(Mathf.Clamp(_defence.aDefence, 0f, 100f)),
                                                 _defence.dDefence,
                                                 _defence.stability);
        return newDefence;
    }

}
