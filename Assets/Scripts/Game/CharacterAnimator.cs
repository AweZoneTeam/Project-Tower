using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GAF.Core;

/// <summary>
/// Аниматор (разумного) персонажа. Он говорит частям тела персонажа, как им надо двигаться.
/// </summary>
public class CharacterAnimator : BaseAnimator
{

    #region consts

    private const float beginRunTime = 0.2f;//Сколько времени длится анимация "Начало бега"?
    private const float stopDragTime = 0.5f;//Сколько времени длится анимация "Конец скольжения"?
    private const float dragSpeed = 5f;//до какой скорости считается, что персонаж ещё тормозится (когда его скорость уменьшается)?

    #endregion

    #region enums

    public enum groundness { grounded = 1, crouch, preGround, inAir };

    private enum speedY{fastUp=30, medUp=10, slowUp=7, slowDown=-1, medDown=-6, fastDown=-10};//Скорости, при которых меняется анимация прыжка

    #endregion //enums

    #region timers

    public float beginRunTimer, stopDragTimer;

    #endregion //timers

    #region fields

    public AnimClass anim;//Идентификатор проигрываемой в данный момент анимации.
	public List<PartController> parts=new List<PartController>();//Части тела, управляемых аниматором
    public VisualData visualData;//Визуальная база данных, в которую мы будем вносить изменения по создаваемому персонажу

	public List<animList> animTypes=new List<animList>();//База данный по анимациям, проигрываемых аниматором, которые отсортированы по типам
    public List<NamedAnimClass> animBase=new List<NamedAnimClass>();//база данных по анимациям, используемых аниматором. 
                                                                                     //В отличие от пердыдущего списка - это одномерный массив. 
                                                                                     //Нужен для удобного написания скриптов.
    public bool play=false, stop=true;//два весёлых була, обеспечивающие проигрывание анимации непосредственно в самом редакторе.

    private Rigidbody2D rigid;//Физика персонажа. Нужен для расчёта скорости, так как они влияют на анимации.
    private Stats stats;//Параметры персонажа

    #endregion //fields

    /// <summary>
    /// Создать все рабочие списки
    /// </summary>
    public void SetDefaultAnimator()
    {
        parts = new List<PartController>();
        animTypes = new List<animList>();
        animBase = new List<NamedAnimClass>();
    }

    public void Start()
    {
        rigid = gameObject.GetComponentInParent<Rigidbody2D>();
    }

	public void Update()
	{
        Sinchronize();
        /*
#if UNITY_EDITOR
        if (play && !stop)//Пусть продолжается работа частей
        {
            SpFunctions.AnimateIt(parts, anim);
        }
        else if (play && stop)//Пусть начнётся работа частей
        {
            stop = false;
            SetPartAnimationsAtBegining(anim.type, anim.numb, SpFunctions.realSign(gameObject.transform.lossyScale.x)>0, true);
        }
        else if (!stop && !play)//Всё спокойно, ничего не происходит
        {
        }

        else if (stop && !play)//Остановить анимацию и поставить её в самое начало
        {
            stop = false;
            SetPartAnimationsAtBegining(anim.type, anim.numb, SpFunctions.realSign(gameObject.transform.lossyScale.x) > 0, false);
        }
        #endif //UNITY_EDITOR*/
    }

    /// <summary>
    /// Функция, которая всем части аниматора поставит в заданные анимации
    /// </summary>
    /// <param name="тип анимации"></param>
    /// <param name="номер анимации"></param>
    public void setPartAnimations(int type, int numb, bool right)
    {
        GAFMovieClip mov;
        AnimationInterpretator interp;
        foreach (PartController part  in parts)
        {
            part.type = type;
            part.numb = numb;
            mov = part.mov;
            interp = part.interp;
            if (right)
            {
                mov.setSequence(interp.animTypes[type].animInfo[numb].rsequence.sequence, true);
            }
            else
            {
                mov.setSequence(interp.animTypes[type].animInfo[numb].lsequence.sequence, true);
            }
        }
    }

    /// <summary>
    /// Функция, которая по заданному названию анимации выдаёт её тип и номер
    /// </summary>
    /// <param name="название анимации"></param>
    /// <returns>тип и номер анимации</returns>
    public AnimClass FindAnimData(string animName)
    {
        AnimClass a = null;
        foreach (NamedAnimClass s in animBase)
        {
            if (string.Equals(s.animName, animName))
            {
                a = new AnimClass(s.type, s.numb);
            }
        }
        return a;
    }

    /// <summary>
    /// Функция, вызываемая внутри самого аниматора для контроля анимаций в редакторе
    /// </summary>
    void SetPartAnimationsAtBegining(int type, int numb, bool right, bool _play)
    {
        setPartAnimations(type, numb, right);
        GAFMovieClip mov;
        foreach (PartController part in parts)
        {
            mov = part.mov;
            mov.gotoAndPlay(mov.currentSequence.startFrame);
            part.SetPlay(_play);
        }
    }

    public void Flip()
    {
        gameObject.transform.localScale = new Vector3(-1* gameObject.transform.localScale.x,
                                                          gameObject.transform.localScale.y,
                                                          gameObject.transform.localScale.z);
    }

    void Animate(string animName)
    {
        anim = FindAnimData(animName);
        SpFunctions.AnimateIt(parts, anim);
        Sinchronize();
    }

    /// <summary>
    /// Контроль синхронности движений
    /// </summary>
    void Sinchronize()
    {
        int frameGap;
        animationInfo aInfo;
        if (parts != null)
        {
            if (parts.Count > 0)
            {
                frameGap = (int)parts[0].mov.getCurrentFrameNumber() - (int)parts[0].mov.currentSequence.startFrame;
                for (int i = 1; i < parts.Count; i++)
                {
                    aInfo = parts[i].interp.animTypes[anim.type].animInfo[anim.numb];
                    if (!aInfo.stepByStep || !aInfo.stopStepByStep)
                    {
                        parts[i].mov.gotoAndPlay((uint)((int)parts[i].mov.currentSequence.startFrame + frameGap));
                    }
                    else if (aInfo.stepByStep)
                    {
                        parts[i].mov.gotoAndStop((uint)((int)parts[i].mov.currentSequence.startFrame + frameGap));
                    }
                }
            }
        }
    }

    #region AnimatedActions

    /// <summary>
    /// Анимировать отсутствие активности
    /// </summary>
    public void GroundStand()
    {
        beginRunTimer = -1f;
        if (Mathf.Abs(rigid.velocity.x) > dragSpeed)
        {
            Animate("DragBegin");
        }
        else
        {
            if (!string.Equals("Idle", animTypes[anim.type].animations[anim.numb]))
            {
                if (stopDragTimer == -1f)
                {
                    stopDragTimer = stopDragTime;
                }
                if (stopDragTimer > 0f)
                {
                    Animate("DragStop");
                    stopDragTimer -= Time.deltaTime;
                }
                else
                {
                    Animate("Idle");
                    stopDragTimer = -1f;
                }
            }
        }
    }

    /// <summary>
    /// Анимировать передвижение по земле
    /// </summary>
    public void GroundMove()
    {
        stopDragTimer = -1f;
        if (!string.Equals("Run", animTypes[anim.type].animations[anim.numb]))
        {
            if (beginRunTimer == -1f)
            {
                beginRunTimer = beginRunTime;
            }
            if (beginRunTimer > 0f)
            {
                Animate("RunBegin");
                beginRunTimer -= Time.deltaTime;
            }
            else
            {
                Animate("Run");
                beginRunTimer = -1f;
            }
        }

    }

    /// <summary>
    /// Анимировать движения, происходящие в воздухе
    /// </summary>
    public void AirMove()
    {
        if (stats.groundness == (int)groundness.preGround) {Animate("Fallen");}
        else if (rigid.velocity.y <= 1f * (int)speedY.fastDown) {Animate("FallEnd");}
        else if (rigid.velocity.y <= 1f * (int)speedY.medDown) {Animate("FallContinue");}
        else if (rigid.velocity.y <= 1f * (int)speedY.slowDown) {Animate("FallBegin");}
        else if (rigid.velocity.y <= 1f * (int)speedY.slowUp) {Animate("JumpEnd");}
        else if (rigid.velocity.y <= 1f * (int)speedY.medUp) {Animate("JumpContinue");}
        else if (rigid.velocity.y <= 1f * (int)speedY.fastUp) {Animate("JumpBegin");}
        else {Animate("StartJump");}
        beginRunTimer = -1f;
    }
    #endregion //AnimatedActions

    /// <summary>
    /// Задать поле статов
    /// </summary>
    /// <param name="задаваемые параметры"></param>
    public void SetStats(Stats _stats)
    {
        stats = _stats;
    }
}

/// <summary>
/// Класс, используемый для создания отсортированной по типам базы данных по используемым аниматором анимациям
/// </summary>
[System.Serializable]
public class animList
{
	public string typeName;
	public List<string>	animations=new List<string>();

	public animList (string name, string animName)
	{
		animations=new List<string>();
		typeName=name;
		animations.Add (animName);
	}

}

[System.Serializable]
public class NamedAnimClass: AnimClass
{
    public string animName;

    public NamedAnimClass(string _name, int _type, int _numb)
    {
        animName = _name;
        type = _type;
        numb = _numb;
    }
}

/// <summary>
/// Класс, необходимый для создания нужного нам отображения полей аниматора персонажа
/// </summary>
[CustomEditor(typeof(CharacterAnimator))]
public class CharacterAnimatorEditor : BaseAnimatorEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CharacterAnimator cAnim = (CharacterAnimator)target;
        if (GUILayout.Button("Delete anim Base"))
        {
            cAnim.animBase = new List<NamedAnimClass>();
        }
        if (GUILayout.Button("Create Database"))
        {
            if (cAnim.animBase == null)
            {
                cAnim.animBase = new List<NamedAnimClass>();
            }
            if (cAnim.animBase.Count == 0)
            {
                for (int i = 0; i < cAnim.animTypes.Count; i++)
                {
                    for (int j = 0; j < cAnim.animTypes[i].animations.Count;j++)
                    {
                        cAnim.animBase.Add(new NamedAnimClass(cAnim.animTypes[i].animations[j], i, j));
                    }
                } 
            }
        }
        /*Dictionary<string, AnimClass> animBase = cAnim.animBase;
        Dictionary<string, AnimClass>.KeyCollection animKeys = animBase.Keys;
        for (int i = 0; i < cAnim.animBase.Count; i++)
        {
        }*/
    }
}