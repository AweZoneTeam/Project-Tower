using UnityEngine;
using System.Collections;

/// <summary>
/// Класс с параметрами, характеризующими защиту персонажей
/// </summary>
[System.Serializable]
public class DefenceClass 
{
    public int pDefence, fDefence, aDefence, dDefence, stability;

    public DefenceClass ()
    {
        pDefence = 0;
        fDefence = 0;dDefence = 0;
        dDefence = 0; stability = 0;
    }

    public DefenceClass(DefenceClass _defence)
    {
        pDefence = _defence.pDefence;
        fDefence = _defence.fDefence;
        aDefence = _defence.aDefence;
        dDefence = _defence.dDefence;
        stability = _defence.stability;
    }

    public DefenceClass(int _pDef, int _fDef,  int _aDef, int _dDef, int _stability)
    {
        pDefence = _pDef;
        fDefence = _fDef;
        aDefence = _aDef;
        dDefence = _dDef;
        stability = _stability;
    }

    public static DefenceClass operator +(DefenceClass _defence, DefenceClass ddefence)
    {
        DefenceClass newDefence = new DefenceClass(_defence.pDefence + ddefence.pDefence,
                                                   _defence.fDefence + ddefence.fDefence,
                                                   _defence.aDefence + ddefence.aDefence,
                                                   _defence.dDefence + ddefence.dDefence,
                                                   _defence.stability + ddefence.stability);
        return newDefence;
    }

    public static DefenceClass operator -(DefenceClass _defence, DefenceClass ddefence)
    {
        DefenceClass newDefence = new DefenceClass(_defence.pDefence - ddefence.pDefence,
                                                   _defence.fDefence - ddefence.fDefence,
                                                   _defence.aDefence - ddefence.aDefence,
                                                   _defence.dDefence - ddefence.dDefence,
                                                   _defence.stability - ddefence.stability);
        return newDefence;
    }
}

