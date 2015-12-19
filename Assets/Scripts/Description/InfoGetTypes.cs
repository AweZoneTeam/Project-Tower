using UnityEngine;
using System.Collections;

public class InfoGetTypes : MonoBehaviour 
{	
	public int k1;

	//тип 0 номер 1
	public bool Overlaper(InfoGets infogets, int numb,SpFunctions sp)
	{
		return Physics2D.OverlapCircle (sp.VectorConvert (infogets.infoGets [numb].OBJ [0].transform.position),
		                               infogets.infoGets [numb].PRM2 [0],
		                               infogets.infoGets [numb].LYR);
		k1++;
	}
	
	//тип 0 номер 2
	public bool Raycaster(InfoGets infogets, int numb, Vector2 vect, SpFunctions sp)
	{	
		return Physics2D.Raycast(sp.VectorConvert(infogets.infoGets[numb].OBJ[0].transform.position),
		                         vect.normalized,
		                         infogets.infoGets[numb].PRM2[0],
		                         infogets.infoGets[numb].LYR);
	}
}
