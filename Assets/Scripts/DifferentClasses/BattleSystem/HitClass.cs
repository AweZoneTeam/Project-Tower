using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HitClass
{
	public string hitName;
    public float pDamage, fDamage, dDamage, aDamage; //физический, огненный, тёмный, ядовитый уроны соответственно
    public int attack;//насколько данная атака пробивает стабилити
    public float backStabKoof;//во сколько увеличивается урон, если удар нанесён со спины? 
	public Vector3 direction;//сила с которой противника толкнёт при ударе
	public Vector3 hitForce;//сила с которой персонажа толкнёт при ударе
    public Vector3 hitPosition;
    public Vector3 hitSize;
    public float hitTime, beginTime, endTime, comboTime;//Сколько времени длится сама атака, когда начинает наноситься удар и когда он заканчивается наноситься.
    public List<BuffClass> effects;//Как ещё подействует атака на цель (какие бафф и дебаффы навесятся при ударе?)

}
