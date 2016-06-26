using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Контроллер издаваемых персонажем звуков
/// </summary>
public class CharacterAudio : InterObjAudio
{

    #region fields

    public List<MaterialSwitchSoundData> materialSwitches = new List<MaterialSwitchSoundData>();

    protected List<string> whatIsGround = new List<string>(){"ground","metal","wood","grass","tile","stairs"};

    #endregion //fields

    public override void Initialize()
    {
        base.Initialize();
    }

    public void AnalyzeUnderMaterial(string layerName)
    {
        if (whatIsGround.Contains(layerName))
        {
            MaterialSwitchSoundData matSwitch = null;
            matSwitch = materialSwitches.Find(x => string.Equals(LayerMask.LayerToName(x.materialName),layerName));
            if (matSwitch != null)
            {
                AkSoundEngine.SetSwitch((uint)matSwitch.GroupID, (uint)matSwitch.ValueID, gameObject);
            }
        }
    }
}

/// <summary>
/// Специальный класс, которым учитывается звучание при взаимодействии с различными материалами
/// </summary>
[System.Serializable]
public class MaterialSwitchSoundData
{

    #region fields

    public int materialName;
    public string switchName;
    protected int groupID, valueID;
    public int GroupID { get { return groupID; } }
    public int ValueID{ get { return valueID; } }

    #endregion //fields

    #region parametres

    public string unitName;
    public int unitIndex = 0, groupIndex = 0, valueIndex=0; 
    
    #endregion //parametres

    public MaterialSwitchSoundData()
    {
        materialName = 0;
    }

    public void SetIDs(int _group, int _value)
    {
        groupID = _group;
        valueID = _value;
    }

}
