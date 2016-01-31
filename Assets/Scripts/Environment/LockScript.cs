using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, который характеризует счобой замок. Замки - это специальные запреты на использование того или иного предмета,
/// если у игрока нет ключа, отпирающий замок. Им можно закрывать двери и сундуки.
/// </summary>
[System.Serializable]
public class LockScript{

    public enum lockTypes {nothing, iron, silver,gold};
    public int lockType; //Какой тип ключа требует этот замок
    public bool opened; // Был ли замок отпёрт, или всё ещё требует ключ

    public LockScript()
    {
        lockType = (int)lockTypes.nothing;
        opened = true;
    }

    public LockScript(int _type, bool _opened)
    {
        lockType = _type;
        opened = _opened;
    }
}

/// <summary>
/// Этот тип замков используется для особых дверей, которым нужен уникальный ключ
/// </summary>
[System.Serializable]
public class SpecialLockScript: LockScript
{

    private string keyID; //какой нужен ключ, чтобы отпереть этот замок
    
    public SpecialLockScript()
    {
        lockType = (int)lockTypes.nothing;
        opened = true;
        keyID = "";
    }

    public SpecialLockScript(string ID, bool _opened)
    {

        opened = _opened;
        keyID = ID;
    }

    public string GetKeyID()
    {
        return keyID;
    }

    public void SetKeyID(string _keyID)
    {
        keyID = _keyID;
    }
}