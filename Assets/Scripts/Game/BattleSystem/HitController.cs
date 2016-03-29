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
        if (enemies.Contains(other.gameObject.tag))
        {
            DmgObjController target = other.gameObject.GetComponent<DmgObjController>();
            if (target != null)
            {
                if (!list.Contains(target.gameObject))
                {
                    list.Add(other.gameObject);
                    Hit(target);
                }
            }
        }
    }

    void Hit(DmgObjController dmg)
    {
        OrganismStats target = (OrganismStats)dmg.GetStats();
        GameObject obj = dmg.gameObject;
        if (target != null)
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
                Rigidbody rigid = obj.GetComponent<Rigidbody>();
                if (hitData.direction == 1)
                {
                    rigid.AddForce(new Vector3(SpFunctions.RealSign(gameObject.transform.lossyScale.x) * 2000f, 0f, 0f));
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
            else if ((target.stability <= hitData.attack) && ((int)target.hitted < 1))
            {
                target.stunTimer = target.microStun;
                StartCoroutine(target.Stunned(target.microStun));
                target.hitted = (hittedEnum)1;
            }
            else if ((int)target.hitted < 1)
            {
                target.hitted = 0;
            }
            if (SpFunctions.RealSign(gameObject.transform.lossyScale.x * obj.transform.lossyScale.x) > 0f)
            {
                target.health -= (hitData.pDamage * (100 - target.pDefence) / 100 + hitData.fDamage * (100 - target.fDefence) / 100 +
                                 hitData.aDamage * (100 - target.aDefence) / 100 + hitData.dDamage * (100 - target.dDefence) / 100) * hitData.backStabKoof;
            }
            else
            {
                target.health -= hitData.pDamage * (100 - target.pDefence) / 100 + hitData.fDamage * (100 - target.fDefence) / 100 +
                                 hitData.aDamage * (100 - target.aDefence) / 100 + hitData.dDamage * (100 - target.dDefence) / 100;
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
        }
    }

}
