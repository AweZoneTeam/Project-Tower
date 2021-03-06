﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой пространство - особая область, которая знает, что находится внутри неё и за пределами которой камера выйти не может.
/// Такие области имеют определённый размер (3D коллайдер) и все объекты комнаты находятся внутри него.
/// Также пространства могут иметь подпространства и соседние пространства.
/// </summary>
public class AreaClass : MonoBehaviour
{
    public AreaID id=new AreaID();
    public List<RoomConnection> subAreas = new List<RoomConnection>();//какие пространства являются подчинёнными
    public List<RoomConnection> neigbAreas = new List<RoomConnection>();//какие пространства соседствуют с этим
    public List<GameObject> container=new List<GameObject>();//Что содержит в себе это пространство. Содержание подпространств не учитывается.
    public List<GameObject> lightSources = new List<GameObject>();//Источники освещения в данном пространстве
    public List<GameObject> hideForeground = new List<GameObject>();//Какие объекты исчезают из поля зрения камеры, при входе в область
    public Vector3 position;//координаты центра пространства (пространство по форме обязательно представляет собой параллелепипед)
    public Vector3 size;//Каковы размеры пространства: длина, глубина и высота

    public GameObject GetDoor(AreaClass room)
    {
        for (int i = 0; i < subAreas.Count; i++)
        {
            if (subAreas[i].room == room)
            {
                return subAreas[i].door;

            }
        }
        for (int i = 0; i < neigbAreas.Count; i++)
        {
            if (neigbAreas[i].room == room)
            {
                return neigbAreas[i].door;
            }
        }
        return null;
    }
}

/// <summary>
/// Набор параметров, идентифицирующих пространство, в котором персонаж находится
/// </summary>
[System.Serializable]
public class AreaID
{
    public string name; // имя пространства (если у него нет родителей), или имя родительского пространства (например, в случае комнат)
    public int floor, room;//Этаж и номер комнаты или пространства
    public string plane;//план, на котором расположено пространство.
    public float coordZ; //z-координата, на которой должен располагаться персонаж, если он перешёл в данное пространство.

    /// <summary>
    /// Дефолтный конструктор
    /// </summary>
    public AreaID()
    {
        name = "defName";
        floor = 0;
        room = 1;
        plane = "A";
        coordZ = 0f;
    }
}