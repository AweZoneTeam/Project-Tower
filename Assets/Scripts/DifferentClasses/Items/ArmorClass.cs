using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, отвечающий за доспехи
/// </summary>
[System.Serializable]
public class ArmorClass : ItemClass
{
    public string armorType;
    public DefenceClass defence;//Добавочная защита
    public float velocity;//Насколько увеличивается скорость при надевании доспеха?
    public List<BuffClass> buffs;//Какие баффы накладываются при надевании?
    public int setNumber;//Сколько предметов составляют сет
    public BuffClass setBuff;//Бафф, накладывающийся при надевании доспехов одного сета
}
