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
    public AnimClass anim;//Идентификатор проигрываемой в данный момент анимации.
	public List<PartController> parts=new List<PartController>();//Части тела, управляемых аниматором
    public VisualData visualData;//Визуальная база данных, в которую мы будем вносить изменения по создаваемому персонажу

	public List<animList> animTypes=new List<animList>();//База данный по анимациям, проигрываемых аниматором, которые отсортированы по типам
    public Dictionary<string,AnimClass> animBase=new Dictionary<string, AnimClass>();//база данных по анимациям, используемых аниматором. 
                                                                                     //В отличие от пердыдущего списка - это одномерный массив. 
                                                                                     //Нужен для удобного написания скриптов.
    public List<string> animNames;//Массив, хранящий ключи для animBase
    public bool play=false, stop=true;//два весёлых була, обеспечивающие проигрывание анимации непосредственно в самом редакторе.

    /// <summary>
    /// Создать все рабочие списки
    /// </summary>
    public void SetDefaultAnimator()
    {
        parts = new List<PartController>();
        animTypes = new List<animList>();
        animBase = new Dictionary<string, AnimClass>();
        animNames = new List<string>();
    }

	public void Update()
	{
		SpFunctions.AnimateIt (parts, 
		                   anim);
#if UNITY_EDITOR
        if (play && !stop)//Пусть продолжается работа частей
        {
            SpFunctions.AnimateIt(parts, anim);
        }
        if (play && stop)//Пусть начнётся работа частей
        {
            stop = false;
            SetPartAnimationsAtBegining(anim.type, anim.numb, SpFunctions.realSign(gameObject.transform.lossyScale.x)>0, true);
        }
        if (!stop && !play)//Всё спокойно, ничего не происходит
        {
        }

        if (stop && !play)//Остановить анимацию и поставить её в самое начало
        {
            stop = false;
            SetPartAnimationsAtBegining(anim.type, anim.numb, SpFunctions.realSign(gameObject.transform.lossyScale.x) > 0, false);
        }
        #endif //UNITY_EDITOR
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
        /*Dictionary<string, AnimClass> animBase = cAnim.animBase;
        Dictionary<string, AnimClass>.KeyCollection animKeys = animBase.Keys;
        for (int i = 0; i < cAnim.animBase.Count; i++)
        {
        }*/
    }
}