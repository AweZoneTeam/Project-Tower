using UnityEngine;
using System.Collections;

/// <summary>
/// Функции, ранее использовавшиеся для анализа ситуации, в которой находится персонаж
/// </summary>
public static class InfoGetTypes
{	

	//тип 0 номер 1
	public static bool Overlaper(InfoGetClass infoget)
	{
		return Physics2D.OverlapCircle (SpFunctions.VectorConvert (infoget.indicators[0].transform.position),
		                               infoget.floatParametres[0],
		                               infoget.whatToCheck);
	}
	
	//тип 0 номер 2
	public static bool Raycaster(InfoGetClass infoget, Vector2 vect)
	{	
		return Physics2D.Raycast(SpFunctions.VectorConvert(infoget.indicators[0].transform.position),
		                         vect.normalized,
		                         infoget.floatParametres[0],
		                         infoget.whatToCheck);
	}
}
