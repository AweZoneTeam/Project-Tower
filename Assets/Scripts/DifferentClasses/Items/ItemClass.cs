﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой игровой предмет, который можно засунуть в инвентарь.
/// </summary>
[System.Serializable]
public class ItemClass: ScriptableObject
{
	public string itemName;
    public string type;
    public Sprite image;//Иконка предмета  
}

/// <summary>
/// Специальный класс, воспринимаемый инвентарём, чтобы знать, сколько предметов каждого типа есть у интерактивного объекта
/// </summary>
[System.Serializable]
public class ItemBunch 
{
    public ItemClass item;
    public int quantity;
}