using UnityEngine;
using System.Collections;

//Класс передаваемой анимации. Именно объект этого класса передаётся аниматору, 
//чтобы он смог "сказать" своим анимационным частям, какие анимации проигрывать.
[System.Serializable]
public class animClass
{	
	public int type;
	public int numb;
}
