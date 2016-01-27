using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт, который надо прикрепить к объекту, чтобы он просчитывал, 
/// попадают ли в указанные круговые области объекты, принадлежащие слоям из массива collisions
/// </summary>
public class CollisionRegulation : MonoBehaviour 
{

	public List<CollisionClass> collisionInfos;

	public void FixedUpdate()
	{
		for (int i=0; i<collisionInfos.Count; i++)
			collisionInfos [i].detected = Physics2D.OverlapCircle (SpFunctions.VectorConvert (transform.position), collisionInfos [i].radius, collisionInfos [i].collision);
	}
}

/// <summary>
/// Класс, характеризующий иформацию об окружающей данный объект среде, а именно этот класс используется для ответа на вопрос
/// находится ли объект данного типа на расстоянии меньшем данного числа от интересующего нас объекта
/// </summary>
[System.Serializable]
public class CollisionClass
{
	public LayerMask collision;
	public string name;
	public float radius;
	public bool detected;
}