using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Контроллер простейших интерактивных объектов. Они могут стоять на месте и принимать сигналы взаимодействия. 
/// </summary>
public class InterObjController : MonoBehaviour
{
    #region fields

    protected string objId;//id объекта - именно по нему мы производим его загрузку
    public string ObjId { get { return objId; } set { objId = value; } }

    public string spawnId;//Как называется объект в словаре спавнов?

    protected Direction direction;
    protected InterObjActions intActions;

    public AreaClass currentRoom;//В какой комнате находится интерактивный объект

    [SerializeField]
    protected string info;//Информация о том, как взаимодействовать с данным объектом.
    public string Info { get { return info; } }

    //protected InterObjVisual anim;
    #endregion //fields

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        if (direction == null)
        {
            direction = new Direction();
            direction.dir = (orientationEnum)SpFunctions.RealSign(transform.localScale.x);
        }
        intActions = GetComponent<InterObjActions>();
        if (intActions != null)
        {
            SetAction();
        }
        //anim = transform.GetComponentInChildren < InterObjVisual>();
    }

    protected virtual void SetAction()
    {
        intActions.Initialize();
        intActions.SetDirection(direction);
    }

    protected virtual void SetAudio()
    {

    }

    /// <summary>
    /// Если к такому объекту подойдёт персонаж и будет взаимодействовать, то контроллер предпримет нужные действия
    /// </summary>
    public virtual void Interact(InterObjController interactor)
    {
        if (intActions != null)
        {
            intActions.SetInteractor(interactor);
            intActions.Interact();
        }
    }

    public Direction GetDirection()
    {
        if (direction == null)
        {
            direction = new Direction();
        }
        return direction;
    }

    public virtual AreaClass GetRoomPosition()
    {
        return currentRoom;
    }

    /// <summary>
    /// Сменить комнату
    /// </summary>
    public virtual void ChangeRoom(AreaClass nextRoom)
    {
        if (currentRoom != null)
        {
            currentRoom.container.Remove(this);
        }
        currentRoom = nextRoom;
        currentRoom.container.Add(this);
    }

    /// <summary>
    /// Зарегестрировать объект - дать ему его id
    /// </summary>
    public virtual void RegisterObject(AreaClass room, bool inProcess)
    {
        if (inProcess)
        {
            room.container.Add(this);
        }
        objId = room.id.areaName + "/" + room.MaxRegistrationNumber.ToString();
        currentRoom = room;
    }

    /// <summary>
    /// Здесь описано правильное уничтожение объекта
    /// </summary>
    public virtual void DestroyInterObj()
    {
        if (currentRoom != null)
        {
            currentRoom.container.Remove(this);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Здесь описано правильное уничтожение объекта
    /// </summary>
    public virtual void DestroyInterObj(float t)
    {
        if (currentRoom != null)
        {
            currentRoom.container.Remove(this);
        }
        Destroy(gameObject,t);
    }

    /// <summary>
    /// Проинициализировать объект, учитывая сохранённые данные
    /// </summary>
    public virtual void AfterInitialize(InterObjData intInfo, Map map, Dictionary<string,InterObjController> savedIntObjs)
    {
        transform.position = intInfo.position;
        currentRoom = (map.rooms.Find(x => string.Equals(x.id.areaName, intInfo.roomPosition)));
        direction.dir = (orientationEnum)intInfo.orientation;
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Sign(scale.x * intInfo.orientation) * scale.x, scale.y, scale.z);
    }

    /// <summary>
    /// Собрать информацию об этом объекте
    /// </summary>
    /// <returns></returns>
    public virtual InterObjData GetInfo()
    {
        InterObjData intInfo=new InterObjData();
        intInfo.objId = objId;
        if (spawnId != null ? !string.Equals(spawnId, string.Empty) : false)
        {
            intInfo.spawnId = spawnId;
        }
        else
        {
            intInfo.spawnId = string.Empty;
        }
        intInfo.position = transform.position;
        intInfo.roomPosition = currentRoom.id.areaName;
        intInfo.orientation = (int)direction.dir;
        return intInfo;
    }

}

#if UNITY_EDITOR
/// <summary>
/// Редактор контроллеров интерактивных объектов
/// </summary>
[CustomEditor(typeof(InterObjController))]
public class InterObjEditor : Editor
{
    private Direction direction;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        InterObjController obj = (InterObjController)target;
        direction = obj.GetDirection();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.EnumMaskField("direction", direction.dir);
    }
}
#endif