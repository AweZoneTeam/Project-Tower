using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

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

    //Функция, которая обеспечивает сохранение анимационных матриц
    public void setInterp(List<animList> aList)
    {
        animTypes.Clear();
        for (int i = 0; i < aList.Count; i++)
        {
            animTypes.Add(new animationInfoTypes(aList[i].animations, aList[i].typeName));
        }
    }

	//Функция, которая возвращает интерпретатор по заданному пути
	public AnimationInterpretator FindInterp(string path)
	{
		return AssetDatabase.LoadAssetAtPath(path, typeof(AnimationInterpretator)) as AnimationInterpretator;
	}

    /// <summary>
    /// Во всех анимациях часть тела будет прорисовываться сначала в выбранном дефолтном слое
    /// </summary>
    /// <param name="слой"></param>
    public void SetDefaultLayer(int layer, bool right)
    {
        animationInfo animInfo;
        for (int i = 0; i < animTypes.Count; i++)
        {
            for (int j = 0; j < animTypes[i].animInfo.Count; j++)
            {
                int kk = right ? 1 : 2;
                animInfo = animTypes[i].animInfo[j];
                for (int k = 0; k < kk; k++)
                {
                    List<animationLayerOrderData> orderData = (k == 0) ? animInfo.leftOrderData : animInfo.rightOrderData;
                    if (orderData.Count > 0)
                    {
                        if (orderData[0].time == 0)
                        {
                            orderData.RemoveAt(0);
                        }
                    }
                    orderData.Insert(0, new animationLayerOrderData(0, layer));
                }
            }
        }
    }

    /// <summary>
    /// Функция, используемая для изменения анимационных последовательностей в аниматоре таким образом, чтобы в rSequence
    /// анимации начинались с Right, а в lsequence - с left.
    /// </summary>
    public void OrientateSequences()
    {
        animationInfo animInfo;
        for (int i = 0; i < animTypes.Count; i++)
        {
            for (int j = 0; j < animTypes[i].animInfo.Count; j++)
            {
                animInfo = animTypes[i].animInfo[j];
                animationPartString s= animInfo.rsequence;
                if (!s.sequence.Contains("Right") && !s.sequence.Contains("Left"))
                {
                    animInfo.rsequence.sequence = "Right" + animInfo.rsequence.sequence;
                }
                if (s.parentSequence.Length > 1)
                {
                    if (!s.parentSequence.Contains("Right") && !s.parentSequence.Contains("Left"))
                    {
                        animInfo.rsequence.parentSequence = "Right" + animInfo.rsequence.parentSequence;
                    }
                }
                s = animInfo.lsequence;
                if (!s.sequence.Contains("Right") && !s.sequence.Contains("Left"))
                {
                    animInfo.lsequence.sequence = "Left" + animInfo.lsequence.sequence;
                }
                if (s.parentSequence.Length > 1)
                {
                    if (!s.parentSequence.Contains("Right") && !s.parentSequence.Contains("Left"))
                    {
                        animInfo.rsequence.parentSequence = "Left" + animInfo.rsequence.parentSequence;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Функция, которая меняет местами все левые и правые анимации.
    /// </summary>
    public void InverseSequeces()
    {
        animationInfo animInfo;
        for (int i = 0; i < animTypes.Count; i++)
        {
            for (int j = 0; j < animTypes[i].animInfo.Count; j++)
            {
                animInfo = animTypes[i].animInfo[j];
                string s = animInfo.rsequence.sequence;
                animInfo.rsequence.sequence = animInfo.lsequence.sequence;
                animInfo.lsequence.sequence = s;
                s = animInfo.rsequence.parentSequence;
                animInfo.rsequence.parentSequence = animInfo.lsequence.parentSequence;
                animInfo.lsequence.parentSequence = s;
            }
        }
    }

    public void AddOrientatedSequences()
    {

        animationInfo animInfo;
        for (int i = 0; i < animTypes.Count; i++)
        {
            for (int j = 0; j < animTypes[i].animInfo.Count; j++)
            {
                animInfo = animTypes[i].animInfo[j];
                #region sequences
                if (animInfo.rsequence.sequence.Contains("Right") && !animInfo.lsequence.sequence.Contains("Left"))
                {
                    animInfo.lsequence.sequence = "Left" + animInfo.lsequence.sequence;
                }
                else if (animInfo.rsequence.sequence.Contains("Left") && !animInfo.lsequence.sequence.Contains("Right"))
                {
                    animInfo.lsequence.sequence = "Right" + animInfo.lsequence.sequence;
                }
                else
                {
                    animInfo.lsequence.sequence = animInfo.rsequence.sequence;
                }
                #endregion //sequences

                #region parentSequences
                if (animInfo.rsequence.parentSequence.Contains("Right") && !animInfo.lsequence.parentSequence.Contains("Left"))
                {
                    animInfo.lsequence.parentSequence = "Left" + animInfo.lsequence.parentSequence;
                }
                else if (animInfo.rsequence.parentSequence.Contains("Left") && !animInfo.lsequence.parentSequence.Contains("Right"))
                {
                    animInfo.lsequence.parentSequence = "Right" + animInfo.lsequence.parentSequence;
                }
                else
                {
                    animInfo.lsequence.parentSequence = animInfo.rsequence.parentSequence;
                }
                #endregion //parentSequences
            }
        }
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

    public animationInfoTypes (List<string> sList, string _name)
	{
		animInfo=new List<animationInfo>();
		name = _name;
		for (int i = 0; i < sList.Count; i++) {
			animInfo.Add(new animationInfo (sList[i]));
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
        FPS = 30;
	}
}

//набор звуков, которые издаются при проигрывании анимации
[System.Serializable]
public class animationSoundData
{
	public bool played;//была ли эта запись уже сыграна?
	public int time;//Кадр анимации, при котором проигрывается звук
	public List<AudioClip> audios;//Какой звук проигрывается (если звуков несколько, то случайно выбирается один их них)

	//Конструктор
	public animationSoundData(animationSoundData soundData)
	{
		played = soundData.played;
		time = soundData.time;
		audios = soundData.audios;
	}

    public animationSoundData(bool _played, int _time)
    {
        played = _played;
        time = _time;
        audios = new List<AudioClip>();
    }
}

//Эта штука способна задать, в каком порядке и как именно прорисовывается часть тела при проигрывании анимации в зависимости от кадра анимации
[System.Serializable]
public class animationLayerOrderData: IComparable<animationLayerOrderData>
{
	public int time;//кадр анимации, в котором надо поменять порядок прорисовки
	public int order;//Порядок прорисовки

	//Конструктор
	public animationLayerOrderData(animationLayerOrderData orderData)
	{
		time = orderData.time;
		order = orderData.order;
	}

    public animationLayerOrderData(int _time, int _order)
    {
        time = _time;
        order = _order;
    }

    public int CompareTo(animationLayerOrderData other)
    {
        if (other == null)
        {
            return 1;
        }

        return time - other.time;
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