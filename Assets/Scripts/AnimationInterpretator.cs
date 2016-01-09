using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//Интерпретатор анимаций - своеобразная матрица, в которое паре числе (тип и номер) ставится в соответствие какая анимация проигрывается, а также какие у неё 
//параметры (зацикливание, озвучивание, порядок отрисовки). Используется анимационными частями

[System.Serializable]
public class AnimationInterpretator : ScriptableObject 
{
	public string partPath; //Где находится оригинал этой информации по анимированию
	public List<animationInfoTypes> animTypes=new List<animationInfoTypes>(); //Здесь собраны все используемые частью типы анимаций

	public AnimationInterpretator (AnimationInterpretator interp)
	{
		animTypes = new List<animationInfoTypes> ();
		for (int i = 0; i < interp.animTypes.Count; i++) {
			animTypes.Add (new animationInfoTypes (interp.animTypes [i]));
		}
		partPath = interp.partPath;
	}

	//Функция, вызываемая при добавлении новой анимационной части
	public AnimationInterpretator (string path)
	{
		partPath = path;
		animTypes = new List<animationInfoTypes> ();
	}

	//Функция, которая обеспечивает сохранение анимационных матриц. Все нижеописанные конструкторы необходимы исключительно для этого
	public void setInterp(AnimationInterpretator interp)
	{
		partPath = interp.partPath;
		animTypes.Clear ();
		for (int i = 0; i < interp.animTypes.Count; i++) {
			animTypes.Add (new animationInfoTypes (interp.animTypes [i]));
		}
	}

	//Функция, которая возвращает интерпретатор по заданному пути
	public AnimationInterpretator FindInterp(string path)
	{
		return AssetDatabase.LoadAssetAtPath(path, typeof(AnimationInterpretator)) as AnimationInterpretator;
	}
}

[System.Serializable]
public class animationInfoTypes //Здесь собирается вся информация об анимациях одного типа, например, анимации передвижения, или ударов мечом
{
	public string name;
	public List<animationInfo> animInfo; 

	//Конструктор, используемый при сохранении интерпретатора
	public animationInfoTypes (animationInfoTypes type)
	{
		animInfo=new List<animationInfo>();
		name = type.name;
		for (int i = 0; i < type.animInfo.Count; i++) {
			animInfo.Add(new animationInfo (type.animInfo[i]));
		}
	}

	//Конструктор, используемый при добавлении новой анимации
	public animationInfoTypes (string typeName, string animName)
	{
		animInfo=new List<animationInfo>();
		name = typeName;
		animInfo.Add (new animationInfo (animName));
	}
}

[System.Serializable]
public class animationInfo //Здесь собирается вся информация об одной анимации (какую последовательность надо включить, если персонаж идёт?)
{
	public animationPartString rsequence;//Какая анимация проигрывается, если персонаж повёрнут вправо
	public animationPartString lsequence;//А если влево?
	public List<animationSoundData> soundData;//как анимация звучит
	public List<animationLayerOrderData> leftOrderData, rightOrderData;//как прорисовывается (в каком порядке) анимация в зависимости от кадра 
																	   //и поворота персонажа.
	public bool loop;//Зациклена ли анимация?
	public bool stepByStep;//этот 
	public bool stopStepByStep;// и этот параметры обеспечивают нормальное воспроизведение таких анимаций, как подъём по лестнице
	public int FPS;//скорость анимации (кадры в секунду)

	//Конструктор, используемый при сохранении интерпретатора
	public animationInfo(animationInfo info)
	{
		soundData=new List<animationSoundData>();
		leftOrderData=new List<animationLayerOrderData>();
		rightOrderData=new List<animationLayerOrderData>();
		rsequence = new animationPartString (info.rsequence);
		lsequence = new animationPartString (info.lsequence);
		for (int i = 0; i < info.soundData.Count; i++)
			soundData.Add (new animationSoundData (info.soundData [i]));
		for (int i = 0; i < info.leftOrderData.Count; i++)
			leftOrderData.Add (new animationLayerOrderData (info.leftOrderData [i]));
		for (int i = 0; i < info.rightOrderData.Count; i++)
			rightOrderData.Add (new animationLayerOrderData (info.rightOrderData [i]));
		loop = info.loop;
		stepByStep = info.stepByStep;
		stopStepByStep = info.stopStepByStep;
		FPS = info.FPS;
	}

	//Конструктор, используемый при добавлении новой анимации
	public animationInfo(string name)
	{
		soundData=new List<animationSoundData>();
		leftOrderData=new List<animationLayerOrderData> ();
		rightOrderData=new List<animationLayerOrderData> ();
		rsequence=new animationPartString (name);
		lsequence=new animationPartString (name);
	}
}

//набор звуков, которые издаются при проигрывании анимации
[System.Serializable]
public class animationSoundData
{
	public bool played;//была ли эта запись уже сыграна?
	public int time;//Кадр анимации, при котором проигрывается звук
	public AudioClip[] audios;//Какой звук проигрывается (если звуков несколько, то случайно выбирается один их них)

	//Конструктор
	public animationSoundData(animationSoundData soundData)
	{
		played = soundData.played;
		time = soundData.time;
		audios = soundData.audios;
	}
}

//Эта штука способна задать, в каком порядке и как именно прорисовывается часть тела при проигрывании анимации в зависимости от кадра анимации
[System.Serializable]
public class animationLayerOrderData
{
	public int time;//кадр анимации, в котором надо поменять порядок прорисовки
	public int order;//Порядок прорисовки

	//Конструктор
	public animationLayerOrderData(animationLayerOrderData orderData)
	{
		time = orderData.time;
		order = orderData.order;
	}
}

//Если у части есть родительская часть, то она может задать ей, какую она должна проигрывать анимацию. Если ничего она не задаёт, то надо оставить строку
//parentSequence пустой
[System.Serializable]
public class animationPartString
{
	public string sequence;
	public string parentSequence;

	//Конструктор
	public animationPartString(animationPartString partString)
	{
		sequence = partString.sequence;
		parentSequence = partString.parentSequence;
	}

	public animationPartString (string _sequence)
	{
		sequence = _sequence;
	}
}