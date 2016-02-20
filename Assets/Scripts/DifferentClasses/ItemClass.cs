using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой игровой предмет, который можно засунуть в инвентарь.
/// </summary>
[System.Serializable]
public class ItemClass
{
	public string name;
	public string type;
    public int quantity;//Количество предметов
    public Sprite image;//Иконка предмета  
}
