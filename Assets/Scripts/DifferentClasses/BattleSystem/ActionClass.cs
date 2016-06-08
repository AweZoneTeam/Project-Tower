using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, которым можно описать любое элементарное действие, происходящее в игре
/// </summary>
[System.Serializable]
public class ActionClass
{
	public string func;
	public orientationEnum dir;
	public ActionClass(orientationEnum direct)
	{
		dir = direct;
	}
	public ActionClass()
	{
	}
	//ну это всё вроде не нужно
	/*public int type;//Тип и номер совершаемого действия, 2 числа, которым в соответствие 
	public int numb;//ставится некая совершаемая функция, описанная в ActionTypes
	public List<int> intParametres;//Параметры типа int, которые регулирует действие
	public List<float> floatParametres;//Параметры типа float, которые регулируют действие
	public List<GameObject> actObjects;//Объекты, над которыми производится действие*/
}
