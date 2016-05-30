using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс целей, с которыми ИИ нужно провзаимодействовать определённым образом
/// </summary>
[System.Serializable]
public class TargetWithCondition
{
    public Vector3 position;//Чтобы пользоваться вэйпоинтом, нужно либо знать его позицию (если он статичный),
    public GameObject target;// либо знать с каким объектом он ассоциируется
    public AreaClass areaPosition;//В какой комнате находится вэйпоинт
    /*public string condition;//Условие, чтобы цель была выполнена*/
    public string id;//идентификатор условия
    public int argument;//Аргумент данного условия
    public string targetType;

    public TargetWithCondition(GameObject _target, string _targetType)
    {
        target = _target;   
        targetType = _targetType;
    }

    public TargetWithCondition(GameObject _target, string _targetType, int _argument)
    {
        target = _target;
        targetType = _targetType;
        argument = _argument;
    }

    public bool HasTarget()
    {
        return (target != null);
    }

}

/// <summary>
/// Класс, представляющий собой маршрут, которым пользует ИИ
/// </summary>
[System.Serializable]
public class RouteClass
{
    public string routeName;
    public List<TargetWithCondition> waypoints = new List<TargetWithCondition>();
}
