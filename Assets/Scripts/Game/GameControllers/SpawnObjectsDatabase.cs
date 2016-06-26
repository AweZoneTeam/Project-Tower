using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Здесь содержится информация обо всех объектах, что могут заспавниться в любой момент игры
/// </summary>
public class SpawnObjectsDatabase : MonoBehaviour
{

    #region dictionaries

    protected Dictionary<string, GameObject> spawnDict = new Dictionary<string, GameObject>();//Словарь интерактивных объектов, что могут быть созданы в любой момент
    public Dictionary<string, GameObject> SpawnDict {get { return spawnDict; }}

    #endregion //dictionaries

    #region fields

    public List<GameObject> spawnObjList = new List<GameObject>();//Список интерактивных объектов, которые могут заспавниться

    public GameObject dropPrefab;//здесь находится экземпляр дропающегося объекта, который будет настраиваться так, как нам нужно

    #endregion //fields

    public void Awake()
    {
        FormDictionaries();
    }

    void FormDictionaries()
    {

        spawnDict = new Dictionary<string, GameObject>();

        foreach (GameObject spawn in spawnObjList)
        {
            spawnDict.Add(spawn.GetComponent<InterObjController>().spawnId, spawn);
        }

    }

}
