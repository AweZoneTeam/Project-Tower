using UnityEngine;
using System.Collections;

/// <summary>
///Класс, используемый клавишером. Он смотрит, какие клавиши 
///нажаты и как долго, и проставляет им соответствующие числа таймера и степени зажатости
/// </summary>
[System.Serializable]
public class ButtonClass
{
	public string button;//С какой клавишей имеем дело
	public int push;//Какова сейчас степень нажатости? 0 - не нажата, 1-быстрое нажатие, 2-долгое нажатие, 3-зажатие. 
	public float timer;//Как долго зажата?
}	
/// <summary>
///А этот класс ранее использовался экшн листами. Он содержит в себе ряд клавиш и смотрит,
///нажата ли хотя бы из них должным образом. Назовём это условием clv
/// </summary>
[System.Serializable]
public class ClaveClass
{
	public ButtonClass[] but;
}

/// <summary>
///Этот класс также использовался экшн листами. Он тоже содержит массив 
///ClaveClass'ов, и смотрит, все ли элементы условия clv соблюдаются
/// </summary>
[System.Serializable]
public class ClaveNoteClass
{
	public ClaveClass[] clv;
}
