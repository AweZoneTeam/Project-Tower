using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер босса
/// </summary>
public class BossController: AIController
{
    protected override void FormActionDictionaries()
    {
        base.FormActionDictionaries();

        #region actionBase

        actionBase.Add("set HP", SetUpHPBar);

        #endregion //actionBase

    }

    #region actions

    /// <summary>
    /// Отобразить на экране своё хп
    /// </summary>
    public virtual void SetUpHPBar(string id, int argument)
    {
        SpFunctions.SetUpBossHealth(this, string.Equals("set", id));
    }

    #endregion //actions

}
