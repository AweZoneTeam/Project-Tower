using UnityEngine;
using System.Collections;

/// <summary>
/// Функции, ранее использовавшиеся для анализа ситуации, в которой находится персонаж
/// </summary>
public class InfoGetTypes
{	
	public int k1;

	//тип 0 номер 1
	public bool Overlaper(InfoGets infogets, int numb)
	{
		return Physics2D.OverlapCircle (SpFunctions.VectorConvert (infogets.infoGets [numb].OBJ [0].transform.position),
		                               infogets.infoGets [numb].PRM2 [0],
		                               infogets.infoGets [numb].LYR);
		k1++;
	}
	
	//тип 0 номер 2
	public bool Raycaster(InfoGets infogets, int numb, Vector2 vect)
	{	
		return Physics2D.Raycast(SpFunctions(infogets.infoGets[numb].OBJ[0].transform.position),
		                         vect.normalized,
		                         infogets.infoGets[numb].PRM2[0],
		                         infogets.infoGets[numb].LYR);
	}
}
