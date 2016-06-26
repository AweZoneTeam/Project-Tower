using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Событийные данные о состоянии интересующего персонажа
/// </summary>
public class OrganismEventArgs : EventArgs
{
    public OrganismEventArgs(float _hp)
    {
        hp = _hp;
    }

    public OrganismEventArgs(float _maxHP, float _hp, DefenceClass _defence, float _velocity)
    {
        maxHP = _maxHP;
        hp = _hp;
        defence = _defence;
        velocity = _velocity;
    }

    private float maxHP;
    private float hp;
    private DefenceClass defence;
    private float velocity;

    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }

    public float MAXHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public DefenceClass DEFENCE
    {
        get { return defence; }
        set { defence = value; }
    }

    public float VELOCITY
    {
        get { return velocity; }
        set { velocity = value; }
    }

}

/// <summary>
/// Событийные данные о смене комнаты
/// </summary>
public class RoomChangedEventArgs : EventArgs
{
    public RoomChangedEventArgs(AreaClass _room)
    {
        room = _room;
        prevRoom = null;
    }

    public RoomChangedEventArgs(AreaClass _room, AreaClass _prevRoom)
    {
        room = _room;
        prevRoom = _prevRoom;
    }

    private AreaClass room;//следующая комната
    private AreaClass prevRoom;//предыдущая комната

    public AreaClass Room
    {
        get { return room; }
        set { room = value; }
    }

    public AreaClass PrevRoom
    {
        get { return prevRoom; }
        set { prevRoom = value; }
    }
}

/// <summary>
/// Событийные данные о смене активного вооружения
/// </summary>
public class ItemChangedEventArgs : EventArgs
{
    public ItemChangedEventArgs(ItemBunch _itemBunch, string _type)
    {
        itemBunch = _itemBunch;
        itemType = _type;
    }

    public ItemChangedEventArgs(ItemClass _item, string _type)
    {
        item = _item;
        itemType = _type;
    }

    private ItemBunch itemBunch;
    private ItemClass item;
    private string itemType;

    public ItemClass Item
    {
        get { return item; }
        set { item = value; }
    }

    public ItemBunch ItemBunch
    {
        get { return itemBunch; }
        set { itemBunch = value; }
    }

    public string ItemType
    {
        get { return itemType; }
        set { itemType = value; }
    }

}

/// <summary>
/// Событийные данные о изменении числа ресурсов
/// </summary>
public class ResourceChangedEventArgs: EventArgs
{
    public ResourceChangedEventArgs(int _ironKey, int _silverKey, int _goldKey, int _gold)
    {
        ironKey = _ironKey;
        silverKey = _silverKey;
        goldKey = _goldKey;
        gold = _gold;
    }

    public ResourceChangedEventArgs()
    {
    }

    private int ironKey, silverKey, goldKey, gold;

    public int IronKey
    {
        get { return ironKey; }
        set { ironKey = value; }
    }

    public int SilverKey
    {
        get { return silverKey; }
        set { silverKey = value; }
    }

    public int GoldKey
    {
        get { return goldKey; }
        set { goldKey = value; }
    }

    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

}

/// <summary>
/// Событийные данные об игровом сообщении, что высветится на экране.
/// </summary>
public class MessageSentEventArgs : EventArgs
{
    public MessageSentEventArgs(string _message, int _textNumb, float _time)
    {
        message = _message;
        textNumb = _textNumb;
        textTime = _time;
    }

    private string message;
    private int textNumb;//Какая из 2-ух строчек игрового окна сообщений будет использоваться?
    private float textTime;//Сколько времени показывается сообщение?

    public string Message
    {
        get { return message; }
        set { message = value; }
    }

    public int TextNumb
    {
        get { return textNumb; }
        set { textNumb = value; }
    }

    public float TextTime
    {
        get { return textTime; }
        set { textTime = value; }
    }
}

/// <summary>
/// Событийные данные о том, что баффы, действующие на персонажа как-то изменились
/// </summary>
public class BuffsChangedEventArgs : EventArgs
{
    public BuffsChangedEventArgs(BuffClass _buff)
    {
        buff = _buff;
    }

    private BuffClass buff;

    public BuffClass Buff
    {
        get { return buff; }
        set { buff = value; }
    }
    
}

/// <summary>
/// Событийные данные, используемые для осуществления журнальных событий
/// </summary>
public class JournalEventArgs : EventArgs
{
    public JournalEventArgs()
    {
    }
}

/// <summary>
/// Событийные данные об обновлении журнала
/// </summary>
public class JournalRefreshEventArgs : EventArgs
{
    public JournalRefreshEventArgs(JournalData _jData)
    {
        jData = _jData;
    }
    private JournalData jData;

    public JournalData JData
    {
        get { return jData; }
        set { jData = value; }
    }
}

public class MountEventArgs : EventArgs
{
    public MountActions mount;
    public MountEventArgs(MountActions _m)
    {
        mount = _m;
    }
}