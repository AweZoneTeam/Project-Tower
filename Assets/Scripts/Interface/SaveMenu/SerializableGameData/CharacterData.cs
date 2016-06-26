using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Необходимая информация об интерактивном объекте
/// </summary>
[XmlType("InterObjData")]
[XmlInclude(typeof(ChestData))]
[XmlInclude(typeof(FlameObjData))]
[XmlInclude(typeof(MechData))]
[XmlInclude(typeof(CharacterData))]
[XmlInclude(typeof(PersonData))]
[XmlInclude(typeof(AIData))]
[XmlInclude(typeof(SpiderData))]
public class InterObjData
{

    [XmlElement("ID")]
    public string objId;

    [XmlElement("SpawnID")]
    public string spawnId;

    [XmlElement("Position")]
    public Vector3 position;

    [XmlElement("RoomPosition")]
    public string roomPosition;

    [XmlElement("Orientation")]
    public int orientation;

    public InterObjData()
    {
    }

    public InterObjData(InterObjController _obj)
    {
        position = _obj.transform.position;
        roomPosition = _obj.GetRoomPosition().id.areaName;
        orientation = (int)_obj.GetDirection().dir;
    }

}

/// <summary>
/// Информация о сундуке и его содержимом
/// </summary>
[XmlType("ChestData")]
public class ChestData: InterObjData
{

    [XmlElement("Opened")]
    public bool opened;

    [XmlArray("ChestContent")]
    [XmlArrayItem("ChestItem")]
    public List<BagSlotInfo> chestContent = new List<BagSlotInfo>();

    public ChestData()
    { }

}

/// <summary>
/// Информация о поджигаемом объекте
/// </summary>
[XmlType("FlammableObjData")]
public class FlameObjData : InterObjData
{

    [XmlElement("Burned")]
    public bool burned;

    public FlameObjData()
    { }

}

/// <summary>
/// Информация о механизме
/// </summary>
[XmlType("MechanismData")]
public class MechData : InterObjData
{
    [XmlElement("Activated")]
    public bool activated;

    public MechData()
    { }

}

/// <summary>
/// Информация о живом объекте
/// </summary>
[XmlType("CharacterData")]
[XmlInclude(typeof(PersonData))]
[XmlInclude(typeof(AIData))]
[XmlInclude(typeof(SpiderData))]
public class CharacterData: InterObjData
{
    [XmlElement("MaxHealth")]
    public float maxHealth;

    [XmlElement("Health")]
    public float health;

    public CharacterData()
    { }

    public CharacterData(DmgObjController _character)
    {
        position = _character.transform.position;
        roomPosition = _character.GetRoomPosition().id.areaName;
        orientation = (int)_character.GetDirection().dir;
        OrganismStats orgStats = _character.GetOrgStats();
        maxHealth = orgStats.maxHealth;
        health = orgStats.health;  
    }

}

/// <summary>
/// Информация о персонаже (в частости о главном герое)
/// </summary>
[XmlType("PersonData")]
[XmlInclude(typeof(AIData))]
[XmlInclude(typeof(SpiderData))]
public class PersonData : CharacterData
{

    [XmlArray("Buffs")]
    [XmlArrayItem("Buff")]
    public List<string> buffs = new List<string>();

    public PersonData()
    { }

    public PersonData(PersonController _character)
    {
        position = _character.transform.position;
        roomPosition = _character.GetRoomPosition().id.areaName;
        orientation = (int)_character.GetDirection().dir;

        OrganismStats orgStats = _character.GetOrgStats();
        maxHealth = orgStats.maxHealth;
        health = orgStats.health;

        buffs = new List<string>();
        foreach (BuffClass buff in _character.buffList)
        {
            if (!buff.armorSetBuff)
            {
                buffs.Add(buff.buffName);
            }
        }
    }
}

/// <summary>
/// Информация о персонаже с искусственным интеллектом
/// </summary>
[XmlType("AIData")]
[XmlInclude(typeof(TargetData))]
[XmlInclude(typeof(SpiderData))]
public class AIData : PersonData
{
    [XmlElement("Behaviour")]
    public string behaviour;

    [XmlElement("MainTarget")]
    public TargetData mainTarget;

    [XmlElement("CurrentTarget")]
    public TargetData currentTarget;

    [XmlArray("Enemies")]
    [XmlArrayItem("Enemy")]
    public List<string> enemies = new List<string>();

    [XmlElement("StartSpeech")]
    public string startSpeech;

    [XmlAttribute("CanTalk")]
    public bool canTalk;

    public AIData()
    { }

}

/// <summary>
/// Информация о цели искусственного интеллекта
/// </summary>
[XmlType("TargetData")]
public class TargetData
{
    [XmlElement("TargetName")]
    public string targetName;

    [XmlElement("TargetPosition")]
    public Vector3 targetPosition;

    [XmlElement("TargetRoom")]
    public string targetRoom;

    [XmlElement("TargetType")]
    public string targetType;

    [XmlElement("TargetArgument")]
    public int targetArgument;

    public TargetData()
    {
    }

    public TargetData(TargetWithCondition _target)
    {
        if (_target != null)
        {
            InterObjController interObj = null;
            if (_target.target != null ? (interObj = _target.target.GetComponent<InterObjController>()) != null : false)
            {
                targetName = interObj.ObjId;
                targetRoom = interObj.GetRoomPosition().id.areaName;
            }
            else
            {
                targetName = string.Empty;
                targetPosition = _target.position;
                targetRoom = _target.areaPosition!=null?_target.areaPosition.id.areaName:string.Empty;
            }
            targetType = _target.targetType;
            targetArgument = _target.argument;
        }
        else
        {
            targetName = "!";
        }
    }
}

/// <summary>
/// Информация о пауке
/// </summary>
[XmlType("SpiderData")]
public class SpiderData: AIData
{
    [XmlElement("GroundOrientation")]
    public int grOrientation;

    public SpiderData()
    { }

}