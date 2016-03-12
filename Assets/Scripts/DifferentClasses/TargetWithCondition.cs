using UnityEngine;
using System.Collections;

/// <summary>
/// Класс целей, с которыми ИИ нужно провзаимодействовать определённым образом
/// </summary>
[System.Serializable]
public class TargetWithCondition
{
    public GameObject target;
    /*public string condition;//Условие, чтобы цель была выполнена
    public string id;//идентификатор условия
    public int argument;//Аргумент данного условия*/
    public string targetType;

    public TargetWithCondition(GameObject _target, string _targetType)
    {
        target = _target;
        targetType = _targetType;
    }

    public bool HasTarget()
    {
        return (target != null);
    }
}
