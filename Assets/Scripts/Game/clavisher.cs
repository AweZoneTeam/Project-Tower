using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Прикрепите к объекту, и он будет следить за нажатием всех нужных вам клавиш.
/// </summary>
public class Clavisher : MonoBehaviour
{
	public List<ButtonClass> buttons;//Массив рассматриваемых клавиш
	public bool k1, k2;//Два bool'а проверяющие, как именно нажаты клавиши
	public bool[] numbK1, numbK2;//Два bool'а проверяющие, как именно нажаты клавиши, обозначающие цифры
	public int addLength;//Кол-во вспомогательных элементов массива button, которые нужны для группирования клавиш и анализа этих групп
	public int minTimer;//Минимальное значение таймера, которое присваивается, как бы быстро не нажали на клавишу. 
	public int averageTimer;//Сколько времени нужно жать клавишу, чтобы push из 1 перешёл в 2
	public int releaseTimer;//Сколько времени нужно жать клавишу, чтобы push из 2 перешёл в 3
	public int endTimer;//Максимальное значение таймера


	void FixedUpdate()
	{
		for (int j=0; j<addLength; j++) 
		{
			numbK1[j]=false;
			numbK2[j]=false;
		}
		for (int i=0; i<buttons.Count-addLength; i++) {
			switch (buttons [i].button) {
			case "LeftShift":
				k1 = Input.GetKeyDown (KeyCode.LeftShift);
				k2 = Input.GetKey (KeyCode.LeftShift);
				break;
			case "Space":
				k1 = Input.GetKeyDown (KeyCode.Space);
				k2 = Input.GetKey (KeyCode.Space);
				break;
			case "RightArrow":
				k1 = Input.GetKeyDown (KeyCode.RightArrow);
				k2 = Input.GetKey (KeyCode.RightArrow);
				break;
			case "UpArrow":
				k1 = Input.GetKeyDown (KeyCode.UpArrow);
				k2 = Input.GetKey (KeyCode.UpArrow);
				break;
			case "LeftArrow":
				k1 = Input.GetKeyDown (KeyCode.LeftArrow);
				k2 = Input.GetKey (KeyCode.LeftArrow);
				break;
			case "DownArrow":
				k1 = Input.GetKeyDown (KeyCode.DownArrow);
				k2 = Input.GetKey (KeyCode.DownArrow);
				break;
			case "0":
				k1 = Input.GetKeyDown (KeyCode.Alpha0);
				k2 = Input.GetKey (KeyCode.Alpha0);
				break;
			case "1":
				k1 = Input.GetKeyDown (KeyCode.Alpha1);
				k2 = Input.GetKey (KeyCode.Alpha1);
				break;
			case "2":
				k1 = Input.GetKeyDown (KeyCode.Alpha2);
				k2 = Input.GetKey (KeyCode.Alpha2);
				break;
			case "3":
				k1 = Input.GetKeyDown (KeyCode.Alpha3);
				k2 = Input.GetKey (KeyCode.Alpha3);
				break;
			case "4":
				k1 = Input.GetKeyDown (KeyCode.Alpha4);
				k2 = Input.GetKey (KeyCode.Alpha4);
				break;
			case "5":
				k1 = Input.GetKeyDown (KeyCode.Alpha5);
				k2 = Input.GetKey (KeyCode.Alpha5);
				break;
			case "6":
				k1 = Input.GetKeyDown (KeyCode.Alpha6);
				k2 = Input.GetKey (KeyCode.Alpha6);
				break;
			case "7":
				k1 = Input.GetKeyDown (KeyCode.Alpha7);
				k2 = Input.GetKey (KeyCode.Alpha7);
				break;
			case "8":
				k1 = Input.GetKeyDown (KeyCode.Alpha8);
				k2 = Input.GetKey (KeyCode.Alpha8);
				break;
			case "9":
				k1 = Input.GetKeyDown (KeyCode.Alpha9);
				k2 = Input.GetKey (KeyCode.Alpha9);
				break;
			default:
				k1 = Input.GetKeyDown (buttons [i].button);
				k2 = Input.GetKey (buttons [i].button);
				break;
			}
			if (k1) 
			{
				buttons [i].timer = minTimer;
			} 
			else if (k2) {
				if (buttons [i].timer < endTimer)
					buttons [i].timer++;
				if (buttons [i].timer < averageTimer)
					buttons [i].push = 1;
				else if ((buttons [i].timer < releaseTimer) && (buttons [i].timer >= averageTimer))
					buttons [i].push = 2;
				else if (buttons [i].timer >= releaseTimer)
					buttons [i].push = 3;
			} else {
				if (buttons [i].push >= 2) {
					buttons [i].push = 0;
					buttons [i].timer = 0;
				}
				if (buttons [i].timer > 0)
					buttons [i].timer--;
				else
					buttons [i].push = 0;
			}
		}
		for (int  j=0; j<addLength; j++) {
			if (numbK1[j]) 
			{
				buttons [buttons.Count - addLength+j].timer = minTimer;
			} 
			else if (numbK2[j]) 
			{
				if (buttons [buttons.Count - addLength+j].timer < endTimer)
					buttons [buttons.Count - addLength+j].timer++;
				if (buttons [buttons.Count - addLength+j].timer < averageTimer)
					buttons [buttons.Count - addLength+j].push = 1;
				else if ((buttons [buttons.Count - addLength+j].timer < releaseTimer) && (buttons [buttons.Count - addLength+j].timer >= averageTimer))
					buttons [buttons.Count - addLength+j].push = 2;
				else if (buttons [buttons.Count - addLength+j].timer >= releaseTimer)
					buttons [buttons.Count - addLength+j].push = 3;
			}
			else 
			{
				if (buttons [buttons.Count - addLength+j].push >= 2) 
				{
					buttons [buttons.Count - addLength+j].push = 0;
					buttons [buttons.Count - addLength+j].timer = 0;
				}
				if (buttons [buttons.Count - addLength+j].timer > 0)
					buttons [buttons.Count - addLength+j].timer--;
				else
					buttons [buttons.Count - addLength+j].push = 0;
			}
		}
	}
}
