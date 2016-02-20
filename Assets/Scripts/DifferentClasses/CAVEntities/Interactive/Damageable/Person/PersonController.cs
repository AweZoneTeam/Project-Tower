using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер персонажей
/// </summary>
public class PersonController : DmgObjController
{

    #region fields
    [SerializeField]private Stats stats;
    [SerializeField]private EquipmentClass equip;
    [SerializeField]private PersonController actions;
    #endregion //fields

    #region Interface

    public virtual EquipmentClass GetEquipment()
    {
        return equip;
    }

    #endregion //Interface
}
