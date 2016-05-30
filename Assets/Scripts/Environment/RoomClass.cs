using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Комнаты - это подвид пространств. Они обязательно должны иметь потолок, стены, пол, а также двери, соединяющие соседние комнаты
/// </summary>
public class RoomClass : AreaClass
{

    #region consts

    //Здесь описаны параметры для комнаты минимального размера. Это нужно для автоматического корректирования размера комнат.
    const float minDepth = 47f;
    const float minLength = 85f;
    const float minHeight = 65f;
    const float wallThickness = 2f;
    private Vector2 sideDoorSize = new Vector2(5f, 25f);
    private Vector2 backDoorSize = new Vector2(20f, 25f);
    
    #endregion //consts

    #region content

    public List<WallClass> walls = new List<WallClass>();
    public List<DoorClass> doors = new List<DoorClass>();

    #endregion //content

    public RoomClass(AreaClass _area): base(_area)
    {
                
    }

}

/// <summary>
/// Стены - это особые объекты, которые определяются тем, что должны находится лишь в определённых местах комнаты
/// </summary>
[System.Serializable]
public class WallClass
{
    public GameObject wall;//сама стена с её текстуркой
    public enum wallTypes { left, right, back, forward, down, up};//За какой тип стены отвечает каждый номер типа
    public int wallType;//Тип стены

    public WallClass(GameObject _wall, int _type)
    {
        wall = _wall;
        wallType = _type;
    }
}

/// <summary>
/// Специальный класс, который показывает, в какую комнату какая дверь ведёт
/// </summary>
[System.Serializable]
public class RoomConnection
{
    public GameObject door;
    public AreaClass room;
}
