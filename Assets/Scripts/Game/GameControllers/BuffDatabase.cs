using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// База данных, в которой находятся все баффы игры
/// </summary>
public class BuffDatabase : MonoBehaviour
{

    #region dictionaries

    protected Dictionary<string, BuffClass> buffDict = new Dictionary<string, BuffClass>();//Словарь всех баффов игры
    public Dictionary<string, BuffClass> BuffDict { get { return buffDict; } }

    #endregion //dictionaries

    #region fields

    public List<BuffClass> buffList = new List<BuffClass>();//Список всех баффов игры

    #endregion //fields

    public void Awake()
    {
        FormDictionaries();
    }

    /// <summary>
    /// Сформировать словарь
    /// </summary>
    void FormDictionaries()
    {
        buffDict = new Dictionary<string, BuffClass>();

        foreach (BuffClass buff in buffList)
        {
            buffDict.Add(buff.buffName, buff);
        }
    }

}
