using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Части - это основы управления любой анимации в Project Tower. 
public class PartController : MonoBehaviour 
{
	
	
	public int numb, type, mod, animationMod, frame, addictiveFrame;
	public GAF.Core.GAFMovieClip mov;
	
	public string currentState, nextState;
	public bool loop;
	[HideInInspector]
	public bool isWeaponFx;
	public int FPS;
	[HideInInspector]
	public float orientation;//В какую сторону повёрнут персонаж? Считаем, что все анимации сделаны на персонажа, повёрнутого вправо
	public bool inversed;//если правда, то все правые анимации меняются с левыми. Удобно использовать для одноручных оружий.
	public List<PartController> parts;
	/*public List<GameObjecat> prefabParts;//Сюда запихиваются префабные части, а значит, они никуда не денутся, если мы, например, перейдём в новую сцену.
	[HideInInspector]
	public List<Vector2> partPositions; // Если у части еть зависимые части, то здесь будет список их относительного положения. Задаются эти положения в режакторе анимаций*/
		
	public int partsNumb;
	public List<animationSoundData> soundData;
	
	public AnimationInterpretator interp;
	private SoundManager sManager;
	public AudioSource efxSource;
	private uint k = 1;
	
	public int jj;

	//Инициализация
	public void Awake () 
	{
		partsNumb = 0;
		sManager=GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SoundManager> ();
	}

	//Работа части заключается в том, чтобы интерпретировать полученные 2 числа от аниматора в анимацию, которую должен проигрывать подчинённый гаф.
	public void Work()
	{
		orientation = Mathf.Sign(transform.lossyScale.x);
		if (inversed) {
			orientation *= -1;
		}
		animationInfo animInfo=interp.animTypes [type].animInfo[numb];
		//Зависимые части способны задавать анимации, так как руку надо сжимать, чтобы держать меч
		if (parts.Count != 0) {
			if (((orientation >= 0) && (parts [parts.Count - 1].interp.animTypes [type].animInfo [numb].rsequence.parentSequence.Length > 1))||
				((orientation <= 0) && (parts [parts.Count - 1].interp.animTypes [type].animInfo [numb].lsequence.parentSequence.Length > 1))) 
			{
				animInfo = parts [parts.Count - 1].interp.animTypes [type].animInfo [numb];
			} 
		}
		//Анимации могут зависеть от ориентации персонажа
		if (orientation >= 0)
			nextState = interp.animTypes [type].animInfo[numb].rsequence.sequence;
		else
			nextState = interp.animTypes [type].animInfo[numb].lsequence.sequence;
		//Здесь происходит смена анимации
		if ((nextState!=currentState)&&(!interp.animTypes [type].animInfo [numb].stopStepByStep))
		{	
			currentState=nextState;
			loop=interp.animTypes[type].animInfo[numb].loop;
			FPS=interp.animTypes[type].animInfo[numb].FPS;
			soundData=interp.animTypes [type].animInfo [numb].soundData;
			mov.setSequence(currentState,true);
			mov.settings.targetFPS=k*(uint)FPS;
			if (loop)
				mov.settings.wrapMode=GAF.Core.GAFWrapMode.Loop;
			else
				mov.settings.wrapMode=GAF.Core.GAFWrapMode.Once;
			mov.setPlaying(true);
			mov.play();
		}

		//StepByStep - обеспечивает такие анимации, как поднятие по верёвке и по лестнице, которые проигрываются только при свершении действия
		if (interp.animTypes [type].animInfo [numb].stepByStep)
			mov.setPlaying(true);
		if (interp.animTypes [type].animInfo [numb].stopStepByStep)
			mov.gotoAndStop(mov.getCurrentFrameNumber());
		//Здесь происходит озвучивание анимации
		for (int i=0;i< interp.animTypes [type].animInfo [numb].soundData.Count;i++)
		{
			if ((interp.animTypes [type].animInfo [numb].soundData[i].time<=frame)&&
				(!interp.animTypes [type].animInfo [numb].soundData[i].played))
			{
				sManager.RandomizeSfx(efxSource, interp.animTypes [type].animInfo [numb].soundData[i].audios);
				interp.animTypes [type].animInfo [numb].soundData[i].played=true;
			}
			else if ((interp.animTypes [type].animInfo [numb].soundData[i].time >frame)&&
				(interp.animTypes [type].animInfo [numb].soundData[i].played))
			{
				interp.animTypes [type].animInfo [numb].soundData[i].played=false;					
			}
		}

		//Здесь происходит учёт динамики порядка прорисовки
		if (orientation >= 0) {
			for (int i=0; i<interp.animTypes[type].animInfo[numb].rightOrderData.Count; i++)
				if ((frame >= interp.animTypes [type].animInfo [numb].rightOrderData [i].time) && (interp.animTypes [type].animInfo [numb].rightOrderData [i].order != mov.settings.spriteLayerValue))
					SpFunctions.ChangeRenderOrder (interp.animTypes [type].animInfo [numb].rightOrderData [i].order, mov.gameObject);
		}
		else 
		{
			for (int i=0; i<interp.animTypes[type].animInfo[numb].leftOrderData.Count; i++)
				if ((frame >= interp.animTypes [type].animInfo [numb].leftOrderData [i].time) && (interp.animTypes[type].animInfo [numb].leftOrderData [i].order != mov.settings.spriteLayerValue))
					SpFunctions.ChangeRenderOrder (interp.animTypes [type].animInfo [numb].leftOrderData [i].order, mov.gameObject);
		}
	}

	//Функция вызываемая при создании части кодом, тогда автоматически должны создаваться подчинённые части, если они есть. С подчинёнными частями - своя морока. 
	/*public int InstantiateParts(PartConroller parentPart)
	{
		GameObject part;
		for (int i = 0; i < parentPart.prefabParts.Count; i++) {
			part = Instantiate (parentPart.prefabParts [i], Vector3.zero, Quaternion.identity) as GameObject;
			part.transform.parent = parentPart.gameObject.transform;
		}
	}*/
	//Но так как эта функция не нужна, я не буду её дописывать.

}
	