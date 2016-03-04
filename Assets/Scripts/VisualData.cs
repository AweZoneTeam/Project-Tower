using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Данные от том, как персонаж должен анимироваться
//То есть это анимируемые части и их взаимное положение относительно друг друга.
//Эти данные добавляются в аниматор... добавляешь их в аниматор, и он сам расставляет части как надо
//Как оказалось - не самая нужная штука. Она пригодилась разве что для тренировки, чтобы я мог понять принцип работы редактора и ScriptableObject.
//Всё, я понял, что VisualData - неплохой способ хранить данные о том, какие данные во всей файловой структуре проекта относятся к данному персонажу
[System.Serializable]
public class VisualData : ScriptableObject 
{
	public string name;
    public string type;//тип аниматора, который должен быть у соответствующего данным объекта
	public GameObject visual;//Ссылка на префаб в редакторе, которому соответстует данная база визуальных данных
	public List<AnimationInterpretator> animInterpretators= new List<AnimationInterpretator>();

	public VisualData (VisualData _data)
	{
		name = _data.name;
		visual = _data.visual;
	}

    public void SetData(VisualData _data)
    {
        name = _data.name;
        type = _data.type;
        visual = _data.visual;
        animInterpretators = _data.animInterpretators;
    }

	public VisualData (string _name)
	{
		name = _name;
		visual = null;
		animInterpretators=new List<AnimationInterpretator>();

	}

	//Функция, которая инициализирует поля visual, animInterpretators
	public void SetEmpty()
	{
		visual = null;
		animInterpretators = new List<AnimationInterpretator> ();
	}
		
}
