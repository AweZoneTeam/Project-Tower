using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, ранее исользовавшийся для подбираемых предметов... 
/// Какие игровые объекты получит персонаж, если он подберёт его.
/// </summary>
public class ItemClass : MonoBehaviour 
{
	public string name;
	public string type;
	public List<GameObject> objects;
}
