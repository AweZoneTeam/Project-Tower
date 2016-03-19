using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
//Части - это основы управления любой анимации в Project Tower. 
public class PartController : MonoBehaviour 
{
	
	
	public int numb, type, frame, addictiveFrame;
	public GAF.Core.GAFMovieClip mov;//Созданный при помощи технологии GAF клип, которым мы и управляем
    public string path;//Путь, в котором находится анимационная база данных для данной части.
    public AnimationInterpretator interp;//База данных об анимациях: когда, как и что проигрывать 

    //Данные об анимации проигрываемой в данный момент
    public string currentState="default", nextState="default";
	public bool loop;
    public int k1 = 0;
	public int FPS;
	[HideInInspector]
	public float orientation;//В какую сторону повёрнут персонаж? Считаем, что все анимации сделаны на персонажа, повёрнутого вправо
	public List<PartController> childParts=new List<PartController>();//Подчинённые части. Проигрываемые зависимыми частями анимации также влияют на анимацию родительской части.		
	public List<animationSoundData> soundData=new List<animationSoundData>();
    public List<animationLayerOrderData> rOrderData = new List<animationLayerOrderData>();
    public List<animationLayerOrderData> lOrderData = new List<animationLayerOrderData>();

	private SoundManager sManager;//К этому объекту обращаются с целью проиграть тот или иной звук
	public AudioSource efxSource;//По идее у каждой части тела есть собственный аудиопроигрыватель

	private uint k = 1;

    private bool play;//Делать ли части тела свою работу (функцию Work) непосредственно в редакторе?

	//Инициализация
    [ExecuteInEditMode]
	public void Awake () 
	{
		sManager=GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SoundManager> ();
#if UNITY_EDITOR
        k1++;
        interp =AssetDatabase.LoadAssetAtPath(path, typeof(AnimationInterpretator)) as AnimationInterpretator;
#endif
    }

	//Работа части заключается в том, чтобы интерпретировать полученные 2 числа от аниматора в анимацию, которую должен проигрывать подчинённый гаф.
	public void Work()
	{
		orientation = Mathf.Sign(transform.lossyScale.x);
        frame = (int)mov.getCurrentFrameNumber();
        animationInfo animInfo=interp.animTypes [type].animInfo[numb];
        //Анимации могут зависеть от ориентации персонажа
        if (orientation >= 0)
            nextState = animInfo.rsequence.sequence;
        else
            nextState = animInfo.lsequence.sequence;
        //Зависимые части способны задавать анимации, так как руку надо сжимать, чтобы держать меч
        if (childParts.Count != 0)
        {
            if (((orientation >= 0) && (childParts[childParts.Count - 1].interp.animTypes[type].animInfo[numb].rsequence.parentSequence.Length > 1)) ||
                ((orientation <= 0) && (childParts[childParts.Count - 1].interp.animTypes[type].animInfo[numb].lsequence.parentSequence.Length > 1)))
            {
                animationInfo cAnimInfo = childParts[childParts.Count - 1].interp.animTypes[type].animInfo[numb];
                if (orientation > 0)
                {
                    nextState = cAnimInfo.rsequence.parentSequence;
                }
                else
                {
                    nextState = cAnimInfo.lsequence.parentSequence;
                }
            }
        }
		//Здесь происходит смена анимации
		if ((nextState!=currentState)&&(!interp.animTypes [type].animInfo [numb].stopStepByStep))
		{	
			currentState=nextState;
			loop=interp.animTypes[type].animInfo[numb].loop;
			FPS=interp.animTypes[type].animInfo[numb].FPS;
            rOrderData = interp.animTypes[type].animInfo[numb].rightOrderData;
            lOrderData = interp.animTypes[type].animInfo[numb].leftOrderData;
            soundData =interp.animTypes [type].animInfo [numb].soundData;
			mov.setSequence(currentState,true);
            mov.gotoAndPlay(mov.currentSequence.startFrame);
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
        if (soundData != null)
        {
            for (int i = 0; i < soundData.Count; i++)
            {
                if ((soundData[i].time <= frame) && (soundData[i].played))
                {
                    sManager.RandomizeSfx(efxSource, soundData[i].audios);
                    soundData[i].played = true;
                }
                else if ((soundData[i].time > frame) &&
                    (soundData[i].played))
                {
                    soundData[i].played = false;
                }
            }
        }
		//Здесь происходит учёт динамики порядка прорисовки
		if (orientation >= 0) {
            if (rOrderData != null)
            {
                for (int i = 0; i < rOrderData.Count; i++)
                {
                    if ((frame >= rOrderData[i].time) && (rOrderData[i].order != mov.settings.spriteLayerValue))
                    {
                        SpFunctions.ChangeRenderOrder(rOrderData[i].order, mov.gameObject);
                    }
                }
            }
		}
		else 
		{
            if (lOrderData != null)
            {
                for (int i = 0; i < lOrderData.Count; i++)
                {
                    if ((frame >= lOrderData[i].time) && (lOrderData[i].order != mov.settings.spriteLayerValue))
                    {
                        SpFunctions.ChangeRenderOrder(lOrderData[i].order, mov.gameObject);
                    }
                }
            }
		}
	}

    void Update()
    {
#if UNITY_EDITOR
        // AnimationEngine();
#endif //UNITY_EDITOR
        Work();
    }
    void LateUpdate()
    {
#if UNITY_EDITOR
        //Пусть части тела будут менять свой порядок прорисовки непосредственно в редакторе
        if (interp != null)
        {
            if ((interp.animTypes.Count != 0)&&(!play))
            {
                orientation = SpFunctions.RealSign(gameObject.transform.lossyScale.x);
                frame = (int)mov.getCurrentFrameNumber();
                if (orientation >= 0)
                {
                    for (int i = 0; i < interp.animTypes[type].animInfo[numb].rightOrderData.Count; i++)
                        if ((frame >= interp.animTypes[type].animInfo[numb].rightOrderData[i].time) && (interp.animTypes[type].animInfo[numb].rightOrderData[i].order != mov.settings.spriteLayerValue))
                            SpFunctions.ChangeRenderOrder(interp.animTypes[type].animInfo[numb].rightOrderData[i].order, mov.gameObject);
                }
                else
                {
                    for (int i = 0; i < interp.animTypes[type].animInfo[numb].leftOrderData.Count; i++)
                        if ((frame >= interp.animTypes[type].animInfo[numb].leftOrderData[i].time) && (interp.animTypes[type].animInfo[numb].leftOrderData[i].order != mov.settings.spriteLayerValue))
                            SpFunctions.ChangeRenderOrder(interp.animTypes[type].animInfo[numb].leftOrderData[i].order, mov.gameObject);
                }
            }
        }

        /*
        if (play)
        {
            Work(); 
        }*/

    #endif //UNITY_EDITOR
    }

    /// <summary>
    /// Функция, обеспечивающая смену кадров, рассматривая ФПС и течение времени.
    /// </summary>
    void AnimationEngine()
    {
        addictiveFrame = Mathf.RoundToInt(Time.deltaTime * FPS);
        frame += addictiveFrame;
        if ((loop)&&(frame>(int)mov.currentSequence.endFrame))
            frame -= (int)mov.currentSequence.endFrame;
        if (frame <= (int)mov.currentSequence.endFrame)
        {
            mov.gotoAndPlay((uint)frame);
        }
    }

    /// <summary>
    /// Функция, запускающая работу части в редакторе
    /// </summary>
    /// <param name="_play"></param>
    public void SetPlay(bool _play)
    {
        play = _play;
    }
}
    