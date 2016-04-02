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
    private float hp;

    public float HP
    {
        get { return hp; }
        set { hp = value; }
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
    }

    private AreaClass room;

    public AreaClass Room
    {
        get { return room; }
        set { room = value; }
    }
}

/// <summary>
/// Событийные данные о смене активного вооружения
/// </summary>
public class ItemChangedEventArgs : EventArgs
{
    public ItemChangedEventArgs(ItemClass _item, string _type)
    {
        item = _item;
        itemType = _type;
    }

    private ItemClass item;
    private string itemType;

    public ItemClass Item
    {
        get { return item; }
        set { item = value; }
    }

    public string ItemType
    {
        get { return itemType; }
        set { itemType = value; }
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