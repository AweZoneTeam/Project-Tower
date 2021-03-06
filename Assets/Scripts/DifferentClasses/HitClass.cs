﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HitClass
{
    public string hitName;
    public float pDamage, fDamage, dDamage, aDamage; //физический, огненный, тёмный, ядовитый уроны соответственно
    public int attack;//насколько данная атака пробивает стабилити
    public float backStabKoof;//во сколько увеличивается урон, если удар нанесён со спины? 
    public int direction;//1-атака толкает прямо, 2-вверх, 3-сбивает с ног, 4-просто оглушает
    public Vector3 hitPosition;
    public Vector3 hitSize;
    public float hitTime, beginTime, endTime;//Сколько времени длится сама атака, когда начинает наноситься удар и когда он заканчивается наноситься.
    public List<BuffClass> effects;//Как ещё подействует атака на цель (какие бафф и дебаффы навесятся при ударе?)
}
